using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System.Net;

namespace PkmnFoundations.GTS
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
            writer.Write(longToken); // fixme: this throws an OutOfBoundsException if the passed token contains non-ascii.
            writer.Flush();

            return m_sha1.ComputeHash(data).ToHexStringLower();
        }

        /// <summary>
        /// Decrypts the NDS &data= querystring into readable binary data.
        /// The PID (little endian) is left at the start of the output
        /// but the (unencrypted) checksum is removed.
        /// </summary>
        public static byte[] DecryptData(String data)
        {
            byte[] data2 = FromUrlSafeBase64String(data);
            if (data2.Length < 4) throw new FormatException("Data must contain at least 4 bytes.");

            byte[] data3 = new byte[data2.Length - 4];
            int checksum = BitConverter.ToInt32(data2, 0);
            checksum = IPAddress.NetworkToHostOrder(checksum); // endian flip
            checksum ^= 0x4a3b2c1d;
            int rand = checksum | (checksum << 16);

            // todo: prune first 8 bytes, pass pid to this function to validate
            for (int pos = 0; pos < data3.Length; pos++)
            {
                rand = DecryptRNG(rand);
                data3[pos] = (byte)(data2[pos + 4] ^ (byte)(rand >> 16));
            }

            int checkedsum = 0;
            foreach (byte b in data3)
                checkedsum += b;

            if (checkedsum != checksum) throw new FormatException("Data checksum is incorrect.");

            return data3;
        }
    }
}
