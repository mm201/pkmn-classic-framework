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

    [Flags]
    public enum GenerationFlags
    {
        Generation1 = 1,
        Generation2 = 2,
        Generation3 = 4,
        Generation4 = 8,
        Generation5 = 16,
        Generation6 = 32
    }

    public enum Versions : byte
    {
        // todo: fact check that these GenIII values
        // are retained as-is through pal park.
        FatefulEncounter = 0x00,
        Sapphire = 0x01,
        Ruby = 0x02,
        Emerald = 0x03,
        FireRed = 0x04,
        LeafGreen = 0x05,
        Colosseum = 0x0f,

        Diamond = 0x0a,
        Pearl = 0x0b,
        Platinum = 0x0c,
        HeartGold = 0x07,
        SoulSilver = 0x08,

        White = 0x14,
        Black = 0x15,
        White2 = 0x16,
        Black2 = 0x17,

        X = 0x18,
        Y = 0x19,
        AlphaSapphire = 0x1a,
        OmegaRuby = 0x1b,
    }

    public enum Languages : byte
    {
        Japanese = 0x01,
        English = 0x02,
        French = 0x03,
        Italian = 0x04,
        German = 0x05,
        Spanish = 0x07,
        Korean = 0x08,
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

    [Flags]
    public enum StatFlags
    {
        None = 0,
        Hp = 1,
        Attack = 2,
        Defense = 4,
        Speed = 8,
        SpecialAttack = 16,
        SpecialDefense = 32
    }

    public enum Potential
    {
        Decent,
        AboveAverage,
        RelativelySuperior,
        Outstanding
    }

    public enum Conditions
    {
        Cool = 1,
        Beauty = 2,
        Cute = 3,
        Smart = 4,
        Tough = 5,
        Sheen = 6,
    }

    public enum DamageClass
    {
        None = 0,
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

    public enum TrainerGenders : byte
    {
        Male = 0,
        Female = 1
    }

    [Flags]
    public enum Markings : byte
    {
        Circle = 0x01,
        Triangle = 0x02,
        Square = 0x04,
        Heart = 0x08,
        Star = 0x10,
        Diamond = 0x20
    }

    public enum EggGroups : byte
    {
        None = 0,
        Monster = 1,
        Water1 = 2,
        Bug = 3,
        Flying = 4,
        Ground = 5,
        Fairy = 6,
        Plant = 7,
        HumanShape = 8,
        Water3 = 9,
        Mineral = 10,
        Indeterminate = 11,
        Water2 = 12,
        Ditto = 13,
        Dragon = 14,
        NoEggs = 15
    }

    public enum BattleTargets : byte
    {
        // Pasted straight from veekun.
        // todo: Review and maybe clarify these names
        SpecificMove = 1,
        SelectedPokemonMeFirst = 2,
        Ally = 3,
        UsersField = 4,
        UserOrAlly = 5,
        OpponentsField = 6,
        User = 7,
        RandomOpponent = 8,
        AllOtherPokemon = 9,
        SelectedPokemon = 10,
        AllOpponents = 11,
        EntireField = 12,
        UserAndAllies = 13,
        AllPokemon = 14
    }

    public enum Natures : uint
    {
        Hardy = 0, Lonely = 1, Brave = 2, Adamant = 3, Naughty = 4,
        Bold = 5, Docile = 6, Relaxed = 7, Impish = 8, Lax = 9,
        Timid = 10, Hasty = 11, Serious = 12, Jolly = 13, Naive = 14,
        Modest = 15, Mild = 16, Quiet = 17, Bashful = 18, Rash = 19,
        Calm = 20, Gentle = 21, Sassy = 22, Careful = 23, Quirky = 24
    }

    public enum Pokerus
    {
        None,
        Infected,
        Cured
    }
}
