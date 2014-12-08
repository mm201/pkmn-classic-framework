using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;

namespace PkmnFoundations.Pokedex
{
    public class Item
    {
        public Item(Pokedex pokedex, int id, int ? value3, int ? value4, int ? value5, int ? value6, int price, LocalizedString name)
        {
            m_pokedex = pokedex;

            ID = id;
            Value3 = value3;
            Value4 = value4;
            Value5 = value5;
            Value6 = value6;
            Price = price;
            Name = name;
        }

        private Pokedex m_pokedex;

        public int ID { get; private set; }
        public int ? Value3 { get; private set; }
        public int ? Value4 { get; private set; }
        public int ? Value5 { get; private set; }
        public int ? Value6 { get; private set; }
        public int Price { get; private set; }
        public LocalizedString Name { get; private set; }
    }
}
