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
        /// Reads bytes from a stream and waits until it can read them all.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
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

    }
}
