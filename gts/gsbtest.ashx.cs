using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using GamestatsBase;

namespace PkmnFoundations.GTS
{
    /// <summary>
    /// Summary description for gsbtest
    /// </summary>
    public class gsbtest : GamestatsHandler
    {
        public gsbtest() : base("uLMOGEiiJogofchScpXb000244fd00006015100000005b440e7epokemondpds", GamestatsRequestVersions.Version2, GamestatsResponseVersions.Version2)
        {

        }

        public override void ProcessGamestatsRequest(byte[] data, MemoryStream response, string url, int pid, HttpContext context, GamestatsSession session)
        {
            
        }
    }
}