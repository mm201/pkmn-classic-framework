using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace PkmnFoundations.GTS
{
    // This was uglifying my ashx handler so I'm putting it in its own class
    // instead.

    /// <summary>
    /// Provides a source of fake battle tower opponents.
    /// </summary>
    public static class FakeOpponentGenerator4
    {
        const int FAKE_OPPONENTS_COUNT = 4;

        /// <summary>
        /// Randomly selects some fake opponents without repeats (if possible)
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static BattleTowerRecord4[] GenerateFakeOpponents(int count)
        {
            // todo: allow more with repeats
            if (count > FAKE_OPPONENTS_COUNT) throw new ArgumentOutOfRangeException("count");
            List<int> values = Enumerable.Range(0, FAKE_OPPONENTS_COUNT).ToList();
            BattleTowerRecord4[] result = new BattleTowerRecord4[count];

            Random rand = new Random();

            for (int x = 0; x < count; x++)
            {
                int index = rand.Next(values.Count);
                int index2 = values[index];
                values.RemoveAt(index);
                result[x] = GenerateFakeOpponent(index2);
            }

            return result;
        }

        public static BattleTowerRecord4 GenerateFakeOpponent(int index)
        {
            if (index >= FAKE_OPPONENTS_COUNT) throw new ArgumentOutOfRangeException("index");

            BattleTowerRecord4 record = new BattleTowerRecord4();
            record.Party = new BattleTowerPokemon4[3];
            record.Unknown3 = 7983;

            switch (index)
            {
                case 0:
                default:
                    record.Party[0] = new BattleTowerPokemon4(
                        9, // Blastoise
                        234, // Leftovers
                        new ushort[] { 
                            57, // Surf
                            58, // Ice beam
                            252, // Fake out
                            156 // Rest
                        },
                        0x01020304, 15, // Modest
                        BattleTowerPokemon4.PackIVs(31, 10, 20, 31, 31, 20),
                        new byte[] { 6, 0, 0, 252, 252, 0 },
                        0, Languages.English, 67, // Torrent
                        255, new EncodedString4("Leonardo", 22)
                    );

                    record.Party[1] = new BattleTowerPokemon4(
                        389, // Torterra
                        287, // Choice scarf
                        new ushort[] { 
                            452, // Wood hammer
                            89, // Earthquake
                            276, // Superpower
                            242 // Crunch
                        },
                        0x01020304, 13, // Jolly
                        BattleTowerPokemon4.PackIVs(31, 31, 20, 31, 10, 20),
                        new byte[] { 6, 252, 0, 252, 0, 0 },
                        0, Languages.English, 65, // Overgrow
                        255, new EncodedString4("Donatello", 22)
                    );

                    record.Party[2] = new BattleTowerPokemon4(
                        324, // Torkoal
                        217, // Quick claw
                        new ushort[] { 
                            133, // Amnesia
                            156, // Rest
                            261, // Will-o-wisp
                            90 // Fissure
                        },
                        0x01020304, 23, // Careful
                        BattleTowerPokemon4.PackIVs(31, 10, 31, 20, 10, 31),
                        new byte[] { 252, 0, 6, 0, 0, 252 },
                        0, Languages.English, 73, // White smoke
                        255, new EncodedString4("Raphael", 22)
                    );

                    record.Profile = new BattleTowerProfile4(
                        new EncodedString4("Splnter", 16),
                        Versions.Platinum, Languages.English,
                        0, 0, 0x01020304,
                        new TrendyPhrase4(0, 16, 291, 7), // Ninjask! Squirtle power!
                        0, 14 // Black belt
                        );

                    record.PhraseChallenged = new TrendyPhrase4(0, 16, 291, 7);
                    record.PhraseWon = new TrendyPhrase4(1, 11, 766, 65535);
                    record.PhraseLost = new TrendyPhrase4(2, 8, 1406, 65535);
                    break;

                case 1:
                    record.Party[0] = new BattleTowerPokemon4(
                        376, // Metagross
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
                        255, new EncodedString4("Goldfinger", 22)
                    );

                    record.Party[1] = new BattleTowerPokemon4(
                        282, // Gardevoir
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
                        255, new EncodedString4("Curly", 22)
                    );

                    record.Party[2] = new BattleTowerPokemon4(
                        134, // Vaporeon
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
                        255, new EncodedString4("Seabiscuit", 22)
                    );

                    record.Profile = new BattleTowerProfile4(
                        new EncodedString4("Kate", 16),
                        Versions.Platinum, Languages.English,
                        0, 0, 0x02030405,
                        new TrendyPhrase4(3, 8, 1487, 65535), // There's only WI-FI left!
                        2, 33 // Lady
                        );

                    record.PhraseChallenged = new TrendyPhrase4(3, 8, 1487, 65535);
                    record.PhraseWon = new TrendyPhrase4(3, 3, 1439, 65535); // This BATTLE TOWER is DIFFICULT, isn't it?
                    record.PhraseLost = new TrendyPhrase4(3, 2, 1493, 1492); // I love GTS! I love BATTLE TOWER too!
                    break;

                case 2:
                    record.Party[0] = new BattleTowerPokemon4(
                        392, // Infernape
                        275, // Focus sash
                        new ushort[] { 
                            252, // Fake out
                            283, // Endeavour
                            183, // Mach punch
                            7 // Fire punch
                        },
                        0x02030405, 13, // Jolly
                        BattleTowerPokemon4.PackIVs(31, 31, 20, 31, 10, 20),
                        new byte[] { 6, 252, 0, 252, 0, 0 },
                        0, Languages.English, 66, // Blaze
                        255, new EncodedString4("FunkyMunky", 22)
                    );

                    record.Party[1] = new BattleTowerPokemon4(
                        235, // Smeargle
                        210, // Custap
                        new ushort[] { 
                            147, // Spore
                            169, // Spider web
                            286, // Imprison
                            144 // Transform
                        },
                        0x02030405, 10, // Timid
                        BattleTowerPokemon4.PackIVs(31, 10, 31, 31, 10, 20),
                        new byte[] { 252, 0, 6, 252, 0, 0 },
                        0, Languages.English, 101, // Technician
                        255, new EncodedString4("Yourself", 22)
                    );

                    record.Party[2] = new BattleTowerPokemon4(
                        365, // Walrein
                        217, // Quick claw
                        new ushort[] { 
                            156, // Rest
                            214, // Sleep talk
                            104, // Double team
                            329 // Sheer cold
                        },
                        0x02030405, 5, // Bold
                        BattleTowerPokemon4.PackIVs(31, 10, 20, 31, 10, 31),
                        new byte[] { 252, 0, 0, 252, 0, 6 },
                        0, Languages.English, 47, // Thick fat
                        255, new EncodedString4("Problem?", 22)
                    );

                    record.Profile = new BattleTowerProfile4(
                        new EncodedString4("Boss", 16),
                        Versions.Platinum, Languages.English,
                        0, 0, 0x02030405,
                        new TrendyPhrase4(1, 12, 1147, 65535), // I get the happiest with MOTHER
                        0, 32 // Rich boy
                        );

                    record.PhraseChallenged = new TrendyPhrase4(1, 12, 1147, 65535);
                    record.PhraseWon = new TrendyPhrase4(2, 8, 1140, 65535); // You're WEAK, aren't you?
                    record.PhraseLost = new TrendyPhrase4(2, 6, 1421, 65535); // ROFL! How awful!
                    break;

                case 3:
                    record.Party[0] = new BattleTowerPokemon4(
                        248, // Tyranitar
                        189, // Chople
                        new ushort[] { 
                            446, // Stealth rock
                            349, // Dragon dance
                            89, // EQ
                            444 // Stone edge
                        },
                        0x03040506, 13, // Jolly
                        BattleTowerPokemon4.PackIVs(31, 31, 20, 31, 10, 20),
                        new byte[] { 6, 252, 0, 252, 0, 0 },
                        0, Languages.English, 45, // Sand stream
                        255, new EncodedString4("Tyranitar", 22)
                    );

                    record.Party[1] = new BattleTowerPokemon4(
                        212, // Scizor
                        270, // Life orb
                        new ushort[] { 
                            418, // Bullet punch
                            450, // Bug bite
                            14, // Swords dance
                            355 // Roost
                        },
                        0x03040506, 3, // Adamant
                        BattleTowerPokemon4.PackIVs(31, 31, 20, 31, 10, 20),
                        new byte[] { 6, 252, 0, 252, 0, 0 },
                        0, Languages.English, 101, // Technician
                        255, new EncodedString4("Scizor", 22)
                    );

                    record.Party[2] = new BattleTowerPokemon4(
                        485, // Heatran
                        234, // Leftovers
                        new ushort[] { 
                            436, // Lava plume
                            414, // Earth power
                            156, // Rest
                            214 // Sleep talk
                        },
                        0x03040506, 3, // Modest
                        // fixme: these IVs are unreasonably high for Soft Resetting.
                        BattleTowerPokemon4.PackIVs(31, 10, 20, 20, 31, 31),
                        new byte[] { 250, 0, 0, 0, 56, 204 },
                        0, Languages.English, 18, // Flash fire
                        255, new EncodedString4("Heatran", 22)
                    );

                    record.Profile = new BattleTowerProfile4(
                        new EncodedString4("Indiana", 16),
                        Versions.Platinum, Languages.English,
                        0, 0, 0x03040506,
                        new TrendyPhrase4(3, 4, 1342, 65535), // I can do anything for TREASURE
                        0, 48 // Ruin Maniac
                        );

                    record.PhraseChallenged = new TrendyPhrase4(3, 4, 1342, 65535);
                    record.PhraseWon = new TrendyPhrase4(3, 6, 1148, 1107); // GRANDFATHER is the real NO.1
                    record.PhraseLost = new TrendyPhrase4(3, 10, 1389, 65535); // I prefer VACATION after all
                    break;



                /*

#5: Hail team

Abomasnow@Choice Scarf: Hasty, 228Spe/164Atk/116SAtk
Snow Warning
Blizzard
Wood Hammer
HP:Fire
EQ

Glaceon@Nevermeltice: Modest, 252SAtk/252Spe/4HP
Snow Cloak
Blizzard
Shadow Ball
Wish
Protect

Weavile@Expert Belt: Jolly, 252Atk/216Spe/40HP
Pursuit
Swords Dance
Night Slash
Ice Punch
Low Kick

                 */
                /*

#6: Trick Room

Bronzong@Leftovers: Sassy, 252HP/252Def
Levitate
Trick Room
Gyro Ball
Hypnosis
Explosion

Rhyperior@Life Orb: Brave, 252Atk/248HP/8Def
EQ
Stone Edge
Aqua tail
Megahorn

Magnezone@Expert Belt: Quiet, 252SpAtk/252HP/4Def
HP:Grass
Flash Cannon
Thunderbolt
Magnet Rise


             */
            }

            return record;
        }
    }
}