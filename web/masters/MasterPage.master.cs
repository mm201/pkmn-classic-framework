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
            int avail4, avail5;
            ulong bvCount4, bvCount5;

            // todo: move this to a CacheManager sort of class
            if (Cache["pkmncfPokemonCount4"] == null)
            {
                avail4 = Database.Instance.GtsAvailablePokemon4();
                Cache.Insert("pkmncfPokemonCount4", avail4, null,
                    DateTime.Now.AddMinutes(1),
                    System.Web.Caching.Cache.NoSlidingExpiration);
            }
            else
                avail4 = Convert.ToInt32(Cache["pkmncfPokemonCount4"]);

            if (Cache["pkmncfPokemonCount5"] == null)
            {
                avail5 = Database.Instance.GtsAvailablePokemon5();
                Cache.Insert("pkmncfPokemonCount5", avail5, null,
                    DateTime.Now.AddMinutes(1),
                    System.Web.Caching.Cache.NoSlidingExpiration);
            }
            else
                avail5 = Convert.ToInt32(Cache["pkmncfPokemonCount5"]);

            if (Cache["pkmncfBattleVideoCount4"] == null)
            {
                bvCount4 = Database.Instance.BattleVideoCount4();
                Cache.Insert("pkmncfBattleVideoCount4", bvCount4, null,
                    DateTime.Now.AddMinutes(1),
                    System.Web.Caching.Cache.NoSlidingExpiration);
            }
            else
                bvCount4 = Convert.ToUInt64(Cache["pkmncfBattleVideoCount4"]);

            if (Cache["pkmncfBattleVideoCount5"] == null)
            {
                bvCount5 = Database.Instance.BattleVideoCount5();
                Cache.Insert("pkmncfBattleVideoCount5", bvCount5, null,
                    DateTime.Now.AddMinutes(1),
                    System.Web.Caching.Cache.NoSlidingExpiration);
            }
            else
                bvCount5 = Convert.ToUInt64(Cache["pkmncfBattleVideoCount5"]);

            litPokemon.Text = (avail4 + avail5).ToString();
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
