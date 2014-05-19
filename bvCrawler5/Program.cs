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
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace PkmnFoundations
{
    public class BvCrawler5
    {
        public static void Main(string[] args)
        {
            m_upload_dir = ConfigurationManager.AppSettings["pkmnFoundationsBoxUpload5Dir"];

            Console.WriteLine("Pokémon BW/BW2 Battle Video Crawler by mm201");
            int pid = 330241374; // White 1 Jenny (?)
            Directory.CreateDirectory(String.Format("{0}", m_upload_dir));
            DateTime last_top30 = DateTime.MinValue;
            DateTime last_top_link = DateTime.MinValue;
            DateTime last_top_subway = DateTime.MinValue;
            DateTime last_retry_all = DateTime.MinValue;

            m_session_key = new byte[]{
                0x66, 0x87, 0xF1, 0xB5, 0x96, 0x47, 0x4D, 0xFB, 
                0x0E, 0x0B, 0x19, 0xBD, 0xBD, 0x69, 0x5E, 0x71,
                0x03, 0x39, 0xED, 0xB2, 0x38, 0xA7, 0xD5, 0x5A,
                0x19, 0x80, 0x09, 0xD4, 0xAA, 0x7F, 0xAE, 0xD3,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            m_session_time = DateTime.MinValue;

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
                        if (last_retry_all < DateTime.Now.AddHours(-6))
                        {
                            last_retry_all = DateTime.Now;
                            RetryAll();
                            continue;
                        }
                        if (last_top30 < DateTime.Now.AddMinutes(-60))
                        {
                            last_top30 = DateTime.Now;
                            QueueSpecial(pid, SearchSpecial.Latest30);
                            continue;
                        }
                        if (last_top_link < DateTime.Now.AddMinutes(-120))
                        {
                            last_top_link = DateTime.Now;
                            QueueSpecial(pid, SearchSpecial.TopLinkBattles);
                            continue;
                        }
                        if (last_top_subway < DateTime.Now.AddMinutes(-120))
                        {
                            last_top_subway = DateTime.Now;
                            QueueSpecial(pid, SearchSpecial.TopSubwayBattles);
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

        private static String m_upload_dir;
        private static byte[] m_session_key;
        private static String m_session_str;
        private static DateTime m_session_time;

        public static String FormatVideoId(ulong videoId)
        {
            String number = videoId.ToString("D12");
            String[] split = new String[3];
            split[0] = number.Substring(0, number.Length - 10);
            split[1] = number.Substring(number.Length - 10, 5);
            split[2] = number.Substring(number.Length - 5, 5);
            return String.Join("-", split);
        }

        public static void WriteBase64shit(Stream s)
        {
            // First 32 bytes shared by all:
            // 6687F1B596474DFB0E0B19BDBD695E710339EDB238A7D55A198009D4AA7FAED3
            //
            // The last 32 bytes seem to be random but verifiable:
            // White 1: A194196DAC7CAA4B75A9038E0FF2C71E64E61C40CFA0340178FBB9B6B72C4A63
            // White 1: F8A9E7EA602F169A146429A49AB7BF2D26BBB178AA2BCF1DA259310A263D41ED
            // Black 2: C307898C422687861BCA69B25C9FC007B6516A098A05E027BB49764EF2E8B5B1
            // Black 2: BFAA5408C6474EDDE340CF57D3405379E61A78B331191637AA6CE2818ECCE42B
            //
            // I'm guessing it's Hash(Secret + Random)
            // The bytes don't vary with a session but are changed when you reset your DS.

            if (m_session_str == null || m_session_time.AddMinutes(60) < DateTime.Now)
            {
                m_session_time = DateTime.Now;
                Random rnd = new Random();
                byte[] data = new byte[32];

                rnd.NextBytes(data);

                data = new byte[]{
                    0xF8, 0xA9, 0xE7, 0xEA, 0x60, 0x2F, 0x16, 0x9A,
                    0x14, 0x64, 0x29, 0xA4, 0x9A, 0xB7, 0xBF, 0x2D,
                    0x26, 0xBB, 0xB1, 0x78, 0xAA, 0x2B, 0xCF, 0x1D,
                    0xA2, 0x59, 0x31, 0x0A, 0x26, 0x3D, 0x41, 0xED
                };

                Array.Copy(data, 0, m_session_key, 32, 32);
                m_session_str = Convert.ToBase64String(m_session_key);
            }

            StreamWriter w = new StreamWriter(s);
            w.Write(m_session_str);
            w.Flush();
        }

        public static byte[] GetBattleVideo(int pid, ulong videoId)
        {
            String formatted = FormatVideoId(videoId);
            Console.WriteLine("Attempting to retrieve battle video {0} from server.", formatted);

            byte[] data = new byte[0x14c];
            MemoryStream request = new MemoryStream(data);
            request.Write(new byte[4], 0, 4); // length goes here, see end
            request.Write(new byte[] { 0xf2, 0x55, 0x00, 0x00 }, 0, 4); // request type, sanity 0000
            request.Write(BitConverter.GetBytes(pid), 0, 4); // pid, hopefully this doesn't ban me
            request.Write(new byte[] { 0x17, 0x02 }, 0, 2);

            WriteBase64shit(request);

            request.Write(new byte[0xda], 0, 0xda);

            request.Write(BitConverter.GetBytes(videoId), 0, 8);
            request.Write(new byte[] { 0x64, 0x00, 0x00, 0x00 }, 0, 4);
            request.Flush();
            PutLength(data);

            byte[] response = Conversation(data);

            if (response.Length < 9) throw new InvalidDataException("Battle video was not retrieved.");

            Console.WriteLine("Successfully retrieved {0} byte response for battle video {1}.", response.Length, formatted);
            return response;
        }

        public static bool RunSearch(int pid)
        {
            Random rand = new Random();

            SearchMetagames[] metagames = (SearchMetagames[])Enum.GetValues(typeof(SearchMetagames));
            int metaCount = metagames.Length - 1; // remove None from the list
            int metaIndex = rand.Next(1, metaCount + 1);
            SearchMetagames metagame = metagames[metaIndex];

            ushort species = (ushort)rand.Next(0, 649);
            byte country = 0xff;
            byte region = 0xff;

            using (MySqlConnection db = CreateConnection())
            {
                db.Open();

                if ((long)db.ExecuteScalar(
                        "SELECT Count(*) FROM BattleVideoSearchHistory5 WHERE Metagame = @metagame " +
                        "AND Species = @species AND Country = @country AND Region = @region AND Special = 0",
                        new MySqlParameter("@metagame", (int)metagame),
                        new MySqlParameter("@species", (int)species),
                        new MySqlParameter("@country", (int)country),
                        new MySqlParameter("@region", (int)region))
                    == 0)
                {
                    // exact match
                    QueueSearch(pid, SearchSpecial.None, species, metagame, country, region);
                    return true;
                }

                DataTable dt;
                dt = db.ExecuteDataTable("SELECT DISTINCT Metagame, Species, Country, Region " +
                    "FROM BattleVideoSearchHistory5 WHERE Metagame = @metagame AND Special = 0 ORDER BY Species",
                    new MySqlParameter("@metagame", (int)metagame));
                if (dt.Rows.Count < 649)
                {
                    int prevSpecies = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        if ((int)row["Species"] != prevSpecies)
                        {
                            QueueSearch(pid, SearchSpecial.None, (ushort)(prevSpecies), metagame, country, region);
                            return true;
                        }
                        prevSpecies++;
                    }
                }

                dt = db.ExecuteDataTable("SELECT DISTINCT Metagame, Species, Country, Region " +
                    "FROM BattleVideoSearchHistory5 WHERE Species = @species AND Special = 0 ORDER BY Metagame",
                    new MySqlParameter("@species", (int)species));
                if (dt.Rows.Count < metaCount)
                {
                    int prevMeta = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        if ((int)row["Metagame"] != (int)metagames[prevMeta])
                        {
                            QueueSearch(pid, SearchSpecial.None, species, metagames[prevMeta], country, region);
                            return true;
                        }
                        prevMeta++;
                    }
                }

                dt = db.ExecuteDataTable("SELECT DISTINCT Metagame, Species, Country, Region " +
                    "FROM BattleVideoSearchHistory5 WHERE Special = 0 ORDER BY Metagame, Species");
                if (dt.Rows.Count < 649 * metaCount)
                {
                    int prevSpecies = 1;
                    int prevMeta = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        if ((int)row["Species"] != prevSpecies || (int)row["Metagame"] != (int)metagames[prevMeta])
                        {
                            QueueSearch(pid, SearchSpecial.None, (ushort)(prevSpecies), metagames[prevMeta], country, region);
                            return true;
                        }
                        prevSpecies++;
                        if (prevSpecies > 649)
                        {
                            prevSpecies = 1;
                            prevMeta++;
                        }
                    }
                }
            }

            return false;
        }

        public static void QueueSpecial(int pid, SearchSpecial special)
        {
            QueueSearch(pid, special, 0x0000, SearchMetagames.None, 0x00, 0x00);
        }

        public static void QueueSearch(int pid, SearchSpecial special, ushort species, SearchMetagames meta, byte country, byte region)
        {
            switch (special)
            {
                case SearchSpecial.Latest30:
                    Console.WriteLine("Searching for latest 30 videos.");
                    break;
                case SearchSpecial.TopLinkBattles:
                    Console.WriteLine("Searching for top link battles.");
                    break;
                case SearchSpecial.TopSubwayBattles:
                    Console.WriteLine("Searching for top Subway battles.");
                    break;
                default:
                    {
                        Console.Write("Searching for ");
                        if (species != 0xffff)
                            Console.Write("species {0}, ", species);
                        if (meta != SearchMetagames.None)
                            Console.Write("{0}, ", meta);
                        if (country != 0xff)
                            Console.Write("country {0}, ", region);
                        if (region != 0xff)
                            Console.Write("region {0}", region);
                    } break;
            }

            byte[] data = new byte[0x15c];
            MemoryStream request = new MemoryStream(data);
            request.Write(new byte[4], 0, 4); // length goes here, see end
            request.Write(new byte[] { 0xf1, 0x55, 0x00, 0x00 }, 0, 4); // request type, sanity 0000
            request.Write(BitConverter.GetBytes(pid), 0, 4); // pid, hopefully this doesn't ban me
            request.Write(new byte[] { 0x14, 0x02 }, 0, 2);

            WriteBase64shit(request);

            request.Write(new byte[0xda], 0, 0xda);

            request.Write(BitConverter.GetBytes((uint)special), 0, 4);
            request.Write(BitConverter.GetBytes(species), 0, 2);
            request.Write(BitConverter.GetBytes((uint)meta), 0, 4);
            request.WriteByte(country);
            request.WriteByte(region);

            request.Write(new byte[] {
                                        0x64, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00
            }, 0, 16);
            request.Flush();
            PutLength(data);

            byte[] response = Conversation(data);

            if (special != SearchSpecial.Latest30)
            {
                using (MySqlConnection db = CreateConnection())
                {
                    db.Open();
                    db.ExecuteNonQuery("INSERT INTO BattleVideoSearchHistory5 (Metagame, Species, " +
                        "Country, Region, Special) VALUES (@metagame, @species, @country, @region, @special)",
                        new MySqlParameter("@metagame", (int)meta),
                        new MySqlParameter("@species", (int)species),
                        new MySqlParameter("@country", (int)country),
                        new MySqlParameter("@region", (int)region),
                        new MySqlParameter("@special", (int)special));
                    db.Close();
                }
            }

            QueueSearchResults(response);
        }

        public static void QueueSearchResults(byte[] data)
        {
            if (data.Length % 208 != 12) throw new InvalidDataException("Search results blob should be 12 bytes + 208 per result.");

            int count = data.Length / 208;
            Console.WriteLine("{0} results found.", count);

            if (count == 0)
            {
                // Nothing found. Sleep as to not spam the server with lots of empty searches
                Thread.Sleep(1000 * 15);
                return;
            }

            // 12 bytes of header plus 208 bytes per search result.
            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                for (int x = 0; x < count; x++)
                {
                    ulong videoId = BitConverter.ToUInt64(data, 208 + x * 208);
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
                long count = (long)tran.ExecuteScalar("SELECT Count(*) FROM BattleVideoCrawlQueue5 WHERE SerialNumber = @serial_number", new MySqlParameter("@serial_number", id));
                if (count > 0)
                {
                    tran.Rollback();
                    Console.WriteLine("Skipped video {0}. Already present in database.", formatted);
                    return;
                }
                tran.ExecuteNonQuery("INSERT INTO BattleVideoCrawlQueue5 (SerialNumber, `Timestamp`) VALUES (@serial_number, NOW())", new MySqlParameter("@serial_number", id));
                tran.Commit();
                Console.WriteLine("Queued video {0}.", formatted);
            }
        }

        public static ulong DequeueVideo(MySqlConnection db)
        {
            using (MySqlTransaction tran = db.BeginTransaction())
            {
                object o = tran.ExecuteScalar("SELECT SerialNumber FROM BattleVideoCrawlQueue5 WHERE Complete = 0 ORDER BY `Timestamp` LIMIT 1");
                if (o == null || o == DBNull.Value)
                {
                    tran.Rollback();
                    return 0;
                }
                ulong id = (ulong)o;
                tran.ExecuteNonQuery("UPDATE BattleVideoCrawlQueue5 SET Complete = 1 WHERE SerialNumber = @serial_number", new MySqlParameter("@serial_number", id));
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

            using (TcpClient client = new TcpClient("pkgdsprod.nintendo.co.jp", 12401))
            {
                SslStream sslClient = new SslStream(client.GetStream(), false, delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; });
                sslClient.AuthenticateAsClient("pkgdsprod.nintendo.co.jp");

                sslClient.Write(request, 0, request.Length);
                sslClient.CopyTo(response);
                sslClient.Close();
            }
            response.Flush();
            byte[] dataResponse = response.ToArray();

            int length = BitConverter.ToInt32(dataResponse, 0);
            AssertHelper.Equals(length, dataResponse.Length);

            return dataResponse;
        }

        public static void RetryAll()
        {
            String path = String.Format("{0}", m_upload_dir);

            using (MySqlConnection db = CreateConnection())
            {
                db.Open();

                DataTable SerialNumbers = db.ExecuteDataTable("SELECT SerialNumber FROM BattleVideoCrawlQueue5 WHERE Complete = 1");
                SerialNumbers.PrimaryKey = new DataColumn[] { SerialNumbers.Columns["SerialNumber"] };
                IEnumerable<String> filenames = Directory.EnumerateFiles(path);

                foreach (String s in filenames)
                {
                    int slash = s.LastIndexOf(Path.DirectorySeparatorChar) + 1;
                    int dot = s.LastIndexOf(".");
                    if (dot < 0) dot = s.Length;
                    if (dot < slash) dot = s.Length;

                    ulong SerialNumber;
                    UInt64.TryParse(s.Substring(slash, dot - slash).Replace("-", ""), 
                        out SerialNumber);

                    if (SerialNumber == 0) continue;
                    DataRow row = SerialNumbers.Rows.Find(SerialNumber);
                    if (row == null) continue; // video in the folder but not database. todo: insert.

                    SerialNumbers.Rows.Remove(row);
                }

                StringBuilder toRecheck = new StringBuilder();
                bool hasRows = false;

                foreach (DataRow row in SerialNumbers.Rows)
                {
                    ulong SerialNumber = (ulong)row["SerialNumber"];
                    if (hasRows) toRecheck.Append(',');
                    toRecheck.Append(SerialNumber.ToString());
                    hasRows = true;

                    Console.WriteLine("Battle video {0} in database but not in directory. Requeueing.", FormatVideoId(SerialNumber));
                }
                if (hasRows)
                {
                    db.ExecuteNonQuery("UPDATE BattleVideoCrawlQueue5 SET Complete = 0 " +
                        "WHERE SerialNumber IN (" + toRecheck.ToString() + ")");
                }

                db.Clone();
            }
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

        public enum SearchSpecial : uint
        {
            None = 0x00000000,

            // if special, all other fields are 0
            Latest30 = 0x00000001,            // 01 00 00 00 00 00 00 00 00 00 00 00
            TopLinkBattles = 0x00000003,      // 03 00 00 00 00 00 00 00 00 00 00 00
            TopSubwayBattles = 0x00000002,    // 02 00 00 00 00 00 00 00 00 00 00 00
        }

        public enum SearchMetagames : uint
        {
            None = 0x00000000,

            // Spcial (4), Species (2), Meta (4), Country (1), Region (1)
            ColosseumSingleNoLauncher = 0x00bf0018,     // 00 00 00 00 ff ff 18 00 bf 00 ff ff
            ColosseumSingleLauncher = 0x00bf0098,       // 00 00 00 00 ff ff 98 00 bf 00 ff ff
            ColosseumDoubleNoLauncher = 0x00bf0019,     // 00 00 00 00 ff ff 19 00 bf 00 ff ff
            ColosseumDoubleLauncher = 0x00bf0099,       // 00 00 00 00 ff ff 99 00 bf 00 ff ff
            ColosseumTripleNoLauncher = 0x00bf001a,     // 00 00 00 00 ff ff 1a 00 bf 00 ff ff
            ColosseumTripleLauncher = 0x00bf009a,       // 00 00 00 00 ff ff 9a 00 bf 00 ff ff
            ColosseumRotationNoLauncher = 0x00bf001b,   // 00 00 00 00 ff ff 1b 00 bf 00 ff ff
            ColosseumRotationLauncher = 0x00bf009b,     // 00 00 00 00 ff ff 9b 00 bf 00 ff ff
            ColosseumMultiNoLauncher = 0x00bf001c,      // 00 00 00 00 ff ff 1c 00 bf 00 ff ff
            ColosseumMultiLauncher = 0x00bf009c,        // 00 00 00 00 ff ff 9c 00 bf 00 ff ff

            BattleSubwaySingle = 0x003f0000,            // 00 00 00 00 ff ff 00 00 3f 00 ff ff
            BattleSubwayDouble = 0x003f0001,            // 00 00 00 00 ff ff 01 00 3f 00 ff ff
            BattleSubwayMulti = 0x003f0004,             // 00 00 00 00 ff ff 04 00 3f 00 ff ff

            RandomMatchupSingle = 0x003f0028,           // 00 00 00 00 ff ff 28 00 3f 00 ff ff
            RandomMatchupDouble = 0x003f0029,           // 00 00 00 00 ff ff 29 00 3f 00 ff ff
            RandomMatchupTriple = 0x003f002a,           // 00 00 00 00 ff ff 2a 00 3f 00 ff ff
            RandomMatchupRotation = 0x003f002b,         // 00 00 00 00 ff ff 2b 00 3f 00 ff ff
            RandomMatchupLauncher = 0x00bf00aa,         // 00 00 00 00 ff ff aa 00 bf 00 ff ff

            BattleCompetition = 0x00380038,             // 00 00 00 00 ff ff 38 00 38 00 ff ff
        }
    }
}
