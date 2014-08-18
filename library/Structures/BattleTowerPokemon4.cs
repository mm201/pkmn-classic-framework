using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;

namespace PkmnFoundations.Structures
{
    public class BattleTowerPokemon4
    {
        public BattleTowerPokemon4()
        {
        }

        public BattleTowerPokemon4(ushort species, ushort held_item, ushort[] moveset,
            uint ot, uint personality, uint ivs, byte[] evs, byte unknown1,
            Languages language, byte ability, byte happiness, EncodedString4 nickname)
        {
            if (moveset == null) throw new ArgumentNullException("moveset");
            if (moveset.Length != 4) throw new ArgumentException("moveset");
            if (evs == null) throw new ArgumentNullException("evs");
            if (evs.Length != 6) throw new ArgumentException("evs");
            if (nickname == null) throw new ArgumentNullException("nickname");
            if (nickname.Size != 22) throw new ArgumentException("nickname");

            Species = species;
            HeldItem = held_item;
            Moveset = moveset.ToArray();
            OT = ot;
            Personality = personality;
            IVs = ivs;
            EVs = evs.ToArray();
            Unknown1 = unknown1;
            Language = language;
            Ability = ability;
            Happiness = happiness;
            Nickname = nickname; // todo: clone
        }

        public BattleTowerPokemon4(byte[] data)
        {
            Load(data, 0);
        }

        public BattleTowerPokemon4(byte[] data, int start)
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
        public byte Unknown1;
        public Languages Language;
        public byte Ability;
        public byte Happiness;
        public EncodedString4 Nickname;

        // todo: add IVs class with indexer?
        // byte myHp = myPokemon.IVs[Stats.HP];
        public byte IV(Stats stat)
        {
            return UnpackIV(IVs, stat);
        }

        public byte[] Save()
        {
            byte[] data = new byte[0x38];
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

            writer.Flush();
            ms.Flush();
            return data;
        }

        public void Load(byte[] data, int start)
        {
            if (start + 0x38 > data.Length) throw new ArgumentOutOfRangeException("start");

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
            Nickname = new EncodedString4(data, 0x22 + start, 0x16);
        }

        public BattleTowerPokemon4 Clone()
        {
            BattleTowerPokemon4 result = new BattleTowerPokemon4();
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
            result.Nickname = new EncodedString4(Nickname.RawData.ToArray());

            return result;
        }

        // todo: move me to the actual pokemon class once it's made.
        public static byte UnpackIV(uint ivs, Stats stat)
        {
            int shift = (int)stat * 5 - 5;
            return (byte)(ivs << shift & 0x1f);
        }

        // todo: Move to IVs class
        public static uint PackIVs(byte HP, byte Attack, byte Defense, byte Speed, byte SpAttack, byte SpDefense)
        {
            return (uint)((HP & 31) |
                ((Attack & 31) << 5) |
                ((Defense & 31) << 10) |
                ((Speed & 31) << 15) |
                ((SpAttack & 31) << 20) |
                ((SpDefense & 31) << 25));
        }
    }
}
