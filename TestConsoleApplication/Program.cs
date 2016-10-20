using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
//using System.Threading;
using System.Threading.Tasks;
using WindowsMetrics;
using WindowsMetrics.Helpers;
using Timer = System.Timers.Timer;

namespace TestConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            // Getting active window text every second
            //for (;;)
            //{
            //    var x = WinAPI.GetTextOfForegroundWindow();
            //    Console.WriteLine(x);
            //    Thread.Sleep(1000);
            //}

            // Getting username
            //Console.WriteLine(WinAPI.GetSystemUserName());
            //Console.ReadKey();

            // Getting time
            //var t = WinAPI.GetLocalDateTime();
            //var y = WinAPI.GetGMTDateTime();

            //Timer timer = new Timer(1000);
            //timer.Elapsed += (source, e) => { Console.WriteLine("x"); };
            //timer.AutoReset = true;
            //timer.Enabled = true;

            //Console.WriteLine("\nPress the Enter key to exit the application...\n");
            //Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);



            //Console.ReadLine();
            //timer.Stop();
            //timer.Dispose();

            //Console.WriteLine("Terminating the application...");

            //timer.Start();
            //Console.Write("Press any key to exit... ");
            //Console.ReadKey();

            Task task = new Task(() =>
                {
                    Timer timer = new Timer(1000);
                    timer.Elapsed += (source, e) => { Console.WriteLine("x"); };
                    timer.AutoReset = false;
                    timer.Enabled = true;
                    
                    Console.WriteLine("\nPress the Enter key to exit the application...\n");
                    Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
                    Console.ReadKey();
                });
            task.Start();

            Thread.CurrentThread.Join();
            //Thread.Sleep(5000);

        }

        private static Task HandleTimer()
        {
            return new Task(() => Console.WriteLine("dsfsdf"));
        }
    }
}
