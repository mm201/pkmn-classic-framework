using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace PkmnFoundations.GTS.test
{
    public partial class RngTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int seed = 0;
            byte[] data = new byte[512];

            for (int x = 0; x < data.Length; x++)
            {
                seed = Rng1(seed);
                data[x] = (byte)((seed >> 8) & 0xff);
            }

            litRandom.Text = RenderHex(data.ToHexStringLower());

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