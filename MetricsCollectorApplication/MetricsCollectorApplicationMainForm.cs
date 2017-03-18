using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsMetrics;

namespace MetricsCollectorApplication
{
    public partial class MetricsCollectorApplicationMainForm : Form
    {
        private bool started;
        private readonly int _stateScanIntervalSec;

        private Collector collector;
        private Writer writer;

        public MetricsCollectorApplicationMainForm()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string connectionString = config.ConnectionStrings.ConnectionStrings["DefaultConnection"].ConnectionString;
            int dataSavingIntervalSec = Convert.ToInt32(config.AppSettings.Settings["DataSavingIntervalSec"].Value);
            _stateScanIntervalSec = Convert.ToInt32(config.AppSettings.Settings["StateScanIntervalSec"].Value);

            InitializeComponent();

            started = false;
            writer = new Writer(connectionString, dataSavingIntervalSec);
            writer.CreateDatabase(); // TODO exception
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (started)
                return;

            if (!AtLeastOneTrackingEventChosen())
            {
                MessageBox.Show("Choose at least one event to track");
                return;
            }

            collector = new Collector(
                writer: writer,
                stateScanIntervalSec: _stateScanIntervalSec,
                enableForegroundWindowChangeTracking: checkBoxForegroundWindowChangeTracking.Checked,
                enableLeftClickTracking: checkBoxMouseLeftClickTracking.Checked,
                enableStateScanning: checkBoxStateScanning.Checked
            );

            collector.Start();
            writer.Start();

            groupBoxSettings.Enabled = false;
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
            started = true;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (!started)
                return;

            writer.Stop();
            bool success = collector.Stop();
            if (success)
            {
                groupBoxSettings.Enabled = true;
                buttonStart.Enabled = true;
                buttonStop.Enabled = false;
                started = false;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                trayIcon.Visible = true;
                this.Hide();
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                trayIcon.Visible = false;
            }
        }

        private void trayIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            trayIcon.Visible = false;
        }

        private bool AtLeastOneTrackingEventChosen()
        {
            return checkBoxForegroundWindowChangeTracking.Checked ||
                   checkBoxMouseLeftClickTracking.Checked ||
                   checkBoxStateScanning.Checked;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
    }
}
