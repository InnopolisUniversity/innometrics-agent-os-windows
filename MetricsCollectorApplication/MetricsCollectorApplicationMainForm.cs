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
using CommonModels.Helpers;

namespace MetricsCollectorApplication
{
    public partial class MetricsCollectorApplicationMainForm : Form
    {
        private readonly string ConfigFileName = "MetricsCollectorApplication.exe.config";

        private bool exceptionOnDatabaseCheckOccured;
        private bool started;
        private int _stateScanIntervalSec;

        private Collector collector;
        private Writer writer;

        public MetricsCollectorApplicationMainForm()
        {
            InitializeComponent();
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
            SettingsForm form = new SettingsForm(this);
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
            var appSettings = ConfigHelper.GetAppSettings(ConfigFileName);
            string connectionString = ConfigHelper.GetConnectionString(ConfigFileName, "DefaultConnection");
            _stateScanIntervalSec = Convert.ToInt32(appSettings["StateScanIntervalSec"]);

            started = false;
            writer = new Writer(connectionString, Convert.ToInt32(appSettings["DataSavingIntervalSec"]));
            try
            {
                writer.CreateDatabaseIfNotExists();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured while creating database.\n***\n{ex.Message}");
                Application.Exit();
            }
        }
    }
}
