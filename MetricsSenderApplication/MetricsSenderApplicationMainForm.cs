using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonModels;
using CommonModels.Helpers;
using MetricsProcessing;
using Transmission;
using Update;

namespace MetricsSenderApplication
{
    public partial class MetricsSenderApplicationMainForm : Form, IUpdateable
    {
        private const string ApplicationId = "MetricsCollectionSystem";

        private StraightMetricsProcessor processor;
        private Sender sender;
        private ActivitiesList activitiesTempStorage;
        private Uri updateXmlUri;
        private Updater updater;
        private string[] assemblies;
        private int activitiesToSendAtOneTime;

        public string ApplicationName => ApplicationId;
        public string ApplicationID => ApplicationId;
        public Assembly ApplicationAssembly => Assembly.GetExecutingAssembly();
        public Icon ApllicationIcon => this.Icon;
        public Uri UpdateXmlUri => updateXmlUri;
        public Form Context => this;
        public Updater Updater => updater;
        public string[] Assemblies => assemblies;

        private event Action<SynchronizationContext, LoginForm> LoginFormSubmitted;

        public MetricsSenderApplicationMainForm()
        {
            InitializeComponent();

            string connectionString = ConfigHelper.GetConnectionString("MetricsSenderApplication.exe.config", "DefaultConnection");
            var appSettings = ConfigHelper.GetAppSettings("MetricsSenderApplication.exe.config");
            try
            {
                updateXmlUri = new Uri(appSettings["UpdateXmlUri"]);
            }
            catch (Exception)
            {
                updateXmlUri = null;
            }
            assemblies = appSettings["Assemblies"].Split(';');
            dateTimePickerFrom.Value = DateTime.Now - new TimeSpan(24, 0, 0);
            LoginFormSubmitted += OnLoginFormSubmitted;
            processor = new StraightMetricsProcessor(connectionString);
            sender = new Sender(appSettings["AuthorizationUri"], appSettings["SendDataUri"]);
            updater = new Updater(this);
            activitiesToSendAtOneTime = int.Parse(appSettings["ActivitiesToSendAtOneTime"]);
        }

        #region helper methods

        private static DataGridViewRow ToDataGridViewRow(Activity activity)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.Cells.Add(new DataGridViewTextBoxCell() {Value = activity.Name});
            foreach (var measurement in activity.Measurements)
            {
                row.Cells.Add(new DataGridViewTextBoxCell() {Value = measurement.Value});
            }
            return row;
        }

        #endregion

        #region filter
        
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

        #endregion

        #region buttons
        
        private void DisableButtons()
        {
            buttonRefresh.Enabled = false;
            buttonTransmit.Enabled = false;
            buttonSettings.Enabled = false;
            buttonDeleteProcessed.Enabled = false;
        }

        private void EnableButtons()
        {
            buttonRefresh.Enabled = true;
            buttonTransmit.Enabled = true;
            buttonSettings.Enabled = true;
            buttonDeleteProcessed.Enabled = true;
        }

        private void EnableButtonsFromAnotherTask(SynchronizationContext sync)
        {
            SendOrPostCallback c = (state) => { EnableButtons(); };
            sync.Post(c, null);
        }

        private void ClearDataFromAnotherTask(SynchronizationContext sync)
        {
            SendOrPostCallback c = (state) =>
            {
                dataGridView.Rows.Clear();
            };
            sync.Post(c, null);
        }

        #endregion

        #region login

        private void LoginWithForm(SynchronizationContext sync)
        {
            LoginForm loginForm = new LoginForm();
            SendOrPostCallback c = (state) =>
            {
                EventHandler handler = (o, args) => { LoginFormSubmitted?.Invoke(sync, loginForm); };
                loginForm.SetLoginClickAction(handler);
                CancelEventHandler handler2 = (o, args) =>
                {
                    if (!loginForm.LoginClicked)
                    {
                        EnableButtonsFromAnotherTask(sync);
                        EnableFilterBoxFromAnotherTask(sync);
                    }
                    loginForm.LoginClicked = false;
                };
                loginForm.SetCloseAction(handler2);
                loginForm.Show();
            };
            sync.Post(c, null);
        }

        private void OnLoginFormSubmitted(SynchronizationContext sync, LoginForm loginForm)
        {
            string login = loginForm.GetLogin();
            string password = loginForm.GetPassword();
            if (login == string.Empty)
            {
                MessageBox.Show("Login missing");
                return;
            }
            if (password == string.Empty)
            {
                MessageBox.Show("Password missing");
                return;
            }
            loginForm.LoginClicked = true;
            SendOrPostCallback c = (state) =>
            {
                loginForm.Close();
            };
            sync.Post(c, null);
            buttonTransmit_Click(this, new LoginPasswordEventArgs() {Login = login, Password = password});
        }

        #endregion

        #region progress form

        private SendingProgressForm ShowProgressForm(SynchronizationContext sync, int itemsCount, CancellationTokenSource tokenSource)
        {
            SendingProgressForm form = new SendingProgressForm(itemsCount);
            SendOrPostCallback c = (state) =>
            {
                EventHandler cancelHandler = (o, args) =>
                {
                    tokenSource.Cancel();
                };
                form.SetCancelClickAction(cancelHandler);
                form.Show();
            };
            sync.Post(c, null);
            return form;
        }

        private void IncrementProgress(SynchronizationContext sync, SendingProgressForm form)
        {
            SendOrPostCallback c = (state) =>
            {
                form.Increment();
            };
            sync.Post(c, null);
        }

        private void CompleteProgress(SynchronizationContext sync, SendingProgressForm form)
        {
            SendOrPostCallback c = (state) =>
            {
                form.To100();
                MessageBox.Show("Completed");
                form.Close();
            };
            sync.Post(c, null);
        }

        private void StopProgressOnCancel(SynchronizationContext sync, SendingProgressForm form)
        {
            SendOrPostCallback c = (state) =>
            {
                MessageBox.Show("Cancelled");
                form.Close();
            };
            sync.Post(c, null);
        }

        #endregion

        #region button handlers

        private void buttonAddFilterTitle_Click(object sender, EventArgs e)
        {
            if (textBoxFilteringTitle.Text != string.Empty &&
                !listBoxFilteringTitle.Items.Contains(textBoxFilteringTitle.Text))
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
            try
            {
                activitiesTempStorage = task.Result;
            }
            catch (Exception)
            {
                MessageBox.Show("An error occured while processing collected data.");
            }
            
            if (activitiesTempStorage != null)
            {
                for (int i = 0; i < activitiesTempStorage.Count; i++)
                {
                    DataGridViewRow row = ToDataGridViewRow(activitiesTempStorage[i]);
                    row.HeaderCell.Value = (i + 1).ToString();
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
            System.Runtime.GCSettings.LatencyMode = GCLatencyMode.LowLatency;

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

            string login = string.Empty, password = string.Empty;
            if (!this.sender.Authorized)
            {
                login = (e as LoginPasswordEventArgs).Login;
                password = (e as LoginPasswordEventArgs).Password;
                if (login == string.Empty || password == string.Empty)
                {
                    EnableButtons();
                    EnableFilterBox();
                    return;
                }
            }
        


            Task authorizationTask = new Task(() =>
            {
                if (!this.sender.Authorized) // after first 'if' data was obtained as EventArgs
                {
                    HttpStatusCode authorizationStatusCode;
                    bool success;
                    try
                    {
                        success = this.sender.Authorize(login, password, out authorizationStatusCode);
                    }
                    catch (WebException ex)
                    {
                        MessageBox.Show($"An error occured while authorization\n{ex.Message}");
                        return;
                    }

                    if (!success)
                    {
                        int authCode = (int)authorizationStatusCode;
                        MessageBox.Show($"Authorization failed. Code {authCode}: {authorizationStatusCode}");
                        EnableButtonsFromAnotherTask(sync);
                        EnableFilterBoxFromAnotherTask(sync);
                    }
                }
            });
            authorizationTask.Start();
            authorizationTask.Wait();

            if (!this.sender.Authorized)
                return;

            Task<List<Report>> splittingActivitiesListTask = new Task<List<Report>>(() =>
            {
                List<Report> r = new List<Report>();
                while (activitiesTempStorage.Count > 0)
                {
                    var a = activitiesTempStorage.Take(activitiesToSendAtOneTime).ToList();
                    r.Add(new Report() { Activities = a });
                    activitiesTempStorage.RemoveRange(0, a.Count);
                }
                return r;
            });
            splittingActivitiesListTask.Start();
            List<Report> reports = splittingActivitiesListTask.Result;



            var tokenSource = new CancellationTokenSource();
            var cancellation = tokenSource.Token;
            SendingProgressForm sendingProgressForm = ShowProgressForm(sync, reports.Count, tokenSource);
            Task sendingTask = new Task((token) =>
            {
                CancellationToken cancellationToken = (CancellationToken) token;

                while (reports.Any())
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var report = reports.First();

                    HttpStatusCode sendStatusCode;
                    string result;
                    try
                    {
                        result = this.sender.SendActivities(report, out sendStatusCode);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occured while sending activities\n{ex.Message}");
                        return;
                    }
                    int code = (int) sendStatusCode;

                    if (sendStatusCode == HttpStatusCode.Created)
                    {
                        reports.Remove(report);
                        IncrementProgress(sync, sendingProgressForm);
                    }
                }

                processor.MarkRegistriesAsProcessed(activitiesTempStorage.RegistriesIds);
            }, 
            cancellation, TaskCreationOptions.LongRunning);
            Task continuation = sendingTask.ContinueWith((obj) =>
            {
                if (cancellation.IsCancellationRequested)
                {
                    StopProgressOnCancel(sync, sendingProgressForm);
                }
                else
                {
                    CompleteProgress(sync, sendingProgressForm);
                }

                activitiesTempStorage = null;

                EnableFilterBoxFromAnotherTask(sync);
                EnableButtonsFromAnotherTask(sync);
                ClearDataFromAnotherTask(sync);
            });
            sendingTask.Start();
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm(this);
            settingsForm.Show();
        }

        private void buttonDeleteProcessed_Click(object sender, EventArgs e)
        {
            Task deletionTask = new Task(() =>
            {
                processor.DeleteProcessedRegistriesFromDb();
            });
            Task continuation = deletionTask.ContinueWith((obj) =>
            {
                MessageBox.Show("All the transmitted activities successfully removed from the storage");
            });

            deletionTask.Start();
        }

        #endregion

        private void ValidateTimeOrderOnValueChanged(object sender, EventArgs e)
        {
            if (dateTimePickerFrom.Value > dateTimePickerUntil.Value)
            {
                MessageBox.Show("'From' time cannot be after 'Until' time.");
                dateTimePickerFrom.Value = dateTimePickerUntil.Value;
            }
        }

        private void listBoxFilteringTitle_DoubleClick(object sender, EventArgs e)
        {
            listBoxFilteringTitle.Items.Remove(listBoxFilteringTitle.SelectedItem);
        }




        // Helper inner class
        private class LoginPasswordEventArgs : EventArgs
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }

    }
}
