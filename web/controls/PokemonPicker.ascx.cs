using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PkmnFoundations.Structures;

namespace PkmnFoundations.Web.controls
{
    public partial class PokemonPicker : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public int ? Value
        {
            get
            {
                String value = theLookup.Value;
                return String.IsNullOrEmpty(value) ? null : (int ?)Convert.ToInt32(value);
            }
            set
            {
                if (value == null) theLookup.Value = null;
                theLookup.Value = value.ToString();
            }
        }

        private Generations m_max_generation;
        public Generations MaxGeneration
        {
            get
            {
                return m_max_generation;
            }
            set
            {
                m_max_generation = value;
                theLookup.SourceUrl = "~/controls/PokemonSource.ashx?limit=" + Pokedex.Pokedex.SpeciesAtGeneration(value).ToString();
            }
        }
    }
}