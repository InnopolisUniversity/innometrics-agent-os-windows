﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsCollectorApplicationTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Tester tester = new Tester();

            tester.TestStartRecording();
        }
    }
}
