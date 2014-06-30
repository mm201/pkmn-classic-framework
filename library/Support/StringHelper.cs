using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PkmnFoundations.Support
{
    public static class StringHelper
    {
        public static String BytesToString(byte[] data, Encoding encoding)
        {
            MemoryStream ms = new MemoryStream(data);
            StreamReader sr = new StreamReader(ms, encoding);
            return sr.ReadToEnd();
        }

        public static String BytesToString(byte[] data, int start, int length, Encoding encoding)
        {
            MemoryStream ms = new MemoryStream(data, start, length);
            StreamReader sr = new StreamReader(ms, encoding);
            return sr.ReadToEnd();
        }
    }
}
