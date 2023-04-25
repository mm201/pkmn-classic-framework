using PkmnFoundations.Structures;
using PkmnFoundations.Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Wfc
{
    public class TrainerRankingsReport
    {
        public TrainerRankingsReport(DateTime start_date, DateTime end_date, TrainerRankingsLeaderboardGroup[] leaderboards)
        {
            StartDate = start_date;
            EndDate = end_date;
            Leaderboards = leaderboards;
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public TrainerRankingsLeaderboardGroup[] Leaderboards { get; set; }

        public void PadResults()
        {
            foreach (var group in Leaderboards)
            {
                group.PadResults(StartDate);
            }
        }
    }

    public class TrainerRankingsLeaderboardGroup
    {
        public TrainerRankingsLeaderboardGroup(TrainerRankingsRecordTypes record_type, TrainerRankingsLeaderboard leaderboard_trainer_class,
            TrainerRankingsLeaderboard leaderboard_birth_month, TrainerRankingsLeaderboard leaderboard_favourite_pokemon)
        {
            RecordType = record_type;
            LeaderboardTrainerClass = leaderboard_trainer_class;
            LeaderboardBirthMonth = leaderboard_birth_month;
            LeaderboardFavouritePokemon = leaderboard_favourite_pokemon;
        }

        public TrainerRankingsRecordTypes RecordType { get; set; }

        public TrainerRankingsLeaderboard LeaderboardTrainerClass { get; set; }
        public TrainerRankingsLeaderboard LeaderboardBirthMonth { get; set; }
        public TrainerRankingsLeaderboard LeaderboardFavouritePokemon { get; set; }

        public void PadResults(DateTime startDate)
        {
            LeaderboardTrainerClass.PadResults(startDate, 16, 0, 16);
            LeaderboardBirthMonth.PadResults(startDate, 12, 1, 12);
            LeaderboardFavouritePokemon.PadResults(startDate, 20, 1, 493);
        }
    }

    public class TrainerRankingsLeaderboard
    {
        public TrainerRankingsLeaderboard(TrainerRankingsTeamCategories team_category, TrainerRankingsLeaderboardEntry[] entries)
        {
            TeamCategory = team_category;
            Entries = entries;
        }

        public TrainerRankingsTeamCategories TeamCategory { get; set; }

        public TrainerRankingsLeaderboardEntry[] Entries { get; set; }

        public void PadResults(DateTime startDate, int entryCount, int minTeam, int teamCount)
        {
            IEnumerable<TrainerRankingsLeaderboardEntry> working = Entries;
            if (Entries.Length < teamCount)
            {
                var unusedValues = Enumerable.Range(minTeam, teamCount)
                    .DrawWithoutReplacement(new Random((int)startDate.Ticks + 69420))
                    .Take(entryCount)
                    .Where(i => !Entries.Select(e => e.Team).Contains(i));

                working = Entries.Concat(unusedValues.Select(i => new TrainerRankingsLeaderboardEntry(i, 0)));
            }

            // todo: shuffle ties maybe...

            Entries = working.Take(entryCount).ToArray();
        }
    }

    public class TrainerRankingsLeaderboardEntry
    {
        public TrainerRankingsLeaderboardEntry(int team, long score)
        {
            Team = team;
            Score = score;
        }

        public int Team { get; set; }
        public long Score { get; set; }
    }

    public enum TrainerRankingsRecordTypes : int
    {
        // todo: document these
        //Invalid = 0x00, // Seems to trigger a 'there is no data' if used.
        HallOfFameEntries = 0x01,
        //Blank02 = 0x02,
        //Blank03 = 0x03,
        //Blank04 = 0x04,
        //BattleTowerSingleBattleWinStreak = 0x05, // crashes when asked for
        //Blank06 = 0x06, // crashes when asked for
        //BattleTowerDoubleBattleWinStreak = 0x07, // crashes when asked for
        //Blank08 = 0x08, // crashes when asked for
        //BattleTowerMultiBattleWinStreak = 0x09, // crashes when asked for
        //Blank0a = 0x0a, // crashes when asked for
        //BattleTowerPartneredMultiBattleWinStreak = 0x0b, // crashes when asked for
        //Blank0c = 0x0c, // crashes when asked for
        //WiFiBattleTowerWinStreak = 0x0d, // crashes when asked for
        //Blank0e = 0x0e, // crashes when asked for
        ContestsEnteredAlone = 0x0f,
        ContestsEnteredWithFriends = 0x10,
        ContestsEnteredAloneAndWon = 0x11,
        ContestsEnteredWithFriendsAndWon = 0x12,
        //Blank13 = 0x13,
        //Blank14 = 0x14,
        //Blank15 = 0x15,
        //Blank16 = 0x16,
        //Blank17 = 0x17,
        TimesWildPokemonFled = 0x18,
        //Blank19 = 0x19,
        BerriesPlanted = 0x1a,
        StepsWalked = 0x1b,
        TimesBattledWildPokemon = 0x1c,
        TrainerBattlesExcludingUnionRoomAndFrontier = 0x1d,
        PokemonCaught = 0x1e,
        PokemonCaughtFishing = 0x1f,
        EggsHatched = 0x20,
        TimesOwnPokemonEvolved = 0x21,
        GameCornerSlotJackpots = 0x22,
        BattleTowerChallenges = 0x23, // seems to say Frontier on Platinum but Tower on HGSS
        //Blank24 = 0x24,
        //Blank25 = 0x25,
        //Blank26 = 0x26,
        //Blank27 = 0x27,
        CompletedGtsPokemonTrades = 0x28,
        BattlesWonOverNintendoWiFiConnection = 0x29,
        //Blank2a = 0x2a,
        BattlesTiedOverNintendoWiFiConnection = 0x2b,
        BattlesWonAtTheBattleTower = 0x2c,
        TotalMoneySpentShopping = 0x2d,
        PokemonLeftWithTheDayCare = 0x2e,
        PokemonDefeated = 0x2f,
        PokemonOfferedOnGts = 0x30,
        TimesPokemonWereTradedWithFriendsAtTheWiFiClub = 0x31,
        TimesYouSignedYourTrainerCard = 0x32,
        PokemonExtractedFromFossils = 0x33,
        TimesFootprintsWereCheckedByDrFootstep = 0x34,
        TimesMailWasSent = 0x35,
        WildPokemonLuredUsingHoney = 0x36,
        TimesYouTalkedToSomeoneInTheWiFiPlaza = 0x37,
        NumberOfSpheresBuriedInTheUnderground = 0x38,
        TimesWatchedTv = 0x39,
        TimesPokemonWereGivenNicknames = 0x3a,
        PremierBallsReceived = 0x3b,
        ItemsFoundByPokemonInAmitySquare = 0x3c,
        PoffinsCookedAlone = 0x3d,
        PoffinsCookedWithFriends = 0x3e,
        PokemonDressUpDataPhotosTaken = 0x3f,
        BouldersPushedUsingTheHiddenMoveStrength = 0x40,
        TimesMiredInASwamp = 0x41,
        MatchesAgainstYourRivalAndGymLeadersAtTheBattleground = 0x42,
        FacilitiesChallengedAtTheBattleFrontier = 0x43,
        TimesYouMetTheFrontierBrains = 0x44,
        WinsAtTheBattleFactory = 0x45,
        WinsAtTheBattleCastle = 0x46,
        WinsAtTheBattleHall = 0x47,
        WinsAtTheBattleArcade = 0x48,
        PokemonTradesAtTheBattleFactory = 0x49,
        TotalCastlePointsEarnedAtTheBattleCastle = 0x4a,
        WinsOverRank10PokemonAtTheBattleHall = 0x4b,
        BattlePointsEarnedFromTheBattleArcadeGameBoard = 0x4c,
        TotalBattlePointsWon = 0x4d,
        TotalBattlePointsSpent = 0x4e,
        WiFiPlazaGamesPlayed = 0x4f,
        EggsTradedUsingSpinTrade = 0x50,
        StarPiecesTradedAtTheFuegoIronworks = 0x51,
        //BattlePointsEarnedFromTheBattleArcadeGameBoard52 = 0x52, // crashes when asked for
        //ItemsAndBerriesEarnedFromTheBattleArcadeGameBoard53 = 0x53, // crashes when asked for
        //TotalBattlePointsWon54 = 0x54, // crashes when asked for
        //TotalBattlePointsSpent55 = 0x55, // crashes when asked for
        //WinsInWiFiPlazaGames = 0x56, // crashes when asked for
        //EggsTradedUsingSpinTrade57 = 0x57, // crashes when asked for
        //Blank58 = 0x58, // crashes when asked for
        //Blank59 = 0x59, // crashes when asked for
        //Blank5a = 0x5a, // crashes when asked for
        //BlueScreen = 0x5b
    }

    public enum TrainerClass : int
    {
        SchoolKidMale = 0x00,
        BugCatcher = 0x01,
        AceTrainerMale = 0x02,
        Roughneck = 0x03,
        RuinManiac = 0x04,
        BlackBelt = 0x05,
        RichBoy = 0x06,
        PsychicMale = 0x07,
        Lass = 0x08,
        BattleGirl = 0x09,
        Beauty = 0x0a,
        AceTrainerFemale = 0x0b,
        Idol = 0x0c,
        Socialite = 0x0d,
        Cowgirl = 0x0e,
        Lady = 0x0f,
        // Plato = 0x10, // ??? o_O
        // 0x11 and up blue screen
    }

    public enum TrainerRankingsTeamCategories
    {
        BirthMonth, TrainerClass, FavouritePokemon
    }

    public class TrainerRankingsSubmission : BinarySerializableBase
    {
        public TrainerRankingsSubmission(int pid, Versions version,
            Languages language, byte birth_month, byte trainer_class,
            ushort favourite_pokemon, ushort unknown1, ushort unknown2,
            ushort unknown3, TrainerRankingsSubmissionEntry[] entries)
        {
            PID = pid;
            Version = version;
            Language = language;
            BirthMonth = birth_month;
            TrainerClass = trainer_class;
            FavouritePokemon = favourite_pokemon;
            Unknown1 = unknown1; // appears to always be 0. Could mean unknown1 and unknown2 are a single int field
            Unknown2 = unknown2; // seems to max at 999. Probably play time in hours
            Unknown3 = unknown3; // seems to max around 15163.
            Entries = entries;
        }

        public TrainerRankingsSubmission(int pid, byte[] data, int offset)
        {
            PID = pid;
            Load(data, offset);
        }

        public TrainerRankingsSubmission(int pid, byte[] data)
        {
            PID = pid;
            Load(data);
        }

        public TrainerRankingsSubmission(int pid, BinaryReader reader)
        {
            PID = pid;
            Load(reader);
        }

        protected override void Load(BinaryReader reader)
        {
            // AdmiralCurtiss's request:
            // 0140: 0c02050900000000 1800303b01000000
            // 0150: 0100000028000000 0000000043000000
            // 0160: 00000000

            // Hikari's request:
            // Platinum EN July [I forget] Gallade
            // 0140: 0c02070bdb010000 e200350f01000000
            // 0150: 0300000028000000 0800000043000000
            // 0160: 28000000

            // 140: Version
            // 141: Language
            // 142: Birth Month
            // 143: Trainer Class
            // 144-145: Favourite Pokémon
            // 146-14b: Unknown
            // 14c-163: Three record records

            // Record record contains 4 bytes of category and 4 bytes of my score in the category.

            Version = (Versions)reader.ReadByte();                   // 00-00
            Language = (Languages)reader.ReadByte();                 // 01-01
            BirthMonth = reader.ReadByte();                          // 02-02
            TrainerClass = reader.ReadByte();                        // 03-03
            FavouritePokemon = reader.ReadUInt16();                  // 04-05
            Unknown1 = reader.ReadUInt16();                          // 06-07
            Unknown2 = reader.ReadUInt16();                          // 08-09
            Unknown3 = reader.ReadUInt16();                          // 0a-0b
            var entries = new TrainerRankingsSubmissionEntry[3];
            entries[0] = new TrainerRankingsSubmissionEntry(reader); // 0c-13
            entries[1] = new TrainerRankingsSubmissionEntry(reader); // 14-1b
            entries[2] = new TrainerRankingsSubmissionEntry(reader); // 1c-23
            Entries = entries;
        }

        protected override void Save(BinaryWriter writer)
        {
            writer.Write((byte)Version);     // 00-00
            writer.Write((byte)Language);    // 01-01
            writer.Write(BirthMonth);        // 02-02
            writer.Write(TrainerClass);      // 03-03
            writer.Write(FavouritePokemon);  // 04-05
            writer.Write(Unknown1);          // 06-07
            writer.Write(Unknown2);          // 08-09
            writer.Write(Unknown3);          // 0a-0b
            writer.Write(Entries[0].Save()); // 0c-13
            writer.Write(Entries[1].Save()); // 14-1b
            writer.Write(Entries[2].Save()); // 1c-23
        }

        public override int Size
        {
            get
            {
                return 36;
            }
        }

        public int PID { get; set; }

        public Versions Version { get; set; }
        public Languages Language { get; set; }
        public byte BirthMonth { get; set; }
        public byte TrainerClass { get; set; }
        public ushort FavouritePokemon { get; set; }

        public ushort Unknown1 { get; set; }
        public ushort Unknown2 { get; set; }
        public ushort Unknown3 { get; set; }

        private TrainerRankingsSubmissionEntry[] m_entries;
        public TrainerRankingsSubmissionEntry[] Entries
        {
            get
            {
                return m_entries;
            }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (value.Length != 3) throw new ArgumentException("TrainerRankingsSubmission must have exactly 3 entries.");
                m_entries = value;
            }
        }
    }

    public class TrainerRankingsSubmissionEntry : BinarySerializableBase
    {
        public TrainerRankingsSubmissionEntry(TrainerRankingsRecordTypes record_type, uint score)
        {
            RecordType = record_type;
            Score = score;
        }

        public TrainerRankingsSubmissionEntry(byte[] data, int offset)
        {
            Load(data, offset);
        }

        public TrainerRankingsSubmissionEntry(byte[] data)
        {
            Load(data);
        }

        public TrainerRankingsSubmissionEntry(BinaryReader reader)
        {
            Load(reader);
        }

        protected override void Load(BinaryReader reader)
        {
            RecordType = (TrainerRankingsRecordTypes)reader.ReadInt32(); // 00
            Score = reader.ReadUInt32();                                 // 04
        }

        protected override void Save(BinaryWriter writer)
        {
            writer.Write((int)RecordType); // 00
            writer.Write(Score);           // 04
        }

        public override int Size
        {
            get
            {
                return 8;
            }
        }

        public TrainerRankingsRecordTypes RecordType { get; set; }
        public uint Score { get; set; }
    }
}
