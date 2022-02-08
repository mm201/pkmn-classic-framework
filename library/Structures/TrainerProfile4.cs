using System;
using System.Collections.Generic;
using System.Linq;
using PkmnFoundations.Support;

namespace PkmnFoundations.Structures
{
    public class TrainerProfile4 : TrainerProfileBase
    {
        public TrainerProfile4() : base()
        {
        }

        public TrainerProfile4(int pid, byte[] data, string ip_address) : base(pid, data, ip_address)
        {
        }

        public EncodedString4 Name
        {
            get
            {
                return new EncodedString4(Data, 8, 16);
            }
        }

        public TrainerProfile4 Clone()
        {
            return new TrainerProfile4(PID, Data.ToArray(), IpAddress);
        }
    }
}
