using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModels;

namespace MetricsProcessing
{
    public class RegistriesList : List<Registry>
    {
        public DateTime EndTime { get; }

        public RegistriesList(List<Registry> list, DateTime endTime) : base(list)
        {
            EndTime = endTime;
        }

        public TimeSpan Duration => EndTime - this[0].Time;
        public Registry First => this[0];
        public bool IsEmpty => this.Count == 0;
        public DateTime StartTime => this[0].Time;
    }
}
