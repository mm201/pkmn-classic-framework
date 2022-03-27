using PkmnFoundations.Data;
using PkmnFoundations.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PkmnFoundations.GTS
{
    public static class BanHelper
    {
        public static BanStatus GetBanStatus(int pid, string IpAddress, Generations generation)
        {
            try
            {
                BanStatus pidBan = Database.Instance.CheckBanStatus(pid);
                BanStatus ipBan = Database.Instance.CheckBanStatus(IpAddress);
                BanStatus macBan = null;
                BanStatus ipRangeBan = null;

                try
                {
                    switch (generation)
                    {
                        case Generations.Generation4:
                        {
                            var profile = Database.Instance.GamestatsGetProfile4(pid);
                            macBan = Database.Instance.CheckBanStatus(profile.MacAddress);
                            break;
                        }
                        case Generations.Generation5:
                        {
                            var profile = Database.Instance.GamestatsGetProfile5(pid);
                            macBan = Database.Instance.CheckBanStatus(profile.MacAddress);
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                }

                try
                {
                    uint ipBinary = IpAddressHelper.Ipv4ToBinary(IpAddress);
                    ipRangeBan = Database.Instance.CheckBanStatus(ipBinary);
                }
                catch (Exception)
                {
                }

                return new[] { pidBan, ipBan, macBan, ipRangeBan }.Where(ban => ban != null).OrderBy(ban => ban.Level).LastOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
