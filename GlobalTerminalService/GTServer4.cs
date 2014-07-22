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
                        ulong serial = DataAbstract.Instance.BoxUpload4(record);

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

                        BoxRecord4[] results = DataAbstract.Instance.BoxSearch4(label, 20);
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2); // result code (0 for OK)
                        response.Write(BitConverter.GetBytes(results.Length), 0, 4);

                        foreach (BoxRecord4 result in results)
                        {
                            response.Write(BitConverter.GetBytes(result.PID), 0, 4);
                            response.Write(BitConverter.GetBytes((int)result.Label), 0, 4);
                            response.Write(BitConverter.GetBytes(result.SerialNumber), 0, 8);
                            response.Write(result.Data, 0, 0x21c);
                        }
                        logEntry.AppendFormat("Retrieved {0} boxes.", results.Length);
                        logEntry.AppendLine();

                    } break;
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
                        ulong serial = DataAbstract.Instance.DressupUpload4(record);

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

                        DressupRecord4[] results = DataAbstract.Instance.DressupSearch4(species, 10);
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
                        ulong serial = DataAbstract.Instance.BattleVideoUpload4(record);

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

                        logEntry.AppendFormat("Searching for ");
                        if (ranking != BattleVideoRankings4.None)
                            logEntry.AppendFormat("{0}, ", ranking);
                        if (species != 0xffff)
                            logEntry.AppendFormat("species {0}, ", species);
                        logEntry.AppendFormat("{0}", meta);
                        if (country != 0xff)
                            logEntry.AppendFormat(", country {0}", country);
                        if (region != 0xff)
                            logEntry.AppendFormat(", region {0}", region);
                        logEntry.AppendLine(".");

                        BattleVideoHeader4[] results = DataAbstract.Instance.BattleVideoSearch4(species, ranking, meta, country, region, 30);
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
                        BattleVideoRecord4 record = DataAbstract.Instance.BattleVideoGet4(serial, true);
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

        private byte Byte6(RequestTypes4 type)
        {
            switch (type)
            {
                case RequestTypes4.BoxUpload:
                case RequestTypes4.BoxSearch:
                    return 0x52;
                case RequestTypes4.DressupUpload:
                case RequestTypes4.DressupSearch:
                    return 0x4e;
                case RequestTypes4.BattleVideoUpload:
                case RequestTypes4.BattleVideoSearch:
                case RequestTypes4.BattleVideoWatch:
                    return 0x59;
                case RequestTypes4.TopTrainersHead:
                case RequestTypes4.TopTrainersSearch:
                    return 0x55;
                default:
                    return 0x00;
            }
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

        TopTrainersHead = 0xf0,
        TopTrainersSearch = 0xf1
    }
}
