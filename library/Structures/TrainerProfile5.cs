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

        public TrainerProfile5(int pid, byte[] data) : base(pid, data)
        {
        }

        public EncodedString5 Name
        {
            get
            {
                return new EncodedString5(Data, 8, 16);
            }
        }

        public TrainerProfile5 Clone()
        {
            return new TrainerProfile5(PID, Data.ToArray());
        }
    }
}
