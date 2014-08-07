using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PkmnFoundations.Data;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace PkmnFoundations.GTS
{
    public partial class RoomLeaders : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGet_Click(object sender, EventArgs e)
        {
            byte rank;
            byte room;

            if (!Byte.TryParse(txtRank.Text, out rank) ||
                !Byte.TryParse(txtRoom.Text, out room))
            {
                litResults.Text = "Please type numbers.";
                return;
            }

            if (rank > 10 || rank < 1)
            {
                litResults.Text = "Rank must be 1-10.";
            }
            if (room > 50 || rank < 1)
            {
                litResults.Text = "Room must be 1-50.";
            }

            rank--;
            room--;

            BattleTowerProfile4[] results = DataAbstract.Instance.BattleTowerGetLeaders4(rank, room);

            StringBuilder builder = new StringBuilder();

            builder.Append("<p>Leaders:</p><ul>");
            foreach (BattleTowerProfile4 profile in results)
            {
                builder.Append("<li>");
                TrendyPhrase4 tp = profile.PhraseLeader;
                builder.Append(tp.Render("<span style=\"color: #0066ff; font-weight: bold;\">{0}</span>"));
                builder.Append("</li>");
            }
            builder.Append("</ul><p>Opponents:</p><ul>");

            BattleTowerRecord4[] opponents = DataAbstract.Instance.BattleTowerGetOpponents4(-1, rank, room);
            foreach (BattleTowerRecord4 record in opponents)
            {
                builder.Append("<li>");


                byte[] phrase = new byte[8];
                System.Buffer.BlockCopy(record.Unknown3, 0, phrase, 0, 8);
                TrendyPhrase4 tp = new TrendyPhrase4(phrase);
                builder.Append(tp.Render("<span style=\"color: #0066ff; font-weight: bold;\">{0}</span>"));
                builder.Append("<br />");

                phrase = new byte[8];
                System.Buffer.BlockCopy(record.Unknown3, 8, phrase, 0, 8);
                tp = new TrendyPhrase4(phrase);
                builder.Append(tp.Render("<span style=\"color: #0066ff; font-weight: bold;\">{0}</span>"));
                builder.Append("<br />");

                phrase = new byte[8];
                System.Buffer.BlockCopy(record.Unknown3, 16, phrase, 0, 8);
                tp = new TrendyPhrase4(phrase);
                builder.Append(tp.Render("<span style=\"color: #0066ff; font-weight: bold;\">{0}</span>"));

                builder.Append("</li>");
            }
            litResults.Text = builder.ToString();
        }
    }
}