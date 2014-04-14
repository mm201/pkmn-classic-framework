using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PokeFoundations.GTS;

namespace gts
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write("<pre>");
            WriteHash("bTII8cU1Kx86cTZPhEqXqLivqpRUUVpU");
            WriteHash("7BEqRQlKsqRh8wTdL3rfgFxu053pgPzO");
            WriteHash("081PAAfk5SQhC7LTu1Iq7mwGtQ77xPOR");
            for (int x = 0; x < 4; x++)
            {
                GtsSession4 ses = new GtsSession4(0, "default.aspx");
                WriteHash(ses.Token);
            }
            Response.Write("</pre>");
        }

        private void WriteHash(String token)
        {
            Response.Write(token);
            Response.Write(" ");
            Response.Write(GtsSession4.ComputeHash(token));
            Response.Write(" <br />\n");
        }
    }
}