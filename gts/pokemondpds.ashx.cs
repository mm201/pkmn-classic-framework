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
    public class pokemondpds : IHttpHandler, IRequiresSessionState
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
                GtsSession4 session = new GtsSession4(pid, context.Request.PathInfo);
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
                    context.Request.QueryString["data"].Length < 12)
                {
                    // arguments missing, partial check for data length.
                    // (Here, we require data to hold at least 7 bytes.
                    // In reality, it must hold at least 8, which is checked 
                    // for below after decoding)
                    Error400(context);
                    return;
                }

                GtsSessionManager manager = GtsSessionManager.FromContext(context);
                if (!manager.Sessions4.ContainsKey(context.Request.QueryString["hash"]))
                {
                    // session hash not matched
                    Error400(context);
                    return;
                }

                GtsSession4 session = manager.Sessions4[context.Request.QueryString["hash"]];
                byte[] data;
                try
                {
                    data = GtsSession4.DecryptData(context.Request.QueryString["data"]);
                    if (data.Length < 4)
                    {
                        // data too short to contain a pid
                        // We check for 4 bytes, not 8, since the decrypt seed
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

                Stream response = context.Response.OutputStream;

                switch (session.URL)
                {
                    default:
                        manager.Remove(session);

                        // unrecognized page url
                        // should be error 404 once we're done debugging
                        context.Response.Write("Almost there. Your path is:\n");
                        context.Response.Write(session.URL);
                        return;

                    // Called during startup. Unknown purpose.
                    case "/worldexchange/info.asp":
                        manager.Remove(session);

                        // todo: find out the meaning of this request.
                        // is it simply done to check whether the GTS is online?
                        response.Write(new byte[] { 0x01, 0x00 }, 0, 2);
                        break;

                    // Called during startup. Seems to contain trainer profile stats.
                    case "/common/setProfile.asp":
                        manager.Remove(session);

                        // todo: Figure out what fun stuff is contained in this blob!

                        response.Write(new byte[] 
                            { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 
                            0, 8);
                        break;

                    // Called during startup and when you check your pokemon's status.
                    case "/worldexchange/result.asp":
                    {
                        manager.Remove(session);

                        /* After the above step(s) or performing any of 
                            * the tasks below other than searching, the game 
                            * makes a request to /pokemondpds/worldexchange/result.asp. 
                            * If the game has had a Pokémon sent to it via a trade, 
                            * the server responds with the entire encrypted Pokémon 
                            * save struct. Otherwise, if there is a Pokémon deposited 
                            * in the GTS, it responds with 0x0004; if not, it responds 
                            * with 0x0005. */

                        GtsRecord4 record = DataAbstract.Instance.GtsDataForUser4(pid);

                        if (record == null)
                        {
                            // No pokemon in the system
                            response.Write(new byte[]
                                { 0x05, 0x00 }, 0, 2);
                        }
                        else if (record.IsExchanged > 0)
                        {
                            // traded pokemon arriving!!!
                            response.Write(record.Save(), 0, 292);
                        }
                        else
                        {
                            // my existing pokemon is in the system, untraded
                            response.Write(new byte[]
                                { 0x04, 0x00 }, 0, 2);
                        }

                    } break;

                    // Called after result.asp returns 4 when you check your pokemon's status
                    case "/worldexchange/get.asp":
                    {
                        manager.Remove(session);

                        // this is only called if result.asp returned 4.
                        // todo: what does this do if the contained pokemon is traded??

                        GtsRecord4 record = DataAbstract.Instance.GtsDataForUser4(pid);

                        if (record == null)
                        {
                            // No pokemon in the system
                            // what do here?
                        }
                        else
                        {
                            // just write the record whether traded or not...
                            response.Write(record.Save(), 0, 292);
                        }
                    } break;

                    // Called after result.asp returns an inbound pokemon record to delete it
                    case "/worldexchange/delete.asp":
                    {
                        manager.Remove(session);

                        GtsRecord4 record = DataAbstract.Instance.GtsDataForUser4(pid);
                        if (record == null)
                        {
                            response.Write(new byte[] 
                                { 0x00, 0x00 }, 0, 2);
                        }
                        else if (record.IsExchanged > 0)
                        {
                            // delete the arrived pokemon from the system
                            // todo: add transactions
                            // todo: log the successful trade?
                            // (either here or when the trade is done)
                            bool success = DataAbstract.Instance.GtsDeletePokemon4(pid);
                            if (success)
                            {
                                manager.RefreshStats();
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

                        GtsRecord4 record = DataAbstract.Instance.GtsDataForUser4(pid);
                        if (record == null)
                        {
                            response.Write(new byte[] 
                                { 0x00, 0x00 }, 0, 2);
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
                            bool success = DataAbstract.Instance.GtsDeletePokemon4(pid);
                            if (success)
                            {
                                manager.RefreshStats();
                                response.Write(new byte[] { 0x01, 0x00 }, 0, 2);
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
                        if (data.Length != 296)
                        {
                            manager.Remove(session);
                            Error400(context);
                            return;
                        }

                        // todo: add transaction
                        if (DataAbstract.Instance.GtsDataForUser4(pid) != null)
                        {
                            // there's already a pokemon inside
                            manager.Remove(session);
                            response.Write(new byte[] { 0x00, 0x00 }, 0, 2);
                            break;
                        }

                        // keep the record in memory while we wait for post_finish.asp request
                        byte[] recordBinary = new byte[292];
                        Array.Copy(data, 4, recordBinary, 0, 292);
                        GtsRecord4 record = new GtsRecord4(recordBinary);
                        if (!record.Validate())
                        {
                            // hack check failed
                            manager.Remove(session);
                            response.Write(new byte[] { 0x0c, 0x00 }, 0, 2);
                            break;
                        }

                        // the following two fields are blank in the uploaded record.
                        // The server must provide them instead.
                        record.TimeDeposited = DateTime.UtcNow;
                        record.TimeWithdrawn = null;
                        record.IsExchanged = 0;
                        record.PID = pid;

                        session.Tag = record;
                        // todo: delete any other post.asp sessions registered under this PID

                        response.Write(new byte[] { 0x01, 0x00 }, 0, 2);

                    } break;

                    case "/worldexchange/post_finish.asp":
                    {
                        manager.Remove(session);

                        if (data.Length != 12)
                        {
                            Error400(context);
                            return;
                        }

                        // todo: these _finish requests seem to come with a magic number of 4 bytes
                        // at offset 0. Find out what this is supposed to do and how to validate it.

                        // find a matching session which contains our record
                        GtsSession4 prevSession = manager.FindSession4(pid, "/worldexchange/post.asp");
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
                        AssertHelper.Assert(prevSession.Tag is GtsRecord4);
                        GtsRecord4 record = (GtsRecord4)prevSession.Tag;

                        if (DataAbstract.Instance.GtsDepositPokemon4(record))
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

                        if (data.Length < 11 || data.Length > 12)
                        {
                            Error400(context);
                            return;
                        }

                        ushort species = BitConverter.ToUInt16(data, 4);
                        if (species < 1)
                        {
                            Error400(context);
                            return;
                        }

                        int resultsCount = (int)data[10];
                        if (resultsCount < 1) break; // optimize away requests for no rows

                        Genders gender = (Genders)data[6];
                        byte minLevel = data[7];
                        byte maxLevel = data[8];
                        // byte 9 unknown
                        byte country = 0;
                        if (data.Length > 11) country = data[11];

                        if (resultsCount > 7) resultsCount = 7; // stop DDOS
                        GtsRecord4[] records = DataAbstract.Instance.GtsSearch4(pid, species, gender, minLevel, maxLevel, country, resultsCount);
                        foreach (GtsRecord4 record in records)
                        {
                            response.Write(record.Save(), 0, 292);
                        }

                    } break;

                    // the exchange request uploads a record of the exchangee pokemon
                    // plus the desired PID to trade for at the very end.
                    case "/worldexchange/exchange.asp":
                    {
                        if (data.Length != 300)
                        {
                            manager.Remove(session);
                            Error400(context);
                            return;
                        }

                        byte[] uploadData = new byte[292];
                        Array.Copy(data, 4, uploadData, 0, 292);
                        GtsRecord4 upload = new GtsRecord4(uploadData);
                        int targetPid = BitConverter.ToInt32(data, 296);
                        GtsRecord4 result = DataAbstract.Instance.GtsDataForUser4(targetPid);

                        if (result == null || result.IsExchanged != 0)
                        {
                            // Pokémon is traded (or was never here to begin with)
                            // todo: I only checked this on GenV. Check that this
                            // is the correct response on GenIV.
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

                    case "/worldexchange/exchange_finish.asp":
                    {
                        manager.Remove(session);

                        if (data.Length != 12)
                        {
                            Error400(context);
                            return;
                        }

                        // find a matching session which contains our record
                        GtsSession4 prevSession = manager.FindSession4(pid, "/worldexchange/exchange.asp");
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
                        AssertHelper.Assert(prevSession.Tag is GtsRecord4[]);
                        GtsRecord4[] tag = (GtsRecord4[])prevSession.Tag;
                        AssertHelper.Assert(tag.Length == 2);
                        AssertHelper.Assert(tag[0] is GtsRecord4);
                        AssertHelper.Assert(tag[0] is GtsRecord4);

                        GtsRecord4 upload = (GtsRecord4)tag[0];
                        GtsRecord4 result = (GtsRecord4)tag[1];

                        if (DataAbstract.Instance.GtsTradePokemon4(upload, result))
                        {
                            manager.RefreshStats();
                            response.Write(new byte[] { 0x01, 0x00 }, 0, 2);
                        }
                        else
                            response.Write(new byte[] { 0x00, 0x00 }, 0, 2);

                    } break;
                }
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