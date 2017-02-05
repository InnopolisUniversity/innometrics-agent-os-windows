using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsMetrics;
using WindowsMetrics.Helpers;
using CommonModels;

namespace TestWindowsFormsApplication
{
    public partial class Form1 : Form
    {
        private MetricsDataContext context;
        private Collector collector;
        private Writer writer;

        public Form1()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string connectionString = config.ConnectionStrings.ConnectionStrings["DefaultConnection"].ConnectionString;
            context = new MetricsDataContext(connectionString);
            if(!context.DatabaseExists())
                context.CreateDatabase();

            InitializeComponent();
            writer = new Writer(Directory.GetCurrentDirectory());
            collector = new Collector(
                writer,
                SynchronizationContext.Current,
                s => richTextBox1.AppendText(s),
                s => richTextBox1.AppendText(s),
                s => richTextBox1.AppendText(s)
                );
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            writer.Stop();
            bool success = collector.Stop();
            if (success)
            {
                richTextBox1.BackColor = Color.LightSteelBlue;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            richTextBox1.BackColor = Color.White;

            collector.Start();
            writer.Start();
        }
    }
}
