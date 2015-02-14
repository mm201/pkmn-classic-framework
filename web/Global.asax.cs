using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using PkmnFoundations.Data;

namespace PkmnFoundations.GTS
{
    public class Global : System.Web.HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            Application["pkmncfPokedex"] = new Pokedex.Pokedex(Database.Instance, false);
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

        void Application_BeginRequest(object sender, EventArgs e)
        {
            String pathInfo, query;
            String targetUrl = RewriteUrl(Request.Url.PathAndQuery, out pathInfo, out query);

            if (targetUrl != null)
            {
                Context.RewritePath(targetUrl, pathInfo, query, false);
            }
        }

        void Application_EndRequest(object sender, EventArgs e)
        {

        }

        public static String RewriteUrl(String url, out String pathInfo, out String query)
        {
            int q = url.IndexOf('?');
            String path;
            pathInfo = "";

            if (q < 0)
            {
                path = url;
                query = "";
            }
            else
            {
                path = url.Substring(0, q);
                query = url.Substring(q + 1);
            }

            // todo: optimize and extend url pattern matching
            // fixme: this doesn't work if the application isn't mounted at root
            String[] split = path.Split('/');
            if (split[0].Length > 0) return null;

            return null;
        }
    }
}
