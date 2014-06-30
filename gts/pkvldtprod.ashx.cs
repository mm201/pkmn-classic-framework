using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using PkmnFoundations.Support;
using System.Text;

namespace PkmnFoundations.GTS
{
    /// <summary>
    /// Summary description for pkvldtprod
    /// </summary>
    public class pkvldtprod : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            byte[] requestData = new byte[(int)context.Request.InputStream.Length];
            context.Request.InputStream.ReadBlock(requestData, 0, (int)context.Request.InputStream.Length);

            // this is a mysterious token of unknown purpose. It seems to vary with the
            // type of request being done.
            // On GTS requests, it's 83 characters long and begins with NDS.
            // In Random Matchup, it looks more like a base64 string, is 88 chars long
            // and encodes 64 bytes of random looking data.
            int tokenLength = Array.IndexOf<byte>(requestData, 0x00);
            String token = StringHelper.BytesToString(requestData, 0, tokenLength, Encoding.UTF8);
            int offset = tokenLength + 1;
            
            RequestType type = (RequestType)BitConverter.ToInt16(requestData, offset);
            offset += 2;

            int pkmCount;
            switch (type)
            {
                case RequestType.RandomMatchup:
                case RequestType.GTS:
                    {
                        pkmCount = (requestData.Length - offset) / 220;
                    } break;
                case RequestType.BattleSubway:
                    {
                        pkmCount = 6;
                        // todo: Need more info on this structure
                    } break;
                default:
                    {
                        // Don't understand this request. Give it a response containing
                        // all 00s so stuff depending on it won't break.
                        // The game accepts a hash of all 00s and doesn't care what's contained
                        // in the response beyond its expected length so this will work.
                        // fixme: Once we start generating real signatures, they will prevent
                        // this from returning the necessary number of 00s in all cases.
                        pkmCount = 6;
                    } break;
            }

            context.Response.ContentType = "text/plain";
            context.Response.OutputStream.WriteByte((byte)PartyValidationResult.Valid); // success
            for (int x = 0; x < pkmCount; x++)
            {
                context.Response.OutputStream.Write(BitConverter.GetBytes((int)PokemonValidationResult.Valid), 0, 4);
            }
            // placeholder for signature.
            // Should be 128 bytes of an unknown hashing/signing algorithm
            context.Response.OutputStream.Write(new byte[128], 0, 128);
        }

        private enum PartyValidationResult : byte
        {
            Valid = 0x00,
            Invalid = 0x01
        }

        private enum PokemonValidationResult : int
        {
            Valid = 0x00000000,
            Invalid = 0x3c000000
        }

        private enum RequestType : short
        {
            RandomMatchup = 0x0000,
            GTS = 0x0100,
            BattleSubway = 0x0400
        }

        private void Error400(HttpContext context)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "text/plain";
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