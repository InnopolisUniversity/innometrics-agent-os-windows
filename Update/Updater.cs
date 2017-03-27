using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Update
{
    public class Updater
    {
        private IUpdateable applicationInfo;
        private BackgroundWorker bgWorker;

        public Updater(IUpdateable applicationInfo)
        {
            this.applicationInfo = applicationInfo;

            this.bgWorker = new BackgroundWorker();
            bgWorker.DoWork += BgWorker_DoWork;
            bgWorker.RunWorkerCompleted += BgWorker_RunWorkerCompleted;
        }

        public void DoUpdate()
        {
            if (!this.bgWorker.IsBusy)
                this.bgWorker.RunWorkerAsync(this.applicationInfo);
        }

        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            IUpdateable application = (IUpdateable)e.Argument;

            bool existsOnServer;
            try
            {
                existsOnServer = UpdateXml.ExistsOnServer(application.UpdateXmlUri);
            }
            catch (Exception ex)
            {
                existsOnServer = false;
            }

            if (!existsOnServer)
                e.Cancel = true;
            else
                e.Result = UpdateXml.Parse(application.UpdateXmlUri, application.ApplicationID);
        }

        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("No updates available.");
            }

            UpdateXml[] updateXmls = e.Result as UpdateXml[];
            UpdateXml senderApplicationXml =
                updateXmls?.SingleOrDefault(xml => xml.FileName == "MetricsSenderApplication.exe"); // Version of sender is the version of the system
            if (updateXmls != null && senderApplicationXml != null &&
                senderApplicationXml.IsNewerThan(applicationInfo.ApplicationAssembly.GetName().Version))
            {
                if (new UpdateAcceptForm(applicationInfo, updateXmls[0]).ShowDialog(applicationInfo.Context) == DialogResult.Yes)
                    this.DownloadUpdate(updateXmls);
            }
            else
            {
                MessageBox.Show("No updates available.");
            }
        }

        private void DownloadUpdate(UpdateXml[] update)
        {
            UpdateArguments[] arguments = new UpdateArguments[update.Length];
            int i = 0;
            foreach (UpdateXml updateXml in update)
            {
                UpdateDownloadForm form = new UpdateDownloadForm(updateXml.Uri, updateXml.Md5, this.applicationInfo.ApllicationIcon);
                DialogResult result = form.ShowDialog(this.applicationInfo.Context);

                if (result == DialogResult.OK)
                {
                    string currentPath = this.applicationInfo.ApplicationAssembly.Location;
                    string newPath = Path.GetDirectoryName(currentPath) + "\\" + updateXml.FileName;

                    arguments[i++] = new UpdateArguments(form.TempFilePath, currentPath, newPath, updateXml.LaunchArgs);
                }
                else if (result == DialogResult.Abort)
                {
                    MessageBox.Show("The update download was cancelled\nThe program hasn't been modified",
                        "Update download cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("There was a problem downloading update",
                        "Update download error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            UpdateApplication(arguments);
            Application.Exit();
        }

        private void UpdateApplication(UpdateArguments[] arguments)
        {
            StringBuilder argument = new StringBuilder();
            foreach (var update in arguments)
            {
                if (argument.Length != 0)
                    argument.Append(" & ");

                string updateArg =
                    $"/C Choice /C Y /N /D Y /T 4 & Del /F /Q \"{update.CurrentPath}\" & Choice /C Y /N /D Y /T 2 & Move /Y \"{update.TempFilePath}\" \"{update.NewPath}\"";
                argument.Append(updateArg);
            }

            ProcessStartInfo info = new ProcessStartInfo();
            info.Arguments = argument.ToString();
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.CreateNoWindow = true;
            info.FileName = "cmd.exe";
            Process.Start(info);
        }




        // Private helper class
        private class UpdateArguments
        {
            public string TempFilePath { get; set; }
            public string CurrentPath { get; set; }
            public string NewPath { get; set; }
            public string LaunchArgs { get; set; }

            public UpdateArguments(string tempFilePath, string currentPath, string newPath, string launchArgs)
            {
                TempFilePath = tempFilePath;
                CurrentPath = currentPath;
                NewPath = newPath;
                LaunchArgs = launchArgs;
            }
        }
    }
}
