using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace PkmnFoundations.GTS
{
    /// <summary>
    /// Provides a source of fake battle tower opponents.
    /// </summary>
    public static class FakeOpponentGenerator
    {
        const int FAKE_OPPONENTS_COUNT = 7;

        /// <summary>
        /// Randomly selects some fake opponents without repeats (if possible)
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static BattleTowerRecordBase[] GenerateFakeOpponents(FakeOpponentFactory factory, int count)
        {
            // todo: allow more with repeats
            if (count > FAKE_OPPONENTS_COUNT) throw new ArgumentOutOfRangeException("count");
            List<int> values = Enumerable.Range(0, FAKE_OPPONENTS_COUNT).ToList();
            BattleTowerRecordBase[] result = new BattleTowerRecordBase[count];

            Random rand = new Random();
            var pokedex = AppStateHelper.Pokedex(HttpContext.Current.Application);

            for (int x = 0; x < count; x++)
            {
                int index = rand.Next(values.Count);
                int index2 = values[index];
                values.RemoveAt(index);
                result[x] = GenerateFakeOpponent(factory, pokedex, index2);
            }

            return result;
        }

        public static BattleTowerRecordBase GenerateFakeOpponent(FakeOpponentFactory factory, Pokedex.Pokedex pokedex, int index)
        {
            if (index >= FAKE_OPPONENTS_COUNT) throw new ArgumentOutOfRangeException("index");

            BattleTowerRecord4 record = new BattleTowerRecord4(pokedex);
            record.Party = new BattleTowerPokemon4[3];
            record.Unknown3 = 7983;

            switch (index)
            {
                case 0:
                default:
                    record.Party[0] = new BattleTowerPokemon4(pokedex,
                        9, // Blastoise
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
                        255, new EncodedString4("Leonardo", 22)
                    );

                    record.Party[1] = new BattleTowerPokemon4(pokedex,
                        389, // Torterra
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
                        255, new EncodedString4("Donatello", 22)
                    );

                    record.Party[2] = new BattleTowerPokemon4(pokedex,
                        324, // Torkoal
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
                    record.PhraseWon = new TrendyPhrase4(1, 11, 766, 65535); // I might have won with HELPING HAND!
                    record.PhraseLost = new TrendyPhrase4(2, 8, 1406, 65535); // You're INCREDIBLE, aren't you?
                    break;

                case 1:
                    record.Party[0] = new BattleTowerPokemon4(pokedex,
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

                    record.Party[1] = new BattleTowerPokemon4(pokedex,
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

                    record.Party[2] = new BattleTowerPokemon4(pokedex,
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
                        new EncodedString4("Megan", 16),
                        Versions.Platinum, Languages.English,
                        0, 0, 0x02030405,
                        new TrendyPhrase4(3, 8, 1487, 65535), // There's only WI-FI left!
                        2, 33 // Lady
                        );

                    record.PhraseChallenged = new TrendyPhrase4(3, 8, 1487, 65535);
                    record.PhraseWon = new TrendyPhrase4(3, 3, 1492, 1439); // This BATTLE TOWER is DIFFICULT, isn't it?
                    record.PhraseLost = new TrendyPhrase4(3, 2, 1493, 1492); // I love GTS! I love BATTLE TOWER too!
                    break;

                case 2:
                    record.Party[0] = new BattleTowerPokemon4(pokedex,
                        392, // Infernape
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
                        255, new EncodedString4("FunkyMunky", 22)
                    );

                    record.Party[1] = new BattleTowerPokemon4(pokedex,
                        235, // Smeargle
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
                        255, new EncodedString4("Yourself", 22)
                    );

                    record.Party[2] = new BattleTowerPokemon4(pokedex,
                        365, // Walrein
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
                        255, new EncodedString4("Problem?", 22)
                    );

                    record.Profile = new BattleTowerProfile4(
                        new EncodedString4("Dennis", 16),
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
                    record.Party[0] = new BattleTowerPokemon4(pokedex,
                        248, // Tyranitar
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
                        255, new EncodedString4("Tyranitar", 22)
                    );

                    record.Party[1] = new BattleTowerPokemon4(pokedex,
                        212, // Scizor
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
                        255, new EncodedString4("Scizor", 22)
                    );

                    record.Party[2] = new BattleTowerPokemon4(pokedex,
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
                        IvStatValues.PackIVs(31, 10, 20, 20, 31, 31),
                        new byte[] { 250, 0, 0, 0, 56, 204 },
                        0, Languages.English, 18, // Flash fire
                        255, new EncodedString4("Heatran", 22)
                    );

                    record.Profile = new BattleTowerProfile4(
                        new EncodedString4("Dusty", 16),
                        Versions.Platinum, Languages.English,
                        0, 0, 0x03040506,
                        new TrendyPhrase4(3, 4, 1342, 65535), // I can do anything for TREASURE
                        0, 48 // Ruin Maniac
                        );

                    record.PhraseChallenged = new TrendyPhrase4(3, 4, 1342, 65535);
                    record.PhraseWon = new TrendyPhrase4(3, 6, 1148, 1107); // GRANDFATHER is the real NO.1
                    record.PhraseLost = new TrendyPhrase4(3, 10, 1389, 65535); // I prefer VACATION after all
                    break;

                case 4:
                    record.Party[0] = new BattleTowerPokemon4(pokedex,
                        460, // Abomasnow
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
                        255, new EncodedString4("Abomasnow", 22)
                    );

                    record.Party[1] = new BattleTowerPokemon4(pokedex,
                        471, // Glaceon
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
                        255, new EncodedString4("Glaceon", 22)
                    );

                    record.Party[2] = new BattleTowerPokemon4(pokedex,
                        461, // Weavile
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
                        255, new EncodedString4("Weavile", 22)
                    );

                    record.Profile = new BattleTowerProfile4(
                        new EncodedString4("Frosty", 16),
                        Versions.Platinum, Languages.English,
                        0, 0, 0x04050607,
                        new TrendyPhrase4(3, 3, 677, 1438), // This POWDER SNOW is NICE, isn't it?
                        2, 35 // Socialite
                        );

                    record.PhraseChallenged = new TrendyPhrase4(3, 3, 677, 1438);
                    record.PhraseWon = new TrendyPhrase4(1, 14, 797, 65535); // This ICE BALL was really good
                    record.PhraseLost = new TrendyPhrase4(2, 5, 752, 65535); // Could it be? HEAT WAVE
                    break;

                case 5:
                    record.Party[0] = new BattleTowerPokemon4(pokedex,
                        437, // Bronzong
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
                        255, new EncodedString4("Bronzong", 22)
                    );

                    record.Party[1] = new BattleTowerPokemon4(pokedex,
                        464, // Rhyperior
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
                        255, new EncodedString4("Rhyperior", 22)
                    );

                    record.Party[2] = new BattleTowerPokemon4(pokedex,
                        462, // Magnezone
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
                        255, new EncodedString4("Magnezone", 22)
                    );

                    record.Profile = new BattleTowerProfile4(
                        new EncodedString4("Cassie", 16),
                        Versions.Platinum, Languages.English,
                        0, 0, 0x05060708,
                        new TrendyPhrase4(2, 3, 1146, 65535), // I want to go home with YOU...
                        2, 85 // Idol
                        );

                    record.PhraseChallenged = new TrendyPhrase4(2, 3, 1146, 65535);
                    record.PhraseWon = new TrendyPhrase4(4, 10, 1245, 65535); // Let's GO AHEAD!
                    record.PhraseLost = new TrendyPhrase4(4, 11, 1348, 65535); // Want to DATE?
                    break;

                case 6:
                    record.Party[0] = new BattleTowerPokemon4(pokedex,
                        65, // Alakazam
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
                        255, new EncodedString4("Alakazam", 22)
                    );

                    record.Party[1] = new BattleTowerPokemon4(pokedex,
                        445, // Garchomp
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
                        255, new EncodedString4("Garchomp", 22)
                    );

                    record.Party[2] = new BattleTowerPokemon4(pokedex,
                        242, // Blissey
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
                        255, new EncodedString4("Blissey", 22)
                    );

                    record.Profile = new BattleTowerProfile4(
                        new EncodedString4("Evan", 16),
                        Versions.Platinum, Languages.English,
                        0, 0, 0x06070809,
                        new TrendyPhrase4(0, 2, 566, 65535), // I'll battle with STRENGTH!
                        0, 24 // Ace trainer M
                        );

                    record.PhraseChallenged = new TrendyPhrase4(0, 2, 566, 65535);
                    record.PhraseWon = new TrendyPhrase4(1, 1, 1418, 65535); // I won! I won with SKILLFUL!
                    record.PhraseLost = new TrendyPhrase4(2, 17, 1428, 65535); // The way I lost... It's like RARE...
                    break;
            }

            return record;
        }
    }

    public abstract class FakeOpponentFactory
    {
        public abstract BattleTowerRecordBase CreateRecord(Pokedex.Pokedex pokedex);

        public abstract BattleTowerPokemonBase CreatePokemon(Pokedex.Pokedex pokedex, ushort species, ushort held_item, ushort[] moveset,
            uint ot, uint personality, uint ivs, byte[] evs, byte pp_ups,
            Languages language, byte ability, byte happiness, string nickname);

        public abstract BattleTowerProfileBase CreateProfile(string name, Versions version,
            Languages language, byte country, byte region, uint ot,
            TrendyPhraseBase phrase_leader, byte gender, byte unknown);

        public abstract TrendyPhraseBase CreateTrendyPhrase(ushort mood, ushort index, ushort word1, ushort word2);
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
            ushort species, ushort held_item, ushort[] moveset, uint ot, 
            uint personality, uint ivs, byte[] evs, byte pp_ups, 
            Languages language, byte ability, byte happiness, 
            string nickname)
        {
            return new BattleTowerPokemon4(pokedex,
                        species,
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
            ushort species, ushort held_item, ushort[] moveset, uint ot,
            uint personality, uint ivs, byte[] evs, byte pp_ups,
            Languages language, byte ability, byte happiness,
            string nickname)
        {
            return new BattleSubwayPokemon5(pokedex,
                        species,
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
    }

}