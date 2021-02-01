using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public abstract class TrainerProfileBase
    {
        protected TrainerProfileBase()
        {

        }

        protected TrainerProfileBase(int pid, byte[] data)
        {
            if (data.Length != 100) throw new ArgumentException("Profile data must be 100 bytes.");

            PID = pid;
            Data = data;
        }

        // todo: encapsulate these so calculated fields are always correct
        public int PID;
        public byte[] Data; // 100 bytes

        public Versions Version
        {
            get
            {
                return (Versions)Data[0];
            }
        }

        public Languages Language
        {
            get
            {
                return (Languages)Data[1];
            }
        }

        public byte Country
        {
            get
            {
                return Data[2];
            }
        }

        public byte Region
        {
            get
            {
                return Data[3];
            }
        }

        public uint OT
        {
            get
            {
                return BitConverter.ToUInt32(Data, 4);
            }
        }

        public byte[] MacAddress
        {
            get
            {
                byte[] result = new byte[6];
                Array.Copy(Data, 28, result, 0, 6);
                return result;
            }
        }

        public string Email
        {
            get
            {
                StreamReader br = new StreamReader(new MemoryStream(Data, 36, 56), Encoding.UTF8);
                string result = br.ReadToEnd().TrimEnd('\0');
                br.Close();
                return result;
            }
        }

        public bool HasNotifications
        {
            get
            {
                return Data[92] != 0;
            }
        }

        public short ClientSecret
        {
            get
            {
                return BitConverter.ToInt16(Data, 96);
            }
        }

        public short MailSecret
        {
            get
            {
                return BitConverter.ToInt16(Data, 98);
            }
        }
    }
}
