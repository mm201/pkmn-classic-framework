using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace PkmnFoundations.Wfc
{
    public class BattleTowerPokemon4 : BattleTowerPokemonBase
    {
        public BattleTowerPokemon4(Pokedex.Pokedex pokedex) : base(pokedex)
        {
        }

        public BattleTowerPokemon4(Pokedex.Pokedex pokedex, int species, byte form, ushort held_item, ushort[] moveset,
            uint ot, uint personality, uint ivs, byte[] evs, byte pp_ups,
            Languages language, byte ability, byte happiness, EncodedString4 nickname) : base(pokedex)
        {
            if (moveset == null) throw new ArgumentNullException("moveset");
            if (moveset.Length != 4) throw new ArgumentException("moveset");
            if (evs == null) throw new ArgumentNullException("evs");
            if (evs.Length != 6) throw new ArgumentException("evs");
            if (nickname == null) throw new ArgumentNullException("nickname");
            if (nickname.Size != 22) throw new ArgumentException("nickname");

            SpeciesID = species;
            FormID = form;
            HeldItemID = held_item;
            GetMovesFromArray(Moves, pokedex, moveset, pp_ups);
            TrainerID = ot;
            Personality = personality;
            IVs = new IvStatValues((int)ivs & 0x3fffffff);
            IvFlags = ivs & 0xc0000000u;
            EVs = new ByteStatValues(evs);
            Language = language;
            AbilityID = ability;
            Happiness = happiness;
            NicknameEncoded = nickname; // todo: clone
        }

        public BattleTowerPokemon4(Pokedex.Pokedex pokedex, int species, byte form, ushort held_item,
            ushort move1, ushort move2, ushort move3, ushort move4, uint ot, uint personality, 
            uint ivs, byte[] evs, byte pp_ups, Languages language, byte ability, byte happiness,
            EncodedString4 nickname) : 
            this(pokedex, species, form, held_item, new ushort[] { move1, move2, move3, move4 },
                ot, personality, ivs, evs, pp_ups, language, ability, happiness, nickname)
        {
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

        protected override void Save(BinaryWriter writer)
        {
            writer.Write(CombineSpeciesForm(SpeciesID, FormID));
            writer.Write((ushort)HeldItemID);

            ushort[] moveset = GetArrayFromMoves(Moves);
            for (int i = 0; i < 4; i++)
            {
                writer.Write(moveset[i]);
            }

            writer.Write(TrainerID);
            writer.Write(Personality);
            writer.Write(IVs.ToInt32() | (int)IvFlags);
            writer.Write(EVs.ToArray(), 0, 6);
            writer.Write(GetPpUpsFromMoves(Moves));
            writer.Write((byte)Language);
            writer.Write((byte)AbilityID);
            writer.Write(Happiness);
            writer.Write(NicknameEncoded.RawData, 0, 22);

            writer.Flush();
        }

        protected override void Load(BinaryReader reader)
        {
            ushort species_form = reader.ReadUInt16();
            SpeciesID = GetSpeciesFromCombined(species_form);
            FormID = GetFormFromCombined(species_form);
            HeldItemID = reader.ReadUInt16();

            ushort[] moveset = new ushort[4];
            for (int i = 0; i < 4; i++)
            {
                moveset[i] = reader.ReadUInt16();
            }

            TrainerID = reader.ReadUInt32();
            Personality = reader.ReadUInt32();
            uint ivs = reader.ReadUInt32();
            IVs = new IvStatValues((int)ivs & 0x3fffffff);
            IvFlags = ivs & 0xc0000000u;

            EVs = new ByteStatValues(reader.ReadBytes(6));
            byte ppUps = reader.ReadByte();
            GetMovesFromArray(Moves, m_pokedex, moveset, ppUps);

            Language = (Languages)reader.ReadByte();
            AbilityID = reader.ReadByte();
            Happiness = reader.ReadByte();
            NicknameEncoded = new EncodedString4(reader.ReadBytes(22));
        }

        public BattleTowerPokemon4 Clone()
        {
            uint ivsField = (uint)(IVs.ToInt32() & 0x3fffffffu) | (IvFlags & 0xc0000000u);
            ushort[] moveset = GetArrayFromMoves(Moves);
            byte ppUps = GetPpUpsFromMoves(Moves);

            BattleTowerPokemon4 result = new BattleTowerPokemon4(m_pokedex,
                SpeciesID, FormID, (ushort)HeldItemID, moveset,
                TrainerID, Personality, ivsField, EVs.ToArray(), ppUps,
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
    }
}
