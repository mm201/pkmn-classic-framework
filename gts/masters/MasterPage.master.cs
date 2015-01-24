using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PkmnFoundations.Data;

namespace PkmnFoundations.GTS
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // todo: Cache this
            int availTotal = Database.Instance.GtsAvailablePokemon4() + Database.Instance.GtsAvailablePokemon5();
            litPokemon.Text = availTotal.ToString();

            ulong bvCount4, bvCount5;

            if (Cache["BattleVideoCount4"] == null ||
                Cache["BattleVideoCount5"] == null)
            {
                bvCount4 = Database.Instance.BattleVideoCount4();
                bvCount5 = Database.Instance.BattleVideoCount5();
                Cache.Insert("BattleVideoCount4", bvCount4, null, 
                    DateTime.Now.AddMinutes(1), 
                    System.Web.Caching.Cache.NoSlidingExpiration);
                Cache.Insert("BattleVideoCount5", bvCount5, null,
                    DateTime.Now.AddMinutes(1),
                    System.Web.Caching.Cache.NoSlidingExpiration);
            }
            else
            {
                bvCount4 = Convert.ToUInt64(Cache["BattleVideoCount4"]);
                bvCount5 = Convert.ToUInt64(Cache["BattleVideoCount5"]);
            }

            litVideos.Text = (bvCount4 + bvCount5).ToString();
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
