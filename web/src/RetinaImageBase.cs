using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Web.UI;


namespace PkmnFoundations.Web
{
    /// <summary>
    /// asp:Image with retina substitution capabilities
    /// </summary>
    public abstract class RetinaImageBase : System.Web.UI.WebControls.Image
    {
        public RetinaImageBase() : base()
        {
            this.PreRender += RetinaImageBase_PreRender;
            this.m_css_class = base.CssClass;
            base.CssClass = "retina";
        }

        protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
        {
            WriteDataAttributes(writer);
            base.AddAttributesToRender(writer);
        }

        public void WriteDataAttributes(HtmlTextWriter writer)
        {
            foreach (ImageRendition r in m_scales)
            {
                writer.AddAttribute(AttributeName(r.Scale), ResolveUrl(r.ImageUrl));
            }
        }

        private static NumberFormatInfo nfi = System.Globalization.CultureInfo.InvariantCulture.NumberFormat;

        public static String AttributeName(float scale)
        {
            return "data-hires-" + scale.ToString(nfi);
        }

        public static float getDevicePixelRatio(Page page)
        {
            float dpr = 1.0f;
            if (page.Request.Cookies["pixelRatio"] == null) return 1.0f;
            if (!Single.TryParse(page.Request.Cookies["pixelRatio"].Value, out dpr)) return 1.0f;
            return dpr;
        }

        public void RetinaImageBase_PreRender(object sender, EventArgs e)
        {
            float scale = getDevicePixelRatio(Page);
            m_scales = GetScales();
            m_scales.Sort();

            ImageRendition r = new ImageRendition("", 0.0f);
            foreach (ImageRendition f in m_scales)
            {
                if (r.Scale > 0.0f && f.Scale > scale && r.Scale >= scale) break;
                r = f;
            }

            this.ImageUrl = r.ImageUrl;
        }

        private List<ImageRendition> m_scales;

        protected abstract List<ImageRendition> GetScales();

        private String m_css_class;
        /// <summary>
        /// Custom CSS class(es). A "retina" class will be appended to the start to drive jQuery selectors.
        /// </summary>
        public override string CssClass
        {
            get
            {
                return m_css_class;
            }
            set
            {
                m_css_class = value;
                base.CssClass = value.Length > 0 ? "retina " + value : "retina";
            }
        }

        protected struct ImageRendition : IComparable<ImageRendition>
        {
            public String ImageUrl;
            public float Scale;

            public ImageRendition(String image_url, float scale)
            {
                ImageUrl = image_url;
                Scale = scale;
            }

            public int CompareTo(ImageRendition other)
            {
                return Scale.CompareTo(other.Scale);
            }
        }
    }
}
