using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PkmnFoundations.Web.controls
{
    public partial class PokemonPicker : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public String Value
        {
            get
            {
                return txtSpecies.Text;
            }
            set
            {
                txtSpecies.Text = value;
            }
        }
    }
}