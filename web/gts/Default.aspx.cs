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
            Pokedex.Pokedex pokedex = AppStateHelper.Pokedex(Application);

            int species = ppSpecies.Value ?? 0;
            int minLevel = Convert.ToInt32(txtLevelMin.Text);
            int maxLevel = Convert.ToInt32(txtLevelMax.Text);
            Genders gender = Genders.Either;
            if (chkMale.Checked && !chkFemale.Checked) gender = Genders.Male;
            if (chkFemale.Checked && !chkMale.Checked) gender = Genders.Female;

            if (rbGen4.Checked)
            {
                GtsRecord4[] records4 = Database.Instance.GtsSearch4(pokedex, 0, (ushort)species, gender, (byte)minLevel, (byte)maxLevel, 0, -1);
                rptPokemon.DataSource = records4;
                rptPokemon.DataBind();
                phNone.Visible = records4.Length == 0;
            }
            else if (rbGen5.Checked)
            {
                GtsRecord5[] records5 = Database.Instance.GtsSearch5(pokedex, 0, (ushort)species, gender, (byte)minLevel, (byte)maxLevel, 0, -1);
                rptPokemon.DataSource = records5;
                rptPokemon.DataBind();
                phNone.Visible = records5.Length == 0;
            }

            txtLevelMin.Attributes["onchange"] = "changedMin(\'" + txtLevelMin.ClientID + "\', \'" + txtLevelMax.ClientID + "\');";
            txtLevelMax.Attributes["onchange"] = "changedMax(\'" + txtLevelMin.ClientID + "\', \'" + txtLevelMax.ClientID + "\');";
        }

        private string FormatLevels(byte min, byte max)
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
            else if (min == max)
            {
                return String.Format("Lv. {0}", min);
            }
            else
            {
                return String.Format("Lv. {0} to {1}", min, max);
            }
        }

        protected string CreateOfferImage(object DataItem)
        {
            try
            {
                GtsRecordBase record = (GtsRecordBase)DataItem;
                return "<img src=\"" + ResolveUrl(WebFormat.PokemonImageLarge(record.Pokemon)) +
                    "\" alt=\"" + Common.HtmlEncode(record.Pokemon.Species.Name.ToString()) +
                    "\" class=\"sprite species\" width=\"96px\" height=\"96px\" />";
            }
            catch
            {
                return "???";
            }
        }

        protected string CreatePokeball(object DataItem)
        {
            try
            {
                GtsRecordBase record = (GtsRecordBase)DataItem;
                // Hide pokeballs with incorrect numbers until we catalog them.
                if (record.Pokemon.Pokeball == null) return "";
                string itemName = Common.HtmlEncode(record.Pokemon.Pokeball.Name.ToString());
                return "<img src=\"" + ResolveUrl(WebFormat.ItemImage(record.Pokemon.Pokeball)) +
                    "\" alt=\"" + itemName + "\" title=\"" + itemName + 
                    "\" class=\"sprite item\" width=\"24px\" height=\"24px\" />";
            }
            catch
            {
                return "???";
            }
        }

        protected string CreatePokerus(object DataItem)
        {
            try
            {
                GtsRecordBase record = (GtsRecordBase)DataItem;
                switch (record.Pokemon.Pokerus)
                {
                    case Pokerus.Infected:
                        return "<span class=\"pkrs\">PKRS</span>";
                    case Pokerus.Cured:
                    case Pokerus.None:
                    default:
                        return "";
                }
            }
            catch
            {
                return "";
            }
        }

        protected string CreateLevel(object DataItem)
        {
            GtsRecordBase record = (GtsRecordBase)DataItem;
            return record.Level.ToString();
        }

        protected string CreateGender(object DataItem)
        {
            GtsRecordBase record = (GtsRecordBase)DataItem;
            return Format.GenderSymbol(record.Gender);
        }

        protected string CreateNickname(object DataItem)
        {
            try
            {
                GtsRecordBase record = (GtsRecordBase)DataItem;
                return Common.HtmlEncode(record.Pokemon.Nickname);
            }
            catch
            {
                return "???";
            }
        }

        protected string CreateSpecies(object DataItem)
        {
            try
            {
                Pokedex.Pokedex pokedex = AppStateHelper.Pokedex(Application);
                GtsRecordBase record = (GtsRecordBase)DataItem;
                return Common.HtmlEncode(pokedex.Species[record.Species].Name.ToString());
            }
            catch
            {
                return "???";
            }
        }

        protected string CreatePokedex(object DataItem)
        {
            try
            {
                GtsRecordBase record = (GtsRecordBase)DataItem;
                return Common.HtmlEncode(record.Species.ToString());
            }
            catch
            {
                return "???";
            }
        }

        protected string CreateHeldItem(object DataItem)
        {
            try
            {
                GtsRecordBase record = (GtsRecordBase)DataItem;
                if (record.Pokemon.HeldItem == null) return "";
                string itemName = Common.HtmlEncode(record.Pokemon.HeldItem.Name.ToString());
                return "<img src=\"" + ResolveUrl(WebFormat.ItemImage(record.Pokemon.HeldItem)) +
                    "\" alt=\"" + itemName + "\" class=\"sprite item\" width=\"24px\" height=\"24px\" />" +
                    itemName;
            }
            catch
            {
                return "???";
            }
        }

        protected string CreateNature(object DataItem)
        {
            try
            {
                GtsRecordBase record = (GtsRecordBase)DataItem;
                return Common.HtmlEncode(record.Pokemon.Nature.ToString());
            }
            catch
            {
                return "???";
            }
        }

        protected string CreateAbility(object DataItem)
        {
            try
            {
                GtsRecordBase record = (GtsRecordBase)DataItem;
                return Common.HtmlEncode(record.Pokemon.Ability.Name.ToString());
            }
            catch
            {
                return "???";
            }
        }

        protected string CreateTrainer(object DataItem)
        {
            GtsRecordBase record = (GtsRecordBase)DataItem;
            return Common.HtmlEncode(record.TrainerName);
        }

        protected string CreateWantedSpecies(object DataItem)
        {
            Pokedex.Pokedex pokedex = AppStateHelper.Pokedex(Application);
            GtsRecordBase record = (GtsRecordBase)DataItem;
            Species species = pokedex.Species[record.RequestedSpecies];

            return "<img src=\"" + ResolveUrl(WebFormat.SpeciesImageSmall(species)) +
                "\" alt=\"" + Common.HtmlEncode(species.Name.ToString()) + 
                "\" class=\"sprite speciesSmall\" width=\"40px\" height=\"30px\" />" +
                String.Format("{0} (#{1})",
                Common.HtmlEncode(species.Name.ToString()),
                record.RequestedSpecies);
        }

        protected string CreateWantedGender(object DataItem)
        {
            GtsRecordBase record = (GtsRecordBase)DataItem;
            return Format.GenderSymbol(record.RequestedGender);
        }

        protected string CreateWantedLevel(object DataItem)
        {
            GtsRecordBase record = (GtsRecordBase)DataItem;
            return FormatLevels(record.RequestedMinLevel, record.RequestedMaxLevel);
        }

        protected string CreateDate(object DataItem)
        {
            GtsRecordBase record = (GtsRecordBase)DataItem;
            if (record.TimeDeposited == null) return "";
            return Common.HtmlEncode(((DateTime)record.TimeDeposited).ToString("f"));
        }
    }
}