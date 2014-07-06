using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public class MusicalRecord5
    {
        public MusicalRecord5()
        {
        }

        public MusicalRecord5(int pid, long serial_number, byte[] data)
        {
            if (data.Length != 560) throw new ArgumentException("Musical data must be 560 bytes.");

            PID = pid;
            SerialNumber = serial_number;
            Data = data;
            UpdateParticipants();
        }

        // todo: encapsulate these so calculated fields are always correct
        public int PID;
        public long SerialNumber;

        private byte[] m_data;
        public byte[] Data
        {
            get
            {
                // fixme: Participants don't update if the consumer modifies this array directly.
                return m_data;
            }
            set
            {
                if (m_data == value) return;
                if (value.Length != 560) throw new ArgumentException("Musical data must be 560 bytes.");

                m_data = value;
                UpdateParticipants();
            }
        }

        public MusicalParticipant5[] Participants;

        public void UpdateParticipants()
        {
            Participants = new MusicalParticipant5[4];
            if (m_data.Length != 560) throw new InvalidOperationException("Musical data must be 560 bytes.");
            for (int x = 0; x < 4; x++)
            {
                Participants[x] = new MusicalParticipant5(m_data, x * 0x58 + 0x84);
            }
        }

        public MusicalRecord5 Clone()
        {
            return new MusicalRecord5(PID, SerialNumber, Data.ToArray());
        }
    }

    public class MusicalParticipant5
    {
        public MusicalParticipant5()
        {
        }

        public MusicalParticipant5(byte[] data)
        {
            if (data.Length != 88) throw new ArgumentException("Musical Participant data must be 88 bytes.");
            Data = data;
        }

        public MusicalParticipant5(byte[] buffer, int offset)
        {
            if (buffer.Length - offset < 88) throw new ArgumentException("Not enough room in buffer to copy 88 bytes of Musical Participant data.");
            Data = new byte[88];
            Array.Copy(buffer, offset, Data, 0, 88);
        }

        public byte[] Data;

        public ushort Species
        {
            get
            {
                return BitConverter.ToUInt16(Data, 0);
            }
        }
    }
}
