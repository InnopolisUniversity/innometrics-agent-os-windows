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
                    foreach (var registry in registries)
                    {
                        registry.IpAddress = context.IpAddresses.Single(ip => ip.Id == registry.Ip);
                        registry.MacAddress = context.MacAddresses.Single(mac => mac.Id == registry.Mac);
                        registry.Username1 = context.Usernames.Single(u => u.Id == registry.Username);
                    }

                    return MakeRegistriesList(registries);
                }

                return null;
            }
        }

        private static RegistriesList MakeRegistriesList(List<Registry> registries)
        {
            return new RegistriesList(registries, registries[registries.Count - 1].Time);
        }
    }
}
