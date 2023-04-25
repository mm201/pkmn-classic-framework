using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PkmnFoundations.Structures;
using PkmnFoundations.Wfc;

namespace PkmnFoundations.GTS.test
{
    public partial class VideoId : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnToSerial_Click(object sender, EventArgs e)
        {
            ulong valueNumeric = Convert.ToUInt64(txtBattleVideo.Text.Replace("-", ""));
            txtSerial.Text = BattleVideoHeader4.SerialToKey(valueNumeric).ToString();
        }

        protected void btnToVideo_Click(object sender, EventArgs e)
        {
            ulong valueNumeric = Convert.ToUInt64(txtSerial.Text.Replace("-", ""));
            txtBattleVideo.Text = BattleVideoHeader4.KeyToSerial(valueNumeric).ToString();
        }
    }
}