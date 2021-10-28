using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;

namespace PkmnFoundations.Structures
{
    public class BattleTowerPokemon4 : BattleTowerPokemonBase
    {
        public BattleTowerPokemon4(Pokedex.Pokedex pokedex) : base(pokedex)
        {
        }

        public BattleTowerPokemon4(Pokedex.Pokedex pokedex, ushort species, ushort held_item, ushort[] moveset,
            uint ot, uint personality, uint ivs, byte[] evs, byte unknown1,
            Languages language, byte ability, byte happiness, EncodedString4 nickname) : base(pokedex)
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
        }

        public BattleTowerPokemon4(Pokedex.Pokedex pokedex, BinaryReader data) : base(pokedex)
        {
            Load(data);
        }

        public BattleTowerPokemon4(Pokedex.Pokedex pokedex, byte[] data) : base(pokedex)
        {
            Load(data);
        }

        public BattleTowerPokemon4(Pokedex.Pokedex pokedex, byte[] data, int offset) : base(pokedex)
        {
            Load(data, offset);
        }

        public EncodedString4 NicknameEncoded;
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
                if (NicknameEncoded == null) NicknameEncoded = new EncodedString4(value, 22);
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
            NicknameEncoded = new EncodedString4(reader.ReadBytes(22));
        }

        public BattleTowerPokemon4 Clone()
        {
            uint ivsField = (uint)(IVs.ToInt32() & 0x3fffffffu) | (IvFlags & 0xc0000000u);
            BattleTowerPokemon4 result = new BattleTowerPokemon4(m_pokedex,
                (ushort)SpeciesID, (ushort)HeldItemID, Moveset.ToArray(),
                TrainerID, Personality, ivsField, EVs.ToArray(), Unknown1,
                Language, (byte)AbilityID, Happiness, NicknameEncoded);

            return result;
        }

        public override Generations Generation
        {
            get
            {
                return Generations.Generation4;
            }
        }

        public override int Size
        {
            get
            {
                return 56;
            }
        }

        // todo: move me to the actual pokemon class once it's made.
        [Obsolete("Use IvStatValues.UnpackIV or IvStatValues indexer, depending on context.")]
        public static byte UnpackIV(uint ivs, Stats stat)
        {
            return IvStatValues.UnpackIV(ivs, stat);
        }

        [Obsolete("Use IvStatValues.PackIVs")]
        // todo: Move to IVs class
        public static uint PackIVs(byte HP, byte Attack, byte Defense, byte Speed, byte SpAttack, byte SpDefense)
        {
            return IvStatValues.PackIVs(HP, Attack, Defense, Speed, SpAttack, SpDefense);
        }
    }
}
