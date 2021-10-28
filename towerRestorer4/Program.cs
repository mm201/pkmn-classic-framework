using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PkmnFoundations.Data;
using PkmnFoundations.Pokedex;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace towerRestorer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: towerRestorer4 <path>");
                Console.WriteLine("Attempts to insert files in <path>");
                Console.WriteLine("into the database in app configuration.");
                Console.WriteLine("Only inserts files whose names match the naming pattern:");
                Console.WriteLine("g*_pid*_rank*_room*");
                Console.WriteLine("Rank and room number are taken from the filename.");
                return;
            }

            Database db = Database.Instance;

            String[] filenames = Directory.GetFiles(args[0]);
            int successCount = 0;
            int failureCount = 0;
            int opponentSuccessCount = 0;
            int opponentFailureCount = 0;
            int leaderSuccessCount = 0;
            int leaderFailureCount = 0;

            Pokedex pokedex = new Pokedex(db, false);

            foreach (String s in filenames)
            {
                String filename = s;

                int slashIndex = filename.LastIndexOf(Path.DirectorySeparatorChar);
                if (slashIndex >= 0)
                {
                    filename = filename.Substring(slashIndex + 1);
                }

                int dotIndex = filename.LastIndexOf('.');
                if (dotIndex >= 0)
                {
                    filename = filename.Substring(0, dotIndex);
                }

                String[] split = filename.Split('_');

                byte rank, room;

                if (split.Length != 4 ||
                    (split[0] != "g4" && split[0] != "g5") ||
                    split[2].Substring(0, 4) != "rank" ||
                    !Byte.TryParse(split[2].Substring(4), out rank) ||
                    split[3].Substring(0, 4) != "room" ||
                    !Byte.TryParse(split[3].Substring(4), out room)
                    )
                {
                    Console.WriteLine("{0}: Filename pattern does not match, skipped.", filename);
                    failureCount++;
                    continue;
                }

                int gen = Convert.ToInt32(split[0].Substring(1));

                rank--;
                room--;

                switch (gen)
                {
                    case 4:
                    {
                        FileStream fs = File.OpenRead(s);
                        if (fs.Length != 0xa38)
                        {
                            Console.WriteLine("{0}: file size is wrong, skipped.", filename);
                            failureCount++;
                            continue;
                        }

                        byte[] data = new byte[0xa38];
                        fs.ReadBlock(data, 0, 0xa38);
                        fs.Close();

                        // battletower/download.asp response: 2616 bytes
                        // 00-63b: BattleTowerRecord objects x7
                        // 63c-a37: BattleTowerTrainerProfile objects x30
                        for (int x = 0; x < 7; x++)
                        {
                            try
                            {
                                BattleTowerRecord4 record = new BattleTowerRecord4(pokedex, data, 0xe4 * x);
                                record.PID = 0;
                                record.Rank = rank;
                                record.RoomNum = room;
                                record.BattlesWon = 7;
                                db.BattleTowerUpdateRecord4(record);
                                opponentSuccessCount++;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                opponentFailureCount++;
                            }
                        }

                        for (int x = 0; x < 30; x++)
                        {
                            try
                            {
                                BattleTowerProfile4 profile = new BattleTowerProfile4(data, 0x63c + 0x22 * x);
                                BattleTowerRecord4 record = new BattleTowerRecord4(pokedex);
                                record.Profile = profile;
                                record.PID = 0;
                                record.Rank = rank;
                                record.RoomNum = room;
                                db.BattleTowerAddLeader4(record);
                                leaderSuccessCount++;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                leaderFailureCount++;
                            }
                        }
                    } break;

                    case 5:
                    {
                        FileStream fs = File.OpenRead(s);
                        if (fs.Length != 0xab4)
                        {
                            Console.WriteLine("{0}: file size is wrong, skipped.", filename);
                            failureCount++;
                            continue;
                        }

                        byte[] data = new byte[0xab4];
                        fs.ReadBlock(data, 0, 0xab4);
                        fs.Close();

                        //web/battletower/download.asp response: 2700 bytes
                        //00-68f: BattleSubwayRecord objects x7
                        //690-a8b: BattleSubwayTrainerProfile objects x30
                        for (int x = 0; x < 7; x++)
                        {
                            try
                            {
                                BattleSubwayRecord5 record = new BattleSubwayRecord5(pokedex, data, 0xf0 * x);
                                record.PID = 0;
                                record.Rank = rank;
                                record.RoomNum = room;
                                record.BattlesWon = 7;
                                record.Unknown4 = new byte[5];
                                db.BattleSubwayUpdateRecord5(record);
                                opponentSuccessCount++;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                opponentFailureCount++;
                            }
                        }

                        for (int x = 0; x < 30; x++)
                        {
                            try
                            {
                                BattleSubwayProfile5 profile = new BattleSubwayProfile5(data, 0x690 + 0x22 * x);
                                BattleSubwayRecord5 record = new BattleSubwayRecord5(pokedex);
                                record.Profile = profile;
                                record.PID = 0;
                                record.Rank = rank;
                                record.RoomNum = room;
                                db.BattleSubwayAddLeader5(record);
                                leaderSuccessCount++;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                leaderFailureCount++;
                            }
                        }
                    } break;
                }

                Console.WriteLine("{0} complete", s);
            }

            Console.WriteLine("Added {0} files, {1} opponents, {2} leaders.", successCount, opponentSuccessCount, leaderSuccessCount);
            Console.WriteLine("Failed: {0} files, {1} opponents, {2} leaders.", failureCount, opponentFailureCount, leaderFailureCount);
            Console.ReadKey();
        }
    }
}
