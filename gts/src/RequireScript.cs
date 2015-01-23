using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PkmnFoundations.Web
{
    public class RequireScript : RequireLinkBase
    {
        public RequireScript() : base()
        {

        }

        public override void RenderHeader(System.Web.UI.HtmlTextWriter writer)
        {
            writer.AddAttribute("src", ResolveUrl(ScriptUrl ?? ""));
            writer.AddAttribute("type", Type ?? "text/javascript");
            writer.RenderBeginTag("script");
            writer.RenderEndTag();
        }

        public String ScriptUrl { get; set; }
        public String Type { get; set; }

        public override string Key
        {
            get
            {
                if (base.Key != null)
                    return base.Key;
                return ScriptUrl;
            }
            set
            {
                base.Key = value;
            }
        }
    }
}