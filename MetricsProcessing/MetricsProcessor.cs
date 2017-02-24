using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModels;
using CommonModels.Helpers;

namespace MetricsProcessing
{
    public class MetricsProcessor : IDisposable
    {
        private Guard _guardRegistriesProcessor;
        private Task _taskForGuardRegistriesProcessor; // where guard works in
        private RegistriesProcessor _processor;
        private event Action Processing;

        // Parameters for RegistriesProcessor
        private int _processRegistriesAtOneTime;
        private readonly List<string> _nameFilter;
        private readonly bool _includeNullTitles;

        // Where all the obtained activities stored
        public List<Activity> Activities { get; private set; }

        private void CommonConstructor(MetricsDataContext context,
            int processRegistriesIntervalSec, int processRegistriesAtOneTime)
        {
            Activities = new List<Activity>();
            _processor = new RegistriesProcessor(context);
            _processRegistriesAtOneTime = processRegistriesAtOneTime;
            Processing += OnRegistriesProcessing;

            _taskForGuardRegistriesProcessor = new Task(() =>
                {
                    _guardRegistriesProcessor = new Guard(
                        actionToDoEveryTick: () => Processing?.Invoke(),
                        secondsToCountdown: processRegistriesIntervalSec
                    );
                }
            );

            _taskForGuardRegistriesProcessor.Start();
        }

        public MetricsProcessor(MetricsDataContext context, 
            int processRegistriesIntervalSec, int processRegistriesAtOneTime, List<string> nameFilter, bool includeNullTitles)
        {
            _nameFilter = nameFilter;
            _includeNullTitles = includeNullTitles;
            CommonConstructor(context, processRegistriesIntervalSec, processRegistriesAtOneTime);
        }

        public MetricsProcessor(MetricsDataContext context, int processRegistriesIntervalSec, int processRegistriesAtOneTime)
        {
            CommonConstructor(context, processRegistriesIntervalSec, processRegistriesAtOneTime);
        }

        public void Start()
        {
            _guardRegistriesProcessor.Start();
        }

        public void Stop()
        {
            _guardRegistriesProcessor.Stop();
        }

        private void OnRegistriesProcessing()
        {
            if (_nameFilter == null)
                Activities.AddRange(_processor.Process(quantity: _processRegistriesAtOneTime));
            else
                Activities.AddRange(
                    _processor.Process(
                        quantity: _processRegistriesAtOneTime,
                        nameFilter: _nameFilter,
                        includeNullTitles: _includeNullTitles
                    ));
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
