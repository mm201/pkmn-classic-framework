using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public class StatValues<T> : StatValuesBase<T> where T : struct
    {
        public StatValues(T hp, T attack, T defense, T speed, T special_attack, T special_defense)
            : base(hp, attack, defense, speed, special_attack, special_defense)
        {

        }

        protected StatValues(T[] s) : base(s)
        {
            if (s.Length != 6) throw new ArgumentException();
        }

        public T Hp { get { return Stats[0]; } set { Stats[0] = value; } }
        public T Attack { get { return Stats[1]; } set { Stats[1] = value; } }
        public T Defense { get { return Stats[2]; } set { Stats[2] = value; } }
        public T Speed { get { return Stats[3]; } set { Stats[3] = value; } }
        public T SpecialAttack { get { return Stats[4]; } set { Stats[4] = value; } }
        public T SpecialDefense { get { return Stats[5]; } set { Stats[5] = value; } }

        public static int StatsIndex(Stats stat)
        {
            return (int)stat - 1;
        }

        public virtual T this[Stats stat]
        {
            get
            {
                int index = StatsIndex(stat);
                if (index < 0 || index >= 6) throw new ArgumentException();
                return Stats[index];
            }
            set
            {
                int index = StatsIndex(stat);
                if (index < 0 || index >= 6) throw new ArgumentException();
                Stats[index] = value;
            }
        }
    }
}
