using System;
using System.Collections.Generic;
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

        public int Value { get; private set; }
        public LocalizedString Name { get; private set; }
    }
}
