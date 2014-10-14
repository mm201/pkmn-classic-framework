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

            switch (rbGeneration.SelectedValue)
            {
                case "4":
                {
                    BattleTowerProfile4[] results = Database.Instance.BattleTowerGetLeaders4(rank, room);

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

                    BattleTowerRecord4[] opponents = Database.Instance.BattleTowerGetOpponents4(-1, rank, room);
                    foreach (BattleTowerRecord4 record in opponents)
                    {
                        builder.Append("<li>");

                        builder.Append(record.PhraseChallenged.Render("<span style=\"color: #0066ff; font-weight: bold;\">{0}</span>"));
                        builder.Append("<br />");

                        builder.Append(record.PhraseWon.Render("<span style=\"color: #0066ff; font-weight: bold;\">{0}</span>"));
                        builder.Append("<br />");

                        builder.Append(record.PhraseLost.Render("<span style=\"color: #0066ff; font-weight: bold;\">{0}</span>"));

                        builder.Append("</li>");
                    }
                    litResults.Text = builder.ToString();
                } break;
                case "5":
                    {
                        BattleSubwayProfile5[] results = Database.Instance.BattleSubwayGetLeaders5(rank, room);

                        StringBuilder builder = new StringBuilder();

                        builder.Append("<p>Leaders:</p><ul>");
                        foreach (BattleSubwayProfile5 profile in results)
                        {
                            builder.Append("<li>");
                            TrendyPhrase5 tp = profile.PhraseLeader;
                            builder.Append(tp.Render("<span style=\"color: #0066ff; font-weight: bold;\">{0}</span>"));
                            builder.Append("</li>");
                        }
                        builder.Append("</ul><p>Opponents:</p><ul>");

                        BattleSubwayRecord5[] opponents = Database.Instance.BattleSubwayGetOpponents5(-1, rank, room);
                        foreach (BattleSubwayRecord5 record in opponents)
                        {
                            builder.Append("<li>");

                            builder.Append(record.PhraseChallenged.Render("<span style=\"color: #0066ff; font-weight: bold;\">{0}</span>"));
                            builder.Append("<br />");

                            builder.Append(record.PhraseWon.Render("<span style=\"color: #0066ff; font-weight: bold;\">{0}</span>"));
                            builder.Append("<br />");

                            builder.Append(record.PhraseLost.Render("<span style=\"color: #0066ff; font-weight: bold;\">{0}</span>"));

                            builder.Append("</li>");
                        }
                        litResults.Text = builder.ToString();
                    } break;
            }
        }
    }
}