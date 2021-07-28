using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public class IntStatValues : StatValues<int>
    {
        public IntStatValues(int hp, int attack, int defense, int speed, int special_attack, int special_defense)
            : base(hp, attack, defense, speed, special_attack, special_defense)
        {
        }

        public IntStatValues(IEnumerable<int> s) : base(s)
        {
        }
    }
}
