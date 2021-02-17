using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PkmnFoundations.Support;

namespace PkmnFoundations.Web.test
{
    public partial class Gsid : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            StringBuilder output = new StringBuilder();
            var gsids = new int[] { 476518182, 330241374 };
            foreach (var gsid in gsids)
            {
                string enc = GameSyncUtils.PidToGsid(gsid);
                int dec = (int)GameSyncUtils.GsidToPid(enc);
                output.AppendLine(enc);
                output.AppendLine(dec.ToString());
                output.AppendLine(String.Format("{0} ({0:X8}) == {1} ({1:X8})? {2}", gsid, dec, gsid == dec));
                output.AppendLine();
            }

            litOutput.Text = Common.FormatReturns(Common.HtmlEncode(output.ToString()), "<br />\n");
        }
    }
}
