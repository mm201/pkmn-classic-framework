using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace PkmnFoundations.GTS
{
    public static class IpAddressHelper
    {
        public static string GetIpAddress(HttpRequest request)
        {
            var allowedProxies = ConfigurationManager.AppSettings["AllowedProxies"].Split(',').Select(s => s.Trim());
            string hostAddress = RemovePort(request.UserHostAddress.Trim());
            
            if (!allowedProxies.Contains(hostAddress)) return hostAddress; // return real IP if not a blessed proxy

            var xForwardedFor = request.Headers["X-Forwarded-For"].Split(',').Select(s => RemovePort(s.Trim()));
            foreach (string s in xForwardedFor.Reverse())
            {
                if (!allowedProxies.Contains(s)) return s; // return LAST IP in the proxy chain that's not trusted. (everything coming earlier could be spoofed)
            }

            // these conditions can only happen if the real user is at a blessed proxy IP address. (probably localhost)
            return xForwardedFor.FirstOrDefault() ?? hostAddress;
        }

        private static string RemovePort(string ip)
        {
            if (ip.Contains(':') && ip.Contains('.'))
                return ip.Substring(0, ip.IndexOf(':'));
            else
                return ip;
        }

        public static uint Ipv4ToBinary(string ip)
        {
            string[] split = ip.Split('.');
            if (split.Length != 4) throw new FormatException("Format not valid for an IPV4 address.");

            return BitConverter.ToUInt32(split.Select(s => Convert.ToByte(s)).Reverse().ToArray(), 0);
        }
    }
}
