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
using System.Data;

namespace bvCrawler4
{
    public class Program
    {
        public static void Main(string[] args)
        {
            m_pad = new byte[256];

            using (FileStream s = File.Open("Box Upload Xor Pad.bin", FileMode.Open))
            {
                s.Read(m_pad, 0, m_pad.Length);
                s.Close();
            }

            m_upload_dir = ConfigurationManager.AppSettings["pkmnFoundationsBoxUpload4Dir"];

            Console.WriteLine("Pokémon Plat/HG/SS Battle Video Crawler by mm201");
            int pid = 207823279; // Platinum Hikari
            Directory.CreateDirectory(String.Format("{0}", m_upload_dir));
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
                        if (last_top30 < DateTime.Now.AddMinutes(-60))
                        {
                            last_top30 = DateTime.Now;
                            QueueTop30(pid);
                            continue;
                        }
                        else if (RunSearch(pid))
                        {
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
                String filename = String.Format("{0}\\{1}.bin", m_upload_dir, formatted);

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

        private static byte[] m_pad;
        private static String m_upload_dir;

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

        public static bool RunSearch(int pid)
        {
            Random rand = new Random();

            SearchMetagames[] metagames = (SearchMetagames[])Enum.GetValues(typeof(SearchMetagames));
            int metaCount = metagames.Length - 1; // exclude Top30 which is at the end
            int metaIndex = rand.Next(0, metaCount);
            SearchMetagames metagame = metagames[metaIndex];

            ushort species = (ushort)rand.Next(0, 493);
            byte country = 0xff;
            byte region = 0xff;

            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                if ((long)db.ExecuteScalar(
                        "SELECT Count(*) FROM BattleVideoSearchHistory WHERE Metagame = @metagame " +
                        "AND Species = @species AND Country = @country AND Region = @region",
                        new MySqlParameter("@metagame", (int)metagame),
                        new MySqlParameter("@species", (int)species),
                        new MySqlParameter("@country", (int)country),
                        new MySqlParameter("@region", (int)region))
                    == 0)
                {
                    // exact match
                    QueueSearch(pid, species, metagame, country, region);
                    return true;
                }

                DataTable dt;
                dt = db.ExecuteDataTable("SELECT DISTINCT Metagame, Species, Country, Region " +
                    "FROM BattleVideoSearchHistory WHERE Metagame = @metagame ORDER BY Species",
                    new MySqlParameter("@metagame", (int)metagame));
                if (dt.Rows.Count < 493)
                {
                    int prevSpecies = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        if ((int)row["Species"] != prevSpecies)
                        {
                            QueueSearch(pid, (ushort)(prevSpecies), metagame, country, region);
                            return true;
                        }
                        prevSpecies++;
                    }
                }

                dt = db.ExecuteDataTable("SELECT DISTINCT Metagame, Species, Country, Region " +
                    "FROM BattleVideoSearchHistory WHERE Species = @species ORDER BY Metagame",
                    new MySqlParameter("@species", (int)species));
                if (dt.Rows.Count < metaCount)
                {
                    int prevMeta = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        if ((int)row["Metagame"] != (int)metagames[prevMeta])
                        {
                            QueueSearch(pid, species, metagames[prevMeta], country, region);
                            return true;
                        }
                        prevMeta++;
                    }
                }

                dt = db.ExecuteDataTable("SELECT DISTINCT Metagame, Species, Country, Region " +
                    "FROM BattleVideoSearchHistory ORDER BY Metagame, Species");
                if (dt.Rows.Count < 493 * metaCount)
                {
                    int prevSpecies = 1;
                    int prevMeta = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        if ((int)row["Species"] != prevSpecies || (int)row["Metagame"] != (int)metagames[prevMeta])
                        {
                            QueueSearch(pid, (ushort)(prevSpecies), metagames[prevMeta], country, region);
                            return true;
                        }
                        prevSpecies++;
                        if (prevSpecies > 493)
                        {
                            prevSpecies = 1;
                            prevMeta++;
                        }
                    }
                }
            }

            return false;
        }

        public static void QueueTop30(int pid)
        {
            QueueSearch(pid, 0xffff, SearchMetagames.Latest30, 0xff, 0xff);
        }

        public static void QueueSearch(int pid, ushort species, SearchMetagames meta, byte country, byte region)
        {
            bool hasSearch = species == 0xffff && meta == SearchMetagames.Latest30 && country == 0xff
                && region == 0xff;
            if (hasSearch)
                Console.WriteLine("Searching for latest 30 videos.");
            else
            {
                Console.Write("Searching for ");
                if (species != 0xffff)
                    Console.Write("species {0}, ", species);
                if (meta != SearchMetagames.Latest30)
                    Console.Write("{0}, ", meta);
                if (country != 0xff)
                    Console.Write("country {0}, ", region);
                if (region != 0xff)
                    Console.Write("region {0}", region);
            }

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
                0x00, 0x00, 0x00, 0x00
            }, 0, 4);

            request.Write(BitConverter.GetBytes(species), 0, 2);
            request.WriteByte((byte)meta);
            request.WriteByte(country);
            request.WriteByte(region);

            request.Write(new byte[] {
                      0x00, 0x00, 0x00, 0x40, 0x01, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0xdc, 0xf6, 0x1b, 0x02,
                0x0c, 0x03, 0x00, 0x00
            }, 0, 19);
            request.Flush();
            Encrypt(data, 0xc9);
            PutLength(data);

            if (hasSearch)
            {
                using (MySqlConnection db = CreateConnection())
                {
                    db.Open();
                    db.ExecuteNonQuery("INSERT INTO BattleVideoSearchHistory (Metagame, Species, " +
                        "Country, Region) VALUES (@metagame, @species, @country, @region)",
                        new MySqlParameter("@metagame", (int)meta),
                        new MySqlParameter("@species", (int)species),
                        new MySqlParameter("@country", (int)country),
                        new MySqlParameter("@region", (int)region));
                    db.Close();
                }
            }

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

            if (count == 0)
            {
                // Nothing found. Sleep as to not spam the server with lots of empty searches
                Thread.Sleep(1000 * 30);
                return;
            }

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
                data[x] ^= m_pad[(x + padOffset) % 256];
            }
        }

        public static void Decrypt(byte[] data)
        {
            int padOffset = (Array.IndexOf(m_pad, data[6]) + 250) % 256;
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

        public enum SearchMetagames : byte
        {
            Latest30 = 0xff,

            ColosseumSingleNoRestrictions = 0xfa,
            ColosseumSingleCupMatch = 0xfb,
            ColosseumDoubleNoRestrictions = 0xfc,
            ColosseumDoubleCupMatch = 0xfd,
            ColosseumMulti = 0x0e,

            BattleTowerSingle = 0x0f,
            BattleTowerDouble = 0x10,
            BattleTowerMulti = 0x11,

            BattleFactoryLv50Single = 0x12,
            BattleFactoryLv50Double = 0x13,
            BattleFactoryLv50Multi = 0x14,

            BattleFactoryOpenSingle = 0x15,
            BattleFactoryOpenDouble = 0x16,
            BattleFactoryOpenMulti = 0x17,

            BattleHallSingle = 0x18,
            BattleHallDouble = 0x19,
            BattleHallMulti = 0x1a,

            BattleCastleSingle = 0x1b,
            BattleCastleDouble = 0x1c,
            BattleCastleMulti = 0x1d,

            BattleArcadeSingle = 0x1e,
            BattleArcadeDouble = 0x1f,
            BattleArcadeMulti = 0x20,
        }
    }
}
