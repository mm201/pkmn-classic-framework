using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PkmnFoundations.Web
{
    public class RequireCss : RequireLinkBase
    {
        public RequireCss() : base()
        {

        }

        public override void RenderHeader(System.Web.UI.HtmlTextWriter writer)
        {
            writer.AddAttribute("rel", "stylesheet");
            writer.AddAttribute("href", ResolveUrl(CssUrl ?? ""));
            writer.AddAttribute("type", "text/css");
            writer.RenderBeginTag("link");
            writer.RenderEndTag();
        }

        public String CssUrl { get; set; }

        public override string Key
        {
            get
            {
                if (base.Key != null)
                    return base.Key;
                return CssUrl;
            }
            set
            {
                base.Key = value;
            }
        }
    }
}