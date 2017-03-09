using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModels;

namespace MetricsProcessing
{
    public class ActivitiesList : List<Activity>
    {
        public List<long> RegistriesIds = new List<long>();
    }
}
