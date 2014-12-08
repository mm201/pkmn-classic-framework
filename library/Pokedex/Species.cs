using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace PkmnFoundations.Pokedex
{
    public class Species
    {
        public Species(Pokedex pokedex, int national_dex, LocalizedString name, 
            GrowthRates growth_rate, byte gender_ratio, EggGroups egg_group_1, 
            EggGroups egg_group_2, int egg_steps, bool gender_variations)
        {
            m_pokedex = pokedex;
            NationalDex = national_dex;
            Name = name;
            GrowthRate = growth_rate;
            GenderRatio = gender_ratio;
            EggGroup1 = egg_group_1;
            EggGroup2 = egg_group_2;
            EggSteps = egg_steps;
            GenderVariations = gender_variations;

            //if (pokedex != null && pokedex.Eager)
            {
                // Retrieve foreign information like Family and Formes.
                // Otherwise, this information will be lazily evaluated
                // from the Pokedex instance.
                // If pokedex is null, this information is unavailable.

                // todo: database ownership/lazy stuff can go in a base class.
            }
        }

        /*
         *                     Species s = new Species(null, Convert.ToInt32(row["id"]), 
                        new LocalizedString(){{"JA", row["name_ja"].ToString()},
                                              {"EN", row["name_en"].ToString()},
                                              {"FR", row["name_fr"].ToString()},
                                              {"IT", row["name_it"].ToString()},
                                              {"DE", row["name_de"].ToString()},
                                              {"ES", row["name_es"].ToString()},
                                              {"KO", row["name_ko"].ToString()}
                        },
                        (GrowthRates)(Convert.ToByte(row["growth_rate_id"])),
                        (byte)Convert.ToInt32(row["gender_rate"]),
                        (byte)Convert.ToInt32(row["hatch_counter"]),
                        Convert.ToByte(row["has_gender_differences"]) != 0
                        );

         * */

        // todo: Implement IEquitable and compare against NationalDex
        // Same goes for all these pokedex classes.

        private Pokedex m_pokedex;

        public int NationalDex { get; private set; }
        public LocalizedString Name { get; private set; }
        public GrowthRates GrowthRate { get; private set; }
        public byte GenderRatio { get; private set; }
        public EggGroups EggGroup1 { get; private set; }
        public EggGroups EggGroup2 { get; private set; }
        public int EggSteps { get; private set; }
        public bool GenderVariations { get; private set; }
    }
}
