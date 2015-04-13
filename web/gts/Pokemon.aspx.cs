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
            imgPokemon.ImageUrl = WebFormat.PokemonImage(pkmn);
            imgPokemon.AlternateText = pkmn.Species.Name.ToString();
            phShiny.Visible = shiny;
            // todo: pokerus
            phPkrs.Visible = false;
            phPkrsCured.Visible = false;
            litMarks.Text = WebFormat.Markings(pkmn.Markings);
            imgPokeball.ImageUrl = ItemFilename(pkmn.Pokeball);
            imgPokeball.AlternateText = pkmn.Pokeball.Name.ToString();
            imgPokeball.ToolTip = pkmn.Pokeball.Name.ToString();
            litLevel.Text = pkmn.Level.ToString();
            litGender.Text = CreateGender(pkmn.Gender);
            litCharacteristic.Text = pkmn.Characteristic.ToString();
            litSpecies.Text = pkmn.Species.Name.ToString();
            litPokedex.Text = pkmn.SpeciesID.ToString("000");
            FormStats fs = pkmn.Form.BaseStats(Generations.Generation4);
            litType1.Text = fs.Type1 == null ? "" : CreateType(fs.Type1);
            litType2.Text = fs.Type2 == null ? "" : CreateType(fs.Type2);
            litOtName.Text = Common.HtmlEncode(pkmn.TrainerName);
            litTrainerId.Text = (pkmn.TrainerID & 0xffff).ToString("00000");
            litExperience.Text = pkmn.Experience.ToString();
            if (pkmn.Level < 100)
            {
                int expCurrLevel = PokemonBase.ExperienceAt(pkmn.Level, pkmn.Species.GrowthRate);
                int expNextLevel = PokemonBase.ExperienceAt(pkmn.Level + 1, pkmn.Species.GrowthRate);
                int progress = pkmn.Experience - expCurrLevel;
                int nextIn = expNextLevel - pkmn.Experience;

                litExperienceNext.Text = String.Format("next in {0}", nextIn);
                litExpProgress.Text = CreateProgress(progress, expNextLevel - expCurrLevel);
            }
            else
            {
                litExperienceNext.Text = "";
                litExpProgress.Text = CreateProgress(0, 1);
            }
            if (pkmn.HeldItem != null)
            {
                imgHeldItem.Visible = true;
                imgHeldItem.ImageUrl = ItemFilename(pkmn.HeldItem);
                litHeldItem.Text = pkmn.HeldItem.Name.ToString();
            }
            else
            {
                imgHeldItem.Visible = false;
                litHeldItem.Text = "";
            }
            litNature.Text = pkmn.Nature.ToString(); // todo: i18n
            litAbility.Text = pkmn.Ability == null ? "" : pkmn.Ability.Name.ToString();
            litHpCurr.Text = pkmn.HP.ToString();
            litHp.Text = pkmn.Stats[Stats.Hp].ToString();
            litHpProgress.Text = CreateProgress(pkmn.HP, pkmn.Stats[Stats.Hp]);
            litAtk.Text = pkmn.Stats[Stats.Attack].ToString();
            litDef.Text = pkmn.Stats[Stats.Defense].ToString();
            litSAtk.Text = pkmn.Stats[Stats.SpecialAttack].ToString();
            litSDef.Text = pkmn.Stats[Stats.SpecialDefense].ToString();
            litSpeed.Text = pkmn.Stats[Stats.Speed].ToString();

            phPkrs.Visible = pkmn.Pokerus == Pokerus.Infected;
            phPkrsCured.Visible = pkmn.Pokerus == Pokerus.Cured;

            rptMoves.DataSource = pkmn.Moves;
            rptMoves.DataBind();

            rptRibbons.DataSource = pkmn.Ribbons;
            rptRibbons.DataBind();

            rptUnknownRibbons.DataSource = pkmn.UnknownRibbons;
            rptUnknownRibbons.DataBind();
        }

        private String ItemFilename(Item item)
        {
            return "~/images/item-sm/" + item.ID.ToString() + ".png";
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

        private String CreateType(Pokedex.Type type)
        {
            return "<span class=\"type " + type.Identifier + "\">" + type.Name.ToString() + "</span>";
        }

        private String CreateProgress(int curr, int max)
        {
            float percent = (float)curr * 100.0f / (float)max;
            return "<div class=\"progress\" style=\"width: " + percent.ToString() + "%;\"></div>";
        }
    }
}