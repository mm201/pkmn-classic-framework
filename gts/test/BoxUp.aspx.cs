using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;

namespace PkmnFoundations.GTS.debug
{
    public partial class BoxUp : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            litMessage.Text = "";
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            phDecoded.Visible = false;
            byte[] data = fuBox.FileBytes;
            FileStream s = File.Open(Server.MapPath("~/Box Upload Xor Pad.bin"), FileMode.Open);
            byte[] pad = new byte[s.Length];
            s.Read(pad, 0, pad.Length);
            s.Close();
            int padOffset = Convert.ToInt32(txtOffset.Text);

            for (int x = 4; x < data.Length; x++)
            {
                data[x] ^= pad[(x + padOffset) % 256];
            }

            litDecoded.Text = RenderHex(data.ToHexStringLower());
            phDecoded.Visible = true;
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
    }
}