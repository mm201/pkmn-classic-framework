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
            Pokemon4 pokemon = null;
            StringBuilder builder = new StringBuilder();

            try
            {
                pokemon = new Pokemon4(m_pokedex, record.Data);
            }
            catch
            {
                builder.Append("Failure to parse pokemon data.<br />");
            }

            try
            {
                try
                {
                    builder.Append(String.Format("{0} (#{1})", pokemon.Species.Name, pokemon.SpeciesID));
                }
                catch (KeyNotFoundException)
                {
                    builder.Append(String.Format("??? (#{0})", record.Species));
                }
                catch (NullReferenceException)
                {
                    builder.Append(String.Format("??? (#{0})", record.Species));
                }
                builder.Append("<br />");
                try
                {
                    if (pokemon.FormID != 0)
                    {
                        try
                        {
                            builder.Append(pokemon.Form.Name);
                        }
                        catch (KeyNotFoundException)
                        {
                            builder.Append("Unknown form");
                        }
                        catch (NullReferenceException)
                        {
                            builder.Append("Unknown form");
                        }
                        builder.Append("<br />");
                    }
                }
                catch (NullReferenceException)
                {
                    builder.Append("Unknown form");
                }
                builder.Append(String.Format("Lv {0}", record.Level));
                builder.Append(String.Format(", {0}", record.Gender));
                builder.Append("<br />");
                try
                {
                    builder.Append(String.Format("Nature: {0}", pokemon.Nature.ToString()));
                }
                catch (NullReferenceException)
                {
                    builder.Append("Nature: ???");
                }
                builder.Append("<br />");
                try
                {
                    builder.Append(String.Format("Ability: {0}", pokemon.Ability.Name));
                }
                catch (KeyNotFoundException)
                {
                    builder.Append("Ability: ???");
                }
                catch (NullReferenceException)
                {
                    builder.Append("Ability: ???");
                }
                builder.Append("<br />");

                return builder.ToString();
            }
            catch
            {
                return "Encountered an error trying to display this offer.";
            }
        }

        protected String CreateWanted4(object DataItem)
        {
            GtsRecord4 record = (GtsRecord4)DataItem;
            return String.Format("{3} (#{0})<br />{1}<br />Lv {2}",
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
            return String.Format("{3} (#{0})<br />{1}<br />Lv {2}", 
                record.Species, 
                record.Gender, 
                record.Level,
                m_pokedex.Species(record.Species).Name);
        }

        protected String CreateWanted5(object DataItem)
        {
            GtsRecord5 record = (GtsRecord5)DataItem;
            return String.Format("{3} (#{0})<br />{1}<br />Lv {2}",
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