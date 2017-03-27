using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModels;

namespace MetricsProcessing
{
    public class StraightMetricsProcessor
    {
        private readonly StraightRegistriesProcessor _registriesProcessor;

        public StraightMetricsProcessor(string connectionString)
        {
            _registriesProcessor = new StraightRegistriesProcessor(connectionString);
        }
        
        public ActivitiesList Process(List<string> nameFilter, bool includeNullTitles, DateTime from, DateTime until)
        {
            return _registriesProcessor.Process(nameFilter, includeNullTitles, from, until);
        }

        public void DeleteRegistriesFromDb(IEnumerable<long> ids)
        {
            _registriesProcessor.DeleteRegistriesFromDb(ids);
        }
    }
}
