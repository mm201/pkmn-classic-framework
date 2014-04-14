using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.IO;

namespace PokeFoundations.GTS
{
    public class GtsSession4 : GtsSessionBase
    {
        public GtsSession4(int pid, String url) : base(pid, url)
        {
            Hash = ComputeHash(Token);
        }

        public static String ComputeHash(String token)
        {
            // todo: add unit tests with some wiresharked examples.
            // examples: (token -> hash)
            // bTII8cU1Kx86cTZPhEqXqLivqpRUUVpU -> 72bd08d60755b572da85d714508d14e68596bbdc
            // 7BEqRQlKsqRh8wTdL3rfgFxu053pgPzO -> e1c8f8b5b4cc0f1062cf215b1e7f36afed124f1e
            // 081PAAfk5SQhC7LTu1Iq7mwGtQ77xPOR -> ef8321dbb5b562d0bf2f3342008a1ba53851a14f

            if (m_sha1 == null) m_sha1 = SHA1.Create();

            String longToken = "sAdeqWo3voLeC5r16DYv" + token;

            byte[] data = new byte[longToken.Length];
            MemoryStream stream = new MemoryStream(data);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(longToken);
            writer.Flush();

            return m_sha1.ComputeHash(data).ToHexStringLower();
        }
    }
}
