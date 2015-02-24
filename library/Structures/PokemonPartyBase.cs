using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Pokedex;
using PkmnFoundations.Support;

namespace PkmnFoundations.Structures
{
    public abstract class PokemonPartyBase : PokemonBase
    {
        public PokemonPartyBase(Pokedex.Pokedex pokedex)
            : base(pokedex)
        {
            Initialize();
        }

        private void Initialize()
        {
        }

        // todo: link experience with level
        public int Experience { get; set; }
        public Markings Markings { get; set; }
        public ConditionValues ContestStats { get; set; }
        public bool IsEgg { get; set; }
        public bool HasNickname { get; set; } // this field decides whether or not its name gets reverted when it evolves.
        public bool FatefulEncounter { get; set; } // aka. obedience flag. A few pokemon, eg. Mew, are disobedient unless this is set.
        public Versions Version { get; set; }

        // todo: create database-driven TrainerMemo class
        // Should expose route, region, encounter type, level, date
        //public abstract TrainerMemo TrainerMemo { get; }

        public abstract String TrainerName { get; set; }
        public abstract TrainerGenders TrainerGender { get; set; }

        // todo: Add pokerus status (None, Infected, Cured) and pokerus days remaining
        //public abstract PokerusStatus PokerusStatus { get; set; }
        //public abstract int PokerusDaysLeft { get; set; }

        public abstract Item Pokeball { get; set; }

        // todo: implement ribbons store (see Pokemon4.Load comment)
        //public HashSet<Ribbon> Ribbons { get; }

        public abstract IntStatValues Stats { get; }
    }
}
