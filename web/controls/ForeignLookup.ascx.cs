using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace PkmnFoundations.Web.controls
{
    public partial class ForeignLookup : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SourceUrl != null)
            {
                String url = ResolveUrl(SourceUrl);
                //url = url + (url.Contains('?') ? "&lang=" : "?lang=") + ((BasePage)Page).Language;

                String _params = "\'" + this.ClientID + "', '"
                                 + txtInput.ClientID + "', '"
                                 + results.ClientID + "', '"
                                 + MaxRows.ToString() + "', '"
                                 + url + "\'";

                main.Attributes["onfocus"] = "pfHandleLookupKeypress(" + _params + ")";
                main.Attributes["onblur"] = "pfHideLookupResults('" + results.ClientID + "')";
                txtInput.Attributes["onfocus"] = "pfHandleLookupKeypress(" + _params + ")";
                txtInput.Attributes["onkeypress"] = "return pfHandleLookupKeypress2(" + _params + ", event)";
                txtInput.Attributes["onkeyup"] = "return pfHandleLookupKeypress3(" + _params + ")";
                txtInput.Attributes["onchange"] = "pfHandleLookupKeypress(" + _params + ")";
                txtInput.Attributes["onblur"] = "setTimeout(function(){pfHideLookupResults('" + results.ClientID + "')},100)";
                hdSelectedValue.Attributes["onchange"] = OnClientValueChanged;
            }
        }

        public String Value
        {
            get
            {
                return hdSelectedValue.Value;
            }
            set
            {
                hdSelectedValue.Value = value;
            }
        }

        public String Text
        {
            get
            {
                return txtInput.Text;
            }
            set
            {
                txtInput.Text = value;
            }
        }

        public String SourceUrl { get; set; }
        public int MaxRows { get; set; }
        public String OnClientValueChanged { get; set; }
        public String HiddenClientID
        {
            get
            {
                return hdSelectedValue.ClientID;
            }
        }

        public String TextClientID
        {
            get
            {
                return txtInput.ClientID;
            }
        }

        protected override void LoadViewState(object savedState)
        {
            ForeignLookupViewState viewstate = (ForeignLookupViewState)savedState;
            base.LoadViewState(viewstate.UserControlViewState);
            SourceUrl = viewstate.SourceUrl;
            MaxRows = viewstate.MaxRows;
            OnClientValueChanged = viewstate.OnClientValueChanged;
        }

        protected override object SaveViewState()
        {
            ForeignLookupViewState viewstate = new ForeignLookupViewState();
            viewstate.UserControlViewState = base.SaveViewState();
            viewstate.SourceUrl = SourceUrl;
            viewstate.MaxRows = MaxRows;
            viewstate.OnClientValueChanged = OnClientValueChanged;
            return viewstate;
        }

        [Serializable()]
        private struct ForeignLookupViewState
        {
            public object UserControlViewState;
            public String SourceUrl;
            public int MaxRows;
            public String OnClientValueChanged;
        }
    }
}
