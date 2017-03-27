using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonModels;
using CommonModels.Helpers;

namespace WindowsMetrics
{
    public class Writer
    {
        private readonly string _connectionString;
        private readonly int _dataSavingIntervalSec;

        private readonly IList<Registry> _report;

        private event Action DataSaving;

        private Guard _guardDataSaver;
        private Task _taskForGuardDataSaver; // where guard works in
        private CancellationTokenSource _tokenSource;

        private static readonly Mutex DatabaseMutex = new Mutex(false, "DatabaseMutex");

        public Writer(string connectionString, int dataSavingIntervalSec)
        {
            _connectionString = connectionString;
            _dataSavingIntervalSec = dataSavingIntervalSec;
            _report = new List<Registry>();
            DataSaving += OnDataSaving;
        }

        private void OnDataSaving()
        {
            using (var context = new MetricsDataContext(_connectionString))
            {
                for (int i = 0; i < _report.Count; i++)
                {
                    var existingUser = context.Usernames.FirstOrDefault(u => u.Value == _report[i].Username1.Value);
                    if (existingUser != null)
                        _report[i].Username1 = existingUser;

                    var existingIp = context.IpAddresses.FirstOrDefault(ip => ip.Value == _report[i].IpAddress.Value);
                    if (existingIp != null)
                        _report[i].IpAddress = existingIp;

                    var existingMac = context.MacAddresses.FirstOrDefault(m => m.Value == _report[i].MacAddress.Value);
                    if (existingMac != null)
                        _report[i].MacAddress = existingMac;

                    context.Registries.InsertOnSubmit(_report[i]);
                }
                context.SubmitChanges(); // TODO transaction
                _report.Clear();
            }
        }

        public void Start()
        {
            _tokenSource = new CancellationTokenSource();
            var cancellation = _tokenSource.Token;
            _taskForGuardDataSaver = new Task((token) =>
            {
                CancellationToken t = (CancellationToken) token;
                _guardDataSaver = new Guard(
                    actionToDoEveryTick: () => DataSaving?.Invoke(),
                    secondsToCountdown: _dataSavingIntervalSec
                );
                _guardDataSaver.Start();
            }, cancellation);
            _taskForGuardDataSaver.Start();
        }

        public void Stop()
        {
            _guardDataSaver.Stop();
            _tokenSource.Cancel();
            _taskForGuardDataSaver.Wait();
            if (_report.Count != 0)
            {
                DataSaving?.Invoke();
            }
        }

        public void Add(Registry registry)
        {
            _report.Add(registry);
        }

        public void CreateDatabaseIfNotExists()
        {
            DatabaseMutex.WaitOne();

            using (var context = new MetricsDataContext(_connectionString))
            {
                if (!context.DatabaseExists())
                {
                    context.CreateDatabase();
                    context.SubmitChanges();
                }
            }

            DatabaseMutex.ReleaseMutex();
        }
    }
}
