using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PkmnFoundations.Support;
using PkmnFoundations.Structures;
using PkmnFoundations.Data;

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

        private void Initialize()
        {
            m_pad = new byte[256];

            using (FileStream s = File.Open("pad.bin", FileMode.Open))
            {
                s.Read(m_pad, 0, m_pad.Length);
                s.Close();
            }
        }

        protected override byte[] ProcessRequest(byte[] data)
        {
            int length = BitConverter.ToInt32(data, 0);
            AssertHelper.Equals(length, data.Length);

            RequestTypes4 requestType = (RequestTypes4)data[4];
            Console.WriteLine("Handling Generation IV {0} request.", requestType);

            CryptMessage(data);

            MemoryStream response = new MemoryStream();
            response.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, 4); // placeholder for length
            response.WriteByte((byte)requestType);
            response.WriteByte(Byte6(requestType));

            try
            {
                // todo: implement each of the request types here
                switch (requestType)
                {
                    case RequestTypes4.BoxUpload:
                    {
                        if (data.Length != 0x360)
                        {
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        int pid = BitConverter.ToInt32(data, 8);
                        BoxLabels4 label = (BoxLabels4)BitConverter.ToInt32(data, 0x140);
                        byte[] boxData = new byte[0x21c];
                        Array.Copy(data, 0x144, boxData, 0, 0x21c);
                        BoxRecord4 record = new BoxRecord4(pid, label, 0, boxData);
                        long serial = DataAbstract.Instance.BoxUpload4(record);

                        if (serial == 0)
                        {
                            Console.WriteLine("Uploaded box already in server.");
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        Console.WriteLine("Box uploaded successfully.");
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2); // result code (0 for OK)
                        response.Write(BitConverter.GetBytes(serial), 0, 8);

                    } break;
                    case RequestTypes4.BoxSearch:
                    {
                        if (data.Length != 0x14c)
                        {
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
                        Console.WriteLine("Retrieved {0} boxes.", results.Length);


                    } break;
                    case RequestTypes4.DressupUpload:
                    {
                        if (data.Length != 0x220)
                        {
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        int pid = BitConverter.ToInt32(data, 8);
                        byte[] dressupData = new byte[0xe0];
                        Array.Copy(data, 0x140, dressupData, 0, 0xe0);
                        DressupRecord4 record = new DressupRecord4(pid, 0, dressupData);
                        long serial = DataAbstract.Instance.DressupUpload4(record);

                        if (serial == 0)
                        {
                            Console.WriteLine("Uploaded dressup already in server.");
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        Console.WriteLine("Dressup uploaded successfully.");
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2); // result code (0 for OK)
                        response.Write(BitConverter.GetBytes(serial), 0, 8);

                    } break;
                    case RequestTypes4.DressupSearch:
                    {
                        if (data.Length != 0x14c)
                        {
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
                        Console.WriteLine("Retrieved {0} dressup results.", results.Length);

                    } break;
                    case RequestTypes4.BattleVideoUpload:
                    {

                    } break;
                    case RequestTypes4.BattleVideoSearch:
                    {

                    } break;
                    case RequestTypes4.BattleVideoWatch:
                    {

                    } break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
            }

            response.Flush();
            byte[] responseData = response.ToArray();
            WriteLength(responseData);
            CryptMessage(responseData);
            return responseData;
        }

        private void WriteLength(byte[] message)
        {
            byte[] data = BitConverter.GetBytes(message.Length);
            Array.Copy(data, 0, message, 0, 4);
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
        BattleVideoWatch = 0xda
    }
}
