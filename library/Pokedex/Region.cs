using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;

namespace PkmnFoundations.Pokedex
{
    public class Region : PokedexRecordBase
    {
        public Region(Pokedex pokedex, int id, LocalizedString name) : base(pokedex)
        {
            ID = id;
            Name = name;
        }

        public Region(Pokedex pokedex, IDataReader reader)
            : this(pokedex, Convert.ToInt32(reader["id"]), LocalizedStringFromReader(reader, "Name_"))
        {
        }

        public int ID { get; private set; }
        public LocalizedString Name { get; private set; }

        public static LazyKeyValuePair<int, Region> CreatePair(Pokedex pokedex)
        {
            return new LazyKeyValuePair<int, Region>(
                k => k == 0 ? null : (pokedex == null ? null : pokedex.Regions(k)),
                v => v == null ? 0 : v.ID);
        }
    }
}
