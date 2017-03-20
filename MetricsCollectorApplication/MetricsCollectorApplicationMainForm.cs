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
        private bool exceptionOnDatabaseCheckOccured;
        private bool started;
        private readonly int _stateScanIntervalSec;

        private Collector collector;
        private Writer writer;

        public MetricsCollectorApplicationMainForm()
        {
            InitializeComponent();
            this.Cursor = System.Windows.Forms.Cursors.AppStarting;

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string connectionString = config.ConnectionStrings.ConnectionStrings["DefaultConnection"].ConnectionString;
            int dataSavingIntervalSec = Convert.ToInt32(config.AppSettings.Settings["DataSavingIntervalSec"].Value);
            _stateScanIntervalSec = Convert.ToInt32(config.AppSettings.Settings["StateScanIntervalSec"].Value);
            
            exceptionOnDatabaseCheckOccured = false;
            started = false;
            writer = new Writer(connectionString, dataSavingIntervalSec);
            try
            {
                writer.CreateDatabaseIfNotExists();
            }
            catch (Exception e)
            {
                exceptionOnDatabaseCheckOccured = true;
                MessageBox.Show($"An error occured while creating database.\n***\n{e.Message}");
            }
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
            buttonSettings.Enabled = false;
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
                buttonSettings.Enabled = true;
                started = false;
            }
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            SettingsForm form = new SettingsForm();
            form.Show();
        }

        private void MetricsCollectorApplicationMainForm_Resize(object sender, EventArgs e)
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

        private void MetricsCollectorApplicationMainForm_Load(object sender, EventArgs e)
        {
            Cursor = System.Windows.Forms.Cursors.Default;
            if (exceptionOnDatabaseCheckOccured)
                Application.Exit();
        }
    }
}
