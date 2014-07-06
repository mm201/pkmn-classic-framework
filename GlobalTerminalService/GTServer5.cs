using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;
using System.IO;
using PkmnFoundations.Structures;
using PkmnFoundations.Data;

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

        private void Initialize()
        {

        }

        protected override byte[] ProcessRequest(byte[] data)
        {
            int length = BitConverter.ToInt32(data, 0);
            AssertHelper.Equals(length, data.Length);

            RequestTypes5 requestType = (RequestTypes5)data[4];
            Console.WriteLine("Handling Generation V {0} request.", requestType);

            MemoryStream response = new MemoryStream();
            response.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, 4); // placeholder for length
            response.WriteByte((byte)requestType);
            response.WriteByte(Byte6(requestType));

            try
            {
                int pid = BitConverter.ToInt32(data, 8);
                byte version = data[0x0c];
                byte language = data[0x0d];

                switch (requestType)
                {
                    case RequestTypes5.MusicalUpload:
                    {
                        if (data.Length != 0x370)
                        {
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        byte[] musicalData = new byte[0x230];
                        Array.Copy(data, 0x140, musicalData, 0, 0x230);
                        MusicalRecord5 record = new MusicalRecord5(pid, 0, musicalData);
                        long serial = DataAbstract.Instance.MusicalUpload5(record);

                        if (serial == 0)
                        {
                            Console.WriteLine("Uploaded musical already in server.");
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        Console.WriteLine("Musical uploaded successfully.");
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2); // result code (0 for OK)
                        response.Write(BitConverter.GetBytes(serial), 0, 8);

                    } break;
                    case RequestTypes5.MusicalSearch:
                    {
                        if (data.Length != 0x14c)
                        {
                            response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                            break;
                        }

                        // todo: validate or log some of this?
                        ushort species = BitConverter.ToUInt16(data, 0x144);

                        MusicalRecord5[] results = DataAbstract.Instance.MusicalSearch5(species, 5);
                        response.Write(new byte[] { 0x00, 0x00 }, 0, 2); // result code (0 for OK)
                        response.Write(BitConverter.GetBytes(results.Length), 0, 4);

                        foreach (MusicalRecord5 result in results)
                        {
                            response.Write(BitConverter.GetBytes(result.PID), 0, 4);
                            response.Write(BitConverter.GetBytes(result.SerialNumber), 0, 8);
                            response.Write(result.Data, 0, 0x230);
                        }
                        Console.WriteLine("Retrieved {0} dressup results.", results.Length);

                    } break;
                    default:
                        response.Write(new byte[] { 0x02, 0x00 }, 0, 2);
                        break;
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
        BattleVideoWatch = 0xf2
    }
}
