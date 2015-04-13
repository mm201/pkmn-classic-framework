using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PkmnFoundations.Data;
using PkmnFoundations.Pokedex;
using PkmnFoundations.Structures;
using PkmnFoundations.Web;

namespace PkmnFoundations.GTS
{
    public partial class AllPokemon : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            m_pokedex = (Pokedex.Pokedex)Application["pkmncfPokedex"];

            int species; Int32.TryParse(ppSpecies.Value, out species);
            int minLevel = Convert.ToInt32(txtLevelMin.Text);
            int maxLevel = Convert.ToInt32(txtLevelMax.Text);
            Genders gender = Genders.Either;
            if (chkMale.Checked && !chkFemale.Checked) gender = Genders.Male;
            if (chkFemale.Checked && !chkMale.Checked) gender = Genders.Female;

            if (rbGen4.Checked)
            {
                GtsRecord4[] records4 = Database.Instance.GtsSearch4(0, (ushort)species, gender, (byte)minLevel, (byte)maxLevel, 0, -1);
                rptPokemon4.DataSource = records4;
                rptPokemon4.DataBind();
                rptPokemon4.Visible = true;
                rptPokemon5.Visible = false;
            }
            else if (rbGen5.Checked)
            {
                GtsRecord5[] records5 = Database.Instance.GtsSearch5(0, (ushort)species, gender, (byte)minLevel, (byte)maxLevel, 0, -1);
                rptPokemon5.DataSource = records5;
                rptPokemon5.DataBind();
                rptPokemon4.Visible = false;
                rptPokemon5.Visible = true;
            }
        }

        private Pokedex.Pokedex m_pokedex;

        private String FormatLevels(byte min, byte max)
        {
            if (min == 0 && max == 0)
            {
                return "Any level";
            }
            else if (min == 0)
            {
                return String.Format("Lv. {0} and under", max);
            }
            else if (max == 0)
            {
                return String.Format("Lv. {0} and up", min);
            }
            else
            {
                return String.Format("Lv. {0} to {1}", min, max);
            }
        }

        protected String CreateOfferImage(object DataItem)
        {
            return "<img src=\"" + ResolveUrl("~/images/pkmn-lg/todo.png") +
                "\" alt=\"todo\" class=\"sprite species\" width=\"96px\" height=\"96px\" />";
        }

        protected String CreatePokeball(object DataItem)
        {
            return "<img src=\"" + ResolveUrl("~/images/item-sm/todo.png") +
                "\" alt=\"todo\" title=\"todo\" class=\"sprite item\" width=\"24px\" height=\"24px\" />";
        }

        protected String CreatePokerus(object DataItem)
        {
            return "";
        }


        // fixme: I can remove the need for separate 4/5 functions by making
        // the GtsRecords inherit from IGtsRecord and make the the Pokemon field
        // return PokemonPartyBase.

        protected String CreateLevel(object DataItem)
        {
            GtsRecord4 record = (GtsRecord4)DataItem;
            return record.Level.ToString();
        }

        protected String CreateGender(object DataItem)
        {
            GtsRecord4 record = (GtsRecord4)DataItem;
            return Format.GenderSymbol(record.Gender);
        }

        protected String CreateNickname(object DataItem)
        {
            return "todo";
        }

        protected String CreateSpecies(object DataItem)
        {
            GtsRecord4 record = (GtsRecord4)DataItem;
            return m_pokedex.Species(record.Species).Name.ToString();
        }

        protected String CreatePokedex(object DataItem)
        {
            GtsRecord4 record = (GtsRecord4)DataItem;
            return record.Species.ToString();
        }

        protected String CreateHeldItem(object DataItem)
        {
            return "todo";
        }

        protected String CreateNature(object DataItem)
        {
            return "todo";
        }

        protected String CreateAbility(object DataItem)
        {
            return "todo";
        }

        protected String CreateTrainer(object DataItem)
        {
            GtsRecord4 record = (GtsRecord4)DataItem;
            return Common.HtmlEncode(record.TrainerName.Text);
        }

        protected String CreateWantedSpecies(object DataItem)
        {
            GtsRecord4 record = (GtsRecord4)DataItem;
            return String.Format("{0} (#{1})",
                m_pokedex.Species(record.RequestedSpecies).Name,
                record.RequestedSpecies);
        }

        protected String CreateWantedGender(object DataItem)
        {
            GtsRecord4 record = (GtsRecord4)DataItem;
            return Format.GenderSymbol(record.RequestedGender);
        }

        protected String CreateWantedLevel(object DataItem)
        {
            GtsRecord4 record = (GtsRecord4)DataItem;
            return FormatLevels(record.RequestedMinLevel, record.RequestedMaxLevel);
        }





        protected String CreateOffer5(object DataItem)
        {
            GtsRecord5 record = (GtsRecord5)DataItem;
            return String.Format("{3} (#{0})<br />{1}<br />Lv {2}", 
                record.Species, 
                record.Gender, 
                record.Level,
                m_pokedex.Species(record.Species).Name);
        }

        protected String CreateWanted5(object DataItem)
        {
            GtsRecord5 record = (GtsRecord5)DataItem;
            return String.Format("{3} (#{0})<br />{1}<br />{2}",
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