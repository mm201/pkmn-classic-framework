using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using PkmnFoundations.Data;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;
using System.IO;

namespace PkmnFoundations.GTS
{
    /// <summary>
    /// Summary description for pokemondpds
    /// </summary>
    public class syachi2ds : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            int pid;

            if (context.Request.QueryString["pid"] == null ||
                !Int32.TryParse(context.Request.QueryString["pid"], out pid))
            {
                // pid missing or bad format
                Error400(context);
                return;
            }

            if (context.Request.QueryString.Count == 1)
            {
                // this is a new session request
                GtsSession5 session = new GtsSession5(pid, context.Request.PathInfo);
                GtsSessionManager manager = GtsSessionManager.FromContext(context);
                manager.Add(session);

                context.Response.Write(session.Token);
                return;
            }
            else if (context.Request.QueryString.Count == 3)
            {
                // this is a main request
                if (context.Request.QueryString["hash"] == null ||
                    context.Request.QueryString["data"] == null ||
                    context.Request.QueryString["data"].Length < 16)
                {
                    // arguments missing, partial check for data length.
                    // (Here, we require data to hold at least 10 bytes.
                    // In reality, it must hold at least 12, which is checked 
                    // for below after decoding)
                    Error400(context);
                    return;
                }

                GtsSessionManager manager = GtsSessionManager.FromContext(context);
                if (!manager.Sessions5.ContainsKey(context.Request.QueryString["hash"]))
                {
                    // session hash not matched
                    Error400(context);
                    return;
                }

                GtsSession5 session = manager.Sessions5[context.Request.QueryString["hash"]];
                byte[] data;
                try
                {
                    data = GtsSession5.DecryptData(context.Request.QueryString["data"]);
                    if (data.Length < 8)
                    {
                        // data too short to contain a pid and length.
                        // We check for 8 bytes, not 12, since the hash
                        // isn't included in DecryptData's result.
                        Error400(context);
                        return;
                    }
                }
                catch (FormatException)
                {
                    // data too short to contain a checksum,
                    // base64 format errors
                    Error400(context);
                    return;
                }

                int pid2 = BitConverter.ToInt32(data, 0);
                if (pid2 != pid)
                {
                    // packed pid doesn't match ?pid=
                    Error400(context);
                    return;
                }

                int length = BitConverter.ToInt32(data, 4);
                if (length + 8 != data.Length)
                {
                    // message length is incorrect
                    Error400(context);
                    return;
                }

                MemoryStream response = new MemoryStream();

                switch (session.URL)
                {
                    default:
                        manager.Remove(session);

                        // unrecognized page url
                        context.Response.StatusCode = 404;
                        context.Response.Write("This handler has not been implemented.\n");
                        context.Response.Write(session.URL);
                        return;

                    #region Common
                    // Called during startup. Seems to contain trainer profile stats.
                    case "/common/setProfile.asp":
                        manager.Remove(session);

                        if (data.Length != 108)
                        {
                            Error400(context);
                            return;
                        }

#if !DEBUG
                        try
                        {
#endif
                        // todo: Figure out what fun stuff is contained in this blob!
                        byte[] profileBinary = new byte[100];
                        Array.Copy(data, 8, profileBinary, 0, 100);
                        TrainerProfile5 profile = new TrainerProfile5(pid, profileBinary);
                        DataAbstract.Instance.GamestatsSetProfile5(profile);
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
                    case "/worldexchange/info.asp":
                        manager.Remove(session);

                        // todo: find out the meaning of this request.
                        // is it simply done to check whether the GTS is online?
                        response.Write(new byte[] { 0x01, 0x00 }, 0, 2);
                        break;

                    // Called during startup and when you check your pokemon's status.
                    case "/worldexchange/result.asp":
                    {
                        manager.Remove(session);

                        // todo: more fun stuff is contained in this blob on genV.
                        // my guess is that it's trainer profile info like setProfile.asp
                        // There's a long string of 0s which could be a trainer card signature raster

                        GtsRecord5 record = DataAbstract.Instance.GtsDataForUser5(pid);

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
                    case "/worldexchange/get.asp":
                    {
                        manager.Remove(session);

                        // this is only called if result.asp returned 4.
                        // todo: what does this do if the contained pokemon is traded??
                        // todo: the same big blob of stuff from result.asp is sent here too.

                        GtsRecord5 record = DataAbstract.Instance.GtsDataForUser5(pid);

                        if (record == null)
                        {
                            // No pokemon in the system
                            // what do here?
                        }
                        else
                        {
                            // just write the record whether traded or not...
                            response.Write(record.Save(), 0, 296);
                        }
                    } break;

                    // Called after result.asp returns an inbound pokemon record to delete it
                    case "/worldexchange/delete.asp":
                    {
                        manager.Remove(session);

                        // todo: the same big blob of stuff from result.asp is sent here too.

                        GtsRecord5 record = DataAbstract.Instance.GtsDataForUser5(pid);
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
                            bool success = DataAbstract.Instance.GtsDeletePokemon5(pid);
                            if (success)
                            {
#if !DEBUG
                                try
                                {
#endif
                                    DataAbstract.Instance.GtsLogTrade5(record, DateTime.UtcNow);
                                    manager.RefreshStats();
#if !DEBUG
                                }
                                catch { }
#endif
                                response.Write(new byte[] { 0x01, 0x00 }, 0, 2);
                            }
                            else
                            {
                                response.Write(new byte[] { 0x00, 0x00 }, 0, 2);
                            }
                        }
                        else
                        {
                            // own pokemon is there, fail. Use return.asp instead.
                            response.Write(new byte[] { 0x00, 0x00 }, 0, 2);
                        }
                    } break;

                    // called to delete your own pokemon after taking it back
                    case "/worldexchange/return.asp":
                    {
                        manager.Remove(session);

                        GtsRecord5 record = DataAbstract.Instance.GtsDataForUser5(pid);
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
                            bool success = DataAbstract.Instance.GtsDeletePokemon5(pid);
                            if (success)
                            {
                                response.Write(new byte[] { 0x01, 0x00 }, 0, 2);
                                manager.RefreshStats();
                            }
                            else
                            {
                                response.Write(new byte[] { 0x00, 0x00 }, 0, 2);
                            }
                        }
                    } break;

                    // Called when you deposit a pokemon into the system.
                    case "/worldexchange/post.asp":
                    {
                        if (data.Length != 440)
                        {
                            manager.Remove(session);
                            Error400(context);
                            return;
                        }

                        // todo: add transaction
                        if (DataAbstract.Instance.GtsDataForUser5(pid) != null)
                        {
                            // there's already a pokemon inside
                            manager.Remove(session);
                            response.Write(new byte[] { 0x00, 0x00 }, 0, 2);
                            break;
                        }

                        // keep the record in memory while we wait for post_finish.asp request
                        byte[] recordBinary = new byte[296];
                        Array.Copy(data, 8, recordBinary, 0, 296);
                        GtsRecord5 record = new GtsRecord5(recordBinary);

                        // todo: figure out what bytes 304-439 do:
                        // appears to be 4 bytes of 00, 128 bytes of stuff, 4 bytes of 80 00 00 00
                        // probably a pkvldtprod signature

                        if (!record.Validate())
                        {
                            // hack check failed
                            // todo: test that 0c 00 is the correct code for GenV
                            manager.Remove(session);
                            response.Write(new byte[] { 0x0c, 0x00 }, 0, 2);
                            break;
                        }

                        // the following two fields are blank in the uploaded record.
                        // The server must provide them instead.
                        record.TimeDeposited = DateTime.UtcNow;
                        record.TimeExchanged = null;
                        record.IsExchanged = 0;
                        record.PID = pid;

                        session.Tag = record;
                        // todo: delete any other post.asp sessions registered under this PID

                        response.Write(new byte[] { 0x01, 0x00 }, 0, 2);

                    } break;

                    case "/worldexchange/post_finish.asp":
                    {
                        manager.Remove(session);

                        if (data.Length != 16)
                        {
                            Error400(context);
                            return;
                        }

                        // find a matching session which contains our record
                        GtsSession5 prevSession = manager.FindSession5(pid, "/worldexchange/post.asp");
                        if (prevSession == null)
                        {
                            response.Write(new byte[] { 0x00, 0x00 }, 0, 2);
                            return;
                        }

                        manager.Remove(prevSession);
                        if (prevSession.Tag == null)
                        {
                            response.Write(new byte[] { 0x00, 0x00 }, 0, 2);
                            return;
                        }
                        AssertHelper.Assert(prevSession.Tag is GtsRecord5);
                        GtsRecord5 record = (GtsRecord5)prevSession.Tag;

                        if (DataAbstract.Instance.GtsDepositPokemon5(record))
                        {
                            manager.RefreshStats();
                            response.Write(new byte[] { 0x01, 0x00 }, 0, 2);
                        }
                        else
                            response.Write(new byte[] { 0x00, 0x00 }, 0, 2);

                    } break;

                    // the search request has a funny bit string request of search terms
                    // and just returns a chunk of records end to end.
                    case "/worldexchange/search.asp":
                    {
                        manager.Remove(session);

                        if (data.Length < 15 || data.Length > 16)
                        {
                            Error400(context);
                            return;
                        }

                        int resultsCount = (int)data[14];

                        ushort species = BitConverter.ToUInt16(data, 8);
                        if (species < 1)
                        {
                            Error400(context);
                            return;
                        }

                        response.Write(new byte[] { 0x01, 0x00 }, 0, 2);

                        if (resultsCount < 1) break; // optimize away requests for no rows

                        Genders gender = (Genders)data[10];
                        byte minLevel = data[11];
                        byte maxLevel = data[12];
                        // byte 13 unknown
                        byte country = 0;
                        if (data.Length > 15) country = data[15];

                        if (resultsCount > 7) resultsCount = 7; // stop DDOS
                        GtsRecord5[] records = DataAbstract.Instance.GtsSearch5(pid, species, gender, minLevel, maxLevel, country, resultsCount);
                        foreach (GtsRecord5 record in records)
                        {
                            response.Write(record.Save(), 0, 296);
                        }

                    } break;

                    // the exchange request uploads a record of the exchangee pokemon
                    // plus the desired PID to trade for at the very end.
                    case "/worldexchange/exchange.asp":
                    {
                        if (data.Length != 440)
                        {
                            manager.Remove(session);
                            Error400(context);
                            return;
                        }

                        byte[] uploadData = new byte[296];
                        Array.Copy(data, 8, uploadData, 0, 296);
                        GtsRecord5 upload = new GtsRecord5(uploadData);
                        upload.IsExchanged = 0;
                        int targetPid = BitConverter.ToInt32(data, 304);
                        GtsRecord5 result = DataAbstract.Instance.GtsDataForUser5(targetPid);

                        if (result == null || result.IsExchanged != 0)
                        {
                            // Pokémon is traded (or was never here to begin with)
                            manager.Remove(session);
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        // enforce request requirements server side
                        if (!upload.Validate() || !upload.CanTrade(result))
                        {
                            // todo: find the correct codes for these
                            manager.Remove(session);
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

                    case "/worldexchange/exchange_finish.asp":
                    {
                        manager.Remove(session);

                        if (data.Length != 16)
                        {
                            Error400(context);
                            return;
                        }

                        // find a matching session which contains our record
                        GtsSession5 prevSession = manager.FindSession5(pid, "/worldexchange/exchange.asp");
                        if (prevSession == null)
                        {
                            response.Write(new byte[] { 0x00, 0x00 }, 0, 2);
                            return;
                        }

                        manager.Remove(prevSession);
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

                        if (DataAbstract.Instance.GtsTradePokemon5(upload, result))
                        {
#if !DEBUG
                            try
                            {
#endif
                                DataAbstract.Instance.GtsLogTrade5(result, null);
                                manager.RefreshStats();
#if !DEBUG
                            }
                            catch { }
#endif
                            response.Write(new byte[] { 0x01, 0x00 }, 0, 2);
                        }
                        else
                            response.Write(new byte[] { 0x00, 0x00 }, 0, 2);

                    } break;
                    #endregion
                }

                response.Flush();
                byte[] responseArray = response.ToArray();
                response.Dispose();
                response = null;

                // write GenV response checksum
                context.Response.OutputStream.Write(responseArray, 0, responseArray.Length);
                context.Response.Write(GtsSession5.ResponseChecksum(responseArray));
            }
            else
            // wrong number of querystring arguments
                Error400(context);
        }

        private void Error400(HttpContext context)
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Bad request");
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}