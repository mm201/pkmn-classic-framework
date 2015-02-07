using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public class ByteStatValues : StatValues<byte>
    {
        public ByteStatValues(byte hp, byte attack, byte defense, byte speed, byte special_attack, byte special_defense)
            : base(hp, attack, defense, special_attack, special_attack, special_defense)
        {
        }

        protected ByteStatValues(byte[] s) : base(s)
        {

        }
    }
}
