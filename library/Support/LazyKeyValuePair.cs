using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Support
{
    public class LazyKeyValuePair<TKey, TValue> : ILazyKeyValuePair<TKey, TValue>
    {
        public LazyKeyValuePair(Func<TKey, TValue> evaluator, Func<TValue, TKey> key_evaluator)
        {
            m_evaluator = evaluator;
            m_key_evaluator = key_evaluator;
            m_has_key = true;
            m_has_value = false;
        }

        private TKey m_key;
        private bool m_has_key;
        private Func<TKey, TValue> m_evaluator;
        public TKey Key
        {
            get
            {
                EvaluateKey();
                return m_key;
            }
            set
            {
                m_key = value;
                m_has_key = true;
                m_has_value = false;
            }
        }

        private TValue m_value;
        private bool m_has_value;
        private Func<TValue, TKey> m_key_evaluator;
        public TValue Value
        {
            get
            {
                Evaluate();
                return m_value;
            }
            set
            {
                m_value = value;
                m_has_value = true;
                m_has_key = false;
            }
        }

        public void EvaluateKey()
        {
#if DEBUG
            AssertHelper.Assert(m_has_key || m_has_value);
#endif
            if (!m_has_key) m_key = m_key_evaluator(m_value);
            m_has_key = true;
        }

        public void Evaluate()
        {
#if DEBUG
            AssertHelper.Assert(m_has_key || m_has_value);
#endif
            if (!m_has_value) m_value = m_evaluator(m_key);
            m_has_value = true;
        }

        /// <summary>
        /// Causes the Key to be lost so it will be recomputed from the Value.
        /// </summary>
        public void InvalidateKey()
        {
            if (!m_has_value) throw new InvalidOperationException();
            m_has_key = false;
        }

        /// <summary>
        /// Causes the Value to be lost so it will be recomputed from the Key.
        /// </summary>
        public void Invalidate()
        {
            if (!m_has_key) throw new InvalidOperationException();
            m_has_value = false;
        }
    }

    public interface ILazyKeyValuePair<out TKey, out TValue>
    {
        TKey Key { get; }
        TValue Value { get; }
        void EvaluateKey();
        void Evaluate();
    }
}
