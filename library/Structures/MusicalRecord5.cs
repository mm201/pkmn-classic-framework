using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public class MusicalRecord5 : BinarySerializableBase
    {
        public MusicalRecord5()
        {
        }

        public MusicalRecord5(int pid, ulong serial_number, BinaryReader data)
            : base()
        {
            PID = pid;
            SerialNumber = serial_number;
            Load(data);
        }

        public MusicalRecord5(int pid, ulong serial_number, byte[] data)
            : base()
        {
            PID = pid;
            SerialNumber = serial_number;
            Load(data);
        }

        public MusicalRecord5(int pid, ulong serial_number, byte[] data, int offset)
            : base()
        {
            PID = pid;
            SerialNumber = serial_number;
            Load(data, offset);
        }

        // todo: encapsulate these so calculated fields are always correct
        public int PID { get; set; }
        public ulong SerialNumber { get; set; }

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
                if (value == null)
                {
                    m_data = null;
                    return;
                }

                if (value.Length != 560) throw new ArgumentException("Musical data must be 560 bytes.");
                m_data = value.ToArray();
                UpdateParticipants();
            }
        }

        private MusicalParticipant5[] m_participants;
        public MusicalParticipant5[] Participants
        {
            get
            {
                // fixme: Data doesn't update if the consumer modifies this array.
                // todo: Split out participants from main data.
                return m_participants;
            }
        }

        public void UpdateParticipants()
        {
            if (m_data == null)
            {
                m_participants = null;
                return;
            }

            m_participants = new MusicalParticipant5[4];
            for (int x = 0; x < 4; x++)
            {
                Participants[x] = new MusicalParticipant5(m_data, x * 0x58 + 0x84);
            }
        }

        public override int Size
        {
            get
            {
                return 560;
            }
        }

        protected override void Load(System.IO.BinaryReader reader)
        {
            m_data = reader.ReadBytes(560);
            UpdateParticipants();
        }

        protected override void Save(System.IO.BinaryWriter writer)
        {
            writer.Write(m_data);
        }

        public MusicalRecord5 Clone()
        {
            return new MusicalRecord5(PID, SerialNumber, Data);
        }
    }

    public class MusicalParticipant5 : BinarySerializableBase
    {
        public MusicalParticipant5()
        {
        }

        public MusicalParticipant5(BinaryReader data)
            : base()
        {
            Load(data);
        }

        public MusicalParticipant5(byte[] data)
            : base()
        {
            Load(data);
        }

        public MusicalParticipant5(byte[] data, int offset)
            : base()
        {
            Load(data, offset);
        }

        private byte[] m_data;
        public byte[] Data
        {
            get
            {
                return m_data;
            }
            set
            {
                if (m_data == value) return;
                if (value == null)
                {
                    m_data = null;
                    return;
                }

                if (value.Length != 88) throw new ArgumentException("Musical Participant data must be 88 bytes.");
                m_data = value.ToArray();
            }
        }

        public ushort Species
        {
            get
            {
                return BitConverter.ToUInt16(Data, 0);
            }
        }

        public override int Size
        {
            get
            {
                return 88;
            }
        }

        protected override void Load(System.IO.BinaryReader reader)
        {
            m_data = reader.ReadBytes(88);
        }

        protected override void Save(System.IO.BinaryWriter writer)
        {
            writer.Write(m_data);
        }

        public MusicalParticipant5 Clone()
        {
            return new MusicalParticipant5(Data);
        }
    }
}
