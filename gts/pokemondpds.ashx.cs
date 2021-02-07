using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using PkmnFoundations.Data;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;
using System.IO;
using System.Threading;
using GamestatsBase;

namespace PkmnFoundations.GTS
{
    /// <summary>
    /// Summary description for pokemondpds
    /// </summary>
    public class pokemondpds : GamestatsHandler
    {
        public pokemondpds()
            : base("sAdeqWo3voLeC5r16DYv", 
            0x45, 0x1111, 0x80000000, 0x4a3b2c1d, "pokemondpds", 
            GamestatsRequestVersions.Version2, GamestatsResponseVersions.Version1, true, true)
        {

        }

        public override void ProcessGamestatsRequest(byte[] data, MemoryStream response, string url, int pid, HttpContext context, GamestatsSession session)
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
                case "/pokemondpds/common/setProfile.asp":
                {
                    SessionManager.Remove(session);

                    if (data.Length != 100)
                    {
                        ShowError(context, 400);
                        return;
                    }

#if !DEBUG
                    try
                    {
#endif
                        byte[] profileBinary = new byte[100];
                        Array.Copy(data, 0, profileBinary, 0, 100);
                        TrainerProfile4 profile = new TrainerProfile4(pid, profileBinary);
                        Database.Instance.GamestatsSetProfile4(profile);
#if !DEBUG
                    }
                    catch { }
#endif
                        short clientSecret = BitConverter.ToInt16(data, 96);
                        short mailSecret = BitConverter.ToInt16(data, 98);

                        // response:
                        // 4 bytes of response code A
                        // 4 bytes of response code B
                        // Response code A values:
                        // 0: Continues normally.
                        // 1: The data was corrupted. It could not be sent.
                        // 2: The server is undergoing maintenance. Please connect again later.
                        // 3: BSOD
                        if (mailSecret == -1)
                        {
                            // Register wii mail
                            // Response code B values:
                            // 0: There was a communication error.
                            // 1: The Registration Code has been sent to your Wii console. Please enter the Registration Code.
                            // 2: There was an error while attempting to send an authentication Wii message.
                            // 3: There was a communication error.
                            // 4: BSOD
                            response.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00 },
                                0, 8);
                        }
                        else if (mailSecret != 0 || clientSecret != 0)
                        {
                            // Send wii mail confirmation code OR GTS when mail is configured (we can't tell them apart T__T)
                            // (todo: We could use database to tell them apart.
                            // If the previously stored profile has mailSecret == -1 then this is a wii mail confirmation.
                            // If the previously stored profile has mailSecret == this mailSecret then this is GTS.)
                            // Response code B values:
                            // 0: Your Wii Number has been registered.
                            // 1: There was a communication error.
                            // 2: There was a communication error.
                            // 3: Incorrect Registration Code.
                            // 4: BSOD
                            response.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                                0, 8);
                        }
                        else
                        {
                            // GTS
                            // Response code B values:
                            // 0: Continues normally
                            // 1: There was a communication error.
                            // 2: There was a communication error.
                            // 3: There was a Wii message authentication error.
                            // 4: BSOD
                            response.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                                0, 8);
                        }
                    }
                    break;
                #endregion

                #region GTS
                // Called during startup. Unknown purpose.
                case "/pokemondpds/worldexchange/info.asp":
                    SessionManager.Remove(session);

                    // todo: find out the meaning of this request.
                    // is it simply done to check whether the GTS is online?

                    Database.Instance.GamestatsBumpProfile4(pid);
                    response.Write(new byte[] { 0x01, 0x00 }, 0, 2);
                    break;

                // Called during startup and when you check your pokemon's status.
                case "/pokemondpds/worldexchange/result.asp":
                {
                    SessionManager.Remove(session);

                    /* After the above step(s) or performing any of 
                    * the tasks below other than searching, the game 
                    * makes a request to /pokemondpds/worldexchange/result.asp.
                    * If the game has had a Pokémon sent to it via a trade, 
                    * the server responds with the entire encrypted Pokémon 
                    * save struct. Otherwise, if there is a Pokémon deposited 
                    * in the GTS, it responds with 0x0004; if not, it responds 
                    * with 0x0005. */

                        GtsRecord4 record = Database.Instance.GtsDataForUser4(pokedex, pid);

                    if (record == null)
                    {
                        // No pokemon in the system
                        response.Write(new byte[] { 0x05, 0x00 }, 0, 2);
                    }
                    else if (record.IsExchanged > 0)
                    {
                        // traded pokemon arriving!!!
                        response.Write(record.Save(), 0, 292);
                    }
                    else
                    {
                        // my existing pokemon is in the system, untraded
                        response.Write(new byte[] { 0x04, 0x00 }, 0, 2);
                    }

                    // other responses:
                    // 0-2 causes a BSOD but it flashes siezure. Scary
                    // 3 causes it to be "checking GTS's status" forever.
                    // 6 is also the flashy BSOD. So probably all invalid values do that.

                } break;

                // Called after result.asp returns 4 when you check your pokemon's status
                case "/pokemondpds/worldexchange/get.asp":
                {
                    SessionManager.Remove(session);

                    // this is only called if result.asp returned 4.
                    // todo: what does this do if the contained pokemon is traded??

                    GtsRecord4 record = Database.Instance.GtsDataForUser4(pokedex, pid);

                    if (record == null)
                    {
                        // No pokemon in the system
                        // what do here?
                        ShowError(context, 400);
                        return;
                    }
                    else
                    {
                        // just write the record whether traded or not...
                        response.Write(record.Save(), 0, 292);
                    }
                } break;

                // Called after result.asp returns an inbound pokemon record to delete it
                case "/pokemondpds/worldexchange/delete.asp":
                {
                    SessionManager.Remove(session);

                    GtsRecord4 record = Database.Instance.GtsDataForUser4(pokedex, pid);
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
                        bool success = Database.Instance.GtsDeletePokemon4(pid);
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
                case "/pokemondpds/worldexchange/return.asp":
                {
                    SessionManager.Remove(session);

                    GtsRecord4 record = Database.Instance.GtsDataForUser4(pokedex, pid);
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
                        bool success = Database.Instance.GtsDeletePokemon4(pid);
                        if (success)
                        {
                            response.Write(new byte[] { 0x01, 0x00 }, 0, 2);
                        }
                        else
                        {
                            response.Write(new byte[] { 0x00, 0x00 }, 0, 2);
                        }
                    }
                } break;

                // Called when you deposit a pokemon into the system.
                case "/pokemondpds/worldexchange/post.asp":
                {
                    if (data.Length != 292)
                    {
                        SessionManager.Remove(session);
                        ShowError(context, 400);
                        return;
                    }

                    // todo: add transaction
                    if (Database.Instance.GtsDataForUser4(pokedex, pid) != null)
                    {
                        // there's already a pokemon inside.
                        // Force the player out so they'll recheck its status.
                        SessionManager.Remove(session);
                        response.Write(new byte[] { 0x0e, 0x00 }, 0, 2);
                        break;
                    }

                    // keep the record in memory while we wait for post_finish.asp request
                    byte[] recordBinary = new byte[292];
                    Array.Copy(data, 0, recordBinary, 0, 292);
                    GtsRecord4 record = new GtsRecord4(pokedex, recordBinary);
                    record.IsExchanged = 0;
                    if (!record.Validate())
                    {
                        // hack check failed
                        SessionManager.Remove(session);

                        // responses:
                        // 0x00: Appears to start depositing? todo: test if this code leads to a normal deposit.
                        // 0x01: successful deposit
                        // 0x02-0x03: Communication error...
                        // 0x04-0x06: bsod
                        // 0x07: The GTS is very crowded now. Please try again later. (and it boots you!)
                        // 0x08-0x0d: That Pokémon may not be offered for trade!
                        // 0x0e: You were disconnected from the GTS. Returning to the reception counter.
                        // 0x0f: Blue screen of death
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

                case "/pokemondpds/worldexchange/post_finish.asp":
                {
                    SessionManager.Remove(session);

                    if (data.Length != 8)
                    {
                        ShowError(context, 400);
                        return;
                    }

                    // todo: these _finish requests seem to come with a magic number of 4 bytes
                    // at offset 0. Find out what this is supposed to do and how to validate it.

                    // find a matching session which contains our record
                    GamestatsSession prevSession = SessionManager.FindSession(pid, "/pokemondpds/worldexchange/post.asp");
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
                    AssertHelper.Assert(prevSession.Tag is GtsRecord4);
                    GtsRecord4 record = (GtsRecord4)prevSession.Tag;

                    if (Database.Instance.GtsDepositPokemon4(record))
                    {
                        response.Write(new byte[] { 0x01, 0x00 }, 0, 2);
                    }
                    else
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2);

                } break;

                // the search request has a funny bit string request of search terms
                // and just returns a chunk of records end to end.
                case "/pokemondpds/worldexchange/search.asp":
                {
                    SessionManager.Remove(session);

                    if (data.Length < 7 || data.Length > 8)
                    {
                        ShowError(context, 400);
                        return;
                    }

                    ushort species = BitConverter.ToUInt16(data, 0);
                    if (species < 1)
                    {
                        ShowError(context, 400);
                        return;
                    }

                    int resultsCount = (int)data[6];
                    if (resultsCount < 1) break; // optimize away requests for no rows

                    Genders gender = (Genders)data[2];
                    byte minLevel = data[3];
                    byte maxLevel = data[4];
                    // byte 5 unknown
                    byte country = 0;
                    if (data.Length > 7) country = data[7];

                    if (resultsCount > 7) resultsCount = 7; // stop DDOS
                    GtsRecord4[] records = Database.Instance.GtsSearch4(pokedex, pid, species, gender, minLevel, maxLevel, country, resultsCount);
                    foreach (GtsRecord4 record in records)
                    {
                        response.Write(record.Save(), 0, 292);
                    }

                    Database.Instance.GtsSetLastSearch4(pid);

                } break;

                // the exchange request uploads a record of the exchangee pokemon
                // plus the desired PID to trade for at the very end.
                case "/pokemondpds/worldexchange/exchange.asp":
                {
                    if (data.Length != 296)
                    {
                        SessionManager.Remove(session);
                        ShowError(context, 400);
                        return;
                    }

                    byte[] uploadData = new byte[292];
                    Array.Copy(data, 0, uploadData, 0, 292);
                    GtsRecord4 upload = new GtsRecord4(pokedex, uploadData);
                    upload.IsExchanged = 0;
                    int targetPid = BitConverter.ToInt32(data, 292);
                    GtsRecord4 result = Database.Instance.GtsDataForUser4(pokedex, targetPid);
                    DateTime ? searchTime = Database.Instance.GtsGetLastSearch4(pid);

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
                        // 0x08-0x0d: That Pokémon may not be offered for trade!
                        // 0x0e: You were disconnected from the GTS. Returning to the reception counter.
                        // 0x0f: bsod
                        response.Write(new byte[] { 0x0c, 0x00 }, 0, 2);
                        return;
                    }

                    object[] tag = new GtsRecord4[2];
                    tag[0] = upload;
                    tag[1] = result;
                    session.Tag = tag;

                    GtsRecord4 tradedResult = result.Clone();
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

                    // I also have a hunch that failure to send the exchange_finish request
                    // is what causes the notorious GTS glitch where a pokemon is listed
                    // under the wrong species and you can't trade it

                    response.Write(result.Save(), 0, 292);

                } break;

                case "/pokemondpds/worldexchange/exchange_finish.asp":
                {
                    SessionManager.Remove(session);

                    if (data.Length != 8)
                    {
                        ShowError(context, 400);
                        return;
                    }

                    // find a matching session which contains our record
                    GamestatsSession prevSession = SessionManager.FindSession(pid, "/pokemondpds/worldexchange/exchange.asp");
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
                    AssertHelper.Assert(prevSession.Tag is GtsRecord4[]);
                    GtsRecord4[] tag = (GtsRecord4[])prevSession.Tag;
                    AssertHelper.Assert(tag.Length == 2);
                    AssertHelper.Assert(tag[0] is GtsRecord4);
                    AssertHelper.Assert(tag[0] is GtsRecord4);

                    GtsRecord4 upload = (GtsRecord4)tag[0];
                    GtsRecord4 result = (GtsRecord4)tag[1];

                    if (Database.Instance.GtsTradePokemon4(upload, result, pid))
                        response.Write(new byte[] { 0x01, 0x00 }, 0, 2);
                    else
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2);

                } break;
                #endregion

                #region Battle Tower
                case "/pokemondpds/battletower/info.asp":
                    SessionManager.Remove(session);

                    // Probably an availability/status code.
                    // todo: See how the game reacts to various values.
                    Database.Instance.GamestatsBumpProfile4(pid);
                    response.Write(new byte[] { 0x01, 0x00 }, 0, 2);
                    break;

                case "/pokemondpds/battletower/roomnum.asp":
                    SessionManager.Remove(session);

                    //byte rank = data[0x00];
                    response.Write(new byte[] { 0x32, 0x00 }, 0, 2);
                    break;

                case "/pokemondpds/battletower/download.asp":
                {
                    SessionManager.Remove(session);

                    if (data.Length != 2)
                    {
                        ShowError(context, 400);
                        return;
                    }

                    byte rank = data[0x00];
                    byte roomNum = data[0x01];

                    if (rank > 9 || roomNum > 49)
                    {
                        ShowError(context, 400);
                        return;
                    }

                    BattleTowerRecord4[] opponents = Database.Instance.BattleTowerGetOpponents4(pid, rank, roomNum);
                    BattleTowerProfile4[] leaders = Database.Instance.BattleTowerGetLeaders4(rank, roomNum);
                    BattleTowerRecord4[] fakeOpponents = FakeOpponentGenerator4.GenerateFakeOpponents(7 - opponents.Length);

                    foreach (BattleTowerRecord4 record in fakeOpponents)
                    {
                        response.Write(record.Save(), 0, 228);
                    }

                    foreach (BattleTowerRecord4 record in opponents)
                    {
                        response.Write(record.Save(), 0, 228);
                    }

                    foreach (BattleTowerProfile4 leader in leaders)
                    {
                        response.Write(leader.Save(), 0, 34);
                    }

                    // This is completely insane. The game crashes when you
                    // use Check Leaders if the response arrives too fast,
                    // so we artificially delay it.
                    // todo: This is slower than it needs to be if the
                    // database is slow to respond. We should sleep for a
                    // variable time based on when the request was received.
                    Thread.Sleep(500);

                } break;

                case "/pokemondpds/battletower/upload.asp":
                {
                    SessionManager.Remove(session);

                    if (data.Length != 239)
                    {
                        ShowError(context, 400);
                        return;
                    }

                    BattleTowerRecord4 record = new BattleTowerRecord4(data, 0);

                    record.Rank = data[0xe4];
                    record.RoomNum = data[0xe5];
                    record.BattlesWon = data[0xe6];
                    record.Unknown5 = BitConverter.ToUInt64(data, 0xe7);

                    // todo: Do we want to store their record anyway if they lost the first round?
                    if (record.BattlesWon > 0)
                        Database.Instance.BattleTowerUpdateRecord4(record);
                    if (record.BattlesWon == 7)
                        Database.Instance.BattleTowerAddLeader4(record);

                    response.Write(new byte[] { 0x01, 0x00 }, 0, 2);

                } break;

                #endregion
            }
        }
    }
}
