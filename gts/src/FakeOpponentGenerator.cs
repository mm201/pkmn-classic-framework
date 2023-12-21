using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;
using PkmnFoundations.Wfc;

namespace PkmnFoundations.GTS
{
    /// <summary>
    /// Provides a source of fake battle tower opponents.
    /// </summary>
    public static class FakeOpponentGenerator
    {
        const int FAKE_OPPONENTS_COUNT_4 = 8;
        const int FAKE_OPPONENTS_COUNT_5 = 8;

        /// <summary>
        /// Randomly selects some fake opponents without repeats (if possible)
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static BattleTowerRecordBase[] GenerateFakeOpponents(FakeOpponentFactory factory, int count)
        {
            if (count == 0) return new BattleTowerRecordBase[0];
            int fakeOpponentsCount = (factory.Generation < Generations.Generation5) ? FAKE_OPPONENTS_COUNT_4 : FAKE_OPPONENTS_COUNT_5;

            int residualCount = count % fakeOpponentsCount;
            int repeatCount = count / fakeOpponentsCount;

            List<int> values = new List<int>(count);
            Random rand = new Random();

            for (int x = 0; x < repeatCount; x++)
            {
                values.AddRange(Enumerable.Range(0, fakeOpponentsCount));
            }
            values.AddRange(Enumerable.Range(0, fakeOpponentsCount).DrawWithoutReplacement(rand).Take(residualCount));

            var pokedex = AppStateHelper.Pokedex(HttpContext.Current.Application);
            var results = values.DrawWithoutReplacement(rand).Select(i => GenerateFakeOpponent(factory, pokedex, i));

            return results.ToArray();
        }

        public static BattleTowerRecordBase GenerateFakeOpponent(FakeOpponentFactory factory, Pokedex.Pokedex pokedex, int index)
        {
            BattleTowerRecordBase record = factory.CreateRecord(pokedex);
            bool gen4 = factory.Generation <= Generations.Generation4;

            // Trainer classes in gen4: https://projectpokemon.org/rawdb/diamond/msg/560.php
            // Trainer classes in gen5: https://projectpokemon.org/rawdb/black/msg/191.php
            switch (index)
            {
                default:
                    throw new ArgumentOutOfRangeException("index");

                case 0:
                    record.Party[0] = factory.CreatePokemon(pokedex,
                        129, 0, // Magikarp
                        3133, // Cheri
                        new ushort[] {
                            150, // Splash
                            33, // Tackle
                            175, // Flail
                            340 // Bounce
                        },
                        0x01020304, 3, // Adamant
                        IvStatValues.PackIVs(31, 31, 21, 31, 21, 21),
                        new byte[] { 6, 252, 0, 252, 0, 0 },
                        0, Languages.English, 33, // Swift swim
                        255, "Carp"
                    );

                    record.Party[1] = factory.CreatePokemon(pokedex,
                        223, 0, // Remoraid
                        3134, // Chesto
                        new ushort[] {
                            60, // Psybeam
                            61, // Bubblebeam
                            62, // Aurora beam
                            324 // Signal beam
                        },
                        0x01020304, 10, // Timid
                        IvStatValues.PackIVs(31, 21, 21, 31, 31, 21),
                        new byte[] { 6, 252, 0, 252, 0, 0 },
                        0, Languages.English, 97, // Sniper
                        255, "Gunter"
                    );

                    record.Party[2] = factory.CreatePokemon(pokedex,
                        349, 0, // Feebas
                        3134, // Chesto
                        new ushort[] {
                            150, // Splash
                            33, // Tackle
                            175, // Flail
                            240 // Rain Dance
                        },
                        0x01020304, 13, // Jolly
                        IvStatValues.PackIVs(31, 21, 21, 31, 31, 21),
                        new byte[] { 252, 0, 6, 0, 0, 252 },
                        0, Languages.English, 33, // Swift swim
                        255, "Meryl"
                    );

                    record.Profile = factory.CreateProfile(
                        "Steve",
                        Versions.Platinum, Languages.English,
                        0, 0, 0x01020304,
                        gen4 ? factory.CreateTrendyPhrase(0, 16, 291, 7) // Ninjask! Squirtle power!
                             : factory.CreateTrendyPhrase(1, 6, 7, 884), // Watch my Squirtle power take care of Metal Claw!
                        0, 11 // Fisherman
                        );

                    if (gen4)
                    {
                        record.PhraseChallenged = factory.CreateTrendyPhrase(1, 14, 1347, 65535); // This FISHING was really good!
                        record.PhraseWon = factory.CreateTrendyPhrase(1, 11, 766, 65535); // I might have won with HELPING HAND!
                        record.PhraseLost = factory.CreateTrendyPhrase(2, 15, 1347, 65535); // I would’ve won if this\nwere FISHING...
                    }
                    else
                    {
                        record.PhraseChallenged = factory.CreateTrendyPhrase(1, 6, 7, 884); // Watch my Squirtle power take care of Metal Claw!
                        record.PhraseWon = factory.CreateTrendyPhrase(5, 0, 1611, 65535);
                        record.PhraseLost = factory.CreateTrendyPhrase(5, 0, 1611, 65535);
                    }
                    break;

                case 1:
                    record.Party[0] = factory.CreatePokemon(pokedex,
                        376, 0, // Metagross
                        268, // Expert belt
                        new ushort[] {
                            89, // EQ
                            309, // Meteor mash
                            9, // Thunderpunch
                            153 // Explosion
                        },
                        15562158, 2106656978, // Actual TID/PV. Adamant, chained shiny
                        493780176, // Actual IVs which are crap.
                        new byte[] { 252, 238, 0, 20, 0, 0 },
                        0, Languages.English, 29, // Clear body
                        255, "Goldfinger"
                    );

                    record.Party[1] = factory.CreatePokemon(pokedex,
                        282, 0, // Gardevoir
                        297, // Choice specs
                        new ushort[] {
                            94, // Psychic
                            85, // Thunderbolt
                            247, // Shadow ball
                            271 // Trick
                        },
                        15562158, 4094067015, // Actual TID/PV. Modest, chained shiny
                        663420771, // Actual IVs, should be decent
                        new byte[] { 254, 0, 56, 144, 56, 0 },
                        0, Languages.English, 36, // Trace
                        255, "Curly"
                    );

                    record.Party[2] = factory.CreatePokemon(pokedex,
                        134, 0, // Vaporeon
                        234, // Leftovers
                        new ushort[] {
                            57, // Surf
                            164, // Substitute
                            273, // Wish
                            226 // Baton pass
                        },
                        15562158, 895218680, // Bold
                        514292539,
                        new byte[] { 204, 0, 254, 0, 52, 0 },
                        0, Languages.English, 11, // Water absorb
                        255, "Seabiscuit"
                    );

                    record.Profile = factory.CreateProfile(
                        "Megan",
                        Versions.Platinum, Languages.English,
                        0, 0, 0x02030405,
                        gen4 ? factory.CreateTrendyPhrase(3, 8, 1487, 65535)  // There's only WI-FI left!
                             : factory.CreateTrendyPhrase(5, 0, 1611, 65535), // fixme
                        2, 33 // Lady
                        );

                    if (gen4)
                    {
                        record.PhraseChallenged = factory.CreateTrendyPhrase(3, 8, 1487, 65535);  // There's only WI-FI left!
                        record.PhraseWon = factory.CreateTrendyPhrase(3, 3, 1492, 1439); // This BATTLE TOWER is DIFFICULT, isn't it?
                        record.PhraseLost = factory.CreateTrendyPhrase(3, 2, 1493, 1492); // I love GTS! I love BATTLE TOWER too!
                    }
                    else
                    {
                        record.PhraseChallenged = factory.CreateTrendyPhrase(5, 0, 1611, 65535); // Glad to meet you! I am MACHINE!
                        record.PhraseWon = factory.CreateTrendyPhrase(5, 0, 1611, 65535);
                        record.PhraseLost = factory.CreateTrendyPhrase(5, 0, 1611, 65535);
                    }
                    break;

                case 2:
                    record.Party[0] = factory.CreatePokemon(pokedex,
                        392, 0, // Infernape
                        275, // Focus sash
                        new ushort[] {
                            252, // Fake out
                            283, // Endeavour
                            183, // Mach punch
                            7 // Fire punch
                        },
                        0x02030405, 13, // Jolly
                        IvStatValues.PackIVs(31, 31, 20, 31, 10, 20),
                        new byte[] { 6, 252, 0, 252, 0, 0 },
                        0, Languages.English, 66, // Blaze
                        255, "FunkyMunky"
                    );

                    record.Party[1] = factory.CreatePokemon(pokedex,
                        235, 0, // Smeargle
                        210, // Custap
                        new ushort[] {
                            147, // Spore
                            169, // Spider web
                            286, // Imprison
                            144 // Transform
                        },
                        0x02030405, 10, // Timid
                        IvStatValues.PackIVs(31, 10, 31, 31, 10, 20),
                        new byte[] { 252, 0, 6, 252, 0, 0 },
                        0, Languages.English, 101, // Technician
                        255, "Yourself"
                    );

                    record.Party[2] = factory.CreatePokemon(pokedex,
                        365, 0, // Walrein
                        217, // Quick claw
                        new ushort[] {
                            156, // Rest
                            214, // Sleep talk
                            104, // Double team
                            329 // Sheer cold
                        },
                        5, 5, // Bold, shiny
                        IvStatValues.PackIVs(31, 10, 20, 31, 10, 31),
                        new byte[] { 252, 0, 0, 252, 0, 6 },
                        0, Languages.English, 47, // Thick fat
                        255, "Problem?"
                    );

                    record.Profile = factory.CreateProfile(
                        "Dennis",
                        Versions.Platinum, Languages.English,
                        0, 0, 0x02030405,
                        gen4 ? factory.CreateTrendyPhrase(1, 12, 1147, 65535) // I get the happiest with MOTHER
                             : factory.CreateTrendyPhrase(5, 0, 1611, 65535), // fixme
                        0, 32 // Rich boy
                        );

                    if (gen4)
                    {
                        record.PhraseChallenged = factory.CreateTrendyPhrase(1, 12, 1147, 65535); // I get the happiest with MOTHER
                        record.PhraseWon = factory.CreateTrendyPhrase(2, 8, 1140, 65535); // You're WEAK, aren't you?
                        record.PhraseLost = factory.CreateTrendyPhrase(2, 6, 1421, 65535); // ROFL! How awful!
                    }
                    else
                    {
                        record.PhraseChallenged = factory.CreateTrendyPhrase(5, 0, 1611, 65535); // Glad to meet you! I am MACHINE!
                        record.PhraseWon = factory.CreateTrendyPhrase(5, 0, 1611, 65535);
                        record.PhraseLost = factory.CreateTrendyPhrase(5, 0, 1611, 65535);
                    }
                    break;

                case 3:
                    record.Party[0] = factory.CreatePokemon(pokedex,
                        248, 0, // Tyranitar
                        189, // Chople
                        new ushort[] {
                            446, // Stealth rock
                            349, // Dragon dance
                            89, // EQ
                            444 // Stone edge
                        },
                        13, 13, // Jolly, shiny
                        IvStatValues.PackIVs(31, 31, 20, 31, 10, 20),
                        new byte[] { 6, 252, 0, 252, 0, 0 },
                        0, Languages.English, 45, // Sand stream
                        255, "Tyranitar"
                    );

                    record.Party[1] = factory.CreatePokemon(pokedex,
                        212, 0, // Scizor
                        270, // Life orb
                        new ushort[] {
                            418, // Bullet punch
                            450, // Bug bite
                            14, // Swords dance
                            355 // Roost
                        },
                        0x03040506, 3, // Adamant
                        IvStatValues.PackIVs(31, 31, 20, 31, 10, 20),
                        new byte[] { 6, 252, 0, 252, 0, 0 },
                        0, Languages.English, 101, // Technician
                        255, "Scizor"
                    );

                    record.Party[2] = factory.CreatePokemon(pokedex,
                        485, 0, // Heatran
                        234, // Leftovers
                        new ushort[] {
                            436, // Lava plume
                            414, // Earth power
                            156, // Rest
                            214 // Sleep talk
                        },
                        0x03040506, 3, // Modest
                                       // fixme: these IVs are unreasonably high for Soft Resetting.
                        IvStatValues.PackIVs(31, 10, 20, 20, 31, 31),
                        new byte[] { 250, 0, 0, 0, 56, 204 },
                        0, Languages.English, 18, // Flash fire
                        255, "Heatran"
                    );

                    record.Profile = factory.CreateProfile(
                        "Dusty",
                        Versions.Platinum, Languages.English,
                        0, 0, 0x03040506,
                        gen4 ? factory.CreateTrendyPhrase(3, 4, 1342, 65535)  // I can do anything for TREASURE
                             : factory.CreateTrendyPhrase(5, 0, 1611, 65535), // fixme
                        0, 48 // Ruin Maniac
                        );

                    if (gen4)
                    {
                        record.PhraseChallenged = factory.CreateTrendyPhrase(3, 4, 1342, 65535); // I can do anything for TREASURE
                        record.PhraseWon = factory.CreateTrendyPhrase(3, 6, 1148, 1107); // GRANDFATHER is the real NO.1
                        record.PhraseLost = factory.CreateTrendyPhrase(3, 10, 1389, 65535); // I prefer VACATION after all
                    }
                    else
                    {
                        record.PhraseChallenged = factory.CreateTrendyPhrase(5, 0, 1611, 65535); // Glad to meet you! I am MACHINE!
                        record.PhraseWon = factory.CreateTrendyPhrase(5, 0, 1611, 65535);
                        record.PhraseLost = factory.CreateTrendyPhrase(5, 0, 1611, 65535);
                    }
                    break;

                case 4:
                    record.Party[0] = factory.CreatePokemon(pokedex,
                        460, 0, // Abomasnow
                        287, // Scarf
                        new ushort[] {
                            59, // Blizzard
                            452, // Wood hammer
                            237, // Hidden power
                            89 // EQ
                        },
                        0x04050607, 11, // Hasty
                        IvStatValues.PackIVs(19, 31, 18, 30, 28, 19), // HP:fire base 59
                                                                      // Original EVs: 228Spe/164Atk/116SAtk
                                                                      // Adjusted for Hidden Power IVs, sacrificing some Attack
                        new byte[] { 0, 148, 0, 234, 128, 0 },
                        0, Languages.English, 117, // Snow warning
                        255, "Abomasnow"
                    );

                    record.Party[1] = factory.CreatePokemon(pokedex,
                        471, 0, // Glaceon
                        246, // Nevermeltice
                        new ushort[] {
                            59, // Blizzard
                            247, // Shadow ball
                            273, // Wish
                            182 // Protect
                        },
                        15, 15, // Modest, shiny
                        IvStatValues.PackIVs(31, 10, 20, 31, 31, 20),
                        new byte[] { 6, 0, 0, 252, 252, 0 },
                        0, Languages.English, 81, // Snow cloak
                        255, "Glaceon"
                    );

                    record.Party[2] = factory.CreatePokemon(pokedex,
                        461, 0, // Weavile
                        275, // Focus sash
                        new ushort[] {
                            14, // Swords dance
                            400, // Night slash
                            8, // Ice punch
                            67 // Low kick
                        },
                        13, 13, // Jolly, shiny
                        IvStatValues.PackIVs(31, 31, 20, 31, 10, 20),
                        new byte[] { 40, 252, 0, 218, 0, 0 },
                        0, Languages.English, 46, // Pressure
                        255, "Weavile"
                    );

                    record.Profile = factory.CreateProfile(
                        "Frosty",
                        Versions.Platinum, Languages.English,
                        0, 0, 0x04050607,
                        gen4 ? factory.CreateTrendyPhrase(3, 3, 677, 1438)    // This POWDER SNOW is NICE, isn't it?
                             : factory.CreateTrendyPhrase(5, 0, 1611, 65535), // fixme
                        2, 35 // Socialite
                        );

                    if (gen4)
                    {
                        record.PhraseChallenged = factory.CreateTrendyPhrase(3, 3, 677, 1438); // This POWDER SNOW is NICE, isn't it?
                        record.PhraseWon = factory.CreateTrendyPhrase(1, 14, 797, 65535); // This ICE BALL was really good
                        record.PhraseLost = factory.CreateTrendyPhrase(2, 5, 752, 65535); // Could it be? HEAT WAVE
                    }
                    else
                    {
                        record.PhraseChallenged = factory.CreateTrendyPhrase(5, 0, 1611, 65535); // Glad to meet you! I am MACHINE!
                        record.PhraseWon = factory.CreateTrendyPhrase(5, 0, 1611, 65535);
                        record.PhraseLost = factory.CreateTrendyPhrase(5, 0, 1611, 65535);
                    }
                    break;

                case 5:
                    record.Party[0] = factory.CreatePokemon(pokedex,
                        437, 0, // Bronzong
                        234, // Leftovers
                        new ushort[] {
                            433, // Trick room
                            360, // Gyro ball
                            95, // Hypnosis
                            153 // Explosion
                        },
                        22, 22, // Sassy, shiny
                        IvStatValues.PackIVs(31, 20, 31, 0, 10, 20),
                        new byte[] { 252, 0, 252, 0, 0, 6 },
                        0, Languages.English, 26, // Levitate
                        255, "Bronzong"
                    );

                    record.Party[1] = factory.CreatePokemon(pokedex,
                        464, 0, // Rhyperior
                        270, // Life orb
                        new ushort[] {
                            89, // EQ
                            444, // Stone edge
                            401, // Aqua tail
                            224 // Megahorn
                        },
                        0x05060708, 2, // Brave
                        IvStatValues.PackIVs(31, 31, 31, 10, 10, 20),
                        new byte[] { 248, 252, 10, 0, 0, 0 },
                        0, Languages.English, 116, // Solid rock
                        255, "Rhyperior"
                    );

                    record.Party[2] = factory.CreatePokemon(pokedex,
                        462, 0, // Magnezone
                        268, // Expert belt
                        new ushort[] {
                            237, // Hidden power
                            430, // Flash cannon
                            85, // Thunderbolt
                            393 // Magnet rise
                        },
                        0x05060708, 17, // Quiet
                        IvStatValues.PackIVs(31, 10, 31, 10, 31, 20),
                        new byte[] { 252, 0, 6, 0, 252, 0 },
                        0, Languages.English, 42, // Magnet pull
                        255, "Magnezone"
                    );

                    record.Profile = factory.CreateProfile(
                        "Cassie",
                        Versions.Platinum, Languages.English,
                        0, 0, 0x05060708,
                        gen4 ? factory.CreateTrendyPhrase(2, 3, 1146, 65535)  // I want to go home with YOU...
                             : factory.CreateTrendyPhrase(5, 0, 1611, 65535), // fixme
                        2, 85 // Idol
                        );

                    if (gen4)
                    {
                        record.PhraseChallenged = factory.CreateTrendyPhrase(2, 3, 1146, 65535); // I want to go home with YOU...
                        record.PhraseWon = factory.CreateTrendyPhrase(4, 10, 1245, 65535); // Let's GO AHEAD!
                        record.PhraseLost = factory.CreateTrendyPhrase(4, 11, 1348, 65535); // Want to DATE?
                    }
                    else
                    {
                        record.PhraseChallenged = factory.CreateTrendyPhrase(5, 0, 1611, 65535); // Glad to meet you! I am MACHINE!
                        record.PhraseWon = factory.CreateTrendyPhrase(5, 0, 1611, 65535);
                        record.PhraseLost = factory.CreateTrendyPhrase(5, 0, 1611, 65535);
                    }
                    break;

                case 6:
                    record.Party[0] = factory.CreatePokemon(pokedex,
                        65, 0, // Alakazam
                        275, // Focus sash
                        new ushort[] {
                            269, // Taunt
                            94, // Psychic
                            411, // Focus blast
                            324 // Signal beam
                        },
                        15, 15, // Modest, shiny
                        IvStatValues.PackIVs(31, 10, 20, 31, 31, 20),
                        new byte[] { 6, 0, 0, 252, 252, 6 },
                        0, Languages.English, 39, // Inner focus
                        255, "Alakazam"
                    );

                    record.Party[1] = factory.CreatePokemon(pokedex,
                        445, 0, // Garchomp
                        270, // Life orb
                        new ushort[] {
                            14, // Swords dance
                            89, // EQ
                            200, // Outrage
                            424 // Fire fang
                        },
                        0x06070809, 13, // Jolly
                        IvStatValues.PackIVs(31, 31, 20, 31, 10, 20),
                        new byte[] { 6, 252, 0, 252, 0, 0 },
                        0, Languages.English, 8, // Sand veil
                        255, "Garchomp"
                    );

                    record.Party[2] = factory.CreatePokemon(pokedex,
                        242, 0, // Blissey
                        234, // Leftovers
                        new ushort[] {
                            135, // Softboiled
                            104, // Double team
                            92, // Toxic
                            69 // Seismic toss
                        },
                        0x06070809, 5, // Bold
                        IvStatValues.PackIVs(31, 10, 31, 31, 20, 20),
                        new byte[] { 252, 0, 252, 6, 0, 0 },
                        0, Languages.English, 30, // Natural cure
                        255, "Blissey"
                    );

                    record.Profile = factory.CreateProfile(
                        "Evan",
                        Versions.Platinum, Languages.English,
                        0, 0, 0x06070809,
                        gen4 ? factory.CreateTrendyPhrase(0, 2, 566, 65535)   // I'll battle with STRENGTH!
                             : factory.CreateTrendyPhrase(5, 0, 1611, 65535), // fixme
                        0, 24 // Ace trainer M
                        );

                    if (gen4)
                    {
                        record.PhraseChallenged = factory.CreateTrendyPhrase(0, 2, 566, 65535); // I'll battle with STRENGTH!
                        record.PhraseWon = factory.CreateTrendyPhrase(1, 1, 1418, 65535); // I won! I won with SKILLFUL!
                        record.PhraseLost = factory.CreateTrendyPhrase(2, 17, 1428, 65535); // The way I lost... It's like RARE...
                    }
                    else
                    {
                        record.PhraseChallenged = factory.CreateTrendyPhrase(5, 0, 1611, 65535); // Glad to meet you! I am MACHINE!
                        record.PhraseWon = factory.CreateTrendyPhrase(5, 0, 1611, 65535);
                        record.PhraseLost = factory.CreateTrendyPhrase(5, 0, 1611, 65535);
                    }
                    break;

                case 7:
                    record.Party[0] = factory.CreatePokemon(pokedex,
                        9, 0, // Blastoise
                        234, // Leftovers
                        new ushort[] {
                            57, // Surf
                            58, // Ice beam
                            252, // Fake out
                            156 // Rest
                        },
                        0x01020304, 15, // Modest
                        IvStatValues.PackIVs(31, 10, 20, 31, 31, 20),
                        new byte[] { 6, 0, 0, 252, 252, 0 },
                        0, Languages.English, 67, // Torrent
                        255, "Leonardo"
                    );

                    record.Party[1] = factory.CreatePokemon(pokedex,
                        389, 0, // Torterra
                        287, // Choice scarf
                        new ushort[] {
                            452, // Wood hammer
                            89, // Earthquake
                            276, // Superpower
                            242 // Crunch
                        },
                        0x01020304, 13, // Jolly
                        IvStatValues.PackIVs(31, 31, 20, 31, 10, 20),
                        new byte[] { 6, 252, 0, 252, 0, 0 },
                        0, Languages.English, 65, // Overgrow
                        255, "Donatello"
                    );

                    record.Party[2] = factory.CreatePokemon(pokedex,
                        324, 0, // Torkoal
                        217, // Quick claw
                        new ushort[] {
                            133, // Amnesia
                            156, // Rest
                            261, // Will-o-wisp
                            90 // Fissure
                        },
                        0x01020304, 23, // Careful
                        IvStatValues.PackIVs(31, 10, 31, 20, 10, 31),
                        new byte[] { 252, 0, 6, 0, 0, 252 },
                        0, Languages.English, 73, // White smoke
                        255, "Raphael"
                    );

                    record.Profile = factory.CreateProfile(
                        "Splnter",
                        Versions.Platinum, Languages.English,
                        0, 0, 0x01020304,
                        gen4 ? factory.CreateTrendyPhrase(0, 16, 291, 7) // Ninjask! Squirtle power!
                             : factory.CreateTrendyPhrase(1, 6, 7, 884), // Watch my Squirtle power take care of Metal Claw!
                        0, 14 // Black belt
                        );

                    if (gen4)
                    {
                        record.PhraseChallenged = factory.CreateTrendyPhrase(0, 16, 291, 7); // Ninjask! Squirtle power!
                        record.PhraseWon = factory.CreateTrendyPhrase(1, 11, 766, 65535); // I might have won with HELPING HAND!
                        record.PhraseLost = factory.CreateTrendyPhrase(2, 8, 1406, 65535); // You're INCREDIBLE, aren't you?
                    }
                    else
                    {
                        record.PhraseChallenged = factory.CreateTrendyPhrase(1, 6, 7, 884); // Watch my Squirtle power take care of Metal Claw!
                        record.PhraseWon = factory.CreateTrendyPhrase(5, 0, 1611, 65535);
                        record.PhraseLost = factory.CreateTrendyPhrase(5, 0, 1611, 65535);
                    }
                    break;
            }

            return record;
        }
    }

    public abstract class FakeOpponentFactory
    {
        public abstract BattleTowerRecordBase CreateRecord(Pokedex.Pokedex pokedex);

        public abstract BattleTowerPokemonBase CreatePokemon(Pokedex.Pokedex pokedex, 
            ushort species, byte form, ushort held_item, ushort[] moveset,
            uint ot, uint personality, uint ivs, byte[] evs, byte pp_ups,
            Languages language, byte ability, byte happiness, string nickname);

        public abstract BattleTowerProfileBase CreateProfile(string name, Versions version,
            Languages language, byte country, byte region, uint ot,
            TrendyPhraseBase phrase_leader, byte gender, byte unknown);

        public abstract TrendyPhraseBase CreateTrendyPhrase(ushort mood, ushort index, ushort word1, ushort word2);

        public abstract Generations Generation { get; }
    }

    public class FakeOpponentFactory4 : FakeOpponentFactory
    {
        public override BattleTowerRecordBase CreateRecord(Pokedex.Pokedex pokedex)
        {
            BattleTowerRecord4 record = new BattleTowerRecord4(pokedex);
            record.Party = new BattleTowerPokemon4[3];
            record.Unknown3 = 6969;
            return record;
        }

        public override BattleTowerPokemonBase CreatePokemon(Pokedex.Pokedex pokedex, 
            ushort species, byte form, ushort held_item, ushort[] moveset, uint ot, 
            uint personality, uint ivs, byte[] evs, byte pp_ups, 
            Languages language, byte ability, byte happiness, 
            string nickname)
        {
            return new BattleTowerPokemon4(pokedex,
                        species, form,
                        held_item,
                        moveset,
                        ot, personality,
                        ivs,
                        evs,
                        pp_ups, language, ability,
                        happiness, new EncodedString4(nickname, 22)
                    );
        }

        public override BattleTowerProfileBase CreateProfile(string name, 
            Versions version, Languages language, byte country, byte region, 
            uint ot, TrendyPhraseBase phrase_leader, byte gender, byte unknown)
        {
            return new BattleTowerProfile4(
                new EncodedString4(name, 16),
                version, language,
                country, region, ot,
                (TrendyPhrase4)phrase_leader,
                gender, unknown
                );
        }

        public override TrendyPhraseBase CreateTrendyPhrase(ushort mood, 
            ushort index, ushort word1, ushort word2)
        {
            return new TrendyPhrase4(mood, index, word1, word2);
        }

        public override Generations Generation
        {
            get
            {
                return Generations.Generation4;
            }
        }
    }

    public class FakeOpponentFactory5 : FakeOpponentFactory
    {
        public override BattleTowerRecordBase CreateRecord(Pokedex.Pokedex pokedex)
        {
            BattleSubwayRecord5 record = new BattleSubwayRecord5(pokedex);
            record.Party = new BattleSubwayPokemon5[3];
            record.Unknown3 = 6969;
            return record;
        }

        public override BattleTowerPokemonBase CreatePokemon(Pokedex.Pokedex pokedex,
            ushort species, byte form, ushort held_item, ushort[] moveset, uint ot,
            uint personality, uint ivs, byte[] evs, byte pp_ups,
            Languages language, byte ability, byte happiness,
            string nickname)
        {
            return new BattleSubwayPokemon5(pokedex,
                        species, form,
                        held_item,
                        moveset,
                        ot, personality,
                        ivs,
                        evs,
                        pp_ups, language, ability,
                        happiness, new EncodedString5(nickname, 22), 0
                    );
        }

        public override BattleTowerProfileBase CreateProfile(string name, 
            Versions version, Languages language, byte country, byte region, 
            uint ot, TrendyPhraseBase phrase_leader, byte gender, byte unknown)
        {
            return new BattleSubwayProfile5(
                new EncodedString5(name, 16),
                version, language,
                country, region, ot,
                (TrendyPhrase5)phrase_leader,
                gender, unknown
                );
        }

        public override TrendyPhraseBase CreateTrendyPhrase(ushort mood, ushort index, ushort word1, ushort word2)
        {
            return new TrendyPhrase5(mood, index, word1, word2);
        }

        public override Generations Generation
        {
            get
            {
                return Generations.Generation5;
            }
        }
    }
}
