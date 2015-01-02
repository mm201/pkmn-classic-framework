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
                m_has_key = false;
            }
        }

        public void EvaluateKey()
        {
            if (!m_has_key) m_key = m_key_evaluator(m_value);
            m_has_key = true;
        }

        public void Evaluate()
        {
            if (!m_has_value) m_value = m_evaluator(m_key);
            m_has_value = true;
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
