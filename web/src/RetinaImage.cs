using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace PkmnFoundations.Web
{
    public class RetinaImage : RetinaImageBase
    {
        /// <summary>
        /// Image with DPI substitution. Alternate filenames are generated based on a naming pattern.
        /// </summary>
        public RetinaImage()
            : base()
        {
            this.FormatString = "{3}{0}@{2:#.##}x.{1}";
            this.AlternateSizes = "";
            this.FormatScaleFactor = 1;
            this.Format1x = false;
        }

        public static String ScaledImageName(String filename, String format, float scale)
        {
            // todo: handle ? querystring
            String namepart;
            String extension = Common.GetExtension(filename, out namepart);
            int slash = namepart.LastIndexOf('/');
            String fileNamepart, directory;
            if (slash >= 0)
            {
                fileNamepart = namepart.Substring(slash + 1);
                directory = namepart.Substring(0, slash + 1);
            }
            else
            {
                fileNamepart = namepart;
                directory = "";
            }
            return String.Format(format, fileNamepart, extension, scale, directory);
        }

        protected override List<ImageRendition> GetScales()
        {
            List<ImageRendition> result = new List<ImageRendition>();

            String[] split = AlternateSizes.Split(',');
            List<float> scales = new List<float>(split.Length + 1);

            if (Format1x) result.Add(new ImageRendition(ScaledImageName(ImageUrl, FormatString, FormatScaleFactor), 1.0f));
            else result.Add(new ImageRendition(ImageUrl, 1.0f));

            foreach (String s in split)
            {
                float result2;
                if (!Single.TryParse(s, NumberStyles.Number, nfi, out result2)) continue;
                result.Add(new ImageRendition(ScaledImageName(ImageUrl, FormatString, result2 * FormatScaleFactor), result2));
            }

            return result;
        }

        private static NumberFormatInfo nfi = System.Globalization.CultureInfo.InvariantCulture.NumberFormat;

        protected override void LoadViewState(object savedState)
        {
            RetinaImageViewState viewstate = (RetinaImageViewState)savedState;
            base.LoadViewState(viewstate.RetinaImageBaseViewState);
            FormatString = viewstate.FormatString;
            AlternateSizes = viewstate.AlternateSizes;
        }

        protected override object SaveViewState()
        {
            RetinaImageViewState viewstate = new RetinaImageViewState();
            viewstate.RetinaImageBaseViewState = base.SaveViewState();
            viewstate.FormatString = FormatString;
            viewstate.AlternateSizes = AlternateSizes;
            return viewstate;
        }

        /// <summary>
        /// Format string for String.Format.
        /// 0: filename, 1: extension, 2: scale, 3: path
        /// </summary>
        public String FormatString
        {
            get;
            set;
        }

        public String AlternateSizes
        {
            get;
            set;
        }

        /// <summary>
        /// Multipy DPI scale by this constant in filenames. Useful for paths like /thumb_100/ and /thumb_200/
        /// </summary>
        public float FormatScaleFactor
        {
            get;
            set;
        }

        /// <summary>
        /// If true, 1x images go through the format string. Otherwise they use the raw ImageUrl.
        /// </summary>
        public bool Format1x
        {
            get;
            set;
        }

        [Serializable()]
        protected struct RetinaImageViewState
        {
            public object RetinaImageBaseViewState;
            public String FormatString;
            public String AlternateSizes;
        }
    }
}
