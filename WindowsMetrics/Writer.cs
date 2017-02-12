using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsMetrics.Helpers;
using CommonModels;

namespace WindowsMetrics
{
    public class Writer : IDisposable
    {
        private readonly MetricsDataContext _context;
        private readonly IList<Registry> _report;

        private event Action DataSaving;

        private Guard _guardDataSaver;
        private Task _taskForGuardDataSaver; // where guard works in

        public Writer(string connectionString, int dataSavingIntervalSec)
        {
            _context = new MetricsDataContext(connectionString);
            if (!_context.DatabaseExists())
                _context.CreateDatabase();

            _report = new List<Registry>();
            DataSaving += OnDataSaving;

            _taskForGuardDataSaver = new Task(() =>
                {
                    _guardDataSaver = new Guard(
                        actionToDoEveryTick: () => DataSaving?.Invoke(),
                        secondsToCountdown: dataSavingIntervalSec
                    );
                }
            );

            _taskForGuardDataSaver.Start();
        }

        private void OnDataSaving()
        {
            for (int i = 0; i < _report.Count; i++)
            {
                var existingUser = _context.Usernames.FirstOrDefault(u => u.Value == _report[i].Username1.Value); // TODO too complicated
                if (existingUser != null)
                    _report[i].Username1 = existingUser;

                var existingIp = _context.IpAddresses.FirstOrDefault(ip => ip.Value == _report[i].IpAddress.Value);
                if (existingIp != null)
                    _report[i].IpAddress = existingIp;

                var existingMac = _context.MacAddresses.FirstOrDefault(m => m.Value == _report[i].MacAddress.Value);
                if (existingMac != null)
                    _report[i].MacAddress = existingMac;
                
                _context.Registries.InsertOnSubmit(_report[i]);
            }
            _context.SubmitChanges(); // TODO transaction
            _report.Clear();
        }

        public void Start()
        {
            _guardDataSaver.Start();
        }

        public void Stop()
        {
            _guardDataSaver.Stop();
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
    }
}
