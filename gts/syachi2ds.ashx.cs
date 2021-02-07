using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using PkmnFoundations.Data;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;
using System.IO;
using GamestatsBase;

namespace PkmnFoundations.GTS
{
    /// <summary>
    /// Summary description for pokemondpds
    /// </summary>
    public class syachi2ds : GamestatsHandler
    {
        public syachi2ds()
            : base("HZEdGCzcGGLvguqUEKQN0001d93500002dd5000000082db842b2syachi2ds",
            GamestatsRequestVersions.Version3, GamestatsResponseVersions.Version2, false, true)
        {

        }

        public override void ProcessGamestatsRequest(byte[] request, MemoryStream response, string url, int pid, HttpContext context, GamestatsSession session)
        {
            Pokedex.Pokedex pokedex = AppStateHelper.Pokedex(context.Application);

            switch (url)
            {
                default:
                    SessionManager.Remove(session);

                    // unrecognized page url
                    ShowError(context, 404);
                    return;

                #region Common
                // Called during startup. Seems to contain trainer profile stats.
                case "/syachi2ds/web/common/setProfile.asp":
                    SessionManager.Remove(session);

                    if (request.Length != 100)
                    {
                        ShowError(context, 400);
                        return;
                    }

#if !DEBUG
                    try
                    {
#endif
                    // this blob appears to share the same format with GenIV only with (obviously) a GenV string for the trainer name
                    // and the email-related fields dummied out.
                    // Specifically, email, notification status, and the two secrets appear to always be 0.
                        byte[] profileBinary = new byte[100];
                        Array.Copy(request, 0, profileBinary, 0, 100);
                        TrainerProfile5 profile = new TrainerProfile5(pid, profileBinary);
                        Database.Instance.GamestatsSetProfile5(profile);
#if !DEBUG
                    }
                    catch { }
#endif

                    response.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                        0, 8);
                    break;
                #endregion

                #region GTS
                // Called during startup. Unknown purpose.
                case "/syachi2ds/web/worldexchange/info.asp":
                    SessionManager.Remove(session);

                    // todo: find out the meaning of this request.
                    // is it simply done to check whether the GTS is online?
                    response.Write(new byte[] { 0x01, 0x00 }, 0, 2);
                    break;

                // Called during startup and when you check your pokemon's status.
                case "/syachi2ds/web/worldexchange/result.asp":
                {
                    SessionManager.Remove(session);

                    // todo: more fun stuff is contained in this blob on genV.
                    // my guess is that it's trainer profile info like setProfile.asp
                    // There's a long string of 0s which could be a trainer card signature raster

                    GtsRecord5 record = Database.Instance.GtsDataForUser5(pokedex, pid);

                    if (record == null)
                    {
                        // No pokemon in the system
                        response.Write(new byte[] { 0x05, 0x00 }, 0, 2);
                    }
                    else if (record.IsExchanged > 0)
                    {
                        // traded pokemon arriving!!!
                        response.Write(record.Save(), 0, 296);
                    }
                    else
                    {
                        // my existing pokemon is in the system, untraded
                        response.Write(new byte[] { 0x04, 0x00 }, 0, 2);
                    }

                } break;

                // Called after result.asp returns 4 when you check your pokemon's status
                case "/syachi2ds/web/worldexchange/get.asp":
                {
                    SessionManager.Remove(session);

                    // this is only called if result.asp returned 4.
                    // todo: what does this do if the contained pokemon is traded??
                    // todo: the same big blob of stuff from result.asp is sent here too.

                    GtsRecord5 record = Database.Instance.GtsDataForUser5(pokedex, pid);

                    if (record == null)
                    {
                        // No pokemon in the system
                        // what do here?
                        // todo: we should probably repeat the previous record
                        // that was in here before delete.asp was called.
                        // That is... if we still had it. -__-;
                        ShowError(context, 400);
                        return;
                    }
                    else
                    {
                        // just write the record whether traded or not...
                        response.Write(record.Save(), 0, 296);
                    }
                } break;

                // Called after result.asp returns an inbound pokemon record to delete it
                case "/syachi2ds/web/worldexchange/delete.asp":
                {
                    SessionManager.Remove(session);

                    // todo: the same big blob of stuff from result.asp is sent here too.

                    GtsRecord5 record = Database.Instance.GtsDataForUser5(pokedex, pid);
                    if (record == null)
                    {
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2);
                    }
                    else if (record.IsExchanged > 0)
                    {
                        // delete the arrived pokemon from the system
                        // todo: add transactions
                        // todo: log the successful trade?
                        // (either here or when the trade is done)
                        bool success = Database.Instance.GtsDeletePokemon5(pid);
                        if (success)
                            response.Write(new byte[] { 0x01, 0x00 }, 0, 2);
                        else
                            response.Write(new byte[] { 0x00, 0x00 }, 0, 2);
                    }
                    else
                    {
                        // own pokemon is there, fail. Use return.asp instead.
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2);
                    }
                } break;

                // called to delete your own pokemon after taking it back
                case "/syachi2ds/web/worldexchange/return.asp":
                {
                    SessionManager.Remove(session);

                    GtsRecord5 record = Database.Instance.GtsDataForUser5(pokedex, pid);
                    if (record == null)
                    {
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2);
                    }
                    else if (record.IsExchanged > 0)
                    {
                        // a traded pokemon is there, fail. Use delete.asp instead.
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2);
                    }
                    else
                    {
                        // delete own pokemon
                        // todo: add transactions
                        bool success = Database.Instance.GtsDeletePokemon5(pid);
                        if (success)
                        {
                            response.Write(new byte[] { 0x01, 0x00 }, 0, 2);
                            // todo: invalidate cache
                            //manager.RefreshStats();
                        }
                        else
                        {
                            response.Write(new byte[] { 0x00, 0x00 }, 0, 2);
                        }
                    }
                } break;

                // Called when you deposit a pokemon into the system.
                case "/syachi2ds/web/worldexchange/post.asp":
                {
                    if (request.Length != 432)
                    {
                        SessionManager.Remove(session);
                        ShowError(context, 400);
                        return;
                    }

                    // todo: add transaction
                    if (Database.Instance.GtsDataForUser5(pokedex, pid) != null)
                    {
                        // there's already a pokemon inside
                        // Force the player out so they'll recheck its status.
                        SessionManager.Remove(session);
                        response.Write(new byte[] { 0x0e, 0x00 }, 0, 2);
                        break;
                    }

                    // keep the record in memory while we wait for post_finish.asp request
                    byte[] recordBinary = new byte[296];
                    Array.Copy(request, 0, recordBinary, 0, 296);
                    GtsRecord5 record = new GtsRecord5(pokedex, recordBinary);
                    record.IsExchanged = 0;

                    // todo: figure out what bytes 296-431 do:
                    // appears to be 4 bytes of 00, 128 bytes of stuff, 4 bytes of 80 00 00 00
                    // probably a pkvldtprod signature

                    if (!record.Validate())
                    {
                        // hack check failed
                        SessionManager.Remove(session);

                        // responses:
                        // 0x00: bsod
                        // 0x01: successful deposit
                        // 0x02: Communication error 13265
                        // 0x03: Communication error 13264
                        // 0x04-0x06: bsod
                        // 0x07: The GTS is very crowded now. Please try again later (13261). (and it boots you)
                        // 0x08: That Pokémon may not be offered for trade (13268)!
                        // 0x09: That Pokémon may not be offered for trade (13269)!
                        // 0x0a: That Pokémon may not be offered for trade (13270)!
                        // 0x0b: That Pokémon may not be offered for trade (13271)!
                        // 0x0c: That Pokémon may not be offered for trade (13266)!
                        // 0x0d: That Pokémon may not be offered for trade (13267)!
                        // 0x0e: You were disconnected from the GTS. Error code: 13262 (and it boots you)
                        // 0x0f: bsod
                        response.Write(new byte[] { 0x0c, 0x00 }, 0, 2);
                        break;
                    }

                    // the following two fields are blank in the uploaded record.
                    // The server must provide them instead.
                    record.TimeDeposited = DateTime.UtcNow;
                    record.TimeExchanged = null;
                    record.PID = pid;

                    session.Tag = record;
                    // todo: delete any other post.asp sessions registered under this PID

                    response.Write(new byte[] { 0x01, 0x00 }, 0, 2);

                } break;

                case "/syachi2ds/web/worldexchange/post_finish.asp":
                {
                    SessionManager.Remove(session);

                    if (request.Length != 8)
                    {
                        ShowError(context, 400);
                        return;
                    }

                    // find a matching session which contains our record
                    GamestatsSession prevSession = SessionManager.FindSession(pid, "/syachi2ds/web/worldexchange/post.asp");
                    if (prevSession == null)
                    {
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2);
                        return;
                    }

                    SessionManager.Remove(prevSession);
                    if (prevSession.Tag == null)
                    {
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2);
                        return;
                    }
                    AssertHelper.Assert(prevSession.Tag is GtsRecord5);
                    GtsRecord5 record = (GtsRecord5)prevSession.Tag;

                    if (Database.Instance.GtsDepositPokemon5(record))
                    {
                        // todo: invalidate cache
                        //manager.RefreshStats();
                        response.Write(new byte[] { 0x01, 0x00 }, 0, 2);
                    }
                    else
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2);

                } break;

                // the search request has a funny bit string request of search terms
                // and just returns a chunk of records end to end.
                case "/syachi2ds/web/worldexchange/search.asp":
                {
                    SessionManager.Remove(session);

                    if (request.Length < 7 || request.Length > 8)
                    {
                        ShowError(context, 400);
                        return;
                    }

                    int resultsCount = (int)request[6];

                    ushort species = BitConverter.ToUInt16(request, 0);
                    if (species < 1)
                    {
                        ShowError(context, 400);
                        return;
                    }

                    response.Write(new byte[] { 0x01, 0x00 }, 0, 2);

                    if (resultsCount < 1) break; // optimize away requests for no rows

                    Genders gender = (Genders)request[2];
                    byte minLevel = request[3];
                    byte maxLevel = request[4];
                    // byte 5 unknown
                    byte country = 0;
                    if (request.Length > 7) country = request[7];

                    if (resultsCount > 7) resultsCount = 7; // stop DDOS
                    GtsRecord5[] records = Database.Instance.GtsSearch5(pokedex, pid, species, gender, minLevel, maxLevel, country, resultsCount);
                    foreach (GtsRecord5 record in records)
                    {
                        response.Write(record.Save(), 0, 296);
                    }

                    Database.Instance.GtsSetLastSearch5(pid);

                } break;

                // the exchange request uploads a record of the exchangee pokemon
                // plus the desired PID to trade for at the very end.
                case "/syachi2ds/web/worldexchange/exchange.asp":
                {
                    if (request.Length != 432)
                    {
                        SessionManager.Remove(session);
                        ShowError(context, 400);
                        return;
                    }

                    byte[] uploadData = new byte[296];
                    Array.Copy(request, 0, uploadData, 0, 296);
                    GtsRecord5 upload = new GtsRecord5(pokedex, uploadData);
                    upload.IsExchanged = 0;
                    int targetPid = BitConverter.ToInt32(request, 296);
                    GtsRecord5 result = Database.Instance.GtsDataForUser5(pokedex, targetPid);
                    DateTime ? searchTime = Database.Instance.GtsGetLastSearch5(pid);

                    if (result == null || searchTime == null ||
                        result.TimeDeposited > (DateTime)searchTime || // If this condition is met, it means the pokemon in the system is DIFFERENT from the one the user is trying to trade for, ie. it was deposited AFTER the user did their search. The one the user wants was either taken back or traded.
                        result.IsExchanged != 0)
                    {
                        // Pokémon is traded (or was never here to begin with)
                        SessionManager.Remove(session);
                        response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                        break;
                    }

                    // enforce request requirements server side
                    if (!upload.Validate() || !upload.CanTrade(result))
                    {
                        // todo: find the correct codes for these
                        SessionManager.Remove(session);

                        // responses:
                        // 0x00-0x01: bsod
                        // 0x02: Unfortunately, it was traded to another Trainer.
                        // 0x03-0x07: bsod
                        // 0x08: That Pokémon may not be offered for trade (13268)!
                        // 0x09: That Pokémon may not be offered for trade (13269)!
                        // 0x0a: That Pokémon may not be offered for trade (13270)!
                        // 0x0b: That Pokémon may not be offered for trade (13271)!
                        // 0x0c: That Pokémon may not be offered for trade (13266)!
                        // 0x0d: That Pokémon may not be offered for trade (13267)!
                        // 0x0e: You were disconnected from the GTS. Error code: 13262
                        // 0x0f: bsod
                        response.Write(new byte[] { 0x0c, 0x00 }, 0, 2);
                        return;
                    }

                    object[] tag = new GtsRecord5[2];
                    tag[0] = upload;
                    tag[1] = result;
                    session.Tag = tag;

                    GtsRecord5 tradedResult = result.Clone();
                    tradedResult.FlagTraded(upload); // only real purpose is to generate a proper response

                    // todo: we need a mechanism to "reserve" a pokemon being traded at this
                    // point in the process, but be able to relinquish it if exchange_finish
                    // never happens.
                    // Currently, if two people try to take the same pokemon, it will appear
                    // to work for both but then fail for the second after they've saved
                    // their game. This causes a hard crash and a "save file is corrupt, 
                    // "previous will be loaded" error when restarting.
                    // the reservation can be done in application state and has no reason
                    // to touch the database. (exchange_finish won't work anyway if application
                    // state is lost.)

                    response.Write(result.Save(), 0, 296);

                } break;

                case "/syachi2ds/web/worldexchange/exchange_finish.asp":
                {
                    SessionManager.Remove(session);

                    if (request.Length != 8)
                    {
                        ShowError(context, 400);
                        return;
                    }

                    // find a matching session which contains our record
                    GamestatsSession prevSession = SessionManager.FindSession(pid, "/syachi2ds/web/worldexchange/exchange.asp");
                    if (prevSession == null)
                    {
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2);
                        return;
                    }

                    SessionManager.Remove(prevSession);
                    if (prevSession.Tag == null)
                    {
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2);
                        return;
                    }
                    AssertHelper.Assert(prevSession.Tag is GtsRecord5[]);
                    GtsRecord5[] tag = (GtsRecord5[])prevSession.Tag;
                    AssertHelper.Assert(tag.Length == 2);

                    GtsRecord5 upload = (GtsRecord5)tag[0];
                    GtsRecord5 result = (GtsRecord5)tag[1];

                    if (Database.Instance.GtsTradePokemon5(upload, result, pid))
                        response.Write(new byte[] { 0x01, 0x00 }, 0, 2);
                    else
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2);

                } break;
                #endregion

                #region Battle Subway
                case "/syachi2ds/web/battletower/info.asp":
                    SessionManager.Remove(session);

                    // Probably an availability/status code.
                    // todo: See how the game reacts to various values.
                    response.Write(new byte[] { 0x01, 0x00 }, 0, 2);
                    break;

                case "/syachi2ds/web/battletower/roomnum.asp":
                    SessionManager.Remove(session);

                    //byte rank = data[0x00];
                    response.Write(new byte[] { 0x32, 0x00 }, 0, 2);
                    break;

                case "/syachi2ds/web/battletower/download.asp":
                {
                    SessionManager.Remove(session);

                    if (request.Length != 2)
                    {
                        ShowError(context, 400);
                        return;
                    }

                    byte rank = request[0];
                    byte roomNum = request[1];

                    if (rank > 9 || roomNum > 49)
                    {
                        ShowError(context, 400);
                        return;
                    }

                    BattleSubwayRecord5[] opponents = Database.Instance.BattleSubwayGetOpponents5(pid, rank, roomNum);
                    BattleSubwayProfile5[] leaders = Database.Instance.BattleSubwayGetLeaders5(rank, roomNum);

                    if (opponents.Length != 7)
                    {
                        // todo: Implement fake trainers on Gen5 too.
                        ShowError(context, 500);
                        return;
                    }

                    foreach (BattleSubwayRecord5 record in opponents)
                    {
                        response.Write(record.Save(), 0, 240);
                    }

                    foreach (BattleSubwayProfile5 leader in leaders)
                    {
                        response.Write(leader.Save(), 0, 34);
                    }

                } break;

                case "/syachi2ds/web/battletower/upload.asp":
                {
                    SessionManager.Remove(session);

                    if (request.Length != 388)
                    {
                        ShowError(context, 400);
                        return;
                    }

                    BattleSubwayRecord5 record = new BattleSubwayRecord5(request, 0);

                    record.Rank = request[0xf0];
                    record.RoomNum = request[0xf1];
                    record.BattlesWon = request[0xf2];
                    record.Unknown4 = new byte[5];
                    Array.Copy(request, 0xf3, record.Unknown4, 0, 5);
                    record.Unknown5 = BitConverter.ToUInt64(request, 0xf8);

                    // todo: Check pkvldtprod signature and/or revalidate

                    // todo: Do we want to store their record anyway if they lost the first round?
                    if (record.BattlesWon > 0)
                        Database.Instance.BattleSubwayUpdateRecord5(record);
                    if (record.BattlesWon == 7)
                        Database.Instance.BattleSubwayAddLeader5(record);

                    response.Write(new byte[] { 0x01, 0x00 }, 0, 2);

                } break;

                #endregion

            }
        }
    }
}
