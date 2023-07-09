using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace PkmnFoundations.Wfc
{
  public class TrainerProfilePlaza
  {
    public TrainerProfilePlaza()
    {

    }

    public TrainerProfilePlaza(byte[] gamestats_header, byte[] data)
    {
      if (gamestats_header.Length != 20) throw new ArgumentException("Gamestats header must be 20 bytes.");
      if (data.Length != 148) throw new ArgumentException("Profile data must be 148 bytes.");

      GamestatsHeader = gamestats_header;
      Data = data;
    }

    public byte[] GamestatsHeader; // 20 bytes
    public byte[] Data; // 148 bytes

    public int PID
    {
      get
      {
        return BitConverter.ToInt32(GamestatsHeader, 0);
      }
    }

    // Index 4 & 5 is unknown from `GameStatsHeader`

    public Versions Version
    {
      get
      {
        return (Versions)GamestatsHeader[6];
      }
    }

    public Languages Language
    {
      get
      {
        return (Languages)GamestatsHeader[7];
      }
    }

    public PhysicalAddress MacAddres
    {
      get
      {
        return new PhysicalAddress(new byte[] {
                    GamestatsHeader[8], GamestatsHeader[9], GamestatsHeader[10],
                    GamestatsHeader[11], GamestatsHeader[12], GamestatsHeader[13],
                });
      }
    }

    // Index 14-19 is unknown from `GameStatsHeader`
    // Index 0-3 is unknown from `Data`

    public uint OT
    {
      get
      {
        return BitConverter.ToUInt32(Data, 4);
      }
    }

    public EncodedString4 Name
    {
      get
      {
        return new EncodedString4(Data, 8, 16);
      }
    }

    // Index 24-31 is unknown from `Data`

    public PlazaFavoritePokemon[] FavoritePokemon
    {
      get
      {
        List<PlazaFavoritePokemon> favorites = new List<PlazaFavoritePokemon>();

        int form_idx = 44;
        int egg_idx = 50;
        foreach (var species_start in new int[] { 32, 34, 36, 38, 40, 42 })
        {
          favorites.Add(new PlazaFavoritePokemon(
              BitConverter.ToUInt16(Data, species_start),
              Data[form_idx],
              Data[egg_idx]
          ));
          form_idx += 1;
          egg_idx += 1;
        }

        return favorites.ToArray();
      }
    }

    public TrainerGenders TrainerGender
    {
      get
      {
        return (TrainerGenders)Data[56];
      }
    }

    public byte TrainerRegion
    {
      get
      {
        return Data[57];
      }
    }

    public ushort TrainerModel
    {
      get
      {
        return BitConverter.ToUInt16(Data, 58);
      }
    }

    // Bytes 60 - 61 - 62 are unknown for `Data`

    public bool PokedexComplete
    {
      get
      {
        return Data[63] == 1;
      }
    }

    public bool GameCleared
    {
      get
      {
        return Data[64] == 1;
      }
    }

    // Byte 65 is unknown for `Data`

    public Versions VersionInsideData
    {
      get
      {
        return (Versions)Data[66];
      }
    }

    // Bytes 67-136 are unknown for `Data`

    public ushort[] FavoriteMoveTypeIDs
    {
      get
      {
        return new ushort[] {
                    BitConverter.ToUInt16(Data, 136),
                    BitConverter.ToUInt16(Data, 138)
                };
      }
    }

    public TrainerProfilePlaza Clone()
    {
      return new TrainerProfilePlaza(GamestatsHeader.ToArray(), Data.ToArray());
    }
  }

  public class PlazaFavoritePokemon
  {
    public PlazaFavoritePokemon(ushort species_number, byte form, byte was_egg)
    {
      this.SpeciesNumber = species_number;
      this.Form = form;
      this.WasEgg = was_egg == 1;
    }

    public ushort SpeciesNumber;
    public byte Form;
    public bool WasEgg;
  }
}
