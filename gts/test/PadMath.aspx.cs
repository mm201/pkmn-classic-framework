using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;

namespace PkmnFoundations.GTS.test
{
    public partial class PadMath : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FileStream s = File.Open(Server.MapPath("~/Box Upload Xor Pad.bin"), FileMode.Open);
            byte[] pad = new byte[s.Length];
            s.Read(pad, 0, pad.Length);
            s.Close();

            byte[] result = new byte[256];

            // difference between items
            for (int x = 0; x < pad.Length; x++)
            {
                byte bx = (byte)x;
                byte prevOffs = (byte)(bx - 4);
                byte value = (byte)(((pad[bx] ^ pad[prevOffs]) & 0x80) > 0 ? 0xff : 0x00);
                result[x] = value;
            }

            litRandom.Text = RenderHex(result.ToHexStringLower());
        }


        private String RenderHex(String hex)
        {
            StringBuilder builder = new StringBuilder();
            for (int x = 0; x < hex.Length; x += 16)
            {
                if (x % 32 == 0)
                {
                    builder.Append((x >> 1).ToString("x4"));
                    builder.Append(": ");
                }

                builder.Append(hex.Substring(x, Math.Min(16, hex.Length - x)));
                builder.Append((x % 32 == 0) ? " " : "<br />");
            }
            return builder.ToString();
        }

        protected static int Rng1(int prev)
        {
            return (prev * 0x45 + 0x1111) & 0x7fffffff;
        }

        protected static int Rng2(int prev)
        {
            return (int)((0x41C64E6Du * (uint)prev) + 0x6073u);
        }

        protected static int Rng3(int prev)
        {
            return (int)((0x6C078965u * (uint)prev) + 0x1);
        }


    }
}