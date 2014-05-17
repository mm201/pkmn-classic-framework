using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Configuration;
using PkmnFoundations.Data;
using PkmnFoundations.Support;

namespace bvCrawler4
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (FileStream s = File.Open("Box Upload Xor Pad.bin", FileMode.Open))
            {
                s.Read(PAD, 0, PAD.Length);
                s.Close();
            }

            Console.WriteLine("Pokémon Plat/HG/SS Battle Video Crawler by mm201");
            int pid = 207823279; // Platinum Hikari
            Directory.CreateDirectory("videos");
            DateTime last_top30 = DateTime.MinValue;

            while (true)
            {
                ulong videoId;
                try
                {
                    using (MySqlConnection db = CreateConnection())
                    {
                        db.Open();
                        videoId = DequeueVideo(db);
                        db.Close();
                    }
                }
                catch (Exception ex)
                {
                    // haven't touched the server, sleep short
                    LogError(ex);
                    Thread.Sleep(1000 * 1);
                    continue;
                }

                if (videoId == 0)
                {
                    try
                    {
                        if (last_top30 < DateTime.Now.AddHours(-1))
                        {
                            last_top30 = DateTime.Now;
                            QueueTop30(pid);
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError(ex);
                        Thread.Sleep(1000 * 60);
                        continue;
                    }

                    Console.WriteLine("Nothing to do. Idling 1 minute.");
                    Thread.Sleep(1000 * 60);
                    continue;
                }

                String formatted = FormatVideoId(videoId);
                String filename = String.Format("videos\\{0}.bin", formatted);

                if (File.Exists(filename))
                {
                    Console.WriteLine("Skipped video {0}. Already present on disk.", formatted);
                    Thread.Sleep(1000 * 1);
                    continue;
                }

                byte[] data;
                try
                {
                    data = GetBattleVideo(pid, videoId);

                    using (FileStream file = File.Create(filename))
                    {
                        file.Write(data, 0, data.Length);
                        file.Close();
                    }
                }
                catch (Exception ex)
                {
                    LogError(ex);
                    Thread.Sleep(1000 * 60);
                    continue;
                }

                Console.WriteLine("Successfully saved battle video {0}.", formatted);
                Thread.Sleep(1000 * 60);
            }
        }

        private static byte[] PAD = new byte[256];

        public static String FormatVideoId(ulong videoId)
        {
            String number = videoId.ToString("D12");
            String[] split = new String[3];
            split[0] = number.Substring(0, number.Length - 10);
            split[1] = number.Substring(number.Length - 10, 5);
            split[2] = number.Substring(number.Length - 5, 5);
            return String.Join("-", split);
        }

        public static byte[] GetBattleVideo(int pid, ulong videoId)
        {
            String formatted = FormatVideoId(videoId);
            Console.WriteLine("Attempting to retrieve battle video {0} from server.", formatted);

            byte[] data = new byte[0x14c];
            MemoryStream request = new MemoryStream(data);
            request.Write(new byte[4], 0, 4); // length goes here, see end
            request.Write(new byte[] { 0xda, 0xae, 0x00, 0x00 }, 0, 4); // request type, sanity 0000
            request.Write(BitConverter.GetBytes(pid), 0, 4); // pid, hopefully this doesn't ban me
            request.Write(new byte[] { 0x07, 0x02 }, 0, 2);
            // there is some random bytes contained in this sometimes. Could be trainer profile
            // related, I don't know...
            request.Write(new byte[0x132], 0, 0x132);

            request.Write(BitConverter.GetBytes(videoId), 0, 8);
            request.Write(new byte[] { 0x40, 0x01, 0x00, 0x00 }, 0, 4);
            request.Flush();
            Encrypt(data, 0xba);
            PutLength(data);

            byte[] response = Conversation(data);
            Console.WriteLine("Successfully retrieved {0} byte response for battle video {1}.", response.Length, formatted);
            return response;
        }

        public static void QueueTop30(int pid)
        {
            Console.WriteLine("Searching for latest 30 videos.");

            byte[] data = new byte[0x15c];
            MemoryStream request = new MemoryStream(data);
            request.Write(new byte[4], 0, 4); // length goes here, see end
            request.Write(new byte[] { 0xd9, 0xc4, 0x00, 0x00 }, 0, 4); // request type, sanity 0000
            request.Write(BitConverter.GetBytes(pid), 0, 4); // pid, hopefully this doesn't ban me
            request.Write(new byte[] { 0x0c, 0x02 }, 0, 2);
            // there is some random bytes contained in this sometimes. Could be trainer profile
            // related, I don't know...
            request.Write(new byte[0x132], 0, 0x132);
            request.Write(new byte[] {
                0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, 
                0xff, 0x00, 0x00, 0x00, 0x40, 0x01, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0xdc, 0xf6, 0x1b, 0x02,
                0x0c, 0x03, 0x00, 0x00
            }, 0, 28);
            request.Flush();
            Encrypt(data, 0xc9);
            PutLength(data);

            byte[] response = Conversation(data);
            QueueSearchResults(response);
        }

        public static void QueueSearchResults(byte[] data)
        {
            if (data.Length % 240 != 12) throw new ArgumentException("Search results blob should be 12 bytes + 240 per result.");
            Decrypt(data);
            AssertHelper.Assert(data[6] == 0x00);
            AssertHelper.Assert(data[7] == 0x00); // saaaaanity

            int count = data.Length / 240;
            Console.WriteLine("{0} results found.", count);

            // 12 bytes of header plus 240 bytes per search result.
            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                for (int x = 0; x < count; x++)
                {
                    ulong videoId = BitConverter.ToUInt64(data, 16 + x * 240);
                    QueueVideoId(db, videoId);
                }
                db.Close();
            }
        }

        public static void QueueVideoId(MySqlConnection db, ulong id)
        {
            String formatted = FormatVideoId(id);
            String filename = String.Format("videos\\{0}.bin", formatted);

            using (MySqlTransaction tran = db.BeginTransaction())
            {
                long count = (long)tran.ExecuteScalar("SELECT Count(*) FROM BattleVideoCrawlQueue WHERE SerialNumber = @serial_number", new MySqlParameter("@serial_number", id));
                if (count > 0)
                {
                    tran.Rollback();
                    Console.WriteLine("Skipped video {0}. Already present in database.", formatted);
                    return;
                }
                tran.ExecuteNonQuery("INSERT INTO BattleVideoCrawlQueue (SerialNumber, `Timestamp`) VALUES (@serial_number, NOW())", new MySqlParameter("@serial_number", id));
                tran.Commit();
                Console.WriteLine("Queued video {0}.", formatted);
            }
        }

        public static ulong DequeueVideo(MySqlConnection db)
        {
            using (MySqlTransaction tran = db.BeginTransaction())
            {
                object o = tran.ExecuteScalar("SELECT SerialNumber FROM BattleVideoCrawlQueue WHERE Complete = 0 ORDER BY `Timestamp` LIMIT 1");
                if (o == null || o == DBNull.Value)
                {
                    tran.Rollback();
                    return 0;
                }
                ulong id = (ulong)o;
                tran.ExecuteNonQuery("UPDATE BattleVideoCrawlQueue SET Complete = 1 WHERE SerialNumber = @serial_number", new MySqlParameter("@serial_number", id));
                tran.Commit();
                return id;
            }
        }

        public static void PutLength(byte[] data)
        {
            // places the actual length in the first 4 bytes.
            byte[] length = BitConverter.GetBytes(data.Length);
            Array.Copy(length, data, 4);
        }

        public static byte[] Conversation(byte[] request)
        {
            MemoryStream response = new MemoryStream();

            using (TcpClient client = new TcpClient("pkgdsprod.nintendo.co.jp", 12400))
            {
                NetworkStream s = client.GetStream();
                s.Write(request, 0, request.Length);
                s.CopyTo(response);
                s.Close();
            }
            response.Flush();
            byte[] dataResponse = response.ToArray();

            int length = BitConverter.ToInt32(dataResponse, 0);
            AssertHelper.Equals(length, dataResponse.Length);

            return dataResponse;
        }

        public static void Encrypt(byte[] data, int padOffset)
        {
            // encrypt and decrypt are the same operation...
            for (int x = 6; x < data.Length; x++)
            {
                data[x] ^= PAD[(x + padOffset) % 256];
            }
        }

        public static void Decrypt(byte[] data)
        {
            int padOffset = (Array.IndexOf(PAD, data[6]) + 250) % 256;
            Encrypt(data, padOffset);
        }

        public static MySqlConnection CreateConnection()
        {
            return new MySqlConnection(ConfigurationManager.ConnectionStrings["pkmnFoundationsConnectionString"].ConnectionString);
        }

        public static void LogError(Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
}
