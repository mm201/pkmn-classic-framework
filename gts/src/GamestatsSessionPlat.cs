using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System.Net;

namespace PkmnFoundations.GTS
{
    public class GamestatsSessionPlat : GtsSessionBase
    {
        public GamestatsSessionPlat(int pid, String url)
            : base(pid, url)
        {
            Hash = ComputeHash(Token);
        }

        public static String ComputeHash(String token)
        {
            // todo: add unit tests with some wiresharked examples.

            if (m_sha1 == null) m_sha1 = SHA1.Create();

            String longToken = "uLMOGEiiJogofchScpXb" + token;

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
            // overlay_0066.bin offset 0x2b2bc contains the evil xor mask I've been looking for,
            // encoded ... in ASCII... >__< (I was looking for binary)
            // uLMOGEiiJogofchScpXb000244fd00006015100000005b440e7epokemondpds
            checksum ^= 0x5b440e7e;
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

        private static int DecryptRNG(int prev)
        {
            return (prev * 0x000244fd + 0x00006015) | 0x10000000;
        }

        public static String ResponseChecksum(byte[] responseArray)
        {
            if (m_sha1 == null) m_sha1 = SHA1.Create();

            String toCheck = "uLMOGEiiJogofchScpXb" + ToUrlSafeBase64String(responseArray) + "uLMOGEiiJogofchScpXb";

            byte[] data = new byte[toCheck.Length];
            MemoryStream stream = new MemoryStream(data);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(toCheck);
            writer.Flush();

            return m_sha1.ComputeHash(data).ToHexStringLower();
        }
    }
}
