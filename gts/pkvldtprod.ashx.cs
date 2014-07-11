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

            // this is a mysterious token of unknown purpose. It seems to vary
            // with the type of request being done.
            // On GTS requests, it's 83 characters long and begins with NDS.
            // In Random Matchup, it looks more like a base64 string, is 88
            // chars long and encodes 64 bytes of random looking data.
            // It is null terminated (variable length), followed immediately 
            // by the rest of the message.
            int tokenLength = Array.IndexOf<byte>(requestData, 0x00);
            String token = StringHelper.BytesToString(requestData, 0, tokenLength, Encoding.UTF8);
            int offset = tokenLength + 1;
            
            RequestType type = (RequestType)BitConverter.ToInt16(requestData, offset);
            offset += 2;

            PokemonValidationResult[] results;

            switch (type)
            {
                case RequestType.RandomMatchup:
                case RequestType.GTS:
                    {
                        int pkmCount = (requestData.Length - offset) / 220;
                        results = new PokemonValidationResult[pkmCount];

                        for (int x = 0; x < results.Length; x++)
                        {
                            byte[] data = new byte[220];
                            Array.Copy(requestData, offset + x, data, 0, 220);
                            Pokemon5 pkm = new Pokemon5(data);
                            // todo: actual validation goes here
                            results[x] = PokemonValidationResult.Valid;
                        }
                    } break;
                /*
                case RequestType.BattleSubway:
                    {
                        // todo: Need more info on this structure
                    } break;
                */
                // todo: there also appears to be a Battle Video request?
                default:
                    {
                        // Don't understand this request. Give it a response containing
                        // all 00s so stuff depending on it won't break.
                        // The game accepts a hash of all 00s and doesn't care what's contained
                        // in the response beyond its expected length so this will work.
                        // fixme: Once we start generating real signatures, they will prevent
                        // this from returning the necessary number of 00s in all cases.
                        results = new PokemonValidationResult[]{
                            PokemonValidationResult.Valid,
                            PokemonValidationResult.Valid,
                            PokemonValidationResult.Valid,
                            PokemonValidationResult.Valid,
                            PokemonValidationResult.Valid,
                            PokemonValidationResult.Valid};
                    } break;
            }

            PartyValidationResult result = PartyValidationResult.Valid;
            foreach (PokemonValidationResult pkr in results)
            {
                if (pkr != PokemonValidationResult.Valid) result = PartyValidationResult.Invalid;
            }

            context.Response.ContentType = "text/plain";
            context.Response.OutputStream.WriteByte((byte)result); // success
            foreach (PokemonValidationResult pkr in results)
            {
                context.Response.OutputStream.Write(BitConverter.GetBytes((int)pkr), 0, 4);
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

    internal class Pokemon5
    {
        // placeholder until I actually do this for real...
        byte[] Data;
        public Pokemon5(byte[] data)
        {
            if (data.Length != 220) throw new ArgumentException();
            Data = data;
        }
    }
}