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
        private MetricsDataContext context;
        private IList<Registry> report;

        private event Action DataSaving;

        private Guard _guardDataSaver;
        private Task _taskForGuardDataSaver; // where guard works in

        private const int DataSavingIntervalSec = 30; // TODO to config

        public Writer(string connectionString)
        {
            context = new MetricsDataContext(connectionString);
            if (!context.DatabaseExists())
                context.CreateDatabase();

            report = new List<Registry>();
            DataSaving += OnDataSaving;

            _taskForGuardDataSaver = new Task(() =>
                {
                    _guardDataSaver = new Guard(
                        actionToDoEveryTick: () => DataSaving?.Invoke(),
                        secondsToCountdown: DataSavingIntervalSec
                    );
                }
            );

            _taskForGuardDataSaver.Start();
        }

        private void OnDataSaving()
        {
            for (int i = 0; i < report.Count; i++)
            {
                var existingUser = context.Usernames.FirstOrDefault(u => u.Value == report[i].Username1.Value); // TODO too complicated
                if (existingUser != null)
                    report[i].Username1 = existingUser;

                var existingIp = context.IpAddresses.FirstOrDefault(ip => ip.Value == report[i].IpAddress.Value);
                if (existingIp != null)
                    report[i].IpAddress = existingIp;

                var existingMac = context.MacAddresses.FirstOrDefault(m => m.Value == report[i].MacAddress.Value);
                if (existingMac != null)
                    report[i].MacAddress = existingMac;
                
                context.Registries.InsertOnSubmit(report[i]);
            }
            context.SubmitChanges(); // TODO transaction
            report.Clear();
        }

        public void Start()
        {
            _guardDataSaver.Start();
        }

        public void Stop()
        {
            _guardDataSaver.Stop();
            if (report.Count != 0)
            {
                DataSaving?.Invoke();
            }
        }
        
        public void Add(Registry registry)
        {
            report.Add(registry);
        }

        public MetricsDataContext GetContext()
        {
            return context;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
