using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModels;

namespace MetricsProcessingStraight
{
    public static class DbHelper
    {
        public static List<Registry> GetRegistries(string connectionString)
        {
            using (var context = new MetricsDataContext(connectionString))
            {
                var registries = context.Registries.
                    Where(r => r.Processed.HasValue && !r.Processed.Value)
                    .OrderBy(r => r.Time)
                    .ToList();

                foreach (var registry in registries)
                {
                    registry.IpAddress = context.IpAddresses.Single(ip => ip.Id == registry.Ip);
                    registry.MacAddress = context.MacAddresses.Single(mac => mac.Id == registry.Mac);
                    registry.Username1 = context.Usernames.Single(u => u.Id == registry.Username);
                }

                return registries;
            }
        }


    }
}
