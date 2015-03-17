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

        private void WriteDataAttributes(HtmlTextWriter writer)
        {
            foreach (ImageRendition r in m_scales)
            {
                writer.AddAttribute(AttributeName(r.Scale), ResolveUrl(r.ImageUrl));
            }
        }

        private static NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;

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

            base.ImageUrl = r.ImageUrl;
        }

        private List<ImageRendition> m_scales;

        protected abstract List<ImageRendition> GetScales();

        public new string ImageUrl
        {
            get;
            set;
        }

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
                UpdateCssClass();
            }
        }

        private bool m_keep_high_res;
        /// <summary>
        /// If this is true, a high resolution image will be kept in place even
        /// after the image has shrunken down to the next stop. This can
        /// prevent unnecessary image downloads.
        /// </summary>
        public bool KeepHighRes
        {
            get
            {
                return m_keep_high_res;
            }
            set
            {
                m_keep_high_res = value;
                UpdateCssClass();
            }
        }

        private void UpdateCssClass()
        {
            String prefix = m_keep_high_res ? "retina keephr" : "retina";
            base.CssClass = m_css_class.Length > 0 ? (prefix + " " + m_css_class) : prefix;
        }

        protected override void LoadViewState(object savedState)
        {
            RetinaImageBaseViewState viewstate = (RetinaImageBaseViewState)savedState;
            base.LoadViewState(viewstate.ImageViewState);
            this.ImageUrl = viewstate.ImageUrl;
            m_css_class = viewstate.CssClass;
            m_keep_high_res = viewstate.KeepHighRes;
            UpdateCssClass();
        }

        protected override object SaveViewState()
        {
            RetinaImageBaseViewState viewstate = new RetinaImageBaseViewState();
            viewstate.ImageViewState = base.SaveViewState();
            viewstate.ImageUrl = this.ImageUrl;
            viewstate.CssClass = this.CssClass;
            viewstate.KeepHighRes = this.KeepHighRes;
            return viewstate;
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

        [Serializable()]
        private struct RetinaImageBaseViewState
        {
            public object ImageViewState;
            public String ImageUrl;
            public String CssClass;
            public bool KeepHighRes;
        }
    }
}
