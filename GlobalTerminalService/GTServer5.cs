using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;
using System.IO;
using PkmnFoundations.Structures;
using PkmnFoundations.Data;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.Diagnostics;

namespace PkmnFoundations.GlobalTerminalService
{
    public class GTServer5 : GTServerBase
    {
        public GTServer5()
            : base(12401, true)
        {
            Initialize();
        }

        public GTServer5(int threads)
            : base(12401, true, threads)
        {
            Initialize();
        }

        public GTServer5(int threads, int timeout)
            : base(12401, true, threads, timeout)
        {
            Initialize();
        }

        public GTServer5(int threads, int timeout, X509Certificate2 certificate)
            : base(12401, true, threads, timeout, certificate)
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

            RequestTypes5 requestType = (RequestTypes5)data[4];
            StringBuilder logEntry = new StringBuilder();
            logEntry.AppendFormat("Handling Generation V {0} request.\nHost: {1}", requestType, c.Client.RemoteEndPoint);
            logEntry.AppendLine();
            EventLogEntryType type = EventLogEntryType.Information;

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
                    case RequestTypes5.MusicalUpload:
                    {
                        if (data.Length != 0x370)
                        {
                            logEntry.AppendLine("Length did not validate.");
                            type = EventLogEntryType.FailureAudit;
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        byte[] musicalData = new byte[0x230];
                        Array.Copy(data, 0x140, musicalData, 0, 0x230);
                        MusicalRecord5 record = new MusicalRecord5(pid, 0, musicalData);
                        ulong serial = DataAbstract.Instance.MusicalUpload5(record);

                        if (serial == 0)
                        {
                            logEntry.AppendLine("Uploaded musical already in server.");
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        logEntry.AppendFormat("Musical {0} uploaded successfully.", serial);
                        logEntry.AppendLine();
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2); // result code (0 for OK)
                        response.Write(BitConverter.GetBytes(serial), 0, 8);

                    } break;
                    case RequestTypes5.MusicalSearch:
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

                        logEntry.AppendFormat("Searching for musical photos of species {0}.", species);
                        logEntry.AppendLine();

                        MusicalRecord5[] results = DataAbstract.Instance.MusicalSearch5(species, 5);
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2); // result code (0 for OK)
                        response.Write(BitConverter.GetBytes(results.Length), 0, 4);

                        foreach (MusicalRecord5 result in results)
                        {
                            response.Write(BitConverter.GetBytes(result.PID), 0, 4);
                            response.Write(BitConverter.GetBytes(result.SerialNumber), 0, 8);
                            response.Write(result.Data, 0, 0x230);
                        }
                        logEntry.AppendFormat("Retrieved {0} dressup results.", results.Length);
                        logEntry.AppendLine();

                    } break;

                    case RequestTypes5.BattleVideoUpload:
                    {
                        if (data.Length != 0x1ae8)
                        {
                            logEntry.AppendLine("Length did not validate.");
                            type = EventLogEntryType.FailureAudit;
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }
                        int sigLength = BitConverter.ToInt32(data, 0x19e4);
                        if (sigLength > 0x100 || sigLength < 0x00)
                        {
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        byte[] battlevidData = new byte[0x18a4];

                        Array.Copy(data, 0x140, battlevidData, 0, 0x18a4);
                        BattleVideoRecord5 record = new BattleVideoRecord5(pid, 0, battlevidData);
                        byte[] vldtSignature = new byte[sigLength];
                        Array.Copy(data, 0x19e8, vldtSignature, 0, sigLength);
                        // todo: validate signature.

                        ulong serial = DataAbstract.Instance.BattleVideoUpload5(record);

                        if (serial == 0)
                        {
                            logEntry.AppendLine("Uploaded battle video already in server.");
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        logEntry.AppendFormat("Battle video {0} uploaded successfully.", BattleVideoHeader4.FormatSerial(serial));
                        logEntry.AppendLine();
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2); // result code (0 for OK)
                        response.Write(BitConverter.GetBytes(serial), 0, 8);

                    } break;
                    case RequestTypes5.BattleVideoSearch:
                    {
                        if (data.Length != 0x15c)
                        {
                            logEntry.AppendLine("Length did not validate.");
                            type = EventLogEntryType.FailureAudit;
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        // todo: validate or log some of this?
                        BattleVideoRankings5 ranking = (BattleVideoRankings5)BitConverter.ToUInt32(data, 0x140);
                        ushort species = BitConverter.ToUInt16(data, 0x144);
                        BattleVideoMetagames5 meta = (BattleVideoMetagames5)data[0x146];
                        
                        // Byte 148 contains a magic number related to the searched metagame.
                        // If 0, disable metagame search. Metagame being 00 is insufficient
                        // since that value could mean Battle Subway Single.
                        if (data[0x148] == 0x00) meta = BattleVideoMetagames5.SearchNone;

                        byte country = data[0x14a];
                        byte region = data[0x14b];

                        logEntry.Append("Searching for ");
                        if (ranking != BattleVideoRankings5.None)
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

                        BattleVideoHeader5[] results = DataAbstract.Instance.BattleVideoSearch5(species, ranking, meta, country, region, 30);
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2); // result code (0 for OK)
                        response.Write(BitConverter.GetBytes(results.Length), 0, 4);

                        foreach (BattleVideoHeader5 result in results)
                        {
                            response.Write(BitConverter.GetBytes(result.PID), 0, 4);
                            response.Write(BitConverter.GetBytes(result.SerialNumber), 0, 8);
                            response.Write(result.Data, 0, 0xc4);
                        }
                        logEntry.AppendFormat("Retrieved {0} battle video results.", results.Length);
                        logEntry.AppendLine();

                    } break;
                    case RequestTypes5.BattleVideoWatch:
                    {
                        if (data.Length != 0x14c)
                        {
                            logEntry.AppendLine("Length did not validate.");
                            type = EventLogEntryType.FailureAudit;
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        ulong serial = BitConverter.ToUInt64(data, 0x140);
                        BattleVideoRecord5 record = DataAbstract.Instance.BattleVideoGet5(serial, true);
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
                        response.Write(record.Header.Data, 0, 0xc4);
                        response.Write(record.Data, 0, 0x17e0);
                        logEntry.AppendFormat("Retrieved battle video {0}.", BattleVideoHeader4.FormatSerial(serial));
                        logEntry.AppendLine();

                    } break;

                    // todo: A mysterious 0xf3 request type is appearing in my logs. Implement.

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
                response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
            }

            response.Flush();
            byte[] responseData = response.ToArray();
            WriteLength(responseData);

            LogHelper.Write(logEntry.ToString(), type);
            return responseData;
        }

        private byte Byte6(RequestTypes5 type)
        {
            switch (type)
            {
                case RequestTypes5.MusicalUpload:
                case RequestTypes5.MusicalSearch:
                    return 0x52;
                case RequestTypes5.BattleVideoUpload:
                case RequestTypes5.BattleVideoSearch:
                case RequestTypes5.BattleVideoWatch:
                case RequestTypes5.BattleVideoSaved:
                    return 0x55;
                default:
                    return 0x00;
            }
        }

        public override string Title
        {
            get 
            {
                return "Generation V Global Terminal";
            }
        }
    }

    internal enum RequestTypes5 : byte
    {
        MusicalUpload = 0x08,
        MusicalSearch = 0x09,

        BattleVideoUpload = 0xf0,
        BattleVideoSearch = 0xf1,
        BattleVideoWatch = 0xf2,
        BattleVideoSaved = 0xf3
    }
}
