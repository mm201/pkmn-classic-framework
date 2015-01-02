using System;
using System.Collections.Generic;
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

        public int ID { get; private set; }
        public LocalizedString Name { get; private set; }
        public DamageClass DamageClass { get; private set; }
    }
}
