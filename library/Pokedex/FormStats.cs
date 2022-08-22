using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace PkmnFoundations.Pokedex
{
    public class FormStats : PokedexRecordBase
    {
        public FormStats(Pokedex pokedex, int form_id, Generations min_generation, 
            int type1, int type2, IntStatValues base_stats, ByteStatValues reward_evs)
            : base(pokedex)
        {
            m_form_pair = Form.CreatePair(m_pokedex);
            // xxx: Do we maybe want to expose types as some sort of collection maybe?
            m_type1_pair = Type.CreatePair(m_pokedex);
            m_type2_pair = Type.CreatePair(m_pokedex);
            m_lazy_pairs.Add(m_form_pair);
            m_lazy_pairs.Add(m_type1_pair);
            m_lazy_pairs.Add(m_type2_pair);

            m_form_pair.Key = form_id;
            MinGeneration = min_generation;
            m_type1_pair.Key = type1;
            m_type2_pair.Key = type2;
            BaseStats = base_stats;
            RewardEvs = reward_evs;
        }

        public FormStats(Pokedex pokedex, IDataReader reader)
            : this(
                pokedex,
            Convert.ToInt32(reader["form_id"]),
            (Generations)Convert.ToInt32(reader["MinGeneration"]),
            Convert.ToInt32(reader["Type1"]),
            Convert.ToInt32(reader["Type2"]),
            new IntStatValues(
                Convert.ToInt32(reader["BaseHP"]),
                Convert.ToInt32(reader["BaseAttack"]),
                Convert.ToInt32(reader["BaseDefense"]),
                Convert.ToInt32(reader["BaseSpeed"]),
                Convert.ToInt32(reader["BaseSpAttack"]),
                Convert.ToInt32(reader["BaseSpDefense"])
                ),
            new ByteStatValues(
                Convert.ToByte(reader["RewardHP"]),
                Convert.ToByte(reader["RewardAttack"]),
                Convert.ToByte(reader["RewardDefense"]),
                Convert.ToByte(reader["RewardSpeed"]),
                Convert.ToByte(reader["RewardSpAttack"]),
                Convert.ToByte(reader["RewardSpDefense"])
                )
            )
        {
        }

        public Generations MinGeneration { get; private set; }
        public IntStatValues BaseStats { get; private set; }
        public ByteStatValues RewardEvs { get; private set; }

        private LazyKeyValuePair<int, Form> m_form_pair;
        private LazyKeyValuePair<int, PkmnFoundations.Pokedex.Type> m_type1_pair;
        private LazyKeyValuePair<int, PkmnFoundations.Pokedex.Type> m_type2_pair;

        public int FormID
        {
            get { return m_form_pair.Key; }
        }
        public Form Form
        {
            get { return m_form_pair.Value; }
        }

        public int Type1ID
        {
            get { return m_type1_pair.Key; }
        }
        public PkmnFoundations.Pokedex.Type Type1
        {
            get { return m_type1_pair.Value; }
        }

        public int Type2ID
        {
            get { return m_type2_pair.Key; }
        }
        public PkmnFoundations.Pokedex.Type Type2
        {
            get { return m_type2_pair.Value; }
        }
    }
}
