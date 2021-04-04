using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PkmnFoundations.Support;

namespace PkmnFoundations.GTS
{
    /// <summary>
    /// Summary description for pgl
    /// </summary>
    public class pgl : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.StatusCode = 502;// 200;
            return;

            // error messages by status code:
            // 403, 404, 500: Comm error (13209)
            // 502: The server is undergoing maintenance (13212)
            // 503: The server is experiencing high traffic volumes (13211)

            var qs = context.Request.QueryString;

            if (context.Request.HttpMethod == "GET")
            {
                switch (qs["p"])
                {
                    case "account.playstatus":

                        context.Response.OutputStream.WriteBytes(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                        for (int i = 0; i < 0x7c; i++)
                        {
                            context.Response.OutputStream.WriteByte(0x00);
                        }

                        //# The command codes only work if is sleeping.
					    // Command codes:
					    // \x00 - Wake up normally
					    // \x01 - "This Pokemon is not dreaming yet."
					    // \x02 - "This Pokemon is dreaming."
					    // \x03 - Wake up + download new changes from server
					    // \x04 - Wake up normally?
					    // \x05 - ? (Does not work with sleeping pokemon)
					    // \x08 - Leads to account.create.upload

                        context.Response.OutputStream.WriteBytes(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                        for (int i = 0; i < 0x40; i++)
                        {
                            context.Response.OutputStream.WriteByte(0x00);
                        }

                        break;


                    case "sleepily.bitlist":

                        context.Response.OutputStream.WriteBytes(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                        for (int i = 0; i < 0x7c; i++)
                        {
                            context.Response.OutputStream.WriteByte(0x00);
                        }
                        for (int i = 0; i < 0x80; i++)
                        {
                            context.Response.OutputStream.WriteByte(0xff);
                        }

                        break;

                    case "savedata.getbw":
                        context.Response.StatusCode = 502;
                        //context.Response.OutputStream.WriteBytes(new byte[] { 0x05, 0x00, 0x00, 0x00 });
                        break;


                    case "savedata.download":
                        context.Response.OutputStream.WriteBytes(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                        break;

                    case "worldbattle.download":
                        context.Response.StatusCode = 502;
                        break;

                    default:
                        context.Response.StatusCode = 502;
                        break;
                }
            }
            else if (context.Request.HttpMethod == "POST")
            {
                switch (qs["p"])
                {
                    case "account.createdata":
                        context.Response.OutputStream.WriteBytes(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                        break;

                    case "account.create.upload":
                        context.Response.OutputStream.WriteBytes(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                        break;

                    case "savedata.upload":
                        context.Response.OutputStream.WriteBytes(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                        break;

                    case "worldbattle.upload":
                        context.Response.OutputStream.WriteBytes(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                        break;

                    case "savedata.download.finish":
                        context.Response.OutputStream.WriteBytes(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                        break;

                    default:
                        context.Response.StatusCode = 502;
                        break;
                }
            }
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
