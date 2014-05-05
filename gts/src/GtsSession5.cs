using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System.Net;

namespace PkmnFoundations.GTS
{
    public class GtsSession5 : GtsSessionBase
    {
        public GtsSession5(int pid, String url) : base(pid, url)
        {
            Hash = ComputeHash(Token);
        }

        public static String ComputeHash(String token)
        {
            // todo: add unit tests with some wiresharked examples.

            if (m_sha1 == null) m_sha1 = SHA1.Create();

            String longToken = "HZEdGCzcGGLvguqUEKQN" + token;

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
            if (data2.Length < 12) throw new FormatException("Data must contain at least 12 bytes.");

            byte[] data3 = new byte[data2.Length - 4];
            int checksum = BitConverter.ToInt32(data2, 0);
            checksum = IPAddress.NetworkToHostOrder(checksum); // endian flip
            checksum ^= 0x2db842b2;

            int length = BitConverter.ToInt32(data2, 8);

            // prune the checksum but keep pid and length.
            // this maximizes similarity with genIV's and allows
            // the ashx to check for pids to match.
            // todo: prune first 12 bytes, pass pid to this function to validate
            Array.Copy(data2, 4, data3, 0, data2.Length - 4);
            // todo: validate checksum and length

            return data3;
        }

        public static String ResponseChecksum(byte[] responseArray)
        {
            if (m_sha1 == null) m_sha1 = SHA1.Create();

            String toCheck = "HZEdGCzcGGLvguqUEKQN" + ToUrlSafeBase64String(responseArray) + "HZEdGCzcGGLvguqUEKQN";

            byte[] data = new byte[toCheck.Length];
            MemoryStream stream = new MemoryStream(data);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(toCheck);
            writer.Flush();

            return m_sha1.ComputeHash(data).ToHexStringLower();
        }
    }
}
