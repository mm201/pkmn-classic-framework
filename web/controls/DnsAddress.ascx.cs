using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PkmnFoundations.Web.controls
{
    public partial class DnsAddress : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public bool ShowAttribution
        {
            get
            {
                return phAttribution.Visible;
            }
            set
            {
                phAttribution.Visible = value;
            }
        }
    }
}