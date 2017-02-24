using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModels;
using CommonModels.Helpers;

namespace MetricsProcessing
{
    public class ActivitiesProcessor
    {
        private readonly DbHelper _dbHelper;

        public ActivitiesProcessor(DbHelper helper)
        {
            _dbHelper = helper;
        }

        public void StoreActivitiesListInDbAsJson(List<Activity> activities)
        {
            string json = JsonMaker.Serialize(activities);
            _dbHelper.StoreJsonInActivitiesRegistry(json);
        }
    }
}
