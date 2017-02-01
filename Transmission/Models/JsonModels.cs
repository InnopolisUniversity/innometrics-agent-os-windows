using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmission.Models
{
    public class Measurement
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public object Value { get; set; }
    }

    public class Activity
    {
        public string Name { get; set; }
        public IList<Measurement> Measurements { get; set; } = new List<Measurement>();
    }

    public class Report
    {
        public IList<Activity> Activities { get; set; } = new List<Activity>();
    }
}
