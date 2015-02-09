using System;
using System.Collections.Generic;
using System.Text;

namespace PkmnFoundations.Support
{
    /// <summary>
    /// Helper class to aid with the implementation of non-default indexers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Indexer1d<TKey, TValue> : IReadOnlyIndexer1d<TKey, TValue>, IWriteOnlyIndexer1d<TKey, TValue>
    {
        public Indexer1d(Getter1d<TKey, TValue> getter, Setter1d<TKey, TValue> setter)
        {
            m_getter = getter;
            m_setter = setter;
        }

        private Getter1d<TKey, TValue> m_getter;
        private Setter1d<TKey, TValue> m_setter;

        public TValue this[TKey index]
        {
            get
            {
                return m_getter(index);
            }
            set
            {
                m_setter(index, value);
            }
        }
    }

    public class ReadOnlyIndexer1d<TKey, TValue> : IReadOnlyIndexer1d<TKey, TValue>
    {
        public ReadOnlyIndexer1d(Getter1d<TKey, TValue> getter)
        {
            m_getter = getter;
        }

        private Getter1d<TKey, TValue> m_getter;
        
        public TValue this[TKey index]
        {
            get
            {
                return m_getter(index);
            }
        }
    }

    public interface IReadOnlyIndexer1d<in TKey, out TValue>
    {
        TValue this[TKey index] { get; }
    }

    public interface IWriteOnlyIndexer1d<in TKey, in TValue>
    {
        TValue this[TKey index] { set; }
    }

    public delegate TValue Getter1d<TKey, TValue>(TKey index);
    public delegate void Setter1d<TKey, TValue>(TKey index, TValue value);
}
