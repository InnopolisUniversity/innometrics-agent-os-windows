using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModels;
using CommonModels.Helpers;
using MetricsProcessing.Exceptions;

namespace MetricsProcessing
{
    public class RegistriesProcessor
    {
        private readonly string _connectionString;

        public RegistriesProcessor(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <param name="quantity">Number of registries which processor obtains at a time from db and processes</param>
        /// <param name="includeNullTitles">Matters only if nameFilter != null</param>
        /// <param name="nameFilter">Filters out each registry which contains a given substring in its WindowTitle</param>
        /// <returns>
        /// NULL if there's no processible registries obtained or there's no more registries, so there's no more
        /// activities can be obtained.
        /// </returns>
        public List<Activity> Process(int quantity, bool includeNullTitles, IEnumerable<string> nameFilter)
        {
            List<Activity> activities = new List<Activity>();
            RegistriesList registries;
            try
            {
                registries = GetRegistries(quantity);
            }
            catch (OnlyOneNonProcessedRegistryTakenException e)
            {
                MarkAsShouldNotBeProcessed(e.TheOnlyRemainingRegistry);
                return null;
            }
            catch (NoNonProcessedRegistriesException)
            {
                return null;
            }

            registries.Filter(nameFilter, includeNullTitles);

            if (registries.IsEmpty) // if all have been filtered out
            {
                MarkAsProcessed(registries.FilteredRegistries);
                return null;
            }

            while (!registries.IsEmpty)
            {
                Activity activity = null;
                int numOfRegistriesInActivity = DetectActivity(registries);
                RegistriesList activityRegistries = ExtractActivity(registries, numOfRegistriesInActivity);
                activity = CreateActivity(activityRegistries);
                activities.Add(activity);
                MarkAsProcessed(activityRegistries);
            }
            MarkAsProcessed(registries.FilteredRegistries);

            return activities;
        }

        /// <exception cref="OnlyOneNonProcessedRegistryTakenException">
        /// Too few registries obtained, processing is impossible (there's no possibility of determining
        /// end time of the activity represented by the only registry)
        /// </exception>
        /// <exception cref="NoNonProcessedRegistriesException">
        /// Such a situation should not happen in a usual case - the last registry in DB cannot be processed
        /// because end time for the activity it can be involved in is unknown
        /// </exception>
        private RegistriesList GetRegistries(int registriesToProcessAtOnce)
        {
            try
            {
                var registries = DbHelper.GetRegistries(_connectionString, registriesToProcessAtOnce);
                return registries;
            }
            catch (AllTakenRegistiesBeginActivityException e)
            {
                if (DbHelper.AnyMoreRegistriesExist(_connectionString, e.TakenRegistries.Last().Time))
                    return GetRegistries(registriesToProcessAtOnce*2);

                return e.TakenRegistries;
            }
        }

        /// <returns>
        /// Number of registries from the first (activityRegistries[0]) that form an activity
        /// </returns>
        private static int DetectActivity(RegistriesList registries)
        {
            int count = 1;
            var activityWinTitle = registries[0].WindowTitle;
            while (count < registries.Count && registries[count].WindowTitle == activityWinTitle)
                count++;
            return count;
        }

        /// <summary>
        /// Counts the given in 'count' number of registries from the beginning (they form an activity),
        /// removes them from given registries list, returns this sublist (it contains end time of the activity)
        /// </summary>
        private static RegistriesList ExtractActivity(RegistriesList fromRegistries, int count)
        {
            var activity = fromRegistries.Take(count).ToList();
            fromRegistries.RemoveRange(0, count);
            var endTime = fromRegistries.IsEmpty ? fromRegistries.EndTime : fromRegistries[0].Time;
            return new RegistriesList(activity, endTime);
        }

        private static Activity CreateActivity(RegistriesList activityRegistries)
        {
            Activity activity = new Activity()
            {
                Name = activityRegistries.First.WindowTitle,
            };

            activity.Measurements.Add(new Measurement()
            {
                Name = "Duration",
                Type = "TimeSpan",
                Value = activityRegistries.Duration
            });
            activity.Measurements.Add(new Measurement()
            {
                Name = "Executable Path",
                Type = "String",
                Value = activityRegistries.First.ExeModulePath
            });
            activity.Measurements.Add(new Measurement()
            {
                Name = "IP address",
                Type = "String",
                Value = activityRegistries.First.IpAddress.Value
            });
            activity.Measurements.Add(new Measurement()
            {
                Name = "MAC address",
                Type = "String",
                Value = activityRegistries.First.MacAddress.Value
            });
            activity.Measurements.Add(new Measurement()
            {
                Name = "User",
                Type = "String",
                Value = activityRegistries.First.Username1.Value
            });
            if (activityRegistries.Any(r => r.Url != null))
            {
                activity.Measurements.Add(new Measurement()
                {
                    Name = "URL",
                    Type = "String",
                    Value = activityRegistries.First(r => r.Url != null).Url
                });
            }
            
            return activity;
        }

        private void MarkAsProcessed(List<Registry> registries)
        {
            DbHelper.MarkAsProcessed(_connectionString, registries);
        }

        /// <summary>
        /// Marks 'processed' field with NULL, so that the given registry won't be obtainable
        /// but also isn't marked as a processed.
        /// Usings: 1. If only one registry is obtained, there's no more registries in db - cannot be processed
        /// </summary>
        private void MarkAsShouldNotBeProcessed(Registry registry)
        {
            DbHelper.SetProcessedToNull(_connectionString, registry);
        }
    }
}
