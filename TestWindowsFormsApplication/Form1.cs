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
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
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
using CommonModels;
using Transmission;

namespace TestWindowsFormsApplication
{
    public partial class Form1 : Form
    {
        private bool started;
        private readonly int _stateScanIntervalSec;

        private Collector collector;
        private Writer writer;
        private MetricsProcessor processor;
        private Sender sender;

        private event Action<SynchronizationContext, LoginForm> LoginFormEvaluated;

        public Form1()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string connectionString = config.ConnectionStrings.ConnectionStrings["DefaultConnection"].ConnectionString;
            int dataSavingIntervalSec = Convert.ToInt32(config.AppSettings.Settings["DataSavingIntervalSec"].Value);
            _stateScanIntervalSec = Convert.ToInt32(config.AppSettings.Settings["StateScanIntervalSec"].Value);
            int processRegistriesIntervalSec = Convert.ToInt32(config.AppSettings.Settings["ProcessRegistriesIntervalSec"].Value);
            int processRegistriesAtOneTime = Convert.ToInt32(config.AppSettings.Settings["ProcessRegistriesAtOneTime"].Value);
            string authorizationUri = config.AppSettings.Settings["AuthorizationUri"].Value;
            string sendDataUri = config.AppSettings.Settings["SendDataUri"].Value;

            InitializeComponent();
            LoginFormEvaluated += OnLoginFormEvaluated;
            started = false;
            writer = new Writer(connectionString, dataSavingIntervalSec);
            processor = new MetricsProcessor(
                connectionString: connectionString,
                processRegistriesIntervalSec: processRegistriesIntervalSec,
                processRegistriesAtOneTime: processRegistriesAtOneTime,
                includeNullTitles: false
            );
            sender = new Sender(authorizationUri, sendDataUri);
        }

        #region helper methods

        private void DisableFilterOnStart()
        {
            groupBoxFiltering.Enabled = false;

            List<string> filter = new List<string>();
            foreach (var item in listBoxFilter.Items)
            {
                filter.Add(item as string);
            }
            processor.SetNameFilter(filter);
        }

        private bool AtLeastOneTrackingEventChosen()
        {
            return chbEnableForegroundWindowChangeTracking.Checked ||
                   chbEnableLeftClickTracking.Checked ||
                   chbEnableStateScanning.Checked;
        }

        private void EnableTransmissionButton(SynchronizationContext sync)
        {
            SendOrPostCallback c = (state) => { btnTransmit.Enabled = true; };
            sync.Post(c, null);
        }

        private void LoginWithForm(SynchronizationContext sync)
        {
            LoginForm loginForm = new LoginForm();
            SendOrPostCallback c = (state) =>
            {
                EventHandler handler = (o, args) =>
                {
                    LoginFormEvaluated?.Invoke(sync, loginForm);
                };
                loginForm.SetLoginClickAction(handler);
                loginForm.Show();
            };
            sync.Post(c, null);
        }
        
        private void OnLoginFormEvaluated(SynchronizationContext sync, LoginForm loginForm)
        {
            string login = loginForm.GetLogin();
            string password = loginForm.GetPassword();
            SendOrPostCallback c = (state) =>
            {
                loginForm.Close();
            };
            sync.Post(c, null);
            btnTransmit_Click(this, new LoginPasswordEventArgs() {Login = login, Password = password});
        }

        #endregion

        #region button handlers

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!started)
            {
                if (!AtLeastOneTrackingEventChosen())
                {
                    MessageBox.Show("Choose at least one event to track");
                    return;
                }

                DisableFilterOnStart();

                collector = new Collector(
                    writer: writer,
                    stateScanIntervalSec: _stateScanIntervalSec,
                    enableForegroundWindowChangeTracking: chbEnableForegroundWindowChangeTracking.Checked,
                    enableLeftClickTracking: chbEnableLeftClickTracking.Checked,
                    enableStateScanning: chbEnableStateScanning.Checked
                );

                collector.Start();
                writer.Start();
                processor.Start();

                groupBoxCollectionSettings.Enabled = false;
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                started = true;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (started)
            {
                writer.Stop();
                // TODO manual process before stop
                processor.Stop();
                bool success = collector.Stop();

                if (success)
                {
                    groupBoxFiltering.Enabled = true;
                    groupBoxCollectionSettings.Enabled = true;
                    btnStart.Enabled = true;
                    btnStop.Enabled = false;
                    started = false;
                }
            }
        }

        private void btnAddFilter_Click(object sender, EventArgs e)
        {
            if (txbAddFilter.Text != string.Empty && !listBoxFilter.Items.Contains(txbAddFilter.Text))
            {
                listBoxFilter.Items.Add(txbAddFilter.Text);
                txbAddFilter.Clear();
            }
        }

        private void btnTransmit_Click(object sender, EventArgs e)
        {
            var sync = SynchronizationContext.Current;
            btnTransmit.Enabled = false;

            if (!this.sender.Authorized && e.GetType() != typeof(LoginPasswordEventArgs))
            {
                Task.Factory.StartNew(() =>
                {
                    LoginWithForm(sync);
                });
                return;
            }

            Task.Factory.StartNew(() =>
            {
                if (!processor.AnyNonTransmittedJson())
                {
                    MessageBox.Show("Nothing to transmit");
                    EnableTransmissionButton(sync);
                    return;
                }

                if (!this.sender.Authorized)
                {
                    string login = (e as LoginPasswordEventArgs).Login;
                    string password = (e as LoginPasswordEventArgs).Password;

                    HttpStatusCode authorizationStatusCode;
                    bool success = this.sender.Authorize(login, password, out authorizationStatusCode);
                    if (!success)
                    {
                        int authCode = (int)authorizationStatusCode;
                        MessageBox.Show($"Authorization failed with code {authCode}: {authorizationStatusCode}");
                        EnableTransmissionButton(sync);
                        return;
                    }
                }

                ActivitiesRegistry jsonItem = processor.GetJsonItem();

                HttpStatusCode sendStatusCode;
                var result = this.sender.SendActivities(jsonItem.Json, out sendStatusCode); // TODO problem
                int code = (int)sendStatusCode;
                MessageBox.Show($"{code}: {sendStatusCode}");
                EnableTransmissionButton(sync);
            });
        }

        #endregion

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

        private void listBoxFilter_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            listBoxFilter.Items.Remove(listBoxFilter.SelectedItem);
        }

        private class LoginPasswordEventArgs : EventArgs
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }
    }
}
