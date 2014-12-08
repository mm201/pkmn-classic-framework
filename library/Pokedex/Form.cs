using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;

namespace PkmnFoundations.Pokedex
{
    public class Form
    {
        public Form(Pokedex pokedex, int id, int national_dex, byte value,
            LocalizedString name, String suffix, int height, int weight, int experience)
        {
            m_pokedex = pokedex;
            ID = id;
            NationalDex = national_dex;
            Value = value;
            Name = name;
            Suffix = suffix;
            Height = height;
            Weight = weight;
            Experience = experience;
        }

        private Pokedex m_pokedex;

        public int ID { get; private set; }
        public int NationalDex { get; private set; }
        public byte Value { get; private set; }
        public LocalizedString Name { get; private set; }
        public String Suffix { get; private set; }
        public int Height { get; private set; }
        public int Weight { get; private set; }
        public int Experience { get; private set; }
    }
}
