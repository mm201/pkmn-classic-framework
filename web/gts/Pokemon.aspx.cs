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
using PkmnFoundations.Wfc;

namespace PkmnFoundations.Web.gts
{
    public partial class Pokemon : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
#if !DEBUG
            throw new WebException(403);
#endif
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Pokedex.Pokedex pokedex = AppStateHelper.Pokedex(Application);
            PokemonPartyBase pkmn = null;

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
                        GtsRecord4 record = Database.Instance.GtsGetRecord4(pokedex, tradeId, isExchanged, true);
                        if (record != null) pkmn = new PokemonParty4(pokedex, record.Data.ToArray());

                    } break;
                    case "5":
                    {
                        GtsRecord5 record = Database.Instance.GtsGetRecord5(pokedex, tradeId, isExchanged, true);
                        if (record != null) pkmn = new PokemonParty5(pokedex, record.Data.ToArray());

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

        private void Bind(PokemonPartyBase pkmn)
        {
            litNickname.Text = pkmn.Nickname;
            bool shiny = pkmn.IsShiny;
            imgPokemon.ImageUrl = WebFormat.PokemonImageLarge(pkmn);
            imgPokemon.AlternateText = pkmn.Species.Name.ToString();
            phShiny.Visible = shiny;
            litMarks.Text = WebFormat.Markings(pkmn.Markings);
            imgPokeball.ImageUrl = WebFormat.ItemImage(pkmn.Pokeball);
            imgPokeball.AlternateText = pkmn.Pokeball.Name.ToString();
            imgPokeball.ToolTip = pkmn.Pokeball.Name.ToString();
            litLevel.Text = pkmn.Level.ToString();
            litGender.Text = WebFormat.Gender(pkmn.Gender);
            litTrainerMemo.Text = pkmn.TrainerMemo.ToString();
            litCharacteristic.Text = pkmn.Characteristic.ToString();
            litSpecies.Text = pkmn.Species.Name.ToString();
            litPokedex.Text = pkmn.SpeciesID.ToString("000");
            FormStats fs = pkmn.Form.BaseStats(pkmn.Generation);
            litType1.Text = fs.Type1 == null ? "" : WebFormat.RenderType(fs.Type1);
            litType2.Text = fs.Type2 == null ? "" : WebFormat.RenderType(fs.Type2);
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
                litExpProgress.Text = WebFormat.RenderProgress(progress, expNextLevel - expCurrLevel);
            }
            else
            {
                litExperienceNext.Text = "";
                litExpProgress.Text = WebFormat.RenderProgress(0, 1);
            }
            if (pkmn.HeldItem != null)
            {
                imgHeldItem.Visible = true;
                imgHeldItem.ImageUrl = WebFormat.ItemImage(pkmn.HeldItem);
                litHeldItem.Text = pkmn.HeldItem.Name.ToString();
            }
            else
            {
                imgHeldItem.Visible = false;
                litHeldItem.Text = "";
            }
            litNature.Text = pkmn.Nature.ToString(); // todo: i18n
            litAbility.Text = pkmn.Ability == null ? "" : pkmn.Ability.Name.ToString();
            litVersion.Text = pkmn.Version.ToString();
            litHaxCheck.Text = pkmn.Validate().IsValid ? "Pass" : "Fail";

            // xxx: loop
            litHpCurr.Text = pkmn.HP.ToString();
            litHp.Text = pkmn.Stats[Stats.Hp].ToString();
            litHpProgress.Text = WebFormat.RenderProgress(pkmn.HP, pkmn.Stats[Stats.Hp]);
            litAtk.Text = pkmn.Stats[Stats.Attack].ToString();
            litDef.Text = pkmn.Stats[Stats.Defense].ToString();
            litSAtk.Text = pkmn.Stats[Stats.SpecialAttack].ToString();
            litSDef.Text = pkmn.Stats[Stats.SpecialDefense].ToString();
            litSpeed.Text = pkmn.Stats[Stats.Speed].ToString();

            litHpIv.Text = pkmn.IVs[Stats.Hp].ToString();
            litAtkIv.Text = pkmn.IVs[Stats.Attack].ToString();
            litDefIv.Text = pkmn.IVs[Stats.Defense].ToString();
            litSAtkIv.Text = pkmn.IVs[Stats.SpecialAttack].ToString();
            litSDefIv.Text = pkmn.IVs[Stats.SpecialDefense].ToString();
            litSpeedIv.Text = pkmn.IVs[Stats.Speed].ToString();

            litHpEv.Text = pkmn.EVs[Stats.Hp].ToString();
            litAtkEv.Text = pkmn.EVs[Stats.Attack].ToString();
            litDefEv.Text = pkmn.EVs[Stats.Defense].ToString();
            litSAtkEv.Text = pkmn.EVs[Stats.SpecialAttack].ToString();
            litSDefEv.Text = pkmn.EVs[Stats.SpecialDefense].ToString();
            litSpeedEv.Text = pkmn.EVs[Stats.Speed].ToString();

            phPkrs.Visible = pkmn.Pokerus == Pokerus.Infected;
            phPkrsCured.Visible = pkmn.Pokerus == Pokerus.Cured;

            rptMoves.DataSource = pkmn.Moves;
            rptMoves.DataBind();

            rptRibbons.DataSource = pkmn.Ribbons;
            rptRibbons.DataBind();

            rptUnknownRibbons.DataSource = pkmn.UnknownRibbons;
            rptUnknownRibbons.DataBind();
        }

    }
}