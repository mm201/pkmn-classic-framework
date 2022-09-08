using PkmnFoundations.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PkmnFoundations.Web.test
{
    public partial class NameEncode : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string text = txtName.Text;
            EncodedString4 es = new EncodedString4(text, text.Length * 2 + 8);
            litName.Text = Common.HtmlEncode(GamestatsBase.Common.ToHexStringUpper(es.RawData));
        }
    }
}