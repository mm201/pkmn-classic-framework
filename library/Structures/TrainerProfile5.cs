using System;
using System.Collections.Generic;
using System.Linq;
using PkmnFoundations.Support;

namespace PkmnFoundations.Structures
{
    public class TrainerProfile5 : TrainerProfileBase
    {
        public TrainerProfile5() : base()
        {
        }

        public TrainerProfile5(int pid, byte[] data, string ip_address) : base(pid, data, ip_address)
        {
        }

        public EncodedString5 Name
        {
            get
            {
                return new EncodedString5(Data, 8, 16);
            }
            set
            {
                Array.Copy(value.RawData, 0, Data, 8, 16);
            }
        }

        public TrainerProfile5 Clone()
        {
            return new TrainerProfile5(PID, Data.ToArray(), IpAddress);
        }
    }
}
