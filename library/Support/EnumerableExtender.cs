using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Support
{
    public static class EnumerableExtender
    {
        public static IEnumerable<T> OrderByLambda<T>(this IEnumerable<T> arg, Func<T, T, int> comparer)
        {
            // xxx: Should be using Comparer<T>.Create but we need .NET 4.5 for it. and are still targeting 3.5.
            return arg.OrderBy(k => k, new LambdaComparer<T>(comparer));
        }

        private class LambdaComparer<T> : IComparer<T>
        {
            public LambdaComparer(Func<T, T, int> comparer)
            {
                m_comparer = comparer;
            }

            private readonly Func<T, T, int> m_comparer;

            public int Compare(T first, T second)
            {
                return m_comparer(first, second);
            }
        }

        public static IEnumerable<T> DrawWithoutReplacement<T>(this IEnumerable<T> source, Random rng)
        {
            var working = source.ToList();

            for (int i = working.Count; i > 0; i--)
            {
                int rand = rng.Next(i);
                yield return working[rand];
                working[rand] = working[i - 1];
            }
        }
    }
}
