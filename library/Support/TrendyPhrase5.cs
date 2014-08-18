using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Support
{
    public class TrendyPhrase5 : TrendyPhraseBase
    {
        public TrendyPhrase5(byte[] data)
            : base(data)
        {
        }

        public TrendyPhrase5(ushort mood, ushort index, ushort word1, ushort word2)
            : base(Pack(mood, index, word1, word2))
        {
        }

        private static byte[] Pack(ushort mood, ushort index, ushort word1, ushort word2)
        {
            byte[] result = new byte[8];
            Array.Copy(BitConverter.GetBytes(mood), 0, result, 0, 2);
            Array.Copy(BitConverter.GetBytes(index), 0, result, 2, 2);
            Array.Copy(BitConverter.GetBytes(word1), 0, result, 4, 2);
            Array.Copy(BitConverter.GetBytes(word2), 0, result, 6, 2);
            return result;
        }

        public override String Render(String wordFormat)
        {
            throw new NotImplementedException();
        }
    }
}
