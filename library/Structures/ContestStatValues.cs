using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public class ConditionValues : StatValuesBase<byte>
    {
        public ConditionValues(byte cool, byte beauty, byte cute, byte smart, byte tough, byte sheen)
            : base(cool, beauty, cute, smart, tough, sheen)
        {

        }

        public byte Cool { get { return Stats[0]; } set { Stats[0] = value; } }
        public byte Beauty { get { return Stats[1]; } set { Stats[1] = value; } }
        public byte Cute { get { return Stats[2]; } set { Stats[2] = value; } }
        public byte Smart { get { return Stats[3]; } set { Stats[3] = value; } }
        public byte Tough { get { return Stats[4]; } set { Stats[4] = value; } }
        public byte Sheen { get { return Stats[5]; } set { Stats[5] = value; } }

        public static int ConditionsIndex(Conditions condition)
        {
            return (int)condition - 1;
        }

        public virtual byte this[Conditions stat]
        {
            get
            {
                int index = ConditionsIndex(stat);
                if (index < 0 || index >= 6) throw new ArgumentException();
                return Stats[index];
            }
            set
            {
                int index = ConditionsIndex(stat);
                if (index < 0 || index >= 6) throw new ArgumentException();
                Stats[index] = value;
            }
        }
    }
}
