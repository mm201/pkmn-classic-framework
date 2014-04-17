using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PokeFoundations.GTS
{
    public static class GtsContextExtender
    {
        /// <summary>
        /// Returns a collection of all active sessions for Generation IV
        /// stored as a dictionary of Hashes onto GtsSession4 objects
        /// </summary>
        public static Dictionary<String, GtsSession4> AllSessions4(this HttpContext context)
        {
            object oSession = context.Application["GtsSession4"];
            Dictionary<String, GtsSession4> session = oSession as Dictionary<String, GtsSession4>;

            if (session == null)
            {
                session = new Dictionary<string, GtsSession4>();
                context.Application.Add("GtsSession4", session);
            }

            return session;
        }

        public static void PruneSessions4(this HttpContext context)
        {
            Dictionary<String, GtsSession4> sessions = context.AllSessions4();
            DateTime now = DateTime.UtcNow;

            foreach (KeyValuePair<String, GtsSession4> session in sessions)
            {
                if (session.Value.ExpiryDate < now) sessions.Remove(session.Key);
            }
        }
    }
}