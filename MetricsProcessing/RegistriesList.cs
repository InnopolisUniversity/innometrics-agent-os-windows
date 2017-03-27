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

        public RegistriesList(List<Registry> list) : base(list)
        {
            EndTime = list.Last().Time; // TODO
            FilteredRegistries = new List<Registry>();
        }

        public RegistriesList(List<Registry> list, DateTime endTime) : base(list)
        {
            EndTime = endTime;
            FilteredRegistries = new List<Registry>();
        }

        public TimeSpan Duration => EndTime - this[0].Time;
        public Registry First => this[0];
        public bool IsEmpty => this.Count == 0;
        public DateTime StartTime => this[0].Time;

        public IEnumerable<long> GetAllIds()
        {
            return this.Select(r => r.Id);
        }

        public void Filter(IEnumerable<string> nameFilter, bool includeNullTitles)
        {
            if (!includeNullTitles)
            {
                FilteredRegistries.AddRange(this.Where(r => r.WindowTitle == null));
            }

            if (nameFilter != null)
            {
                var filterSubstrings = nameFilter.ToArray();
                if (filterSubstrings.Any())
                {
                    foreach (var filterSubstring in filterSubstrings)
                    {
                        var filterExtract =
                            this.Where(r => r.WindowTitle != null && r.WindowTitle.Contains(filterSubstring));
                        FilteredRegistries.AddRange(filterExtract);
                    }
                }
            }

            this.RemoveAll(r => FilteredRegistries.Contains(r));
        }

        public void Filter(IEnumerable<string> nameFilter, bool includeNullTitles, DateTime from, DateTime until)
        {
            var timeExtract = this.Where(r => r.Time < from || r.Time > until).ToList();
            FilteredRegistries.AddRange(timeExtract);
            this.RemoveAll(r => timeExtract.Contains(r));

            if (!this.IsEmpty)
            {
                Filter(nameFilter, includeNullTitles);
            }
        }
    }
}
