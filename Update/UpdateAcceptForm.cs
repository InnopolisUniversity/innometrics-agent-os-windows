using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Update
{
    internal partial class UpdateAcceptForm : Form
    {
        private IUpdateable applicationInfo;
        private UpdateXml updateInfo;
        private UpdateInfoForm updateInfoForm;

        internal UpdateAcceptForm(IUpdateable applicationInfo, UpdateXml updateInfo)
        {
            InitializeComponent();

            this.applicationInfo = applicationInfo;
            this.updateInfo = updateInfo;
            this.Text = this.applicationInfo.ApplicationName + " - Update available";
            if (this.applicationInfo.ApllicationIcon != null)
                this.Icon = applicationInfo.ApllicationIcon;
            this.labelNewVersion.Text = $"New version {this.updateInfo.Version.ToString()}";
        }
        
        private void buttonYes_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void buttonNo_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void buttonDetails_Click(object sender, EventArgs e)
        {
            if (this.updateInfoForm == null)
                this.updateInfoForm = new UpdateInfoForm(this.applicationInfo, this.updateInfo);

            this.updateInfoForm.ShowDialog(this);
        }
    }
}
