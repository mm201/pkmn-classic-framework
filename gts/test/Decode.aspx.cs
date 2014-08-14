using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Net;

namespace PkmnFoundations.GTS.debug
{
    public partial class Decode : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            litMessage.Text = "";
            litChecksum.Text = "";
        }

        protected void btnDecode_Click(object sender, EventArgs e)
        {
            byte[] data = null;
            phDecoded.Visible = false;
            phChecksum.Visible = false;

            try
            {
                data = GtsSession4.DecryptData(txtData.Text);
                litGeneration.Text = "4";
            }
            catch (FormatException)
            {
            }

            if (data == null) try
            {
                data = GtsSession5.DecryptData(txtData.Text);
                litGeneration.Text = "5";
            }
            catch (FormatException)
            {
            }

            if (data == null) try
            {
                data = GamestatsSessionPlat.DecryptData(txtData.Text);
                litGeneration.Text = "Platinum";
            }
            catch (FormatException)
            {
            }

            if (data == null) try
            {
                data = DecryptData(txtData.Text);

                int checkedsum = 0;
                foreach (byte b in data)
                    checkedsum += b;

                litGeneration.Text = "Unknown (raw)";
                litChecksum.Text = checkedsum.ToString();
                phChecksum.Visible = true;
            }
            catch (FormatException)
            {
            }

            if (data == null) try
            {
                data = GtsSessionBase.FromUrlSafeBase64String(txtData.Text);

                litGeneration.Text = "Unknown (are you sure this is gamestats data?)";
                litChecksum.Text = "";
                phChecksum.Visible = false;
            }
            catch (FormatException)
            {
            }

            if (data == null)
            {
                litMessage.Text = "<p class=\"errorMessage\">Data is not formatted correctly.</p>";
            }
            else
            {
                litDecoded.Text = RenderHex(data.ToHexStringLower());
            }

            phDecoded.Visible = true;
        }

        public static byte[] DecryptData(String data)
        {
            byte[] data2 = GtsSessionBase.FromUrlSafeBase64String(data);
            if (data2.Length < 12) throw new FormatException("Data must contain at least 12 bytes.");

            int checksum = BitConverter.ToInt32(data2, 0);
            checksum = IPAddress.NetworkToHostOrder(checksum); // endian flip
            //checksum ^= 0x2db842b2;

            return data2;
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