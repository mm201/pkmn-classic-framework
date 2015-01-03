using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;

namespace PkmnFoundations.Pokedex
{
    public class Ability : PokedexRecordBase
    {
        public Ability(Pokedex pokedex, int value, LocalizedString name)
            : base(pokedex)
        {
            Value = value;
            Name = name;
            // todo: Nice description text
        }

        public Ability(Pokedex pokedex, IDataReader reader)
            : this(
                pokedex,
            Convert.ToInt32(reader["Value"]),
            LocalizedStringFromReader(reader, "Name_")
            )
        {
        }

        public int Value { get; private set; }
        public LocalizedString Name { get; private set; }
    }
}
