using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using PokeFoundations.Structures;
using PokeFoundations.Data;

namespace PokeFoundations.GTS
{
    public partial class DatabaseTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            byte[] data = Common.FromHexString(txtRecord.Text.Replace("\n", "").Replace("\r", "").Replace(" ", ""));
            GtsRecord4 record = new GtsRecord4();
            record.Load(data);
            DataAbstract.Instance.GtsDepositPokemon4(record);
        }

        protected void btnReceive_Click(object sender, EventArgs e)
        {
            int pid;
            Int32.TryParse(txtPid.Text, out pid);
            GtsRecord4 record = DataAbstract.Instance.GtsDataForUser4(pid);
            if (record == null)
            {
                litRecord.Text = "No data for this PID";
                return;
            }

            byte[] data = record.Save();
            litRecord.Text = RenderHex(data.ToHexStringLower());
        }


        private String RenderHex(String hex)
        {
            StringBuilder builder = new StringBuilder();
            for (int x = 0; x < hex.Length; x += 16)
            {
                builder.Append(hex.Substring(x, Math.Min(16, hex.Length - x)));
                builder.Append("<br />");
            }
            return builder.ToString();
        }

    }
}