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
    public class Writer : IDisposable
    {
        private readonly string _connectionString;
        private readonly int _dataSavingIntervalSec;

        private readonly IList<Registry> _report;

        private event Action DataSaving;

        private Guard _guardDataSaver;
        private Task _taskForGuardDataSaver; // where guard works in
        private CancellationTokenSource _tokenSource;

        public Writer(string connectionString, int dataSavingIntervalSec)
        {
            _connectionString = connectionString;
            _dataSavingIntervalSec = dataSavingIntervalSec;
            _report = new List<Registry>();
            DataSaving += OnDataSaving;

            using (var context = new MetricsDataContext(_connectionString))
            {
                if (!context.DatabaseExists())
                {
                    context.CreateDatabase();
                    context.SubmitChanges();
                }
            }
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
                CancellationToken t = (CancellationToken)token;
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

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        



        #region examples

        //private readonly string _connectionString;

        //private static event Action DataSaving;

        //private Guard _guardDataSaver;
        //private Task _taskForGuardDataSaver; // where guard works in

        //public Writer(string connectionString, int dataSavingIntervalSec)
        //{
        //    _connectionString = connectionString;
        //    DataSaving += OnDataSaving;
        //}

        //private static void OnDataSaving()
        //{
        //    FileWriteHelper.Write("hoorah", "D:\\loyy.txt");
        //}

        //private CancellationTokenSource tokenSource;
        //private Task task;

        //private static Action<object> x = (token) =>
        //{
        //    CancellationToken t = (CancellationToken) token;
        //    var guardDataSaver = new Guard(
        //        actionToDoEveryTick: () => DataSaving.Invoke(),
        //        secondsToCountdown: 2
        //    );
        //    guardDataSaver.Start();
        //};

        //public void Start()
        //{
        //    tokenSource = new CancellationTokenSource();
        //    var cancellation = tokenSource.Token;
        //    task = new Task(x, cancellation);
        //    task.Start();
        //}

        //public void Stop()
        //{
        //    tokenSource.Cancel();
        //    task.Wait();
        //}

        //public void Dispose()
        //{
        //    throw new NotImplementedException();
        //}












        //private readonly string _connectionString;
        //private int _dataSavingIntervalSec;

        //private readonly IList<Registry> _report;

        //private event Action DataSaving;

        //private Guard _guardDataSaver;
        //private Task _taskForGuardDataSaver; // where guard works in
        //private CancellationTokenSource _tokenSource;

        //public Writer(string connectionString, int dataSavingIntervalSec)
        //{
        //    _connectionString = connectionString;
        //    _dataSavingIntervalSec = dataSavingIntervalSec;
        //    _report = new List<Registry>();
        //    DataSaving += OnDataSaving;
        //}

        //private void OnDataSaving()
        //{
        //    FileWriteHelper.Write("hoorah", "D:\\loyy.txt");
        //    using (var context = new MetricsDataContext(_connectionString))
        //    {
        //        IpAddress ip = new IpAddress()
        //        {
        //            Value = "qwert"
        //        };

        //        context.IpAddresses.InsertOnSubmit(ip);
        //        context.SubmitChanges(); // TODO transaction
        //    }
        //}


        //public void Start()
        //{
        //    _report.Add(new Registry()
        //    {
        //        Event = 1, ExeModulePath = "sdgdsf", Time = DateTime.Now, WindowTitle = "sdfdsf", ProcessName = "sdf", WindowId = "dsf", Processed = false
        //    });

        //    _tokenSource = new CancellationTokenSource();
        //    var cancellation = _tokenSource.Token;
        //    _taskForGuardDataSaver = new Task((token) =>
        //    {
        //        CancellationToken t = (CancellationToken)token;
        //        var guardDataSaver = new Guard(
        //            actionToDoEveryTick: () => DataSaving?.Invoke(),
        //            secondsToCountdown: _dataSavingIntervalSec
        //        );
        //        guardDataSaver.Start();
        //    }, cancellation);
        //    _taskForGuardDataSaver.Start();
        //}

        //public void Stop()
        //{
        //    _tokenSource.Cancel();
        //    _taskForGuardDataSaver.Wait();
        //    if (_report.Count != 0)
        //    {
        //        DataSaving?.Invoke();
        //    }
        //}

        //public void Add(Registry registry)
        //{
        //    _report.Add(registry);
        //}

        //public void Dispose()
        //{
        //    throw new NotImplementedException();
        //}

        #endregion
    }
}
