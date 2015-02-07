using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public abstract class StatValuesBase<T> where T : struct
    {
        protected StatValuesBase(T[] s)
        {
            Stats = s;
        }

        protected StatValuesBase(T s0, T s1, T s2, T s3, T s4, T s5)
            : this(new T[6] { s0, s1, s2, s3, s4, s5 })
        {
        }

        protected T[] Stats { get; private set; }

        public T[] ToArray()
        {
            return Stats.ToArray();
        }
    }
}
