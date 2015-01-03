using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PkmnFoundations.Data;
using PkmnFoundations.Pokedex;
using PkmnFoundations.Structures;

namespace PkmnFoundations.GTS
{
    public partial class AllPokemon : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            m_pokedex = (Pokedex.Pokedex)Application["pkmncfPokedex"];

            GtsRecord4[] records4 = Database.Instance.GtsSearch4(0, 0, Genders.Either, 0, 0, 0, -1);
            rptPokemon4.DataSource = records4;
            rptPokemon4.DataBind();

            GtsRecord5[] records5 = Database.Instance.GtsSearch5(0, 0, Genders.Either, 0, 0, 0, -1);
            rptPokemon5.DataSource = records5;
            rptPokemon5.DataBind();
        }

        private Pokedex.Pokedex m_pokedex;

        private String FormatLevels(byte min, byte max)
        {
            if (min == 0 && max == 0)
            {
                return "Any";
            }
            else if (min == 0)
            {
                return String.Format("{0} and under", max);
            }
            else if (max == 0)
            {
                return String.Format("{0} and up", min);
            }
            else
            {
                return String.Format("{0} to {1}", min, max);
            }
        }

        protected String CreateOffer4(object DataItem)
        {
            GtsRecord4 record = (GtsRecord4)DataItem;
            return String.Format("Species: {3} (#{0})<br />Gender: {1}<br />Level: {2}", 
                record.Species, 
                record.Gender, 
                record.Level, 
                m_pokedex.Species(record.Species).Name);
        }

        protected String CreateWanted4(object DataItem)
        {
            GtsRecord4 record = (GtsRecord4)DataItem;
            return String.Format("Species: {3} (#{0})<br />Gender: {1}<br />Level: {2}", 
                record.RequestedSpecies, 
                record.RequestedGender, 
                FormatLevels(record.RequestedMinLevel, record.RequestedMaxLevel),
                m_pokedex.Species(record.RequestedSpecies).Name);
        }

        protected String CreateTrainer4(object DataItem)
        {
            GtsRecord4 record = (GtsRecord4)DataItem;
            return Common.HtmlEncode(record.TrainerName.Text);
        }

        protected String CreateOffer5(object DataItem)
        {
            GtsRecord5 record = (GtsRecord5)DataItem;
            return String.Format("Species: {3} (#{0})<br />Gender: {1}<br />Level: {2}", 
                record.Species, 
                record.Gender, 
                record.Level,
                m_pokedex.Species(record.Species).Name);
        }

        protected String CreateWanted5(object DataItem)
        {
            GtsRecord5 record = (GtsRecord5)DataItem;
            return String.Format("Species: {3} (#{0})<br />Gender: {1}<br />Level: {2}", 
                record.RequestedSpecies, 
                record.RequestedGender, 
                FormatLevels(record.RequestedMinLevel, record.RequestedMaxLevel),
                m_pokedex.Species(record.RequestedSpecies).Name);
        }

        protected String CreateTrainer5(object DataItem)
        {
            GtsRecord5 record = (GtsRecord5)DataItem;
            return Common.HtmlEncode(record.TrainerName.Text);
        }



    }
}