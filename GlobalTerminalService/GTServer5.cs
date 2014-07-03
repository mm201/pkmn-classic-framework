using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;
using System.IO;

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
