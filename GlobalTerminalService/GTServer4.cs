using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PkmnFoundations.Support;

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
            MemoryStream response = new MemoryStream();
            response.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, 4); // placeholder for length
            response.WriteByte((byte)requestType);
            response.WriteByte(Byte6(requestType));

            // todo: implement each of the request types here
            switch (requestType)
            {

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
