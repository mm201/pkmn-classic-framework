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

        public int ? Value
        {
            get
            {
                String value = theLookup.Value;
                return String.IsNullOrEmpty(value) ? null : (int ?)Convert.ToInt32(value);
            }
            set
            {
                if (value == null) theLookup.Value = null;
                theLookup.Value = value.ToString();
            }
        }
    }
}