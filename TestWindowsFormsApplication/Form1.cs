using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsMetrics;
using CommonModels.Helpers;
using MetricsProcessing;
using Microsoft.Win32;
using Registry = CommonModels.Registry;
using System.ServiceProcess;

namespace TestWindowsFormsApplication
{
    public partial class Form1 : Form
    {
        private Collector collector;
        private Writer writer;
        private MetricsProcessor processor;

        public Form1()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string connectionString = config.ConnectionStrings.ConnectionStrings["DefaultConnection"].ConnectionString;
            int dataSavingIntervalSec = Convert.ToInt32(config.AppSettings.Settings["DataSavingIntervalSec"].Value);
            int stateScanIntervalSec = Convert.ToInt32(config.AppSettings.Settings["StateScanIntervalSec"].Value);
            int processRegistriesIntervalSec = Convert.ToInt32(config.AppSettings.Settings["ProcessRegistriesIntervalSec"].Value);
            int processRegistriesAtOneTime = Convert.ToInt32(config.AppSettings.Settings["ProcessRegistriesAtOneTime"].Value);

            InitializeComponent();

            writer = new Writer(connectionString, dataSavingIntervalSec);
            collector = new Collector(
                writer,
                stateScanIntervalSec,
                SynchronizationContext.Current,
                s => richTextBox1.AppendText(s.ToString() + "\n***\n"),
                s => richTextBox1.AppendText(s.ToString() + "\n***\n"),
                s => richTextBox1.AppendText(s.ToString() + "\n***\n")
            );
            processor = new MetricsProcessor(
                connectionString: connectionString,
                processRegistriesIntervalSec: processRegistriesIntervalSec,
                processRegistriesAtOneTime: processRegistriesAtOneTime,
                includeNullTitles: false
            );
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            writer.Stop();
            // TODO manual process before stop
            processor.Stop();
            bool success = collector.Stop();

            if (success)
            {
                richTextBox1.BackColor = Color.LightSteelBlue;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            richTextBox1.BackColor = Color.White;

            collector.Start(
                enableForegroundWindowChangeTracking: true,
                enableLeftClickTracking: true,
                enableStateScanning: true
                );
            writer.Start();
            processor.Start();
        }
    }
}
