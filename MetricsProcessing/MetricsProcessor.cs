using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonModels;
using CommonModels.Helpers;

namespace MetricsProcessing
{
    public class MetricsProcessor : IDisposable
    {
        private Guard _guardRegistriesProcessor;
        private Task _taskForGuardRegistriesProcessor; // where guard works in
        private CancellationTokenSource _tokenSource;

        private RegistriesProcessor _registriesProcessor;
        private ActivitiesProcessor _activitiesProcessor;
        private event Action ProcessingAndStoring;

        // Parameters for RegistriesProcessor
        private int _processRegistriesAtOneTime;
        private int _processRegistriesIntervalSec;
        private List<string> _nameFilter;
        private readonly bool _includeNullTitles;

        private void CommonConstructor(string connectionString,
            int processRegistriesIntervalSec, int processRegistriesAtOneTime)
        {
            _registriesProcessor = new RegistriesProcessor(connectionString);
            _activitiesProcessor = new ActivitiesProcessor(connectionString);
            _processRegistriesAtOneTime = processRegistriesAtOneTime;
            _processRegistriesIntervalSec = processRegistriesIntervalSec;
            ProcessingAndStoring += OnRegistriesProcessingAndStoring;
        }

        public MetricsProcessor(string connectionString,
            int processRegistriesIntervalSec, int processRegistriesAtOneTime,
            bool includeNullTitles, List<string> nameFilter = null)
        {
            _nameFilter = nameFilter;
            _includeNullTitles = includeNullTitles;
            CommonConstructor(connectionString, processRegistriesIntervalSec, processRegistriesAtOneTime);
        }

        public MetricsProcessor(string connectionString,
            int processRegistriesIntervalSec, int processRegistriesAtOneTime)
        {
            _nameFilter = null;
            _includeNullTitles = true;
            CommonConstructor(connectionString, processRegistriesIntervalSec, processRegistriesAtOneTime);
        }

        public void SetNameFilter(List<string> strings)
        {
            _nameFilter = strings;
        }

        public void Start()
        {
            _tokenSource = new CancellationTokenSource();
            var cancellation = _tokenSource.Token;

            _taskForGuardRegistriesProcessor = new Task(() =>
            {
                _guardRegistriesProcessor = new Guard(
                    actionToDoEveryTick: () =>
                    {
                        ProcessingAndStoring?.Invoke();
                    },
                    secondsToCountdown: _processRegistriesIntervalSec
                );
                _guardRegistriesProcessor.Start();
            }, cancellation);
            _taskForGuardRegistriesProcessor.Start();
        }

        public void Stop()
        {
            _guardRegistriesProcessor.Stop();
            _tokenSource.Cancel();
            _taskForGuardRegistriesProcessor.Wait();
        }

        private void OnRegistriesProcessingAndStoring()
        {
            List<Activity> activities = _registriesProcessor.Process(
                quantity: _processRegistriesAtOneTime,
                nameFilter: _nameFilter,
                includeNullTitles: _includeNullTitles
            );
            if (activities != null)
            {
                _activitiesProcessor.StoreActivitiesListInDbAsJson(activities);
            }
        }

        public ActivitiesRegistry GetJsonItem()
        {
            return _activitiesProcessor.GetFirstNonTransmittedJsonFromDb();
        }

        public bool AnyNonTransmittedJson()
        {
            return _activitiesProcessor.AnyNonTransmittedJsonInDb();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
