using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PkmnFoundations.Support;
using PkmnFoundations.Structures;
using PkmnFoundations.Data;
using System.Net.Sockets;
using System.Diagnostics;

namespace PkmnFoundations.GlobalTerminalService
{
    public class GTServer4 : GTServerBase
    {
        public GTServer4()
            : base(12400, false)
        {
            Initialize();
        }

        public GTServer4(int threads)
            : base(12400, false, threads)
        {
            Initialize();
        }

        public GTServer4(int threads, int timeout)
            : base(12400, false, threads, timeout)
        {
            Initialize();
        }

        private void Initialize()
        {

        }
        
        protected override byte[] ProcessRequest(byte[] data, TcpClient c)
        {
            int length = BitConverter.ToInt32(data, 0);
            AssertHelper.Equals(length, data.Length);

            RequestTypes4 requestType = (RequestTypes4)data[4];
            StringBuilder logEntry = new StringBuilder();
            logEntry.AppendFormat("Handling Generation IV {0} request.\nHost: {1}", requestType, c.Client.RemoteEndPoint);
            logEntry.AppendLine();
            EventLogEntryType type = EventLogEntryType.Information;

            CryptMessage(data);

            MemoryStream response = new MemoryStream();
            response.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, 4); // placeholder for length
            response.WriteByte((byte)requestType);
            response.WriteByte(data[5]);

            try
            {
                int pid = BitConverter.ToInt32(data, 8);
                byte version = data[0x0c];
                byte language = data[0x0d];
                logEntry.AppendFormat("pid: {0}", pid);
                logEntry.AppendLine();

                switch (requestType)
                {
                    #region Box upload
                    case RequestTypes4.BoxUpload:
                    {
                        if (data.Length != 0x360)
                        {
                            logEntry.AppendLine("Length did not validate.");
                            type = EventLogEntryType.FailureAudit;
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        BoxLabels4 label = (BoxLabels4)BitConverter.ToInt32(data, 0x140);
                        byte[] boxData = new byte[0x21c];
                        Array.Copy(data, 0x144, boxData, 0, 0x21c);
                        BoxRecord4 record = new BoxRecord4(pid, label, 0, boxData);
                        ulong serial = Database.Instance.BoxUpload4(record);

                        if (serial == 0)
                        {
                            logEntry.AppendLine("Uploaded box already in server.");
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        logEntry.AppendFormat("Box {0} uploaded successfully.", serial);
                        logEntry.AppendLine();
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2); // result code (0 for OK)
                        response.Write(BitConverter.GetBytes(serial), 0, 8);

                    } break;
                    case RequestTypes4.BoxSearch:
                    {
                        if (data.Length != 0x14c)
                        {
                            logEntry.AppendLine("Length did not validate.");
                            type = EventLogEntryType.FailureAudit;
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        // todo: validate or log some of this?
                        BoxLabels4 label = (BoxLabels4)BitConverter.ToInt32(data, 0x144);

                        logEntry.AppendFormat("Searching for {0} boxes.", label);
                        logEntry.AppendLine();

                        BoxRecord4[] results = Database.Instance.BoxSearch4(label, 20);
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2); // result code (0 for OK)
                        response.Write(BitConverter.GetBytes(results.Length), 0, 4);

                        foreach (BoxRecord4 result in results)
                        {
                            response.Write(BitConverter.GetBytes(result.PID), 0, 4);
                            response.Write(BitConverter.GetBytes((int)result.Label), 0, 4);
                            response.Write(BitConverter.GetBytes(result.SerialNumber), 0, 8);
                            // xxx: this may throw if there's database corruption
                            response.Write(result.Data, 0, 0x21c);
                        }
                        logEntry.AppendFormat("Retrieved {0} boxes.", results.Length);
                        logEntry.AppendLine();

                    } break;
                    #endregion

                    #region Dressup
                    case RequestTypes4.DressupUpload:
                    {
                        if (data.Length != 0x220)
                        {
                            logEntry.AppendLine("Length did not validate.");
                            type = EventLogEntryType.FailureAudit;
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        byte[] dressupData = new byte[0xe0];
                        Array.Copy(data, 0x140, dressupData, 0, 0xe0);
                        DressupRecord4 record = new DressupRecord4(pid, 0, dressupData);
                        ulong serial = Database.Instance.DressupUpload4(record);

                        if (serial == 0)
                        {
                            logEntry.AppendLine("Uploaded dressup already in server.");
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        logEntry.AppendFormat("Dressup {0} uploaded successfully.", serial);
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2); // result code (0 for OK)
                        response.Write(BitConverter.GetBytes(serial), 0, 8);

                    } break;
                    case RequestTypes4.DressupSearch:
                    {
                        if (data.Length != 0x14c)
                        {
                            logEntry.AppendLine("Length did not validate.");
                            type = EventLogEntryType.FailureAudit;
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        // todo: validate or log some of this?
                        ushort species = BitConverter.ToUInt16(data, 0x144);

                        logEntry.AppendFormat("Searching for dressups of species {0}.", species);
                        logEntry.AppendLine();

                        DressupRecord4[] results = Database.Instance.DressupSearch4(species, 10);
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2); // result code (0 for OK)
                        response.Write(BitConverter.GetBytes(results.Length), 0, 4);

                        foreach (DressupRecord4 result in results)
                        {
                            response.Write(BitConverter.GetBytes(result.PID), 0, 4);
                            response.Write(BitConverter.GetBytes(result.SerialNumber), 0, 8);
                            response.Write(result.Data, 0, 0xe0);
                        }
                        logEntry.AppendFormat("Retrieved {0} dressup results.", results.Length);
                        logEntry.AppendLine();

                    } break;
                    #endregion

                    #region Battle videos
                    case RequestTypes4.BattleVideoUpload:
                    {
                        if (data.Length != 0x1e8c)
                        {
                            logEntry.AppendLine("Length did not validate.");
                            type = EventLogEntryType.FailureAudit;
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        byte[] battlevidData = new byte[0x1d4c];
                        Array.Copy(data, 0x140, battlevidData, 0, 0x1d4c);
                        BattleVideoRecord4 record = new BattleVideoRecord4(pid, 0, battlevidData);
                        ulong serial = Database.Instance.BattleVideoUpload4(record);

                        if (serial == 0)
                        {
                            logEntry.AppendFormat("Uploaded battle video already in server.");
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        logEntry.AppendFormat("Battle video {0} uploaded successfully.", BattleVideoHeader4.FormatSerial(serial));
                        logEntry.AppendLine();
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2); // result code (0 for OK)
                        response.Write(BitConverter.GetBytes(serial), 0, 8);

                    } break;
                    case RequestTypes4.BattleVideoSearch:
                    {
                        if (data.Length != 0x15c)
                        {
                            logEntry.AppendLine("Length did not validate.");
                            type = EventLogEntryType.FailureAudit;
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        // todo: validate or log some of this?
                        BattleVideoRankings4 ranking = (BattleVideoRankings4)BitConverter.ToUInt32(data, 0x140);
                        ushort species = BitConverter.ToUInt16(data, 0x144);
                        BattleVideoMetagames4 meta = (BattleVideoMetagames4)data[0x146];
                        byte country = data[0x147];
                        byte region = data[0x148];

                        logEntry.Append("Searching for ");
                        if (ranking != BattleVideoRankings4.None)
                            logEntry.AppendFormat("{0}", ranking);
                        else
                        {
                            if (species != 0xffff)
                                logEntry.AppendFormat("species {0}, ", species);
                            logEntry.AppendFormat("{0}", meta);
                            if (country != 0xff)
                                logEntry.AppendFormat(", country {0}", country);
                            if (region != 0xff)
                                logEntry.AppendFormat(", region {0}", region);
                        }
                        logEntry.AppendLine(".");

                        if ((byte)meta == 254)
                        {
                            // todo: Figure out how to make the game perform this search!
                            logEntry.AppendLine("Reverting to latest 30.");
                            meta = BattleVideoMetagames4.SearchLatest30;
                        }

                        BattleVideoHeader4[] results = Database.Instance.BattleVideoSearch4(species, ranking, meta, country, region, 30);
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2); // result code (0 for OK)
                        response.Write(BitConverter.GetBytes(results.Length), 0, 4);

                        foreach (BattleVideoHeader4 result in results)
                        {
                            response.Write(BitConverter.GetBytes(result.PID), 0, 4);
                            response.Write(BitConverter.GetBytes(result.SerialNumber), 0, 8);
                            response.Write(result.Data, 0, 0xe4);
                        }
                        logEntry.AppendFormat("Retrieved {0} battle video results.", results.Length);
                        logEntry.AppendLine();

                    } break;
                    case RequestTypes4.BattleVideoWatch:
                    {
                        if (data.Length != 0x14c)
                        {
                            logEntry.AppendLine("Length did not validate.");
                            type = EventLogEntryType.FailureAudit;
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        ulong serial = BitConverter.ToUInt64(data, 0x140);
                        BattleVideoRecord4 record = Database.Instance.BattleVideoGet4(serial, true);
                        if (record == null)
                        {
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            logEntry.AppendFormat("Requested battle video {0} was missing.", BattleVideoHeader4.FormatSerial(serial));
                            logEntry.AppendLine();
                            type = EventLogEntryType.FailureAudit;
                            break;
                        }

                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2); // result code (0 for OK)
                        response.Write(BitConverter.GetBytes(record.PID), 0, 4);
                        response.Write(BitConverter.GetBytes(record.SerialNumber), 0, 8);
                        response.Write(record.Header.Data, 0, 0xe4);
                        response.Write(record.Data, 0, 0x1c68);
                        logEntry.AppendFormat("Retrieved battle video {0}.", BattleVideoHeader4.FormatSerial(serial));
                        logEntry.AppendLine();

                    } break;
                    case RequestTypes4.BattleVideoSaved:
                    {
                        if (data.Length != 0x148)
                        {
                            logEntry.AppendLine("Length did not validate.");
                            type = EventLogEntryType.FailureAudit;
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        ulong serial = BitConverter.ToUInt64(data, 0x140);

                        if (Database.Instance.BattleVideoFlagSaved4(serial))
                        {
                            response.Write(new byte[] { 0x00, 0x00 }, 0, 2); // result code (0 for OK)
                            logEntry.AppendFormat("Battle video {0} flagged saved.", BattleVideoHeader4.FormatSerial(serial));
                            logEntry.AppendLine();
                        }
                        else
                        {
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            logEntry.AppendFormat("Requested battle video {0} was missing.", BattleVideoHeader4.FormatSerial(serial));
                            logEntry.AppendLine();
                        }
                    } break;
                    #endregion

                    #region Trainer Rankings
                    case RequestTypes4.TrainerRankingsHead:
                    {
                        if (data.Length != 0x140)
                        {
                            logEntry.AppendLine("Length did not validate.");
                            type = EventLogEntryType.FailureAudit;
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        // AdmiralCurtiss's request: completely blank! (just the 0x140 byte header)

                        // AdmiralCurtiss's response: 
                        // 0000: 0c000000f0550000 0128431c

                        // Response format seems to be a list of the three record categories currently being collected.
                        // 0x01: Hall of fame entries
                        // 0x28: Completed GTS trades
                        // 0x43: Facilities challenged at battle frontier
                        // The purpose of 0x1c is unclear to me.

                        if (Database.Instance.TrainerRankingsPerformRollover())
                        {
                            logEntry.AppendLine("Leaderboard rollover.");
                        }
                        var recordTypes = Database.Instance.TrainerRankingsGetActiveRecordTypes();

                        // todo: If we can be sure the player has already sent us their up to date records,
                        // then we can lie here about the active records and collect more complete trainer
                        // stats for better trainer profiles

                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2);

                        // The game will give error 10609 if the response is longer than 4 bytes.
                        // The game will bluescreen if the response is shorter than 3 bytes.
                        // The 4th byte is optional and I don't know what, if anything, it does.
                        int i;
                        for (i = 0; i < 3 && i < recordTypes.Count; i++)
                        {
                            response.WriteByte((byte)recordTypes[i]);
                        }
                        for (; i < 3; i++)
                        {
                            // Must be valid RecordTypes. If we pad with 0x00, it causes a bluescreen.
                            response.WriteByte((byte)0x01);
                        }
                        response.WriteByte(0x1c); // todo: Find out the meaning of this 0x1c.

                    } break;

                    case RequestTypes4.TrainerRankingsSearch:
                    {
#if !DEBUG
                        goto default;
#endif
                        if (data.Length != 0x164)
                        {
                            logEntry.AppendLine("Length did not validate.");
                            type = EventLogEntryType.FailureAudit;
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        // The request is important. It contains the player's records for the above three chosen categories.
                        // It also (presumably) conveys which teams the player is a part of.
                        // (Trainer Class, Birth Month, Favourite Pokémon)

                        // AdmiralCurtiss's request:
                        // 0140: 0c02050900000000 1800303b01000000
                        // 0150: 0100000028000000 0000000043000000
                        // 0160: 00000000

                        // Hikari's request:
                        // Platinum EN July AceTrainerF Gallade
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

                        // Note: Although the game gives instructions that only
                        // your first submission will apply, I think it's more
                        // fun if we allow you to update your results by
                        // submitting again. In this way, you can race against
                        // competing teams to get the highest score.
                        var submission = new TrainerRankingsSubmission(pid, data, 0x140);
                        Database.Instance.TrainerRankingsSubmit(submission);

                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2);

                        // The response is biig and contains all the records for both this week and last week.
                        // This week lacks numbers because they're still growing and to make it a surprise.
                        // Including more than 3 RecordTypes in the response will give error 10609.

                        TrainerRankingsReport thisWeek = Database.Instance.TrainerRankingsGetPendingReport();
                        thisWeek.PadResults();
                        TrainerRankingsReport lastWeek = Database.Instance.TrainerRankingsGetReport(DateTime.MinValue, thisWeek.StartDate.AddDays(-1), 1).FirstOrDefault();
                        if (lastWeek == null) lastWeek = GenerateFakeReport(thisWeek.StartDate.AddDays(-7),
                            new TrainerRankingsRecordTypes[]
                            {
                                0, 0, 0
                            });
                        else
                        {
                            lastWeek.PadResults();
                        }

                        // Last week:
                        foreach (var lbg in lastWeek.Leaderboards)
                        {
                            response.Write(BitConverter.GetBytes((int)lbg.RecordType), 0, 4);

                            foreach (var lbe in lbg.LeaderboardTrainerClass.Entries)
                            {
                                response.WriteByte((byte)lbe.Team);
                            }
                            foreach (var lbe in lbg.LeaderboardTrainerClass.Entries)
                            {
                                response.Write(BitConverter.GetBytes(lbe.Score), 0, 8);
                            }

                            foreach (var lbe in lbg.LeaderboardBirthMonth.Entries)
                            {
                                response.WriteByte((byte)lbe.Team);
                            }
                            foreach (var lbe in lbg.LeaderboardBirthMonth.Entries)
                            {
                                response.Write(BitConverter.GetBytes(lbe.Score), 0, 8);
                            }

                            foreach (var lbe in lbg.LeaderboardFavouritePokemon.Entries)
                            {
                                response.Write(BitConverter.GetBytes((short)lbe.Team), 0, 2);
                            }
                            foreach (var lbe in lbg.LeaderboardFavouritePokemon.Entries)
                            {
                                response.Write(BitConverter.GetBytes(lbe.Score), 0, 8);
                            }
                        }

                        // This week:
                        foreach (var lbg in thisWeek.Leaderboards)
                        {
                            response.Write(BitConverter.GetBytes((int)lbg.RecordType), 0, 4);

                            foreach (var lbe in lbg.LeaderboardTrainerClass.Entries)
                            {
                                response.WriteByte((byte)lbe.Team);
                            }

                            foreach (var lbe in lbg.LeaderboardBirthMonth.Entries)
                            {
                                response.WriteByte((byte)lbe.Team);
                            }

                            foreach (var lbe in lbg.LeaderboardFavouritePokemon.Entries)
                            {
                                response.Write(BitConverter.GetBytes((short)lbe.Team), 0, 2);
                            }
                        }

                    } break;
#endregion

                    default:
                        logEntry.AppendLine("Unrecognized request type.");
                        response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                        break;
                }
            }
            catch (Exception ex)
            {
                logEntry.AppendFormat("Unhandled exception while handling request.\nException: {0}", ex.ToString());
                logEntry.AppendLine();
                type = EventLogEntryType.Error;
                response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
            }

            response.Flush();
            byte[] responseData = response.ToArray();
            WriteLength(responseData);
            CryptMessage(responseData);

            LogHelper.Write(logEntry.ToString(), type);
            return responseData;
        }

        private void CryptMessage(byte[] message)
        {
            if (message.Length < 5) return;
            byte padOffset = (byte)(message[0] + message[4]);

            // encrypt and decrypt are the same operation...
            for (int x = 5; x < message.Length; x++)
                message[x] ^= m_pad[(x + padOffset) & 0xff];
        }

        public override string Title
        {
            get
            {
                return "Generation IV Global Terminal";
            }
        }

        private static readonly byte[] m_pad = new byte[]
        {
            0x1F, 0x98, 0xA5, 0x46, 0x76, 0x5C, 0x3D, 0x0E,
            0x93, 0x18, 0x33, 0x28, 0x0B, 0x07, 0x03, 0x82,
            0x02, 0x43, 0x8A, 0x86, 0xDB, 0x38, 0x34, 0x19,
            0xD6, 0xF9, 0x59, 0xB2, 0xAD, 0x6A, 0x7D, 0xBC,
            0xEE, 0xE0, 0x3A, 0x3F, 0xCA, 0x4C, 0x25, 0x68,
            0xF4, 0xA9, 0x5B, 0xF7, 0x22, 0x60, 0x5A, 0x6F,
            0xFA, 0x1B, 0x79, 0xE9, 0x17, 0xB1, 0x00, 0x9C,
            0xAA, 0x5E, 0x9D, 0xFF, 0xEA, 0xA0, 0x0D, 0x4B,
            0x75, 0xF6, 0x61, 0x85, 0x5D, 0xBB, 0xDC, 0xFB,
            0x64, 0x2E, 0x7A, 0xAB, 0xF1, 0xE8, 0x44, 0x0C,
            0xB8, 0x8F, 0xA8, 0x0A, 0x8E, 0xBD, 0xE1, 0x3B,
            0xFC, 0x3C, 0x9F, 0x1A, 0x56, 0xC5, 0xE2, 0xF5,
            0x47, 0xD9, 0xD7, 0x8C, 0xCD, 0x97, 0xF0, 0x7B,
            0x8B, 0xC3, 0x4F, 0x45, 0x04, 0x90, 0x81, 0x1E,
            0x6B, 0xC9, 0xD3, 0x73, 0xC6, 0xE7, 0x24, 0xBA,
            0x32, 0xF3, 0xC0, 0xEC, 0x57, 0xCC, 0xC4, 0xB6,
            0xC1, 0xAE, 0xAF, 0x88, 0xF2, 0x84, 0xCE, 0x4A,
            0x0F, 0x94, 0x41, 0xB4, 0x74, 0x2A, 0xD1, 0x70,
            0x1C, 0xD4, 0xB0, 0xC2, 0x09, 0x08, 0x16, 0x9B,
            0xB5, 0x8D, 0x2B, 0xD2, 0x89, 0xB7, 0x99, 0xA1,
            0x30, 0x65, 0x54, 0x40, 0x96, 0x71, 0xFE, 0xBF,
            0x31, 0x06, 0xE5, 0x14, 0xE6, 0xDA, 0x48, 0x26,
            0xAC, 0x87, 0x9A, 0xD8, 0xA6, 0xEB, 0x92, 0xCF,
            0xFD, 0x77, 0x1D, 0x21, 0x9E, 0x36, 0x35, 0x53,
            0x3E, 0xD0, 0xD5, 0x62, 0x58, 0x5F, 0x63, 0x7C,
            0x7E, 0x52, 0x29, 0x12, 0x2C, 0x78, 0x05, 0x91,
            0x55, 0xE3, 0xA2, 0xB9, 0xF8, 0x50, 0x95, 0x13,
            0x80, 0x7F, 0x11, 0x27, 0xCB, 0x37, 0x4E, 0x51,
            0x15, 0xEF, 0xA7, 0x72, 0x4D, 0x83, 0x49, 0xA4,
            0x69, 0xDE, 0x20, 0xA3, 0x67, 0xDF, 0x10, 0x42,
            0x39, 0x6C, 0x2D, 0xC7, 0x23, 0xE4, 0xDD, 0xED,
            0xBE, 0x66, 0xB3, 0x2F, 0x01, 0x6E, 0x6D, 0xC8,
        };

        private static TrainerRankingsLeaderboardEntry[] GenerateFakeData(int entryCount, int minTeam, int teamCount, int maxScore)
        {
            Random scoreRandomizer = new Random();
            return Enumerable.Range(minTeam, teamCount).Select(i => new TrainerRankingsLeaderboardEntry(i, scoreRandomizer.Next(maxScore)))
                .OrderBy(e => -e.Score).Take(entryCount).ToArray();
        }

        private static TrainerRankingsReport GenerateFakeReport(DateTime startDate, TrainerRankingsRecordTypes[] recordTypes)
        {
            var leaderboards = recordTypes.Select(r => new TrainerRankingsLeaderboardGroup(r,
                new TrainerRankingsLeaderboard(TrainerRankingsTeamCategories.TrainerClass, GenerateFakeData(16, 0, 16, 100000)),
                new TrainerRankingsLeaderboard(TrainerRankingsTeamCategories.BirthMonth, GenerateFakeData(12, 1, 12, 100000)),
                new TrainerRankingsLeaderboard(TrainerRankingsTeamCategories.FavouritePokemon, GenerateFakeData(20, 1, 493, 100000)))).ToArray();

            return new TrainerRankingsReport(startDate, startDate.AddDays(7), leaderboards);
        }
    }

    internal enum RequestTypes4 : byte
    {
        BoxUpload = 0x08,
        BoxSearch = 0x09,

        DressupUpload = 0x20,
        DressupSearch = 0x21,

        BattleVideoUpload = 0xd8,
        BattleVideoSearch = 0xd9,
        BattleVideoWatch = 0xda,
        BattleVideoSaved = 0xdb,

        TrainerRankingsHead = 0xf0,
        TrainerRankingsSearch = 0xf1
    }
}
