﻿using System;
using System.Collections.Generic;
using System.Data;
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

        public static LocalizedString LocalizedStringFromReader(IDataReader reader, string prefix)
        {
            // fixme: share this field with CreateLocalizedStringQueryPieces
            string[] langs = new string[] { "JA", "EN", "FR", "IT", "DE", "ES", "KO" };
            LocalizedString result = new LocalizedString();
            foreach (string lang in langs)
            {
                try
                {
                    int ordinal = reader.GetOrdinal(prefix + lang);
                    if (reader.IsDBNull(ordinal)) continue;
                    result.Add(lang, reader.GetString(ordinal));
                }
                catch (IndexOutOfRangeException)
                {
                    continue;
                }
            }
            return result;
        }
    }
}
