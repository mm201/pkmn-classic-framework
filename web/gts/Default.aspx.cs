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

            int species; Int32.TryParse(ppSpecies.Value, out species);
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
            }
            else if (rbGen5.Checked)
            {
                GtsRecord5[] records5 = Database.Instance.GtsSearch5(pokedex, 0, (ushort)species, gender, (byte)minLevel, (byte)maxLevel, 0, -1);
                rptPokemon.DataSource = records5;
                rptPokemon.DataBind();
            }
        }

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

        protected String CreatePokeball(object DataItem)
        {
            try
            {
                GtsRecordBase record = (GtsRecordBase)DataItem;
                String itemName = Common.HtmlEncode(record.Pokemon.Pokeball.Name.ToString());
                return "<img src=\"" + ResolveUrl(WebFormat.ItemImage(record.Pokemon.Pokeball)) +
                    "\" alt=\"" + itemName + "\" title=\"" + itemName + 
                    "\" class=\"sprite item\" width=\"24px\" height=\"24px\" />";
            }
            catch
            {
                return "???";
            }
        }

        protected String CreatePokerus(object DataItem)
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

        protected String CreateLevel(object DataItem)
        {
            GtsRecordBase record = (GtsRecordBase)DataItem;
            return record.Level.ToString();
        }

        protected String CreateGender(object DataItem)
        {
            GtsRecordBase record = (GtsRecordBase)DataItem;
            return Format.GenderSymbol(record.Gender);
        }

        protected String CreateNickname(object DataItem)
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

        protected String CreateSpecies(object DataItem)
        {
            try
            {
                Pokedex.Pokedex pokedex = AppStateHelper.Pokedex(Application);
                GtsRecordBase record = (GtsRecordBase)DataItem;
                return Common.HtmlEncode(pokedex.Species(record.Species).Name.ToString());
            }
            catch
            {
                return "???";
            }
        }

        protected String CreatePokedex(object DataItem)
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

        protected String CreateHeldItem(object DataItem)
        {
            try
            {
                GtsRecordBase record = (GtsRecordBase)DataItem;
                if (record.Pokemon.HeldItem == null) return "";
                String itemName = Common.HtmlEncode(record.Pokemon.HeldItem.Name.ToString());
                return "<img src=\"" + ResolveUrl(WebFormat.ItemImage(record.Pokemon.HeldItem)) +
                    "\" alt=\"" + itemName + "\" class=\"sprite item\" width=\"24px\" height=\"24px\" />" +
                    itemName;
            }
            catch
            {
                return "???";
            }
        }

        protected String CreateNature(object DataItem)
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

        protected String CreateAbility(object DataItem)
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

        protected String CreateTrainer(object DataItem)
        {
            GtsRecordBase record = (GtsRecordBase)DataItem;
            return Common.HtmlEncode(record.TrainerName);
        }

        protected String CreateWantedSpecies(object DataItem)
        {
            Pokedex.Pokedex pokedex = AppStateHelper.Pokedex(Application);
            GtsRecordBase record = (GtsRecordBase)DataItem;
            Species species = pokedex.Species(record.RequestedSpecies);

            return "<img src=\"" + ResolveUrl(WebFormat.SpeciesImageSmall(species)) +
                "\" alt=\"" + Common.HtmlEncode(species.Name.ToString()) + 
                "\" class=\"sprite speciesSmall\" width=\"40px\" height=\"32px\" />" +
                String.Format("{0} (#{1})",
                Common.HtmlEncode(species.Name.ToString()),
                record.RequestedSpecies);
        }

        protected String CreateWantedGender(object DataItem)
        {
            GtsRecordBase record = (GtsRecordBase)DataItem;
            return Format.GenderSymbol(record.RequestedGender);
        }

        protected String CreateWantedLevel(object DataItem)
        {
            GtsRecordBase record = (GtsRecordBase)DataItem;
            return FormatLevels(record.RequestedMinLevel, record.RequestedMaxLevel);
        }

        protected String CreateDate(object DataItem)
        {
            GtsRecordBase record = (GtsRecordBase)DataItem;
            if (record.TimeDeposited == null) return "";
            return Common.HtmlEncode(((DateTime)record.TimeDeposited).ToString("f"));
        }
    }
}