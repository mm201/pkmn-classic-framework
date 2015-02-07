using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public class IvStatValues : ByteStatValues
    {
        public IvStatValues(byte hp, byte attack, byte defense, byte speed, byte special_attack, byte special_defense)
            : base(hp, attack, defense, special_attack, special_attack, special_defense)
        {
        }

        public IvStatValues(int ivs) : base(new byte[6])
        {
            for (int x = 0; x < 6; x++)
            {
                Stats[x] = (byte)(ivs & 31);
                ivs >>= 5;
            }
        }

        public override byte this[Stats stat]
        {
            get
            {
                return base[stat];
            }
            set
            {
                if (value > 31) throw new ArgumentOutOfRangeException();
                base[stat] = value;
            }
        }

        public int ToInt32()
        {
            int shift = 0;
            int result = 0;
            foreach (byte iv in Stats)
            {
                result |= iv << shift;
                shift += 5;
            }
            return result;
        }
    }
}
