using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CommonModels;
using MetricsProcessing;
using Transmission;
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO;
using CommonModels.Helpers;
using Microsoft.Win32.SafeHandles;
using Microsoft.Win32;

namespace TestConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Activity a = new Activity { Name = "Activity 1" };
            a.Measurements.Add(new Measurement()
            {
                Name = "Quality",
                Type = typeof(string).Name,
                Value = "bad"
            });

            Activity b = new Activity { Name = "Activity 2" };
            b.Measurements.Add(new Measurement()
            {
                Name = "Time",
                Type = typeof(int).Name,
                Value = 14779
            });
            b.Measurements.Add(new Measurement()
            {
                Name = "Quality",
                Type = typeof(string).Name,
                Value = "good"
            });
            Report report = new Report() { Activities = new List<Activity>() { a, b } };
            string json = JsonMaker.Serialize(report);



            //Activity a = new Activity { Name = "Activity 1" };
            //a.Measurements.Add(new Measurement()
            //{
            //    Name = "Время",
            //    Type = typeof(DateTime).Name,
            //    Value = DateTime.Now
            //});
            //a.Measurements.Add(new Measurement()
            //{
            //    Name = "dgdfg fdgfdg fdgfdg",
            //    Type = typeof(string).Name,
            //    Value = "bad"
            //});

            //Activity b = new Activity { Name = "Activity 2" };
            //b.Measurements.Add(new Measurement()
            //{
            //    Name = "Time",
            //    Type = typeof(int).Name,
            //    Value = 14779
            //});
            //b.Measurements.Add(new Measurement()
            //{
            //    Name = "Quality",
            //    Type = "4444444444444445",
            //    Value = "good"
            //});
            //Report report = new Report() {Activities = new List<Activity>() {a, b}};

            HttpStatusCode statusCode;
            Sender sender = new Sender("http://innometrics.guru:8000/api-token-auth/", "http://innometrics.guru:8000/activities/");
            bool succReg = sender.Authorize("as", "123456", out statusCode);

            string xxx;
            using (FileStream fs = new FileStream("D:\\awe.txt", FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    xxx = reader.ReadLine();
                }
            }

            string res;
            if (succReg)
            {

                    res = sender.SendActivities(xxx, out statusCode);
      
            }



            //string json = JsonMaker.Serialize(report);
            //string statusCode;
            ////var s = Sender.Send("http://httpbin.org/post", json, "application/json", out statusCode);
            //var s = Sender.Send("https://aqueous-escarpment-80312.herokuapp.com/activities/", json, "application/json", out statusCode);

            //Console.WriteLine(s);
            //Console.WriteLine();
            //Console.WriteLine(statusCode);


            var xxxt = In();
            //foreach (var row in asd.Rows)
            //{
            //    var rw = row as DataRow;
            //    var ppp = rw["ServerName"];
            //    var ddd = rw["InstanceName"];
            //}

            

            List<int> ri = new List<int>() {0, 1, 2, 3, 4, 5, 6, 7};

            var ui = Foo(ri);

            int p = 100;
            Foo2(p);

            List<string> filter = new List<string>()
            {
                "Tele"
            };

            //MetricsDataContext context = new MetricsDataContext(@"Data Source=DESKTOP-7CAUMID\SQLEXPRESS;Initial Catalog=WindowsMetrics;Integrated Security=True");
            //RegistriesProcessor rp = new RegistriesProcessor(@"Data Source=DESKTOP-7CAUMID\SQLEXPRESS;Initial Catalog=WindowsMetrics;Integrated Security=True");
            //var act = rp.Process(100, filter, includeNullTitles: true);
            //var act2 = rp.Process(50, filter, includeNullTitles: true);
            //var xx = JsonMaker.Serialize(act);
            //FileWriteHelper.Write(xx, @"D:\aaa.txt");

            Console.ReadKey();
        }


        private static List<int> Foo(List<int> l)
        {
            List<int> res = l.Take(3).ToList();
            l.RemoveRange(0, 3);
            return res;
        }

        private static int Foo2(int x)
        {
            return x-- * 2;
        }


        public static string[] In()
        {
            //DataRowCollection rows = SqlDataSourceEnumerator.Instance.GetDataSources().Rows;
            //string[] instances = new string[rows.Count];

            //for (int i = 0; i < rows.Count; i++)
            //{
            //    instances[i] = Convert.ToString(rows[i]["InstanceName"]);
            //}

            //return instances;

            //System.Data.Sql.SqlDataSourceEnumerator instance =
            //    System.Data.Sql.SqlDataSourceEnumerator.Instance;

            //System.Data.DataTable dataTable = instance.GetDataSources();

            //return dataTable;

            List<string> erter = new List<string>();

            RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey instanceKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);
                if (instanceKey != null)
                {
                    foreach (var instanceName in instanceKey.GetValueNames())
                    {
                      erter.Add(Environment.MachineName + @"\" + instanceName);

                        Console.WriteLine(Environment.MachineName + @"\" + instanceName);
                    }
                }
            }

            return erter.ToArray();
        }



        private static async void Do()
        {
            Console.WriteLine("Do start");
            await CountAsync();
            
            Console.WriteLine("Do end");
        }

        private static async Task CountAsync()
        {
            Task t = new Task(() =>
            {
                Console.WriteLine("Count start");
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine(i);
                }
                Console.WriteLine("Count end");
            });
            await t;
        }
    }
}
