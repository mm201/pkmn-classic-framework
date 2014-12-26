using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;

namespace PkmnFoundations.Pokedex
{
    public class Ability
    {
        public Ability(Pokedex pokedex, int value, LocalizedString name)
        {
            m_pokedex = pokedex;
            Value = value;
            Name = name;
            // todo: Nice description text
        }

        private Pokedex m_pokedex;

        public int Value { get; private set; }
        public LocalizedString Name { get; private set; }
    }
}
