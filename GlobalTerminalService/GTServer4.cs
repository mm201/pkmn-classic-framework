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
            // todo: Customizable pads is an antifeature. Store pad in code here instead of a file.
            m_pad = new byte[256];

            using (FileStream s = File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "pad.bin"))
            {
                s.Read(m_pad, 0, m_pad.Length);
                s.Close();
            }
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

                        logEntry.AppendLine("Replayed response.");
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2);

                        // Response format seems to be a list of the three record categories currently being collected.
                        // 0x01: Hall of fame entries
                        // 0x28: Completed GTS trades
                        // 0x43: Facilities challenged at battle frontier
                        // The purpose of 0x1c is unclear to me.
                        response.Write(new byte[] { 0x01, 0x28, 0x43, 0x1c }, 0, 4);

                    } break;

                    case RequestTypes4.TrainerRankingsSearch:
                    {
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

                        // The response is biig and contains all the records for both this week and last week.
                        // This week lacks numbers because they're still growing and to make it a surprise.

                        logEntry.AppendLine("Replayed response.");
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2);

                        response.WriteBytes(new byte[] {
                            0x1c, 0x00, 0x00, 0x00, 0x02, 0x07, 0x06, 0x00,
                            0x04, 0x01, 0x05, 0x03, 0x0c, 0x09, 0x0f, 0x0e,
                            0x0d, 0x08, 0x0a, 0x0b, 0x89, 0xe9, 0x01, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x8a, 0x66, 0x01, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x8f, 0x04, 0x01, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0xb1, 0xe0, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x31, 0xa4, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0xf5, 0xa0, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0xa8, 0x9d, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x04, 0x71, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0xd6, 0x51, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0xc9, 0x46, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x17, 0x2a, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0xb2, 0x26, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x58, 0x22, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x20, 0x21, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x99, 0x1e, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x6b, 0x12, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x01, 0x09, 0x05, 0x0c,
                            0x07, 0x04, 0x08, 0x0a, 0x02, 0x03, 0x06, 0x0b,
                            0xd3, 0x16, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xf0, 0x05, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xa4, 0xff, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x17, 0xea, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x90, 0xcd, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x86, 0xb6, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x6e, 0xb5, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xdf, 0xad, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xde, 0x92, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x7b, 0x90, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x46, 0x6e, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x89, 0x67, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x9d, 0x00, 0xc0, 0x01, 0xf9, 0x00, 0x5e, 0x01,
                            0xf8, 0x00, 0x06, 0x00, 0x95, 0x01, 0xa0, 0x00,
                            0x97, 0x00, 0x95, 0x00, 0xbd, 0x01, 0x7f, 0x01,
                            0x5f, 0x00, 0xd8, 0x01, 0x88, 0x01, 0x94, 0x00,
                            0xed, 0x01, 0x19, 0x00, 0x93, 0x00, 0x96, 0x00,
                            0xb6, 0x5c, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x95, 0x52, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x61, 0x48, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xb1, 0x47, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x0b, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xd3, 0x34, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xa4, 0x2d, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xee, 0x29, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x22, 0x28, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x1f, 0x28, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x27, 0x26, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x3a, 0x1f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x6f, 0x1e, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x5e, 0x1d, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x41, 0x1a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x20, 0x18, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x33, 0x17, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xeb, 0x16, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xdf, 0x16, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x4c, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x2b, 0x00, 0x00, 0x00, 0x07, 0x03, 0x0f, 0x02,
                            0x05, 0x0e, 0x04, 0x01, 0x09, 0x00, 0x0a, 0x0c,
                            0x06, 0x0d, 0x0b, 0x08, 0x01, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x0c, 0x08, 0x0b, 0x09,
                            0x01, 0x07, 0x03, 0x04, 0x06, 0x0a, 0x05, 0x02,
                            0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xf8, 0x00, 0xbd, 0x00, 0x8d, 0x00, 0x71, 0x01,
                            0x8e, 0x01, 0x98, 0x00, 0xa2, 0x00, 0x7e, 0x00,
                            0x42, 0x01, 0x55, 0x01, 0x48, 0x01, 0xa7, 0x01,
                            0x53, 0x00, 0x4f, 0x01, 0xa6, 0x00, 0xd7, 0x00,
                            0xcc, 0x01, 0x48, 0x00, 0xf3, 0x00, 0x94, 0x01,
                            0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x4a, 0x00, 0x00, 0x00, 0x02, 0x06, 0x07, 0x05,
                            0x00, 0x03, 0x04, 0x01, 0x08, 0x09, 0x0e, 0x0c,
                            0x0d, 0x0a, 0x0b, 0x0f, 0x5e, 0x76, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x8f, 0x39, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x16, 0x28, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x8e, 0x1e, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x4a, 0x13, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0xb6, 0x0c, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x33, 0x0b, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0xfe, 0x0a, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0xaa, 0x0a, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x2b, 0x04, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0xb8, 0x02, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x63, 0x02, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x14, 0x02, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0xad, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x6a, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x02, 0x04, 0x01, 0x09,
                            0x0a, 0x05, 0x03, 0x0b, 0x07, 0x08, 0x0c, 0x06,
                            0x64, 0x2b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xd7, 0x2a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x24, 0x28, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x7a, 0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x5a, 0x23, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xfc, 0x1c, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xf3, 0x19, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x9f, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x3a, 0x11, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x82, 0x0e, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xbb, 0x0c, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xa6, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xf9, 0x00, 0x79, 0x00, 0x9d, 0x00, 0x80, 0x01,
                            0xa0, 0x00, 0x9b, 0x01, 0x95, 0x01, 0x70, 0x01,
                            0xc0, 0x01, 0xd0, 0x01, 0x7f, 0x01, 0x94, 0x01,
                            0x95, 0x00, 0xbd, 0x01, 0x7d, 0x01, 0xe6, 0x01,
                            0x94, 0x00, 0x92, 0x00, 0xd2, 0x01, 0x01, 0x00,
                            0xf5, 0x25, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xec, 0x19, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x85, 0x0f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x1d, 0x0f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x38, 0x0b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x1a, 0x0b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x27, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xd4, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xb7, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x87, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x87, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x6e, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x67, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xd4, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xb2, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x9d, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x97, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xcd, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xb4, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x80, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x01, 0x00, 0x00, 0x00, 0x02, 0x04, 0x06, 0x0f,
                            0x03, 0x05, 0x07, 0x00, 0x08, 0x09, 0x01, 0x0c,
                            0x0e, 0x0b, 0x0d, 0x0a, 0x03, 0x06, 0x01, 0x04,
                            0x0a, 0x05, 0x08, 0x0c, 0x09, 0x0b, 0x02, 0x07,
                            0x8e, 0x00, 0x7c, 0x01, 0x75, 0x01, 0x8e, 0x01,
                            0x9d, 0x00, 0xe4, 0x01, 0x04, 0x01, 0x80, 0x01,
                            0xfe, 0x00, 0xaf, 0x00, 0xe3, 0x01, 0x88, 0x01,
                            0xe7, 0x01, 0x82, 0x00, 0x5e, 0x01, 0xeb, 0x01,
                            0xe5, 0x00, 0xe8, 0x01, 0x93, 0x00, 0x96, 0x00,
                            0x28, 0x00, 0x00, 0x00, 0x03, 0x07, 0x00, 0x0c,
                            0x04, 0x02, 0x05, 0x06, 0x0b, 0x01, 0x0f, 0x0d,
                            0x08, 0x09, 0x0e, 0x0a, 0x0a, 0x07, 0x0c, 0x06,
                            0x01, 0x02, 0x08, 0x03, 0x05, 0x09, 0x0b, 0x04,
                            0x88, 0x01, 0x8f, 0x00, 0x7c, 0x01, 0x8e, 0x01,
                            0xea, 0x00, 0x01, 0x01, 0xa4, 0x01, 0xe6, 0x01,
                            0x5e, 0x01, 0xc0, 0x01, 0x8e, 0x00, 0x9a, 0x00,
                            0xe7, 0x01, 0xbd, 0x01, 0xfe, 0x00, 0x82, 0x00,
                            0xbb, 0x00, 0x04, 0x01, 0xb0, 0x00, 0x9d, 0x00,
                            0x43, 0x00, 0x00, 0x00, 0x00, 0x02, 0x06, 0x04,
                            0x03, 0x08, 0x07, 0x05, 0x0f, 0x01, 0x0c, 0x09,
                            0x0b, 0x0e, 0x0d, 0x0a, 0x0a, 0x05, 0x04, 0x07,
                            0x01, 0x03, 0x0b, 0x0c, 0x09, 0x06, 0x08, 0x02,
                            0x8e, 0x01, 0x75, 0x01, 0x82, 0x00, 0x80, 0x01,
                            0xaf, 0x00, 0xeb, 0x01, 0xe7, 0x01, 0x5e, 0x01,
                            0x9a, 0x00, 0x8f, 0x00, 0x9d, 0x00, 0xe8, 0x01,
                            0x8e, 0x00, 0x7c, 0x01, 0x7d, 0x01, 0xe5, 0x00,
                            0x88, 0x01, 0x1a, 0x00, 0xfe, 0x00, 0xf9, 0x00
                        });

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

        private byte[] m_pad;
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
