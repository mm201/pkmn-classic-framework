using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace PkmnFoundations.GTS.debug
{
    public partial class Decode : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            litMessage.Text = "";
        }

        protected void btnDecode_Click(object sender, EventArgs e)
        {
            byte[] data = null;
            phDecoded.Visible = false;

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

            if (data == null)
            {
                litMessage.Text = "<p class=\"errorMessage\">Data is not formatted correctly.</p>";
                return;
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