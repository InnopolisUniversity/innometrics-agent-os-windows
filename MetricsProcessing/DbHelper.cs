using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModels;
using MetricsProcessing.Exceptions;

namespace MetricsProcessing
{
    public class DbHelper
    {
        private readonly MetricsDataContext _context;

        public DbHelper(string connectionString)
        {
            _context = new MetricsDataContext(connectionString);
        }

        /// <exception cref="OnlyOneNonProcessedRegistryTakenException">
        /// Too few registries obtained, processing is impossible (there's no possibility of determining
        /// end time of the activity represented by the only registry)
        /// </exception>
        /// <exception cref="NoNonProcessedRegistriesException">
        /// Such a situation should not happen in a usual case - the last registry in DB cannot be processed
        /// because end time for the activity it can be involved in is unknown
        /// </exception>
        public RegistriesList GetRegistries(int quantityToTake)
        {
            var registries = _context.Registries.
                Where(r => !r.Processed).Take(quantityToTake).OrderBy(r => r.Time).ToList();
            if (registries.Count == 1)
                throw new OnlyOneNonProcessedRegistryTakenException();
            if (registries.Count == 0)
                throw new NoNonProcessedRegistriesException();

            return MakeRegistriesList(registries);
        }

        /// <summary>
        /// The last registry (or several sequential registries with the same WindowId) may be the beginning
        /// of an activity that continues with further registries. It (they) should be removed.
        /// This method detects such registries in the end of input list, performs removal, and stores the 
        /// start time of the first removed registry to EndTime of registries list - it represents the end time
        /// of the last activity the input list.
        /// </summary>
        /// <exception cref="AllTakenRegistiesBeginActivityException">See description of the exception class</exception>
        private static RegistriesList MakeRegistriesList(List<Registry> registries)
        {
            var lastRegistryWinId = registries.Last().WindowId;

            // if all the registries are of the same WindowId
            if (registries.Count == registries.Count(r => r.WindowId == lastRegistryWinId))
                throw new AllTakenRegistiesBeginActivityException();


            // *****************************************************************************
            // find how many last registies should be removed - which have the same WindowId
            int lastRegistiesToRemove = 1;
            var cursor = registries[registries.Count - 2]; // penultimate element
            while (cursor.WindowId == lastRegistryWinId)
            {
                lastRegistiesToRemove++;
                cursor = registries[(registries.Count - 1) - lastRegistiesToRemove];
            }

            // the first activity of the activities should be removed is necessary for
            // determining when the previous activity ended (it's the first registry of the new activity)
            Registry firstRegistryOfNextActivity = registries[registries.Count - lastRegistiesToRemove];

            // remove all the found registries
            registries.RemoveRange(registries.Count - lastRegistiesToRemove, lastRegistiesToRemove);
            // *****************************************************************************

            // All the rest registries can be divided into complete activities
            return new RegistriesList(registries, firstRegistryOfNextActivity.Time);
        }

        public void MarkAsProcessed(RegistriesList registries)
        {
            for (int i = 0; i < registries.Count; i++)
            {
                var registry = _context.Registries.First(r => r.Id == registries[i].Id);
                registry.Processed = true;
            }
            _context.SubmitChanges();
        }
    }
}
