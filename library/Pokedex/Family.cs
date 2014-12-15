using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Pokedex
{
    public class Family
    {
        public Family(Pokedex pokedex, int id, int basic_male_id, int basic_female_id,
            int baby_male_id, int baby_female_id, int incense_id, byte gender_ratio)
        {
            m_pokedex = pokedex;
            
            ID = id;
            BasicMaleID = basic_male_id;
            BasicFemaleID = basic_female_id;
            BabyMaleID = baby_male_id;
            BabyFemaleID = baby_female_id;
            IncenseID = incense_id;
            GenderRatio = gender_ratio;
        }

        private Pokedex m_pokedex;

        public int ID { get; private set; }
        public int BasicMaleID { get; private set; }
        public int BasicFemaleID { get; private set; }
        public int BabyMaleID { get; private set; }
        public int BabyFemaleID { get; private set; }
        public int IncenseID { get; private set; }
        public byte GenderRatio { get; private set; }
    }
}
