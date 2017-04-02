using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonModels
{
    public class Measurement
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }
    }

    public class Activity
    {
        public string Name { get; set; }
        public IList<Measurement> Measurements { get; set; }

        public Activity()
        {
            Measurements = new List<Measurement>();
        }
    }

    public class Report
    {
        public IList<Activity> Activities { get; set; }

        public Report()
        {
            Activities = new List<Activity>();
        }
    }
}
