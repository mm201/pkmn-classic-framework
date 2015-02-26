using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using GamestatsBase;

namespace PkmnFoundations.GTS
{
    /// <summary>
    /// Summary description for pokemondpds_web
    /// </summary>
    public class pokemondpds_web : GamestatsHandler
    {
        public pokemondpds_web()
            : base("uLMOGEiiJogofchScpXb000244fd00006015100000005b440e7epokemondpds",
            GamestatsRequestVersions.Version3, GamestatsResponseVersions.Version2, true, true)
        {

        }

        public override void ProcessGamestatsRequest(byte[] request, MemoryStream response, string url, int pid, HttpContext context, GamestatsSession session)
        {
            switch (url)
            {
                default:
                    SessionManager.Remove(session);

                    // unrecognized page url
                    ShowError(context, 404);
                    return;

                    // todo: Implement Wi-Fi Plaza gamestats handlers here.
            }
        }
    }
}
