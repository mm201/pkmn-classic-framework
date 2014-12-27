using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Structures;

namespace PkmnFoundations.Pokedex
{
    public class FormStats
    {
        public FormStats(Pokedex pokedex, int form_id, Generations min_generation, int type1, int type2, StatValues base_stats, StatValues reward_evs)
        {
            m_pokedex = pokedex;
            FormID = form_id;
            MinGeneration = min_generation;
            Type1 = type1;
            Type2 = type2;
            BaseStats = base_stats;
            RewardEvs = reward_evs;
        }

        private Pokedex m_pokedex;

        public int FormID { get; private set; }
        public Generations MinGeneration { get; private set; }
        public int Type1 { get; private set; }
        public int Type2 { get; private set; }
        public StatValues BaseStats { get; private set; }
        public StatValues RewardEvs { get; private set; }
    }
}
