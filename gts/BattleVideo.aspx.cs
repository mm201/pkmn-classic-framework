using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Configuration;
using PkmnFoundations.Data;

namespace PkmnFoundations.GTS
{
    public partial class BattleVideo : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            litMessage.Text = "";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            using (MySqlConnection db = CreateConnection())
            {
                db.Open();

                litQueued4.Text = HttpUtility.HtmlEncode(db.ExecuteScalar("SELECT Count(*) FROM BattleVideoCrawlQueue WHERE Complete = 0"));
                litTotal4.Text = HttpUtility.HtmlEncode(db.ExecuteScalar("SELECT Count(*) FROM BattleVideoCrawlQueue WHERE Complete = 1"));

                litQueued5.Text = HttpUtility.HtmlEncode(db.ExecuteScalar("SELECT Count(*) FROM BattleVideoCrawlQueue5 WHERE Complete = 0"));
                litTotal5.Text = HttpUtility.HtmlEncode(db.ExecuteScalar("SELECT Count(*) FROM BattleVideoCrawlQueue5 WHERE Complete = 1"));

                db.Close();
            }
        }

        protected void btnSend4_Click(object sender, EventArgs e)
        {
            ulong id;
            UInt64.TryParse(txtBattleVideo4.Text.Replace('-', ' ').Replace(" ", ""), out id);
            if (id == 0)
            {
                litMessage.Text = "The battle video ID could not be read. Please enter a battle video ID in the format, xx-xxxxx-xxxxx, and try again.";
                return;
            }

            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                QueueVideoId4(db, id);
                db.Close();
            }
        }

        private void QueueVideoId4(MySqlConnection db, ulong id)
        {
            using (MySqlTransaction tran = db.BeginTransaction())
            {
                long count = (long)tran.ExecuteScalar("SELECT Count(*) FROM BattleVideoCrawlQueue WHERE SerialNumber = @serial_number", new MySqlParameter("@serial_number", id));
                if (count > 0)
                {
                    tran.Rollback();
                    litMessage.Text = "The battle video is already queued for retrieval.";
                    return;
                }
                tran.ExecuteNonQuery("INSERT INTO BattleVideoCrawlQueue (SerialNumber, `Timestamp`) VALUES (@serial_number, NOW())", new MySqlParameter("@serial_number", id));
                tran.Commit();
                litMessage.Text = "The battle video has been queued for retrieval.";
            }
        }

        protected void btnSend5_Click(object sender, EventArgs e)
        {
            ulong id;
            UInt64.TryParse(txtBattleVideo5.Text.Replace('-', ' ').Replace(" ", ""), out id);
            if (id == 0)
            {
                litMessage.Text = "The battle video ID could not be read. Please enter a battle video ID in the format, xx-xxxxx-xxxxx, and try again.";
                return;
            }

            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                QueueVideoId5(db, id);
                db.Close();
            }
        }

        private void QueueVideoId5(MySqlConnection db, ulong id)
        {
            using (MySqlTransaction tran = db.BeginTransaction())
            {
                long count = (long)tran.ExecuteScalar("SELECT Count(*) FROM BattleVideoCrawlQueue5 WHERE SerialNumber = @serial_number", new MySqlParameter("@serial_number", id));
                if (count > 0)
                {
                    tran.Rollback();
                    litMessage.Text = "The battle video is already queued for retrieval.";
                    return;
                }
                tran.ExecuteNonQuery("INSERT INTO BattleVideoCrawlQueue5 (SerialNumber, `Timestamp`) VALUES (@serial_number, NOW())", new MySqlParameter("@serial_number", id));
                tran.Commit();
                litMessage.Text = "The battle video has been queued for retrieval.";
            }
        }

        public static MySqlConnection CreateConnection()
        {
            return new MySqlConnection(ConfigurationManager.ConnectionStrings["pkmnFoundationsConnectionString"].ConnectionString);
        }
    }
}