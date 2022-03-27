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

        protected TrainerProfileBase(int pid, byte[] data, string ip_address)
        {
            if (data.Length != 100) throw new ArgumentException("Profile data must be 100 bytes.");

            PID = pid;
            Data = data;
            IpAddress = ip_address;
        }

        // todo: encapsulate these so calculated fields are always correct
        public int PID;
        public byte[] Data; // 100 bytes
        public string IpAddress;

        public Versions Version
        {
            get
            {
                return (Versions)Data[0];
            }
            set
            {
                Data[0] = (byte)value;
            }
        }

        public Languages Language
        {
            get
            {
                return (Languages)Data[1];
            }
            set
            {
                Data[1] = (byte)value;
            }
        }

        public byte Country
        {
            get
            {
                return Data[2];
            }
            set
            {
                Data[2] = value;
            }
        }

        public byte Region
        {
            get
            {
                return Data[3];
            }
            set
            {
                Data[3] = value;
            }
        }

        public uint OT
        {
            get
            {
                return BitConverter.ToUInt32(Data, 4);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, Data, 4, 4);
            }
        }

        /*
        // xxx: This would require C# 9 covariant return types to work. https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/covariant-returns
        public abstract EncodedStringBase Name
        {
            get;
        }
        */

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
