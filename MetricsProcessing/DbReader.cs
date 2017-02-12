using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModels;
using MetricsProcessing.Exceptions;

namespace MetricsProcessing
{
    public class DbReader
    {
        private readonly MetricsDataContext _context;
        private readonly int _registriesToProcessAtOnce;

        public DbReader(string connectionString ,int registriesToProcessAtOnce)
        {
            _context = new MetricsDataContext(connectionString);
            _registriesToProcessAtOnce = registriesToProcessAtOnce;
        }

        /// <returns>
        /// The last registry in the list should not be processed, it belongs to the next activity. Should be used
        /// do determine the end time of the last activity in obtained sequence.
        /// </returns>
        /// <exception cref="OnlyOneNonProcessedRegistryTakenException">
        /// Too few registries obtained, processing is impossible (there's no possibility of determining
        /// end time of the activity represented by the only registry)
        /// </exception>
        public List<Registry> GetRegistries()
        {
            var registries = _context.Registries.
                Where(r => !r.Processed).Take(_registriesToProcessAtOnce).OrderBy(r => r.Time).ToList();
            if (registries.Count == 1)
                throw new OnlyOneNonProcessedRegistryTakenException();

            registries = RemoveLastRegistries(registries);
            return registries;
        }

        /// <summary>
        /// Finds a chain of registries with the same WindowId in the end of the given sequence and removes it.
        /// This should be done because the chain can be extended with registries which are not in the given sequence,
        /// and each sequence with the same WindowId should be treated like an indivisible activity.
        /// </summary>
        /// <returns>
        /// The last registry in the list should not be processed, it belongs to the next activity. Should be used
        /// do determine the end time of the last activity in obtained sequence.
        /// </returns>
        /// <exception cref="AllTakenRegistiesBeginActivityException">See description of the exception class</exception>
        private static List<Registry> RemoveLastRegistries(List<Registry> registries)
        {
            var lastRegistryWinId = registries.Last().WindowId;

            // if all the registries are of the same WindowId
            if (registries.Count == registries.Count(r => r.WindowId == lastRegistryWinId))
                throw new AllTakenRegistiesBeginActivityException();

            // find how many last registies should be removed - which have the same WindowId
            int lastRegistiesToRemove = 0;
            var cursor = registries[registries.Count - 2]; // penultimate element
            while (cursor.WindowId == lastRegistryWinId)
            {
                lastRegistiesToRemove++;
                cursor = registries[(registries.Count - 1) - lastRegistiesToRemove];
            }

            // remove all the found activities except the first of them - necessary for determining
            // when the previous activity ended (it's the first registry of the new activity)
            registries.RemoveRange(registries.Count - lastRegistiesToRemove + 1, lastRegistiesToRemove);
            return registries;
        }
    }
}
