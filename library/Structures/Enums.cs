using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public enum Generations
    {
        Generation1 = 1,
        Generation2 = 2,
        Generation3 = 3,
        Generation4 = 4,
        Generation5 = 5,
        Generation6 = 6
    }

    [FlagsAttribute]
    public enum GenerationFlags
    {
        Generation1 = 1,
        Generation2 = 2,
        Generation3 = 4,
        Generation4 = 8,
        Generation5 = 16,
        Generation6 = 32
    }

    public enum Versions
    {
        Diamond = 0x0a,
        Pearl = 0x0b,
        Platinum = 0x0c,
        HeartGold = 0x07,
        SoulSilver = 0x08,
        White = 0x14,
        Black = 0x15,
    }

    public enum EvolutionTriggers
    {
        Level = 1,
        Item = 2,
        Trade = 3,
        Shed = 4
    }

    public enum TimeOfDay
    {
        Morning = 1,
        Day = 2,
        Night = 3
    }

    public enum Stats
    {
        Hp = 1,
        Attack = 2,
        Defense = 3,
        Speed = 4,
        SpecialAttack = 5,
        SpecialDefense = 6
    }

    public enum StatLines
    {
        Physical = 1,
        Special = 2,
        Support = 3
    }

    public enum GrowthRates
    {
        Slow = 1,
        Medium = 2,
        Fast = 3,
        MediumSlow = 4,
        Erratic = 5,
        Fluctuating = 6
    }

    public enum Genders : byte
    {
        Male = 1,
        Female = 2,
        None = 3,
        Either = 3
    }

    public enum GtsTrainerGenders : byte
    {
        Male = 0,
        Female = 1
    }
}
