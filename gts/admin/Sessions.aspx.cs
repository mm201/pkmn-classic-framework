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
            GtsSessionManager manager = GtsSessionManager.FromContext(Context);

            StringBuilder builder = new StringBuilder();
            builder.Append("Active GenIV sessions (");
            builder.Append(manager.Sessions4.Count);
            builder.Append("):<br />");
            foreach (KeyValuePair<String, GtsSession4> session in manager.Sessions4)
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

            builder.Append("Active GenV sessions (");
            builder.Append(manager.Sessions5.Count);
            builder.Append("):<br />");
            foreach (KeyValuePair<String, GtsSession5> session in manager.Sessions5)
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

            GamestatsSessionManager gsm = GamestatsSessionManager.FromContext(Context);
            builder.Append("Active GSM sessions (");
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
