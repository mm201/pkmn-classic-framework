using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public abstract class StatValuesBase<T> where T : struct
    {
        protected StatValuesBase(IEnumerable<T> s)
        {
            // fail without enumerating if possible
            if (s is IList<T> && ((IList<T>)s).Count != 6) throw new ArgumentException("s must have exactly 6 elements.", "s");
            var arr = s.ToArray();
            if (arr.Length != 6) throw new ArgumentException("s must have exactly 6 elements.", "s");
            Stats = arr;
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
