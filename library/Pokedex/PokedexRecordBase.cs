using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;

namespace PkmnFoundations.Pokedex
{
    public class PokedexRecordBase
    {
        internal PokedexRecordBase(Pokedex pokedex)
        {
            m_pokedex = pokedex;
            m_lazy_pairs = new List<ILazyKeyValuePair<int, object>>();
        }

        protected Pokedex m_pokedex;
        protected List<ILazyKeyValuePair<int, object>> m_lazy_pairs;

        internal virtual void PrefetchRelations()
        {
            foreach (ILazyKeyValuePair<int, object> p in m_lazy_pairs)
                p.Evaluate();
        }
    }
}
