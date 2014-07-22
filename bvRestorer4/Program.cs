using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PkmnFoundations.Support;
using PkmnFoundations.Structures;
using PkmnFoundations.Data;

namespace bvRestorer4
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: bvRestorer4 <path>");
                Console.WriteLine("Attempts to insert all the files in path\ninto the database in app configuration.");
                return;
            }

            m_pad = new byte[256];
            FileStream s = File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "pad.bin");
            s.Read(m_pad, 0, m_pad.Length);
            s.Close();

            String[] filenames = Directory.GetFiles(args[0]);
            int successCount = 0;

            foreach (String filename in filenames)
            {
                FileStream fs = File.OpenRead(filename);
                if (fs.Length != 0x1d60)
                {
                    Console.WriteLine("{0}: file size is wrong, skipped.", filename);
                    continue;
                }

                byte[] data = new byte[0x1d60];
                fs.ReadBlock(data, 0, 0x1d60);
                fs.Close();

                int length = BitConverter.ToInt32(data, 0);
                if (length != 0x1d60)
                {
                    Console.WriteLine("{0}: size field is wrong, skipped.", filename);
                    continue;
                }

                if (data[4] != 0xda)
                {
                    Console.WriteLine("{0}: request type is wrong, skipped.", filename);
                    continue;
                }

                CryptMessage(data);

                if (data[5] != 0x59 || data[6] != 0x00 || data[7] != 0x00)
                {
                    Console.WriteLine("{0}: sanity bytes are wrong, skipped.", filename);
                    continue;
                }

                int pid = BitConverter.ToInt32(data, 0x08);
                ulong serial = BitConverter.ToUInt64(data, 0x0c);
                byte[] mainData = new byte[0x1d4c];
                Array.Copy(data, 0x14, mainData, 0, 0x1d4c);

                BattleVideoRecord4 record = new BattleVideoRecord4(pid, serial, mainData);

                DataAbstract.Instance.BattleVideoUpload4(record);

                Console.WriteLine("Video {0} added successfully.", BattleVideoHeader4.FormatSerial(serial));
                successCount++;
            }
            Console.WriteLine("{0} battle videos successfully added.", successCount);
            Console.ReadKey();
        }

        private static byte[] m_pad;

        private static void CryptMessage(byte[] message)
        {
            if (message.Length < 5) return;
            byte padOffset = (byte)(message[0] + message[4]);

            // encrypt and decrypt are the same operation...
            for (int x = 5; x < message.Length; x++)
                message[x] ^= m_pad[(x + padOffset) & 0xff];
        }
    }
}
