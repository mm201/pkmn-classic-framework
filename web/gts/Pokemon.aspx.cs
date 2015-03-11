using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PkmnFoundations.Data;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace PkmnFoundations.Web.gts
{
    public partial class Pokemon : System.Web.UI.Page
    {
        private Pokedex.Pokedex m_pokedex;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_pokedex = (Pokedex.Pokedex)Application["pkmncfPokedex"];
            Pokemon4 pkmn = null;

            if (Request.QueryString.Count == 0 || Request.QueryString.Count > 2) throw new WebException(400);
            if (Request.QueryString["offer"] != null ||
                Request.QueryString["exchange"] != null)
            {
                String generation = Request.QueryString["g"];
                if (generation == null ||
                    Request.QueryString.Count != 2)
                    throw new WebException(400);

                int tradeId;
                bool isExchanged;

                if (Request.QueryString["offer"] != null)
                {
                    tradeId = Convert.ToInt32(Request.QueryString["offer"]);
                    isExchanged = false;
                }
                else if (Request.QueryString["exchange"] != null)
                {
                    tradeId = Convert.ToInt32(Request.QueryString["exchange"]);
                    isExchanged = true;
                }
                else
                {
                    AssertHelper.Unreachable();
                    throw new WebException(400);
                }

                // todo: when userprofiles are ready, add checks that they allow viewing their GTS history
                switch (generation)
                {
                    case "4":
                    {
                        GtsRecord4 record = Database.Instance.GtsGetRecord4(tradeId, isExchanged, true);
                        if (record != null) pkmn = new Pokemon4(m_pokedex, record.Data);

                    } break;
                    case "5":
                    {
                        GtsRecord5 record = Database.Instance.GtsGetRecord5(tradeId, isExchanged, true);
                        if (record != null) pkmn = new Pokemon4(m_pokedex, record.Data);

                    } break;
                    default:
                        throw new WebException(400);
                }
            }
            else if (Request.QueryString["check"] != null)
            {
                int checkId = Convert.ToInt32(Request.QueryString["check"]);
                throw new NotImplementedException();
            }
            else throw new WebException(400);

            if (pkmn == null)
                throw new WebException(403);

            Bind(pkmn);
        }

        private void Bind(Pokemon4 pkmn)
        {
            litNickname.Text = pkmn.Nickname;
            bool shiny = pkmn.IsShiny;
            imgPokemon.ImageUrl = (shiny ? "~/images/pkmn-lg-s/" : "~/images/pkmn-lg/") +
                SpeciesFilename(pkmn) + ".png";
            imgPokemon.AlternateText = pkmn.Species.Name.ToString();
            phShiny.Visible = shiny;
            // todo: pokerus
            phPkrs.Visible = false;
            phPkrsCured.Visible = false;
            litMarks.Text = CreateMarks(pkmn.Markings);
            imgPokeball.ImageUrl = "~/images/item-sm/" + pkmn.Pokeball.ID.ToString() + ".png";
            imgPokeball.AlternateText = pkmn.Pokeball.Name.ToString();
            litLevel.Text = pkmn.Level.ToString();
            litGender.Text = CreateGender(pkmn.Gender);
        }

        private String[] m_marks = new String[] { "●", "▲", "■", "♥", "★", "♦" };

        private String CreateMarks(Markings markings)
        {
            StringBuilder result = new StringBuilder();
            int marking = 1;
            for (int value = 0; value < 6; value++)
            {
                if (((int)markings & marking) != 0)
                {
                    result.Append("<span class=\"m\">");
                    result.Append(m_marks[value]);
                    result.Append("</span>");
                }
                else
                {
                    result.Append("<span>");
                    result.Append(m_marks[value]);
                    result.Append("</span>");
                }

                marking <<= 1;
            }

            return result.ToString();
        }

        private String SpeciesFilename(Pokemon4 pkmn)
        {
            // todo: move to a more central location
            StringBuilder builder = new StringBuilder();
            builder.Append(pkmn.SpeciesID);
            if (pkmn.Form.Suffix.Length > 0)
            {
                builder.Append('-');
                builder.Append(pkmn.Form.Suffix);
            }
            if (pkmn.Species.GenderVariations && pkmn.Gender == Genders.Female)
                builder.Append("-f");

            return builder.ToString();
        }

        private String CreateGender(Genders gender)
        {
            switch (gender)
            {
                case Genders.Male:
                    return "♂";
                case Genders.Female:
                    return "♀";
                default:
                    return "";
            }
        }
    }
}