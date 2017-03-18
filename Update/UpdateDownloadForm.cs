using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Update
{
    internal partial class UpdateDownloadForm : Form
    {
        private WebClient webClient;
        private BackgroundWorker bgWorker;
        private string tempFilePath;
        private string md5;

        internal string TempFilePath => tempFilePath;

        public UpdateDownloadForm(Uri location, string md5, Icon programIcon)
        {
            InitializeComponent();

            if (programIcon != null)
                this.Icon = programIcon;

            tempFilePath = Path.GetTempFileName();
            this.md5 = md5;

            webClient = new WebClient();
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;

            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += BgWorker_DoWork;
            bgWorker.RunWorkerCompleted += BgWorker_RunWorkerCompleted;

            try
            {
                webClient.DownloadFileAsync(location, tempFilePath);
            }
            catch (Exception e)
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            }
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
            this.labelProgress.Text = $"Downloaded {e.BytesReceived} of {e.TotalBytesToReceive}";
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            }
            else if (e.Cancelled)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
            else
            {
                labelProgress.Text = "Verifying download...";
                progressBar.Style = ProgressBarStyle.Marquee;

                bgWorker.RunWorkerAsync(new string[] { tempFilePath, md5 });
            }
        }

        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string file = string.Empty, updateMd5 = string.Empty;
            try
            {
                file = (e.Argument as string[])[0];
                updateMd5 = (e.Argument as string[])[1];
            }
            catch (Exception ex)
            {
                e.Result = DialogResult.No;
                this.Close();
            }

            string hash = Hasher.HashFile(file, HashType.MD5);
            e.Result = hash == updateMd5 ? DialogResult.OK : DialogResult.No;
        }

        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.DialogResult = (DialogResult)e.Result;
            this.Close();
        }

        private void UpdateDownloadForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (webClient.IsBusy)
            {
                webClient.CancelAsync();
                this.DialogResult = DialogResult.Abort;
            }

            if (bgWorker.IsBusy)
            {
                bgWorker.CancelAsync();
                this.DialogResult = DialogResult.Abort;
            }
        }
    }
}
