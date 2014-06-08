using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PkmnFoundations.Support;

namespace PkmnFoundations.GTS.test
{
    public partial class NameDecode : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            byte[] data = Common.FromHexString(txtName.Text.Replace(" ", ""));
            litName.Text = Common.HtmlEncode(EncodedString4.DecodeString(data));
        }
    }
}