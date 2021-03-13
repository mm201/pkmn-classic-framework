using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using GamestatsBase;

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

        private static byte[] m_pad = new byte[256];

        protected void btnSend_Click(object sender, EventArgs e)
        {
            phDecoded.Visible = false;
            byte[] data = fuBox.FileBytes;
            FileStream s = File.Open(Server.MapPath("~/pad.bin"), FileMode.Open);
            s.Read(m_pad, 0, m_pad.Length);
            s.Close();

            CryptMessage(data);

            switch (rblFormat.SelectedValue)
            {
                case "hd":
                default:
                    litDecoded.Text = RenderHex(data.ToHexStringLower());
                    break;
                case "ca":
                    litDecoded.Text = RenderCArray(data.ToHexStringLower());
                    break;
            }
            phDecoded.Visible = true;
        }

        private string RenderHex(string hex)
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

        private string RenderCArray(string hex)
        {
            StringBuilder builder = new StringBuilder();
            for (int x = 0; x < hex.Length; x += 2)
            {
                if (x > 0)
                {
                    builder.Append(", ");
                    if (x % 16 == 0) builder.Append("<br />\r\n");
                }
                builder.Append("0x");
                builder.Append(hex.Substring(x, Math.Min(2, hex.Length - x)));
            }
            return builder.ToString();
        }

        private void CryptMessage(byte[] message)
        {
            if (message.Length < 5) return;
            byte padOffset = (byte)(message[0] + message[4]);

            // encrypt and decrypt are the same operation...
            for (int x = 5; x < message.Length; x++)
                message[x] ^= m_pad[(x + padOffset) & 0xff];
        }
    }
}