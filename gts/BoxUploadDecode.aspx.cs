using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace PokeFoundations.GTS
{
    public partial class BoxUploadDecode : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            byte data = 0x00;

            StringBuilder builder = new StringBuilder();

            for (int x = 0; x < 512; x++)
            {
                data = runRng(data);
                builder.Append(data.ToString("x2"));
                builder.Append(" ");
                if (x % 16 == 15) builder.Append("<br />");
            }

            litDecrypted.Text = builder.ToString();
        }

        private byte runRng(byte data)
        {
            return (byte)((data + 0x4f) * 0x1c);
        }
    }
}