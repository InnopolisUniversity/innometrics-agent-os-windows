using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Do(); Thread.Sleep(30000);
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
