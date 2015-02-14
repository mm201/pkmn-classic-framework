using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PkmnFoundations.GTS.controls
{
    public partial class LabelTextBox : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public String Text
        {
            get
            {
                return theTextBox.Text;
            }
            set
            {
                theTextBox.Text = value;
            }
        }

        public String Label
        {
            get
            {
                return theLabel.Text;
            }
            set
            {
                theLabel.Text = value;
            }
        }

        public TextBoxMode TextMode
        {
            get
            {
                return theTextBox.TextMode;
            }
            set
            {
                theTextBox.TextMode = value;
            }
        }
    }
}