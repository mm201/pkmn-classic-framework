using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace PkmnFoundations.Pokedex
{
    public class Item : PokedexRecordBase
    {
        public Item(Pokedex pokedex, int id, int ? value3, int ? value4, 
            int ? value5, int ? value6, int ? pokeball_value, int price, LocalizedString name)
            : base(pokedex)
        {
            ID = id;
            // todo: Since ID numbers stopped moving around in Gen 4 -> 5, we
            // only need to store value3, value4 and a minGenerationRequired field
            Value3 = value3;
            Value4 = value4;
            Value5 = value5;
            Value6 = value6;
            PokeballValue = pokeball_value;
            Price = price;
            Name = name;
        }

        public Item(Pokedex pokedex, IDataReader reader)
            : this(
                pokedex,
                Convert.ToInt32(reader["id"]),
                reader["Value3"] is DBNull ? null : (int?)Convert.ToInt32(reader["Value3"]),
                reader["Value4"] is DBNull ? null : (int?)Convert.ToInt32(reader["Value4"]),
                reader["Value5"] is DBNull ? null : (int?)Convert.ToInt32(reader["Value5"]),
                reader["Value6"] is DBNull ? null : (int?)Convert.ToInt32(reader["Value6"]),
                reader["PokeballValue"] is DBNull ? null : (int?)Convert.ToInt32(reader["PokeballValue"]),
                Convert.ToInt32(reader["Price"]),
                LocalizedStringFromReader(reader, "Name_")
            )
        {
        }

        public int ID { get; private set; }
        public int ? Value3 { get; private set; }
        public int ? Value4 { get; private set; }
        public int ? Value5 { get; private set; }
        public int ? Value6 { get; private set; }
        public int Price { get; private set; }
        public LocalizedString Name { get; private set; }

        public int ? Value(Generations generation)
        {
            switch (generation)
            {
                case Generations.Generation1:
                case Generations.Generation2:
                    throw new NotSupportedException();
                case Generations.Generation3:
                    return Value3;
                case Generations.Generation4:
                    return Value4;
                case Generations.Generation5:
                    return Value5;
                case Generations.Generation6:
                default:
                    return Value6;
            }
        }

        public int ? PokeballValue
        {
            get; private set;
        }

        public static LazyKeyValuePair<int, Item> CreatePair(Pokedex pokedex)
        {
            return new LazyKeyValuePair<int, Item>(
                k => k == 0 ? null : (pokedex == null ? null : pokedex.Items[k]), 
                v => v == null ? 0 : v.ID);
        }

        public static LazyKeyValuePair<int, Item> CreatePairForGeneration(Pokedex pokedex, Func<Generations> generationGetter)
        {
            return new LazyKeyValuePair<int, Item>(
                k => k == 0 ? null : (pokedex == null ? null : pokedex.ItemsByGeneration(generationGetter())[k]),
                v => v == null ? 0 : (v.Value(generationGetter()) ?? -1));
        }

        public static LazyKeyValuePair<int, Item> CreatePairPokeball(Pokedex pokedex)
        {
            return new LazyKeyValuePair<int, Item>(
                k => k == 0 ? null : (pokedex == null ? null : pokedex.Pokeballs[k]),
                v => v == null ? 0 : v.ID);
        }
    }
}
