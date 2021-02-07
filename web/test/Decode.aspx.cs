using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Net;
using GamestatsBase;

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
                GamestatsHandler gs4 = new GamestatsHandler("sAdeqWo3voLeC5r16DYv",
                    0x45, 0x1111, 0x80000000, 0x4a3b2c1d, "pokemondpds",
                    GamestatsRequestVersions.Version2, GamestatsResponseVersions.Version1, true);

                data = gs4.DecryptData(txtData.Text);
                litGeneration.Text = "4";
            }
            catch (FormatException)
            {
            }

            if (data == null) try
            {
                GamestatsHandler gs5 = new GamestatsHandler("HZEdGCzcGGLvguqUEKQN0001d93500002dd5000000082db842b2syachi2ds",
                    GamestatsRequestVersions.Version3, GamestatsResponseVersions.Version2, false);
                data = gs5.DecryptData(txtData.Text);
                litGeneration.Text = "5";
            }
            catch (FormatException)
            {
            }

            if (data == null) try
            {
                GamestatsHandler gsPlat = new GamestatsHandler("uLMOGEiiJogofchScpXb000244fd00006015100000005b440e7epokemondpds",
                    GamestatsRequestVersions.Version3, GamestatsResponseVersions.Version2, true);
                data = gsPlat.DecryptData(txtData.Text);
                litGeneration.Text = "Platinum";
            }
            catch (FormatException)
            {
            }

            if (data == null) try
            {
                GamestatsHandler gsDungeonWii = new GamestatsHandler("zjzrhOVXZKLHNspYpGoR0001c7850000620b0000000820556356pokedngnwii",
                    GamestatsRequestVersions.Version3, GamestatsResponseVersions.Version2);
                data = gsDungeonWii.DecryptData(txtData.Text);
                litGeneration.Text = "Mystery Dungeon Wii";
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
                data = GamestatsHandler.FromUrlSafeBase64String(txtData.Text);

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
            byte[] data2 = GamestatsHandler.FromUrlSafeBase64String(data);
            if (data2.Length < 12) throw new FormatException("Data must contain at least 12 bytes.");

            int checksum = BitConverter.ToInt32(data2, 0);
            checksum = IPAddress.NetworkToHostOrder(checksum); // endian flip
            //checksum ^= 0x2db842b2;

            return data2;
        }

        private String RenderHex(String hex)
        {
            // todo: this should be moved to a user control
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