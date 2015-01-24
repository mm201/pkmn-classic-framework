using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GamestatsBase;

namespace PkmnFoundations.GTS.admin
{
    public partial class Sessions : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();

            GamestatsSessionManager gsm = GamestatsSessionManager.FromContext(Context);
            builder.Append("Active sessions (");
            builder.Append(gsm.Sessions.Count);
            builder.Append("):<br />");
            foreach (KeyValuePair<String, GamestatsSession> session in gsm.Sessions)
            {
                builder.Append("Game ID: ");
                builder.Append(session.Value.GameId);
                builder.Append("<br />PID: ");
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

            litDebug.Text = builder.ToString();
        }
    }
}
