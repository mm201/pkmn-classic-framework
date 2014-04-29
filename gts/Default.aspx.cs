using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PkmnFoundations.GTS;
using System.Text;

namespace PkmnFoundations.GTS
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Dictionary<String, GtsSession4> sessions = Context.AllSessions4();

            StringBuilder builder = new StringBuilder();
            builder.Append("Active sessions (");
            builder.Append(sessions.Count);
            builder.Append("):<br />");
            foreach (KeyValuePair<String, GtsSession4> session in sessions)
            {
                builder.Append("PID: ");
                builder.Append(session.Value.PID);
                builder.Append("<br />Token: ");
                builder.Append(session.Value.Token);
                builder.Append("<br />Hash: ");
                builder.Append(session.Value.Hash);
                builder.Append("<br />URL: ");
                builder.Append(session.Value.URL);
                builder.Append("<br />Expires: ");
                builder.Append(session.Value.ExpiryDate);
                builder.Append("<br /><br />");
            }

            if (Request.QueryString["data"] != null)
            {
                byte[] data = GtsSession4.DecryptData(Request.QueryString["data"]);
                builder.Append("Data:<br />");
                builder.Append(RenderHex(data.ToHexStringLower()));
                builder.Append("<br />");
            }

            litDebug.Text = builder.ToString();
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
