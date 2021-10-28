using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;

namespace PkmnFoundations.Structures
{
    public class BattleSubwayPokemon5 : BattleTowerPokemonBase
    {
        public BattleSubwayPokemon5(Pokedex.Pokedex pokedex) : base(pokedex)
        {
        }

        public BattleSubwayPokemon5(Pokedex.Pokedex pokedex, ushort species, ushort held_item, ushort[] moveset,
            uint ot, uint personality, uint ivs, byte[] evs, byte unknown1,
            Languages language, byte ability, byte happiness, EncodedString5 nickname, uint unknown2) : base(pokedex)
        {
            if (moveset == null) throw new ArgumentNullException("moveset");
            if (moveset.Length != 4) throw new ArgumentException("moveset");
            if (evs == null) throw new ArgumentNullException("evs");
            if (evs.Length != 6) throw new ArgumentException("evs");
            if (nickname == null) throw new ArgumentNullException("nickname");
            if (nickname.Size != 22) throw new ArgumentException("nickname");

            SpeciesID = species;
            HeldItemID = held_item;
            Moveset = moveset.ToArray();
            TrainerID = ot;
            Personality = personality;
            IVs = new IvStatValues((int)ivs & 0x3fffffff);
            IvFlags = ivs & 0xc0000000u;
            EVs = new ByteStatValues(evs);
            Unknown1 = unknown1;
            Language = language;
            AbilityID = ability;
            Happiness = happiness;
            NicknameEncoded = nickname; // todo: clone
            Unknown2 = unknown2;
        }

        public BattleSubwayPokemon5(Pokedex.Pokedex pokedex, BinaryReader data) : base(pokedex)
        {
            Load(data);
        }

        public BattleSubwayPokemon5(Pokedex.Pokedex pokedex, byte[] data) : base(pokedex)
        {
            Load(data);
        }

        public BattleSubwayPokemon5(Pokedex.Pokedex pokedex, byte[] data, int offset) : base(pokedex)
        {
            Load(data, offset);
        }

        public EncodedString5 NicknameEncoded;
        public uint Unknown2;

        public override string Nickname
        {
            get
            {
                return (NicknameEncoded == null) ? null : NicknameEncoded.Text;
            }
            set
            {
                // xxx: This comes straight from Pokemon4. What we logically need here is an inheritance diamond.
                if (Nickname == value) return;
                if (NicknameEncoded == null) NicknameEncoded = new EncodedString5(value, 22);
                else NicknameEncoded.Text = value;
            }
        }

        [Obsolete("Use IVs[] indexer.")]
        public byte IV(Stats stat)
        {
            return IVs[stat];
        }

        protected override void Save(BinaryWriter writer)
        {
            writer.Write((ushort)SpeciesID);
            writer.Write((ushort)HeldItemID);
            for (int x = 0; x < 4; x++)
            {
                writer.Write(Moveset[x]);
            }
            writer.Write(TrainerID);
            writer.Write(Personality);
            writer.Write(IVs.ToInt32() | (int)IvFlags);
            writer.Write(EVs.ToArray(), 0, 6);
            writer.Write(Unknown1);
            writer.Write((byte)Language);
            writer.Write((byte)AbilityID);
            writer.Write(Happiness);
            writer.Write(NicknameEncoded.RawData, 0, 22);
            writer.Write(Unknown2); // new to G5

            writer.Flush();
        }

        protected override void Load(BinaryReader reader)
        {
            SpeciesID = reader.ReadUInt16();
            HeldItemID = reader.ReadUInt16();
            Moveset = new ushort[4];
            for (int x = 0; x < 4; x++)
            {
                Moveset[x] = reader.ReadUInt16();
            }
            TrainerID = reader.ReadUInt32();
            Personality = reader.ReadUInt32();
            uint ivs = reader.ReadUInt32();
            IVs = new IvStatValues((int)ivs & 0x3fffffff);
            IvFlags = ivs & 0xc0000000u;

            EVs = new ByteStatValues(reader.ReadBytes(6));
            Unknown1 = reader.ReadByte();
            Language = (Languages)reader.ReadByte();
            AbilityID = reader.ReadByte();
            Happiness = reader.ReadByte();
            NicknameEncoded = new EncodedString5(reader.ReadBytes(22));
            Unknown2 = reader.ReadUInt32(); // new to G5
        }

        public BattleSubwayPokemon5 Clone()
        {
            uint ivsField = (uint)(IVs.ToInt32() & 0x3fffffffu) | (IvFlags & 0xc0000000u);
            BattleSubwayPokemon5 result = new BattleSubwayPokemon5(m_pokedex,
                (ushort)SpeciesID, (ushort)HeldItemID, Moveset.ToArray(),
                TrainerID, Personality, ivsField, EVs.ToArray(), Unknown1,
                Language, (byte)AbilityID, Happiness, NicknameEncoded, Unknown2);

            return result;
        }

        public override Generations Generation
        {
            get
            {
                return Generations.Generation5;
            }
        }

        public override int Size
        {
            get
            {
                return 60;
            }
        }
    }
}
