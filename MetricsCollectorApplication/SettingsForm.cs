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
using CommonModels.Helpers;

namespace MetricsCollectorApplication
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            textBoxDataSavingIntervalSec.Text = config.AppSettings.Settings["DataSavingIntervalSec"].Value;
            textBoxStateScanIntervalSec.Text = config.AppSettings.Settings["StateScanIntervalSec"].Value;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            ConfigHelper.UpdateConfig("MetricsCollectorApplication.exe.config", "DataSavingIntervalSec", textBoxDataSavingIntervalSec.Text);
            ConfigHelper.UpdateConfig("MetricsCollectorApplication.exe.config", "StateScanIntervalSec", textBoxStateScanIntervalSec.Text);
            Application.Exit();
        }
    }
}
