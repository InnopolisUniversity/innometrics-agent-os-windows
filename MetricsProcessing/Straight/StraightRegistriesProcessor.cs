using System;
using System.Collections.Generic;
using System.Linq;
using CommonModels;
using CommonModels.Helpers;

namespace MetricsProcessing.Straight
{
    public class StraightRegistriesProcessor
    {
        private readonly string _connectionString;

        public StraightRegistriesProcessor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void DeleteProcessedRegistriesFromDb()
        {
            StraightDbHelper.DeleteProcessedRegistries(_connectionString);
        }

        public void MarkRegistriesAsProcessed(IEnumerable<long> ids)
        {
            StraightDbHelper.MarkRegistriesAsProcessed(_connectionString, ids);
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
                int numOfRegistriesInActivity = DetectActivity(registries);
                RegistriesList activityRegistries = ExtractActivity(registries, numOfRegistriesInActivity);
                var activity = CreateActivity(activityRegistries);
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
                Name = "WindowsTool-ProcessActivity"
                //activityRegistries.First.WindowTitle.NormalizeToMaxLength255() ?? "NULL"
            };

            activity.Measurements.Add(new Measurement()
            {
                Name = "window title",
                Type = "string",
                Value = activityRegistries.First.WindowTitle.NormalizeToMaxLength255()
            });
            activity.Measurements.Add(new Measurement()
            {
                Name = "From",
                Type = "long",
                Value = activityRegistries.First.Time.GetTimestamp()
            });
            activity.Measurements.Add(new Measurement()
            {
                Name = "Until",
                Type = "long",
                Value = activityRegistries.EndTime.GetTimestamp()
            });
            activity.Measurements.Add(new Measurement()
            {
                Name = "Duration",
                Type = "long",
                Value = (long)activityRegistries.Duration.TotalSeconds
            });
            activity.Measurements.Add(new Measurement()
            {
                Name = "Executable Path",
                Type = "String",
                Value = activityRegistries.First.ExeModulePath.NormalizeToMaxLength255() ?? "NULL"
            });
            activity.Measurements.Add(new Measurement()
            {
                Name = "IP address",
                Type = "String",
                Value = activityRegistries.First.IpAddress.Value.NormalizeToMaxLength255() ?? "NULL"
            });
            activity.Measurements.Add(new Measurement()
            {
                Name = "MAC address",
                Type = "String",
                Value = activityRegistries.First.MacAddress.Value.NormalizeToMaxLength255() ?? "NULL"
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
                    Value = activityRegistries.FirstOrDefault(r => r.Url != null)?.Url.NormalizeToMaxLength255() ?? "NULL"
                });
            }

            return activity;
        }
    }
}
