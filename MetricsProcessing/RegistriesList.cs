using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModels;

namespace MetricsProcessing
{
    public class RegistriesList : List<Registry>
    {
        public DateTime EndTime { get; }
        public List<Registry> FilteredRegistries { get; }

        public RegistriesList(List<Registry> list, DateTime endTime) : base(list)
        {
            EndTime = endTime;
            FilteredRegistries = new List<Registry>();
        }

        public TimeSpan Duration => EndTime - this[0].Time;
        public Registry First => this[0];
        public bool IsEmpty => this.Count == 0;
        public DateTime StartTime => this[0].Time;

        public void Filter(IEnumerable<string> nameFilter, bool includeNullTitles)
        {
            var filterSubstrings = nameFilter as string[];
            if (filterSubstrings != null && filterSubstrings.Any())
            {
                foreach (var filterSubstring in filterSubstrings)
                {
                    var filterExtract = includeNullTitles ?
                        this.Where(r => r.WindowTitle != null && r.WindowTitle.Contains(filterSubstring)):
                        this.Where(r => r.WindowTitle == null || r.WindowTitle.Contains(filterSubstring));
                    FilteredRegistries.AddRange(filterExtract);
                }
            }

            if (!includeNullTitles)
            {
                FilteredRegistries.AddRange(this.Where(r => r.WindowTitle == null));
            }

            this.RemoveAll(r => FilteredRegistries.Contains(r));
        }
    }
}
