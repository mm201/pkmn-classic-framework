using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace PkmnFoundations.Pokedex
{
    public class Type
    {
        public Type(Pokedex pokedex, int id, LocalizedString name, DamageClass damage_class)
        {
            m_pokedex = pokedex;
            ID = id;
            Name = name;
            DamageClass = damage_class;
        }

        private Pokedex m_pokedex;

        public int ID { get; private set; }
        public LocalizedString Name { get; private set; }
        public DamageClass DamageClass { get; private set; }
    }
}
