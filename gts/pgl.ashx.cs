using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PkmnFoundations.GTS
{
    /// <summary>
    /// Summary description for pgl
    /// </summary>
    public class pgl : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello (dream) World");
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}
