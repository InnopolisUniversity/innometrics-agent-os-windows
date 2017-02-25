﻿using System;
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
        private RegistriesProcessor _registriesProcessor;
        private ActivitiesProcessor _activitiesProcessor;
        private event Action ProcessingAndStoring;

        // Parameters for RegistriesProcessor
        private int _processRegistriesAtOneTime;
        private readonly List<string> _nameFilter;
        private readonly bool _includeNullTitles;

        private void CommonConstructor(string connectionString,
            int processRegistriesIntervalSec, int processRegistriesAtOneTime)
        {
            DbHelper helper = new DbHelper(connectionString);
            _registriesProcessor = new RegistriesProcessor(helper);
            _activitiesProcessor = new ActivitiesProcessor(helper);
            _processRegistriesAtOneTime = processRegistriesAtOneTime;
            ProcessingAndStoring += OnRegistriesProcessingAndStoring;

            _taskForGuardRegistriesProcessor = new Task(() =>
                {
                    _guardRegistriesProcessor = new Guard(
                        actionToDoEveryTick: () => ProcessingAndStoring?.Invoke(),
                        secondsToCountdown: processRegistriesIntervalSec
                    );
                }
            );

            _taskForGuardRegistriesProcessor.Start();
        }

        public MetricsProcessor(string connectionString, 
            int processRegistriesIntervalSec, int processRegistriesAtOneTime, List<string> nameFilter, bool includeNullTitles)
        {
            _nameFilter = nameFilter;
            _includeNullTitles = includeNullTitles;
            CommonConstructor(connectionString, processRegistriesIntervalSec, processRegistriesAtOneTime);
        }

        public MetricsProcessor(string connectionString, int processRegistriesIntervalSec, int processRegistriesAtOneTime)
        {
            CommonConstructor(connectionString, processRegistriesIntervalSec, processRegistriesAtOneTime);
        }

        public void Start()
        {
            _guardRegistriesProcessor.Start();
        }

        public void Stop()
        {
            _guardRegistriesProcessor.Stop();
        }

        private void OnRegistriesProcessingAndStoring()
        {
            List<Activity> activities;
            if (_nameFilter == null)
                activities = _registriesProcessor.Process(quantity: _processRegistriesAtOneTime);
            else
                activities = _registriesProcessor.Process(
                    quantity: _processRegistriesAtOneTime,
                    nameFilter: _nameFilter,
                    includeNullTitles: _includeNullTitles
                );
            int u = activities?.Count ?? -1;
            if (activities != null)
            {
                _activitiesProcessor.StoreActivitiesListInDbAsJson(activities);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}