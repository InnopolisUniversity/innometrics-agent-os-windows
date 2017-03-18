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
    internal partial class UpdateInfoForm : Form
    {
        internal UpdateInfoForm(IUpdateable applicationInfo, UpdateXml updateInfo)
        {
            InitializeComponent();

            if (applicationInfo.ApllicationIcon != null)
            {
                this.Icon = applicationInfo.ApllicationIcon;
                this.Text = applicationInfo.ApplicationName + " - Update Info";
                this.labelVersions.Text = 
                    $"Current version {applicationInfo.ApplicationAssembly.GetName().Version.ToString()}\n" +
                    $"Update version {updateInfo.Version.ToString()}";
                this.richTextBoxDescription.Text = updateInfo.Description;
            }
        }
    }
}
