using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using PokeFoundations.Data;

namespace PokeFoundations.GTS
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
                Dictionary<String, GtsSession4> sessions = context.AllSessions4();
                sessions.Add(session.Hash, session);

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

                Dictionary<String, GtsSession4> sessions = context.AllSessions4();
                if (!sessions.ContainsKey(context.Request.QueryString["hash"]))
                {
                    // session hash not matched
                    Error400(context);
                    return;
                }

                GtsSession4 session = sessions[context.Request.QueryString["hash"]];
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

                switch (session.URL)
                {
                    default:
                        // unrecognized page url
                        // should be error 404 once we're done debugging
                        context.Response.Write("Almost there. Your path is:\n");
                        context.Response.Write(session.URL);
                        return;


                    case "/worldexchange/info.asp":
                        context.Response.OutputStream.Write(new byte[] { 0x01, 0x00 }, 0, 2);
                        break;


                    case "/common/setProfile.asp":
                        // todo: Figure out what fun stuff is contained in this blob!

                        context.Response.OutputStream.Write(new byte[] 
                            { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 
                            0, 8);
                        break;


                    case "/worldexchange/result.asp":
                        {
                            

                        }
                        break;


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
                return false;
            }
        }
    }
}