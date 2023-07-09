using System;
using System.IO;
using System.Collections.Generic;

namespace PkmnFoundations.Wfc
{
  /// <summary>The schedule that a plaza will follow.</summary>
  public class PlazaSchedule
  {
    public PlazaSchedule(uint lock_after_seconds, byte[] unk, uint bitflags, PlazaRoomType room_type, PlazaRoomSeason season, PlazaEventAndTime[] schedule)
    {
      this.LockAfterSeconds = lock_after_seconds;
      this.Unk = unk;
      this.BitFlags = bitflags;
      this.RoomType = room_type;
      this.Season = season;
      this.Schedule = schedule;
    }

    /// <summary>
    /// When to close the room and force people from joining. Usually near the
    /// end of the room.
    /// </summary>
    public uint LockAfterSeconds;
    /// <summary>
    ///  An unknown series of 4 bytes. TODO: figure out
    /// </summary>
    public byte[] Unk;
    /// <summary>
    /// A series of bit flags that change behavior.
    ///
    /// The only one identified is: 0x1 -- which in pokemon heart gold causes arceus to have a real footprint.
    /// </summary>
    public uint BitFlags;
    /// <summary>
    /// The type of room this schedule is for.
    /// </summary>
    public PlazaRoomType RoomType;
    /// <summary>
    /// What seasonal styling should be applied to this room.
    /// </summary>
    public PlazaRoomSeason Season;
    /// <summary>
    /// The actual underlying schedule.
    /// </summary>
    public PlazaEventAndTime[] Schedule;

    public byte[] Save()
    {
      byte[] serialized = new byte[16 + (8 * Schedule.Length)];
      MemoryStream ms = new MemoryStream(serialized);
      // BinaryWriter uses little endian which is what we want.
      BinaryWriter writer = new BinaryWriter(ms);

      writer.Write(LockAfterSeconds);
      writer.Write(Unk);
      writer.Write(BitFlags);
      writer.Write((byte)RoomType);
      writer.Write((byte)Season);
      writer.Write((ushort)Schedule.Length);
      foreach (PlazaEventAndTime eventAndTime in Schedule)
      {
        writer.Write(eventAndTime.AfterSeconds);
        writer.Write((int)eventAndTime.Event);
      }

      writer.Flush();
      ms.Flush();
      return serialized;
    }

    public static PlazaSchedule Load(byte[] data, int start)
    {
      uint lockAfter = BitConverter.ToUInt32(data, start);
      byte[] unk = new byte[] { data[start + 4], data[start + 5], data[start + 6], data[start + 7] };
      uint bitflags = BitConverter.ToUInt32(data, start + 8);
      PlazaRoomType roomType = (PlazaRoomType)data[start + 12];
      PlazaRoomSeason season = (PlazaRoomSeason)data[start + 13];

      ushort scheduleLength = BitConverter.ToUInt16(data, start + 14);
      ushort dataIdx = 16;
      List<PlazaEventAndTime> schedule = new List<PlazaEventAndTime>();
      for (ushort eventIdx = 0; eventIdx < scheduleLength; ++eventIdx)
      {
        schedule.Add(new PlazaEventAndTime(
            BitConverter.ToInt32(data, dataIdx),
            BitConverter.ToInt32(data, dataIdx + 4)
        ));
        dataIdx += 8;
      }
      return new PlazaSchedule(lockAfter, unk, bitflags, roomType, season, schedule.ToArray());
    }

    public static PlazaSchedule Generate()
    {
      Random rng = new Random();

      PlazaRoomType room = RoomSampler.Sample();
      uint arceusFlag = 0x0;
      // love flipping coins.
      if (rng.Next(0, 2) == 1)
      {
        arceusFlag = 0x1;
      }
      PlazaRoomSeason season = PlazaRoomSeason.None;
      // Let's flip a coin again to see if should do any seasons at all.
      if (rng.Next(0, 2) == 1)
      {
        // We give our current season a 62.5% chance of being selected, everything else a 12.5%.
        double[] seasonWeights = new double[] { 0.125, 0.125, 0.125, 0.125 };
        int dayOfYear = DateTime.Now.DayOfYear;
        if (dayOfYear >= 80 && dayOfYear < 172)
        {
          seasonWeights[0] = 0.625;
        }
        else if (dayOfYear >= 172 && dayOfYear < 264)
        {
          seasonWeights[1] = 0.625;
        }
        else if (dayOfYear >= 264 && dayOfYear < 355)
        {
          seasonWeights[2] = 0.625;
        }
        else
        {
          seasonWeights[3] = 0.625;
        }
        Support.AliasTable<PlazaRoomSeason> seasonPicker = Support.AliasTable<PlazaRoomSeason>.NewWithWeights(new Dictionary<PlazaRoomSeason, double>
        {
          [PlazaRoomSeason.Spring] = seasonWeights[0],
          [PlazaRoomSeason.Summer] = seasonWeights[1],
          [PlazaRoomSeason.Fall] = seasonWeights[2],
          [PlazaRoomSeason.Winter] = seasonWeights[3]
        });
        season = seasonPicker.Sample();
      }

      PlazaEventAndTime[] schedule = ScheduleSampler.Sample();
      return new PlazaSchedule(
          (uint)schedule[schedule.Length - 1].AfterSeconds,
          new byte[] { 0, 0, 0, 0 },
          arceusFlag,
          room,
          season,
          schedule
      );
    }

    /// <summary>
    /// The random choices for generating a room, and their associated weights.
    ///
    /// These weights are roughly based off of what the PEERCHAT server which is written in python
    /// does. The weights in that project were chosen incredibly roughly. Basically each base room
    /// has little bit less than 1/4th of a chance of appearing. With the Mew room having a 2% chance.
    /// </summary>
    private static Support.AliasTable<PlazaRoomType> RoomSampler = Support.AliasTable<PlazaRoomType>.NewWithWeights(new Dictionary<PlazaRoomType, double>
    {
      [PlazaRoomType.Fire] = 0.244,
      [PlazaRoomType.Water] = 0.244,
      [PlazaRoomType.Grass] = 0.244,
      [PlazaRoomType.Electric] = 0.244,
      [PlazaRoomType.Mew] = 0.024,
    });
    /// <summary>
    /// Random choices for generating a schedule. All are around ~33%, with a slight preference to the actually captured short 20 minutes so we add up to 100%.
    /// </summary>
    private static Support.AliasTable<PlazaEventAndTime[]> ScheduleSampler = Support.AliasTable<PlazaEventAndTime[]>.NewWithWeights(new Dictionary<PlazaEventAndTime[], double>
    {
      [RawTimeTables[0]] = 0.34,
      [RawTimeTables[1]] = 0.33,
      [RawTimeTables[2]] = 0.33,
    });
    /// <summary>
    /// A list of time tables to choose from when generating a schedule.
    ///
    /// We've only gotten a confirmed capture from a 20 minute time schedule which was
    /// the lowest time ever reported. So we've created two other schedules at 25, and
    /// 30 minutes where we just offset the 20 minute schedule so it still hopefully
    /// feels real?
    ///
    /// Maybe someday we should create our own time tables.
    /// </summary>
    private static PlazaEventAndTime[][] RawTimeTables = new PlazaEventAndTime[][] {
            // Straight from a real schedule -- 20 minutes.
            new PlazaEventAndTime[] {
                new PlazaEventAndTime(0, PlazaEvent.OverheadLightingBase),
                new PlazaEventAndTime(0, PlazaEvent.StatueLightingBase),
                new PlazaEventAndTime(0, PlazaEvent.SpotlightLightingBase),
                new PlazaEventAndTime(780, PlazaEvent.StatueEndingPhaseOne),
                new PlazaEventAndTime(840, PlazaEvent.OverheadEndingPhaseOne),
                new PlazaEventAndTime(840, PlazaEvent.StatueEndingPhaseTwo),
                new PlazaEventAndTime(900, PlazaEvent.OverheadEndingPhaseTwo),
                new PlazaEventAndTime(900, PlazaEvent.OverheadEndingPhaseThree),
                new PlazaEventAndTime(900, PlazaEvent.SpotlightEndingPhaseOne),
                new PlazaEventAndTime(960, PlazaEvent.OverheadEndingPhaseThree),
                new PlazaEventAndTime(960, PlazaEvent.StatueEndingPhaseTwo),
                new PlazaEventAndTime(960, PlazaEvent.SpotlightEndingPhaseTwo),
                new PlazaEventAndTime(960, PlazaEvent.EndAllMinigames),
                new PlazaEventAndTime(1020, PlazaEvent.OverheadEndingPhaseFour),
                new PlazaEventAndTime(1020, PlazaEvent.SpotlightEndingPhaseThree),
                new PlazaEventAndTime(1020, PlazaEvent.StartFireworks),
                new PlazaEventAndTime(1075, PlazaEvent.CreateParade),
                new PlazaEventAndTime(1080, PlazaEvent.OverheadEndingPhaseFive),
                new PlazaEventAndTime(1080, PlazaEvent.SpotlightEndingPhaseTwo),
                new PlazaEventAndTime(1080, PlazaEvent.EndFireworks),
                new PlazaEventAndTime(1140, PlazaEvent.SpotlightLightingBase),
                new PlazaEventAndTime(1200, PlazaEvent.ClosePlaza)
            },
            // 25 minute 'schedule' is just the 20 minute schedule that's been offset by 5 minutes.
            new PlazaEventAndTime[] {
                new PlazaEventAndTime(0, PlazaEvent.OverheadLightingBase),
                new PlazaEventAndTime(0, PlazaEvent.StatueLightingBase),
                new PlazaEventAndTime(0, PlazaEvent.SpotlightLightingBase),
                new PlazaEventAndTime(1080, PlazaEvent.StatueEndingPhaseOne),
                new PlazaEventAndTime(1140, PlazaEvent.OverheadEndingPhaseOne),
                new PlazaEventAndTime(1140, PlazaEvent.StatueEndingPhaseTwo),
                new PlazaEventAndTime(1200, PlazaEvent.OverheadEndingPhaseTwo),
                new PlazaEventAndTime(1200, PlazaEvent.OverheadEndingPhaseThree),
                new PlazaEventAndTime(1200, PlazaEvent.SpotlightEndingPhaseOne),
                new PlazaEventAndTime(1260, PlazaEvent.OverheadEndingPhaseThree),
                new PlazaEventAndTime(1260, PlazaEvent.StatueEndingPhaseTwo),
                new PlazaEventAndTime(1260, PlazaEvent.SpotlightEndingPhaseTwo),
                new PlazaEventAndTime(1260, PlazaEvent.EndAllMinigames),
                new PlazaEventAndTime(1320, PlazaEvent.OverheadEndingPhaseFour),
                new PlazaEventAndTime(1320, PlazaEvent.SpotlightEndingPhaseThree),
                new PlazaEventAndTime(1320, PlazaEvent.StartFireworks),
                new PlazaEventAndTime(1375, PlazaEvent.CreateParade),
                new PlazaEventAndTime(1380, PlazaEvent.OverheadEndingPhaseFive),
                new PlazaEventAndTime(1380, PlazaEvent.SpotlightEndingPhaseTwo),
                new PlazaEventAndTime(1380, PlazaEvent.EndFireworks),
                new PlazaEventAndTime(1440, PlazaEvent.SpotlightLightingBase),
                new PlazaEventAndTime(1500, PlazaEvent.ClosePlaza)
            },
            // 30 minute 'schedule' is just the 20 minute schedule that's been offset by 10 minutes.
            new PlazaEventAndTime[] {
                new PlazaEventAndTime(0, PlazaEvent.OverheadLightingBase),
                new PlazaEventAndTime(0, PlazaEvent.StatueLightingBase),
                new PlazaEventAndTime(0, PlazaEvent.SpotlightLightingBase),
                new PlazaEventAndTime(1380, PlazaEvent.StatueEndingPhaseOne),
                new PlazaEventAndTime(1440, PlazaEvent.OverheadEndingPhaseOne),
                new PlazaEventAndTime(1440, PlazaEvent.StatueEndingPhaseTwo),
                new PlazaEventAndTime(1500, PlazaEvent.OverheadEndingPhaseTwo),
                new PlazaEventAndTime(1500, PlazaEvent.OverheadEndingPhaseThree),
                new PlazaEventAndTime(1500, PlazaEvent.SpotlightEndingPhaseOne),
                new PlazaEventAndTime(1560, PlazaEvent.OverheadEndingPhaseThree),
                new PlazaEventAndTime(1560, PlazaEvent.StatueEndingPhaseTwo),
                new PlazaEventAndTime(1560, PlazaEvent.SpotlightEndingPhaseTwo),
                new PlazaEventAndTime(1560, PlazaEvent.EndAllMinigames),
                new PlazaEventAndTime(1620, PlazaEvent.OverheadEndingPhaseFour),
                new PlazaEventAndTime(1620, PlazaEvent.SpotlightEndingPhaseThree),
                new PlazaEventAndTime(1620, PlazaEvent.StartFireworks),
                new PlazaEventAndTime(1675, PlazaEvent.CreateParade),
                new PlazaEventAndTime(1680, PlazaEvent.OverheadEndingPhaseFive),
                new PlazaEventAndTime(1680, PlazaEvent.SpotlightEndingPhaseTwo),
                new PlazaEventAndTime(1680, PlazaEvent.EndFireworks),
                new PlazaEventAndTime(1740, PlazaEvent.SpotlightLightingBase),
                new PlazaEventAndTime(1800, PlazaEvent.ClosePlaza)
            }
        };
  }

  public class PlazaEventAndTime
  {
    public PlazaEventAndTime(int afterSeconds, int plazaEvent)
    {
      this.AfterSeconds = afterSeconds;
      this.Event = (PlazaEvent)plazaEvent;
    }

    public PlazaEventAndTime(int afterSeconds, PlazaEvent plazaEvent)
    {
      this.AfterSeconds = afterSeconds;
      this.Event = plazaEvent;
    }

    /// <summary>How many seconds after the room opens, this event should happen at.</summary>
    public int AfterSeconds;
    /// <summary>The event that should actually happen.</summary>
    public PlazaEvent Event;
  }

  /// <summary>
  /// The "type" of room that gets loaded, this basically just changes what color the room is,
  /// what standees are being used, and what the center standee is.
  /// </summary>
  public enum PlazaRoomType
  {
    /// <summary>
    /// Fire type room, so the base room theme is red, with standees of fire type starters.
    /// </summary>
    Fire = 0,
    /// <summary>
    /// Water type room, so the base room theme is blue, with standees of water type starters.
    /// </summary>
    Water = 1,
    /// <summary>
    /// Electric type room, so the base room theme is yellow, with standees of the electric mouse baby pokemon
    /// (specifically Pichu, Plusle, Minun, and Pachirisu).
    /// </summary>
    Electric = 2,
    /// <summary>
    /// Grass type room, so the base room theme is green, with standees of grass type starters.
    /// </summary>
    Grass = 3,
    /// <summary>
    /// The 'special', or 'rare' mew themed room. All standees are replaced with lamps, the center display
    /// is replaced with a giant statue of Mew, and this room CANNOT be themed with seasons.
    /// </summary>
    Mew = 4
  }

  /// <summary>
  /// A season which basically overlays an existing rooms floors, and trees. Changing it to look like a specific
  /// season. It has no functional differences.
  /// </summary>
  public enum PlazaRoomSeason
  {
    /// <summary>Load just the base room theme. No season styling.</summary>
    None = 0,
    Spring = 1,
    Summer = 2,
    Fall = 3,
    Winter = 4
  }

  /// <summary>
  /// An 'event' or cause a predetermined event to happen within a WiFi-Plaza.
  ///
  /// NOTE: many of these events intrinsically _imply_ that another event also happens. If you send one event,
  /// the games own internal logic may choose to apply yet another event.
  /// </summary>
  public enum PlazaEvent
  {
    /// <summary>Lock the room, preventing new visitors from entering.</summary>
    LockRoom = 0,
    /// <summary>
    /// Initialize the overhead lighting as it's "base" color and lighting
    /// values.
    ///
    /// This is always implied, though the game should still be sent this event
    /// at second 0.
    /// </summary>
    OverheadLightingBase = 1,
    /// <summary>
    /// Start the 'ending' lighting sequence of the room.
    ///
    /// If this is sent, the game will also force starting the spotlights. Though
    /// again the game should be sent this event at the same time as the
    /// spotlight starting.
    ///
    /// the questionnaire person will also start hopping.
    ///
    /// This slightly darkens the room, spotlights start, and the man starts
    /// jumping.
    /// </summary>
    OverheadEndingPhaseOne = 2,
    /// <summary>
    /// Phase 2 of the ending lighting sequence of the room.
    ///
    /// If this is sent, the game will also force starting the spotlights. Though
    /// again the game should have already sent the event with the spotlights
    /// starting.
    ///
    /// the questionnair person will also start hopping.
    ///
    /// Compared to phase 1, the room just darkens more, but is not dark enough
    /// for it to be fully 'dark'. Just like an afternoon shade.
    /// </summary>
    OverheadEndingPhaseTwo = 3,
    /// <summary>
    /// Phase 3 of the ending lighting sequence of the room.
    ///
    /// If this is sent, the game will also force starting the spotlights. Though
    /// again the game should have already sent the event with the spotlights
    /// starting.
    ///
    /// the questionnair person will also start hopping.
    ///
    /// This makes the room actually 'dark' compared to phase two.
    /// </summary>
    OverheadEndingPhaseThree = 4,
    /// <summary>
    /// Phase 4 of the ending lighting sequence of the room.
    ///
    /// If this is sent, the game will also force starting the spotlights. Though
    /// again the game should have already sent the event with the spotlights
    /// starting.
    ///
    /// the questionnair person will also start hopping.
    ///
    /// Compared to phase three, there are significantly more sparkles in the
    /// room at this point.
    /// </summary>
    OverheadEndingPhaseFour = 5,
    /// <summary>
    /// Phase 5 of the ending lighting sequence of the room.
    ///
    /// If this is sent, the game will also force starting the spotlights. Though
    /// again the game should have already sent the event with the spotlights
    /// starting.
    ///
    /// the questionnair person will also start hopping.
    ///
    /// Compared to phase four, the room darkens once more and is at it's darkest
    /// point.
    /// </summary>
    OverheadEndingPhaseFive = 6,
    /// <summary>
    /// Initialize the statue lighting as it's "base" color and lighting
    /// values.
    ///
    /// This is always implied, though the game should still be sent this event
    /// at second 0.
    /// </summary>
    StatueLightingBase = 7,
    /// <summary>
    /// Phase one of the statue ending lighting.
    ///
    /// Should be sent at the same time as overhead ending phase one most of the
    /// time.
    ///
    /// Compared to the base lighting the statue starts having the back of the
    /// statue very slightly darkened.
    /// </summary>
    StatueEndingPhaseOne = 8,
    /// <summary>
    /// Phase two of the statue ending lighting.
    ///
    /// Should be sent at the same time as overhead ending phase three most of
    /// the time.
    ///
    /// Compared to phase one the statue front now has a light in front of the
    /// statue to make the contrast of the statue higher, so it's pops out more.
    /// </summary>
    StatueEndingPhaseTwo = 9,
    /// <summary>
    /// Phase three of the statue ending lighting.
    ///
    /// Should be sent at the same time as overhead ending phase five most of
    /// the time.
    ///
    /// Compared to phase two the light on the front of the statue goes away, and
    /// it gets dark all the way around.
    /// </summary>
    StatueEndingPhaseThree = 10,
    /// <summary>
    /// Initialize the spotlight lighting as it's "base" color and lighting
    /// values.
    ///
    /// This is always implied, though the game should still be sent this event
    /// at second 0.
    /// </summary>
    SpotlightLightingBase = 11,
    /// <summary>
    /// Phase one of the spotlights when the room is ending.
    ///
    /// This should be sent at the same time as overhead ending phase one, and
    /// also causes the spotlights to start, and the question man to start
    /// hopping.
    /// </summary>
    SpotlightEndingPhaseOne = 12,
    /// <summary>Phase two of the spotlights when the room is ending.</summary>
    SpotlightEndingPhaseTwo = 13,
    /// <summary>Phase three of the spotlights when the room is ending.</summary>
    SpotlightEndingPhaseThree = 14,
    /// <summary>Force end all the minigames, and don't let people play anymore.</summary>
    EndAllMinigames = 15,
    /// <summary>
    /// Start the fireworks in the lobby.
    ///
    /// This happens usually about ~3 minutes before room end.
    /// </summary>
    StartFireworks = 16,
    /// <summary>Stop the fireworks in the lobby</summary>
    EndFireworks = 17,
    /// <summary>
    /// Create the parade, and get the floats to start going through.
    ///
    /// This normally happens about a minute after the fireworks start.
    /// </summary>
    CreateParade = 18,
    /// <summary>This forcefully closes the plaza.</summary>
    ClosePlaza = 19
  }
}
