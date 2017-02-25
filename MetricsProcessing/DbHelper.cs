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
        private readonly string _connectionString;

        public DbHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <exception cref="OnlyOneNonProcessedRegistryTakenException">
        /// Too few registries obtained, processing is impossible (there's no possibility of determining
        /// end time of the activity represented by the only registry)
        /// </exception>
        /// <exception cref="NoNonProcessedRegistriesException"></exception>
        /// <exception cref="AllTakenRegistiesBeginActivityException">See description of the exception class</exception>
        public RegistriesList GetRegistries(int quantityToTake)
        {
            using (var context = new MetricsDataContext(_connectionString))
            {
                var registries = context.Registries.
                    Where(r => r.Processed.HasValue && !r.Processed.Value)
                    .Take(quantityToTake)
                    .OrderBy(r => r.Time)
                    .ToList();

                if (registries.Count == 1)
                    throw new OnlyOneNonProcessedRegistryTakenException(registries[0]);
                if (registries.Count == 0)
                    throw new NoNonProcessedRegistriesException();

                return MakeRegistriesList(registries);
            }
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
            var lastRegistryWinTitle = registries.Last().WindowTitle;

            // if all the registries are of the same WindowId
            if (registries.Count == registries.Count(r => r.WindowTitle == lastRegistryWinTitle))
                throw new AllTakenRegistiesBeginActivityException(new RegistriesList(registries, registries.Last().Time));


            // *****************************************************************************
            // find how many last registies should be removed - which have the same WindowTitle
            int lastRegistiesToRemove = 1;
            var cursor = registries[registries.Count - 2]; // penultimate element
            while (cursor.WindowTitle == lastRegistryWinTitle)
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
            using (var context = new MetricsDataContext(_connectionString))
            {
                registries.ForEach(r => r.Processed = true);
                context.SubmitChanges();
            }
        }

        public void MarkFilteredAsProcessed(RegistriesList registries)
        {
            using (var context = new MetricsDataContext(_connectionString))
            {
                registries.FilteredRegistries.ForEach(r => r.Processed = true);
                context.SubmitChanges();
            }
        }

        /// <summary>
        /// Marks 'processed' field with NULL, so that the given registry won't be obtainable
        /// but also isn't marked as a processed.
        /// Usings: 1. If only one registry is obtained, there's no more registries in db - cannot be processed
        /// 2. Filter ban
        /// </summary>
        public void SetProcessedToNull(Registry registry)
        {
            using (var context = new MetricsDataContext(_connectionString))
            {
                var reg = context.Registries.First(r => r.Id == registry.Id);
                reg.Processed = null;
                context.SubmitChanges();
            }
        }

        /// <summary>
        /// Checks if there any more registries in db that stored after given time
        /// </summary>
        public bool AnyMoreRegistriesExist(DateTime after)
        {
            using (var context = new MetricsDataContext(_connectionString))
                return context.Registries.Any(r => r.Time > after);
        }

        public void StoreJsonInActivitiesRegistry(string json)
        {
            using (var context = new MetricsDataContext(_connectionString))
            {
                context.ActivitiesRegistries.InsertOnSubmit(new ActivitiesRegistry()
                {
                    Json = json,
                    Transmitted = false
                });
                context.SubmitChanges();
            }
        }
    }
}
