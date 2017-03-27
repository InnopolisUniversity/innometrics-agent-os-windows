using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModels;

namespace MetricsProcessing
{
    public static class StraightDbHelper
    {
        public static void DeleteRegistries(string connectionString, IEnumerable<long> ids)
        {
            var enumerable = ids as long[] ?? ids.ToArray();
            if (!enumerable.Any())
                return;

            using (var context = new MetricsDataContext(connectionString))
            {
                var registriesToDelete = context.Registries.Where(r => enumerable.Contains(r.Id));
                context.Registries.DeleteAllOnSubmit(registriesToDelete);
                context.SubmitChanges();
            }
        }

        public static RegistriesList GetRegistries(string connectionString)
        {
            using (var context = new MetricsDataContext(connectionString))
            {
                var registries = context.Registries.
                    Where(r => r.Processed.HasValue && !r.Processed.Value)
                    .OrderBy(r => r.Time)
                    .ToList();

                if (registries.Any())
                {
                    Dictionary<long, string> ips = new Dictionary<long, string>();
                    Dictionary<long, string> macs = new Dictionary<long, string>();
                    Dictionary<long, string> usernames = new Dictionary<long, string>();

                    foreach (var registry in registries)
                    {
                        if (ips.ContainsKey(registry.Ip.Value))
                            registry.IpAddress = new IpAddress() {Value = ips[registry.Ip.Value]};
                        else
                        {
                            registry.IpAddress = context.IpAddresses.Single(ip => ip.Id == registry.Ip);
                            ips.Add(registry.Ip.Value, registry.IpAddress.Value);
                        }

                        if (macs.ContainsKey(registry.Mac.Value))
                            registry.MacAddress = new MacAddress() {Value = macs[registry.Mac.Value]};
                        else
                        {
                            registry.MacAddress = context.MacAddresses.Single(mac => mac.Id == registry.Mac);
                            macs.Add(registry.Mac.Value, registry.MacAddress.Value);
                        }

                        if (usernames.ContainsKey(registry.Username.Value))
                            registry.Username1 = new Username() {Value = usernames[registry.Username.Value]};
                        else
                        {
                            registry.Username1 = context.Usernames.Single(u => u.Id == registry.Username);
                            usernames.Add(registry.Username.Value, registry.Username1.Value);
                        }
                    }

                    return MakeRegistriesList(registries);
                }

                return null;
            }
        }

        private static RegistriesList MakeRegistriesList(List<Registry> registries)
        {
            return new RegistriesList(registries);
        }
    }
}
