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

        private static byte[] PAD = new byte[256];

        protected void btnSend_Click(object sender, EventArgs e)
        {
            phDecoded.Visible = false;
            byte[] data = fuBox.FileBytes;
            FileStream s = File.Open(Server.MapPath("~/Box Upload Xor Pad.bin"), FileMode.Open);
            s.Read(PAD, 0, PAD.Length);
            s.Close();

            if (txtOffset.Text.Length > 0)
            {
                int padOffset = Convert.ToInt32(txtOffset.Text);
                Encrypt(data, padOffset);
            }
            else
            {
                Decrypt(data);
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
    }
}