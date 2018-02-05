using System;
using System.Collections.Generic;

namespace MetricsProcessing.Straight
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

        public void MarkRegistriesAsProcessed(IEnumerable<long> ids)
        {
            _registriesProcessor.MarkRegistriesAsProcessed(ids);
        }

        public void DeleteProcessedRegistriesFromDb()
        {
            _registriesProcessor.DeleteProcessedRegistriesFromDb();
        }
    }
}
