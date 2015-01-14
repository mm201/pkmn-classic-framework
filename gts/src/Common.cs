using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace PkmnFoundations.Web
{
    public static class Common
    {
        public static string HtmlEncode(string s)
        {
            return HttpUtility.HtmlEncode(s);
        }

        private static byte[] m_pad = null;

        /// <summary>
        /// Encode and decode Gen4 pkgdsprod requests/responses
        /// </summary>
        /// <param name="message"></param>
        public static void CryptMessage(byte[] message)
        {
            if (m_pad == null)
            {
                m_pad = new byte[256];
                FileStream s = File.Open(HttpContext.Current.Server.MapPath("~/pad.bin"), FileMode.Open);
                s.Read(m_pad, 0, m_pad.Length);
                s.Close();
            }

            if (message.Length < 5) return;
            byte padOffset = (byte)(message[0] + message[4]);

            // encrypt and decrypt are the same operation...
            for (int x = 5; x < message.Length; x++)
                message[x] ^= m_pad[(x + padOffset) & 0xff];
        }

        #region File extensions
        public static String GetExtension(String filename)
        {
            int Dot = filename.LastIndexOf('.') + 1;
            if (Dot < 1) return null;
            return filename.Substring(Dot, filename.Length - Dot).ToLowerInvariant();
        }

        public static String GetExtension(String filename, out String namepart)
        {
            int Dot = filename.LastIndexOf('.') + 1;
            if (Dot < 1)
            {
                namepart = filename;
                return null;
            }
            namepart = filename.Substring(0, Dot - 1);
            return filename.Substring(Dot, filename.Length - Dot).ToLowerInvariant();
        }

        #endregion
    }
}