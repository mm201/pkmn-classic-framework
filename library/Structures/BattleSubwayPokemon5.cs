using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;

namespace PkmnFoundations.Structures
{
    public class BattleSubwayPokemon5
    {
        public BattleSubwayPokemon5()
        {
        }

        public BattleSubwayPokemon5(byte[] data)
        {
            Load(data, 0);
        }

        public BattleSubwayPokemon5(byte[] data, int start)
        {
            Load(data, start);
        }

        public ushort Species;
        public ushort HeldItem;
        public ushort[] Moveset;
        public uint OT;
        public uint Personality;
        public uint IVs;
        public byte[] EVs;
        public byte Unknown1; // probably a bitmask of applied PP ups
        public Languages Language;
        public byte Ability;
        public byte Happiness;
        public EncodedString5 Nickname;
        public uint Unknown2;

        // todo: add IVs class with indexer?
        // byte myHp = myPokemon.IVs[Stats.HP];
        public byte IV(Stats stat)
        {
            return BattleTowerPokemon4.UnpackIV(IVs, stat);
        }

        public byte[] Save()
        {
            byte[] data = new byte[0x3c];
            MemoryStream ms = new MemoryStream(data);
            BinaryWriter writer = new BinaryWriter(ms);

            writer.Write(Species);
            writer.Write(HeldItem);
            for (int x = 0; x < 4; x++)
            {
                writer.Write(Moveset[x]);
            }
            writer.Write(OT);
            writer.Write(Personality);
            writer.Write(IVs);
            for (int x = 0; x < 6; x++)
            {
                writer.Write(EVs[x]);
            }
            writer.Write(Unknown1);
            writer.Write((byte)Language);
            writer.Write(Ability);
            writer.Write(Happiness);
            writer.Write(Nickname.RawData, 0, 0x16);
            writer.Write(Unknown2); // new to G5

            writer.Flush();
            ms.Flush();
            return data;
        }

        public void Load(byte[] data, int start)
        {
            if (start + 0x3c > data.Length) throw new ArgumentOutOfRangeException("start");

            Species = BitConverter.ToUInt16(data, 0x00 + start);
            HeldItem = BitConverter.ToUInt16(data, 0x02 + start);
            Moveset = new ushort[4];
            for (int x = 0; x < 4; x++)
            {
                Moveset[x] = BitConverter.ToUInt16(data, 0x04 + x * 2 + start);
            }
            OT = BitConverter.ToUInt32(data, 0x0c + start);
            Personality = BitConverter.ToUInt32(data, 0x10 + start);
            IVs = BitConverter.ToUInt32(data, 0x14 + start);

            EVs = new byte[6];
            for (int x = 0; x < 6; x++)
            {
                EVs[x] = data[0x18 + x + start];
            }
            Unknown1 = data[0x1e + start];
            Language = (Languages)data[0x1f + start];
            Ability = data[0x20 + start];
            Happiness = data[0x21 + start];
            Nickname = new EncodedString5(data, 0x22 + start, 0x16);
            Unknown2 = BitConverter.ToUInt32(data, 0x38);
        }

        public BattleSubwayPokemon5 Clone()
        {
            BattleSubwayPokemon5 result = new BattleSubwayPokemon5();
            result.Species = Species;
            result.HeldItem = HeldItem;
            result.Moveset = Moveset.ToArray();
            result.OT = OT;
            result.Personality = Personality;
            result.IVs = IVs;
            result.EVs = EVs.ToArray();
            result.Unknown1 = Unknown1;
            result.Language = Language;
            result.Ability = Ability;
            result.Happiness = Happiness;
            result.Nickname = new EncodedString5(Nickname.RawData.ToArray());
            result.Unknown2 = Unknown2;

            return result;
        }

    }
}
