using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PkmnFoundations.Data;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;
using PkmnFoundations.Wfc;

namespace bvRestorer5
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: bvRestorer5 <path>");
                Console.WriteLine("Attempts to insert all the files in path\ninto the database in app configuration.");
                return;
            }

            String[] filenames = Directory.GetFiles(args[0]);
            int successCount = 0;

            foreach (String filename in filenames)
            {
                FileStream fs = File.OpenRead(filename);
                if (fs.Length != 0x18b8)
                {
                    Console.WriteLine("{0}: file size is wrong, skipped.", filename);
                    continue;
                }

                byte[] data = new byte[0x18b8];
                fs.ReadBlock(data, 0, 0x18b8);
                fs.Close();

                int length = BitConverter.ToInt32(data, 0);
                if (length != 0x18b8)
                {
                    Console.WriteLine("{0}: size field is wrong, skipped.", filename);
                    continue;
                }

                if (data[4] != 0xf2)
                {
                    Console.WriteLine("{0}: request type is wrong, skipped.", filename);
                    continue;
                }

                if (data[5] != 0x55 || data[6] != 0x00 || data[7] != 0x00)
                {
                    Console.WriteLine("{0}: sanity bytes are wrong, skipped.", filename);
                    continue;
                }

                int pid = BitConverter.ToInt32(data, 0x08);
                ulong serial = BitConverter.ToUInt64(data, 0x0c);
                byte[] mainData = new byte[0x18a4];
                Array.Copy(data, 0x14, mainData, 0, 0x18a4);

                BattleVideoRecord5 record = new BattleVideoRecord5(pid, serial, mainData);

                Database.Instance.BattleVideoUpload5(record);

                Console.WriteLine("Video {0} added successfully.", BattleVideoHeader4.FormatSerial(serial));
                successCount++;
            }
            Console.WriteLine("{0} battle videos successfully added.", successCount);
            Console.ReadKey();
        }
    }
}
