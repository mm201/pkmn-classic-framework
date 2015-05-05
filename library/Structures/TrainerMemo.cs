using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Pokedex;
using PkmnFoundations.Support;

namespace PkmnFoundations.Structures
{
    public class TrainerMemo
    {
        public TrainerMemo(Pokedex.Pokedex pokedex, LocationNumbering numbering, 
            DateTime ? time_egg_obtained, DateTime ? time_encountered,
            int location_egg_obtained_id, int location_encountered_id,
            bool is_hatched, byte level_encountered)
        {
            m_pokedex = pokedex;
            m_location_egg_obtained_pair = Location.CreatePairForLocationNumbering(m_pokedex, () => Numbering);
            m_location_encountered_pair = Location.CreatePairForLocationNumbering(m_pokedex, () => Numbering);

            Numbering = numbering;
            TimeEggObtained = time_egg_obtained;
            TimeEncountered = time_encountered;
            m_location_egg_obtained_pair.Key = location_egg_obtained_id;
            m_location_encountered_pair.Key = location_encountered_id;
            IsHatched = is_hatched;
            LevelEncountered = level_encountered;
        }

        private Pokedex.Pokedex m_pokedex;
        public LocationNumbering Numbering { get; private set; }

        public DateTime? TimeEggObtained { get; private set; }
        public DateTime? TimeEncountered { get; private set; }

        private LazyKeyValuePair<int, Location> m_location_egg_obtained_pair;
        public int LocationEggObtainedID
        {
            get
            {
                return m_location_egg_obtained_pair.Key;
            }
        }
        public Location LocationEggObtained
        { 
            get
            {
                return m_location_egg_obtained_pair.Value;
            }
        }

        private LazyKeyValuePair<int, Location> m_location_encountered_pair;
        public int LocationEncounteredID
        {
            get
            {
                return m_location_encountered_pair.Key;
            }
        }
        public Location LocationEncountered 
        { 
            get
            {
                return m_location_encountered_pair.Value;
            }
        }

        public bool IsHatched { get; private set; }
        public byte LevelEncountered { get; private set; }

        public override String ToString()
        {
            if (IsHatched)
            {
                String timeEgg = TimeEggObtained == null ? "???" : ((DateTime)TimeEggObtained).ToString("D");
                String timeGiven = TimeEncountered == null ? "???" : ((DateTime)TimeEncountered).ToString("D");
                return String.Format("Egg obtained from {1} on {0:D}. Hatched in {3} on {2:D}.", timeEgg, LocationToString(LocationEggObtained, LocationEggObtainedID), timeGiven, LocationToString(LocationEncountered, LocationEncounteredID));
            }
            else
            {
                String level = LevelEncountered.ToString();
                String time = TimeEncountered == null ? "???" : ((DateTime)TimeEncountered).ToString("D");
                return String.Format("Met at Lv. {0} in {2} on {1:D}.", level, time, LocationToString(LocationEncountered, LocationEncounteredID));
            }
        }

        private String LocationToString(Location l, int id)
        {
            if (l == null || l.Name == null) return "Mystery zone #" + id.ToString();
            return l.Name.ToString();
        }
    }
}
