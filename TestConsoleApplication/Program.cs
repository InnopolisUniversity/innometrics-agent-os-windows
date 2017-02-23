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

namespace TestConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Activity a = new Activity { Name = "Activity 1" };
            a.Measurements.Add(new Measurement()
            {
                Name = "Time",
                Type = typeof(DateTime).Name,
                Value = DateTime.Now
            });
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
                Value = 147
            });
            b.Measurements.Add(new Measurement()
            {
                Name = "Quality",
                Type = typeof(string).Name,
                Value = "good"
            });
            Report report = new Report() {Activities = new List<Activity>() {a, b}};





            //string json = JsonMaker.Serialize(report);
            //string statusCode;
            ////var s = Sender.Send("http://httpbin.org/post", json, "application/json", out statusCode);
            //var s = Sender.Send("https://aqueous-escarpment-80312.herokuapp.com/activities/", json, "application/json", out statusCode);

            //Console.WriteLine(s);
            //Console.WriteLine();
            //Console.WriteLine(statusCode);


            List<int> ri = new List<int>() {0, 1, 2, 3, 4, 5, 6, 7};

            var ui = Foo(ri);

            int p = 100;
            Foo2(p);

            List<string> filter = new List<string>()
            {
                "Tele"
            };

            RegistriesProcessor rp = new RegistriesProcessor(@"Data Source=DESKTOP-7CAUMID\SQLEXPRESS;Initial Catalog=WindowsMetrics;Integrated Security=True");
            var act = rp.Process(100, filter, includeNullTitles: true);
            var act2 = rp.Process(50, filter, includeNullTitles: true);
            var xx = JsonMaker.Serialize(act);
            WindowsMetrics.Helpers.FileWriteHelper.Write(xx, @"D:\aaa.txt");


            //Console.ReadKey();
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
