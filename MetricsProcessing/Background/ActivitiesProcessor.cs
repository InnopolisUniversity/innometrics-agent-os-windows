using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModels;
using CommonModels.Helpers;

namespace MetricsProcessing
{
    [ExcludeFromCodeCoverage]
    public class ActivitiesProcessor
    {
        private readonly string _connectionString;

        public ActivitiesProcessor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void StoreActivitiesListInDbAsJson(List<Activity> activities)
        {
            string json = JsonMaker.Serialize(activities);
            DbHelper.StoreJsonInActivitiesRegistry(_connectionString, json);
        }

        public ActivitiesRegistry GetFirstNonTransmittedJsonFromDb()
        {
            return DbHelper.GetFirstNonTransmittedActivitiesRegistry(_connectionString);
        }

        public bool AnyNonTransmittedJsonInDb()
        {
            return DbHelper.AnyNonTransmittedActivitiesRegistries(_connectionString);
        }
    }
}
