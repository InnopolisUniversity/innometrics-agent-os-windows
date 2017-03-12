using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonModels;
using CommonModels.Helpers;
using MetricsProcessing;
using Transmission;

namespace MetricsSenderApplication
{
    public partial class Form1 : Form
    {
        private StraightMetricsProcessor processor;
        private Sender sender;
        private ActivitiesList activitiesTempStorage;

        private event Action<SynchronizationContext, LoginForm> LoginFormSubmitted;

        public Form1()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string connectionString = config.ConnectionStrings.ConnectionStrings["DefaultConnection"].ConnectionString;
            string authorizationUri = config.AppSettings.Settings["AuthorizationUri"].Value;
            string sendDataUri = config.AppSettings.Settings["SendDataUri"].Value;

            InitializeComponent();
            LoginFormSubmitted += OnLoginFormSubmitted;
            processor = new StraightMetricsProcessor(connectionString);
            sender = new Sender(authorizationUri, sendDataUri);
        }

        #region helper methods

        private DataGridViewRow ToDataGridViewRow(Activity activity)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.Cells.Add(new DataGridViewTextBoxCell() {Value = activity.Name});
            foreach (var measurement in activity.Measurements)
            {
                row.Cells.Add(new DataGridViewTextBoxCell() {Value = measurement.Value});
            }
            return row;
        }

        private List<string> GetFilter()
        {
            List<string> filter = new List<string>();
            foreach (var item in listBoxFilteringTitle.Items)
            {
                filter.Add(item as string);
            }
            return filter;
        }

        private void DisableFilterBox()
        {
            groupBoxFilteringTitle.Enabled = false;
        }

        private void EnableFilterBox()
        {
            groupBoxFilteringTitle.Enabled = true;
        }

        private void EnableFilterBoxFromAnotherTask(SynchronizationContext sync)
        {
            SendOrPostCallback c = (state) => { EnableFilterBox(); };
            sync.Post(c, null);
        }

        private void DisableButtons()
        {
            buttonRefresh.Enabled = false;
            buttonTransmit.Enabled = false;
        }

        private void EnableButtons()
        {
            buttonRefresh.Enabled = true;
            buttonTransmit.Enabled = true;
        }

        private void EnableButtonsFromAnotherTask(SynchronizationContext sync)
        {
            SendOrPostCallback c = (state) => { EnableButtons(); };
            sync.Post(c, null);
        }

        private void LoginWithForm(SynchronizationContext sync)
        {
            LoginForm loginForm = new LoginForm();
            SendOrPostCallback c = (state) =>
            {
                EventHandler handler = (o, args) => { LoginFormSubmitted?.Invoke(sync, loginForm); };
                loginForm.SetLoginClickAction(handler);
                CancelEventHandler handler2 = (o, args) => { EnableButtonsFromAnotherTask(sync); EnableFilterBoxFromAnotherTask(sync); };
                loginForm.SetCloseAction(handler2);
                loginForm.Show();
            };
            sync.Post(c, null);
        }

        private void OnLoginFormSubmitted(SynchronizationContext sync, LoginForm loginForm)
        {
            string login = loginForm.GetLogin();
            string password = loginForm.GetPassword();
            SendOrPostCallback c = (state) =>
            {
                loginForm.Close();
            };
            sync.Post(c, null);
            buttonTransmit_Click(this, new LoginPasswordEventArgs() { Login = login, Password = password });
        }

        #endregion

        #region button handlers

        private void buttonAddFilterTitle_Click(object sender, EventArgs e)
        {
            if (textBoxFilteringTitle.Text != string.Empty && !listBoxFilteringTitle.Items.Contains(textBoxFilteringTitle.Text))
            {
                listBoxFilteringTitle.Items.Add(textBoxFilteringTitle.Text);
                textBoxFilteringTitle.Clear();
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            dataGridView.Rows.Clear();
            DisableFilterBox();
            DisableButtons();

            var nameFilter = GetFilter();
            var from = dateTimePickerFrom.Value;
            var until = dateTimePickerUntil.Value;
            var task = Task<ActivitiesList>.Factory.StartNew(() => processor.Process(nameFilter, false, from, until));
            activitiesTempStorage = task.Result;
            if (activitiesTempStorage != null)
            {
                foreach (var activity in activitiesTempStorage)
                {
                    DataGridViewRow row = ToDataGridViewRow(activity);
                    dataGridView.Rows.Add(row);
                }
            }
            else
            {
                MessageBox.Show("There's no activities");
            }

            EnableFilterBox();
            EnableButtons();
        }

        private void buttonTransmit_Click(object sender, EventArgs e)
        {
            if (activitiesTempStorage == null)
            {
                MessageBox.Show("There's nothing to transmit");
                return;
            }

            DisableFilterBox();
            DisableButtons();
            var sync = SynchronizationContext.Current;
            
            if (!this.sender.Authorized && e.GetType() != typeof(LoginPasswordEventArgs))
            {
                Task.Factory.StartNew(() =>
                {
                    LoginWithForm(sync);
                });
                return;
            }

            string login = (e as LoginPasswordEventArgs).Login;
            string password = (e as LoginPasswordEventArgs).Password;
            if (login == string.Empty || password == string.Empty)
            {
                EnableButtons();
                EnableFilterBox();
                return;
            }

            Task.Factory.StartNew(() =>
            {
                if (!this.sender.Authorized) // after first 'if' data was obtained as EventArgs
                {
                    HttpStatusCode authorizationStatusCode;
                    bool success = this.sender.Authorize(login, password, out authorizationStatusCode);
                    if (!success)
                    {
                        int authCode = (int)authorizationStatusCode;
                        MessageBox.Show($"Authorization failed with code {authCode}: {authorizationStatusCode}");
                        EnableButtonsFromAnotherTask(sync);
                        return;
                    }
                }

                var json = JsonMaker.Serialize(activitiesTempStorage);

                HttpStatusCode sendStatusCode;
                var result = this.sender.SendActivities(json, out sendStatusCode); // TODO problem
                int code = (int)sendStatusCode;
                MessageBox.Show($"{code}: {sendStatusCode}");

                if (sendStatusCode == HttpStatusCode.OK || sendStatusCode == HttpStatusCode.Created)
                    processor.DeleteRegistriesFromDb(activitiesTempStorage.RegistriesIds);

                EnableButtonsFromAnotherTask(sync);
            });
        }

        #endregion

        // Helper inner class
        private class LoginPasswordEventArgs : EventArgs
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }        
    }
}
