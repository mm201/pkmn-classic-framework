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
            // todo: I should store the available pokemon count
            // on the cache in the same way so the website doesn't
            // have a dependency on running in the same application
            // as the gamestats server.
            GtsSessionManager manager = GtsSessionManager.FromContext(Context);
            int availTotal = manager.AvailablePokemon4 + manager.AvailablePokemon5;
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