using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PkmnFoundations.Support
{
    public static class StreamExtender
    {
        /// <summary>
        /// Reads bytes from a stream and blocks until it can read them all.
        /// </summary>
        /// <param name="s">Stream</param>
        /// <param name="buffer">Buffer to dump data into</param>
        /// <param name="offset">Offset in buffer</param>
        /// <param name="count">Desired number of bytes</param>
        /// <returns>Number of bytes obtained. Should only be less than count 
        /// if eof was reached.</returns>
        public static int ReadBlock(this Stream s, byte[] buffer, int offset, int count)
        {
            int readBytes = 0;
            while (readBytes < count)
            {
                int x = s.Read(buffer, offset + readBytes, count - readBytes);
                if (x == 0) return readBytes;
                readBytes += x;
            }
            return readBytes;
        }

        /// <summary>
        /// Reads bytes from a stream and blocks until it can read them all.
        /// </summary>
        /// <param name="r">Stream, encapsulated in a BinaryReader</param>
        /// <param name="buffer">Buffer to dump data into</param>
        /// <param name="offset">Offset in buffer</param>
        /// <param name="count">Desired number of bytes</param>
        /// <returns>Number of bytes obtained. Should only be less than count 
        /// if eof was reached.</returns>
        public static int ReadBlock(this BinaryReader r, byte[] buffer, int offset, int count)
        {
            int readBytes = 0;
            while (readBytes < count)
            {
                int x = r.Read(buffer, offset + readBytes, count - readBytes);
                if (x == 0) return readBytes;
                readBytes += x;
            }
            return readBytes;
        }

        public static void CompatibleCopyTo(this Stream src, Stream dest)
        {
            const int BUFFER_LENGTH = 256;
            byte[] buffer = new byte[BUFFER_LENGTH];

            int lastProgress;

            while ((lastProgress = src.Read(buffer, 0, BUFFER_LENGTH)) > 0)
                dest.Write(buffer, 0, lastProgress);
        }

        public static void WriteBytes(this Stream s, byte[] buffer)
        {
            // Lack of a Stream.Write overload that just writes the whole array
            // seems like a huge omission to me.
            s.Write(buffer, 0, buffer.Length);
        }
    }
}
