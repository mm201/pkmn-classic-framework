using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;

namespace PkmnFoundations.Structures
{
    public abstract class BattleTowerRecordBase
    {
        // todo: move much of implementation in here from derived classes

        public abstract IList<BattleTowerPokemonBase> Party { get; set; }

        public abstract BattleTowerProfileBase Profile { get; set; }

        public abstract TrendyPhraseBase PhraseChallenged { get; set; }
        public abstract TrendyPhraseBase PhraseWon { get; set; }
        public abstract TrendyPhraseBase PhraseLost { get; set; }
    }
}
