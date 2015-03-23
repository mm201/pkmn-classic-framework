using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace PkmnFoundations.Pokedex
{
    public class Ribbon : PokedexRecordBase
    {
        public Ribbon(Pokedex pokedex, int id, LocalizedString name, 
            LocalizedString description,
            int ? position3, int ? position4, int ? position5, int ? position6,
            int ? value3, int ? value4, int ? value5, int ? value6) 
            : base(pokedex)
        {
            ID = id;
            Name = name;
            Description = description;
            Position3 = position3;
            Position4 = position4;
            Position5 = position5;
            Position6 = position6;
            Value3 = value3;
            Value4 = value4;
            Value5 = value5;
            Value6 = value6;
        }

        public Ribbon(Pokedex pokedex, IDataReader reader) :
            this(pokedex, Convert.ToInt32(reader["id"]),
                LocalizedStringFromReader(reader, "Name_"),
                LocalizedStringFromReader(reader, "Description_"),
                reader["Position3"] is DBNull ? null : (int?)Convert.ToInt32(reader["Position3"]),
                reader["Position4"] is DBNull ? null : (int?)Convert.ToInt32(reader["Position4"]),
                reader["Position5"] is DBNull ? null : (int?)Convert.ToInt32(reader["Position5"]),
                reader["Position6"] is DBNull ? null : (int?)Convert.ToInt32(reader["Position6"]),
                reader["Value3"] is DBNull ? null : (int?)Convert.ToInt32(reader["Value3"]),
                reader["Value4"] is DBNull ? null : (int?)Convert.ToInt32(reader["Value4"]),
                reader["Value5"] is DBNull ? null : (int?)Convert.ToInt32(reader["Value5"]),
                reader["Value6"] is DBNull ? null : (int?)Convert.ToInt32(reader["Value6"])
            )
        {
        }

        public int ID { get; private set; }
        public LocalizedString Name { get; private set; }
        public LocalizedString Description { get; private set; }
        public int? Position3 { get; private set; }
        public int? Position4 { get; private set; }
        public int? Position5 { get; private set; }
        public int? Position6 { get; private set; }
        public int? Value3 { get; private set; }
        public int? Value4 { get; private set; }
        public int? Value5 { get; private set; }
        public int? Value6 { get; private set; }

        public int? Position(Generations generation)
        {
            switch (generation)
            {
                case Generations.Generation1:
                case Generations.Generation2:
                    throw new NotSupportedException();
                case Generations.Generation3:
                    return Position3;
                case Generations.Generation4:
                    return Position4;
                case Generations.Generation5:
                    return Position5;
                case Generations.Generation6:
                default:
                    return Position6;
            }
        }

        public int? Value(Generations generation)
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


    }
}
