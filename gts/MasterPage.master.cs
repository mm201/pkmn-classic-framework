using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PkmnFoundations.GTS
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GtsSessionManager manager = GtsSessionManager.FromContext(Context);
            int availTotal = manager.AvailablePokemon4 + manager.AvailablePokemon5;
            litPokemon.Text = availTotal.ToString();
        }

        public String HeaderCssClass
        {
            get
            {
                return litHeaderCssClassKeep.Text;
            }
            set
            {
                litHeaderCssClassKeep.Text = value;
            }
        }


    }
}