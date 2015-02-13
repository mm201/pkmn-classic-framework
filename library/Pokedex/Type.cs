using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace PkmnFoundations.Pokedex
{
    public class Type : PokedexRecordBase
    {
        public Type(Pokedex pokedex, int id, LocalizedString name, DamageClass damage_class)
            : base(pokedex)
        {
            ID = id;
            Name = name;
            DamageClass = damage_class;
        }

        public Type(Pokedex pokedex, IDataReader reader)
            : this(
                pokedex,
                Convert.ToInt32(reader["id"]),
                LocalizedStringFromReader(reader, "Name_"),
                (DamageClass)Convert.ToInt32(reader["DamageClass"])
            )
        {
        }

        public int ID { get; private set; }
        public LocalizedString Name { get; private set; }
        public DamageClass DamageClass { get; private set; }

        public static LazyKeyValuePair<int, Type> CreatePair(Pokedex pokedex)
        {
            return new LazyKeyValuePair<int, Type>(
                k => k == 0 ? null : (pokedex == null ? null : pokedex.Types(k)),
                v => v == null ? 0 : v.ID);
        }
    }
}
