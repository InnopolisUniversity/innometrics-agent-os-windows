using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModels;
using MetricsProcessing.Exceptions;

namespace MetricsProcessing
{
    public class StraightRegistriesProcessor
    {
        private readonly string _connectionString;

        public StraightRegistriesProcessor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void DeleteRegistriesFromDb(IEnumerable<long> ids)
        {
            StraightDbHelper.DeleteRegistries(_connectionString, ids);
        }

        public ActivitiesList Process(IEnumerable<string> nameFilter, bool includeNullTitles, DateTime from, DateTime until)
        {
            ActivitiesList activities = new ActivitiesList();

            RegistriesList registries = StraightDbHelper.GetRegistries(_connectionString);

            if (registries == null)
                return null;

            registries.Filter(nameFilter, includeNullTitles, from, until);
            if (registries.IsEmpty)
                return null;

            while (!registries.IsEmpty)
            {
                Activity activity = null;
                int numOfRegistriesInActivity = DetectActivity(registries);
                RegistriesList activityRegistries = ExtractActivity(registries, numOfRegistriesInActivity);
                activity = CreateActivity(activityRegistries);
                activities.Add(activity);
                activities.RegistriesIds.AddRange(activityRegistries.GetAllIds());
            }

            return activities;
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
                Name = "From",
                Type = "DateTime",
                Value = activityRegistries.First.Time
            });
            activity.Measurements.Add(new Measurement()
            {
                Name = "Until",
                Type = "DateTime",
                Value = activityRegistries.EndTime
            });
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

    }

    //public class StraightRegistriesProcessor : RegistriesProcessor
    //{
    //    private readonly int _registriesToProcessAtOnce;

    //    public StraightRegistriesProcessor(string connectionString, int registriesToProcessAtOnce) : base(connectionString)
    //    {
    //        _registriesToProcessAtOnce = registriesToProcessAtOnce;
    //    }

    //    public List<Activity> Process(bool includeNullTitles, IEnumerable<string> nameFilter, DateTime from, DateTime until)
    //    {
    //        List<Activity> activities = new List<Activity>();

    //        while (true)
    //        {
    //            RegistriesList registries;
    //            try
    //            {
    //                registries = GetRegistries(_registriesToProcessAtOnce);
    //            }
    //            catch (OnlyOneNonProcessedRegistryTakenException e)
    //            {
    //                MarkAllAsNonProcessed();
    //                //MarkAsShouldNotBeProcessed(e.TheOnlyRemainingRegistry);
    //                return activities.Count != 0 ? activities : null;
    //            }
    //            catch (NoNonProcessedRegistriesException)
    //            {
    //                MarkAllAsNonProcessed();
    //                return activities.Count != 0 ? activities : null;
    //            }

    //            registries.Filter(nameFilter, includeNullTitles, from, until);

    //            if (registries.IsEmpty) // if all have been filtered out
    //            {
    //                MarkAsProcessed(registries.FilteredRegistries);
    //            }

    //            while (!registries.IsEmpty)
    //            {
    //                Activity activity = null;
    //                int numOfRegistriesInActivity = DetectActivity(registries);
    //                RegistriesList activityRegistries = ExtractActivity(registries, numOfRegistriesInActivity);
    //                activity = CreateActivity(activityRegistries);
    //                activities.Add(activity);
    //                MarkAsProcessed(activityRegistries);
    //            }
    //            MarkAsProcessed(registries.FilteredRegistries);
    //        }
    //    }

    //    private void MarkAllAsNonProcessed()
    //    {
    //        DbHelper.MarkAllAsNonProcessed(_connectionString);
    //    }
    //}
}
