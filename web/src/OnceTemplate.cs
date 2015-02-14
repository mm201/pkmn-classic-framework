using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PkmnFoundations.Web
{
    /// <summary>
    /// Control which only renders once per unique key on a given page.
    /// </summary>
    public class OnceTemplate : System.Web.UI.WebControls.PlaceHolder
    {
        public OnceTemplate() : base()
        {
        }

        public String Key { get; set; }

        private HashSet<String> m_keys = null;
        private HashSet<String> Keys
        {
            get
            {
                if (m_keys != null) return m_keys;
                if (!Page.Items.Contains("pkmncfOnceTemplate"))
                {
                    m_keys = new HashSet<String>();
                    Page.Items.Add("pkmncfOnceTemplate", m_keys);
                }
                else m_keys = (HashSet<String>)Page.Items["pkmncfOnceTemplate"];
                return m_keys;
            }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (Keys.Contains(Key)) return;
            Keys.Add(Key);
            base.Render(writer);
        }
    }
}
