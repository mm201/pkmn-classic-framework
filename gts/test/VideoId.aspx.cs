using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PkmnFoundations.Structures;

namespace PkmnFoundations.GTS.test
{
    public partial class VideoId : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /*
        60-47238-13640
        91-83735-52841
        25-08496-37872
        15-28740-23153
        57-53008-95014
        16-21089-27755
        99-85554-07696
        74-88678-22357
        37-83435-19378
        32-02800-77969
        */

        private byte[][] m_templates = new byte[][]{
            new byte[]{6, 0, 4, 7, 2, 3, 8, 1, 3, 6, 5, 0},
            new byte[]{9, 1, 8, 3, 7, 3, 5, 5, 2, 8, 5, 1},
            new byte[]{2, 5, 0, 8, 4, 9, 6, 3, 7, 8, 8, 2},
            new byte[]{1, 5, 2, 8, 7, 4, 0, 2, 3, 1, 6, 3},
            new byte[]{5, 7, 5, 3, 0, 0, 8, 9, 5, 0, 2, 4},
            new byte[]{1, 6, 2, 1, 0, 8, 9, 2, 7, 7, 6, 5},
            new byte[]{9, 9, 8, 5, 5, 5, 4, 0, 7, 6, 0, 6},
            new byte[]{7, 4, 8, 8, 6, 7, 8, 2, 2, 3, 6, 7},
            new byte[]{3, 7, 8, 3, 4, 3, 5, 1, 9, 3, 8, 8},
            new byte[]{3, 2, 0, 2, 8, 0, 0, 7, 7, 9, 7, 9}
        };

        protected void btnToSerial_Click(object sender, EventArgs e)
        {
            long valueNumeric = Convert.ToInt64(txtBattleVideo.Text.Replace("-", ""));
            txtSerial.Text = BattleVideoHeader4.SerialToKey(valueNumeric).ToString();
        }

        protected void btnToVideo_Click(object sender, EventArgs e)
        {
            long valueNumeric = Convert.ToInt64(txtSerial.Text.Replace("-", ""));
            txtBattleVideo.Text = BattleVideoHeader4.KeyToSerial(valueNumeric).ToString();
        }
    }
}