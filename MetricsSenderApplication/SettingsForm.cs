using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonModels.Helpers;
using Update;

namespace MetricsSenderApplication
{
    public partial class SettingsForm : Form
    {
        private readonly MetricsSenderApplicationMainForm mainForm;

        public SettingsForm(MetricsSenderApplicationMainForm form)
        {
            InitializeComponent();
            mainForm = form;
            string divisor = "----------";
            string mainDivisor = "**********";

            Assembly[] assemblies = TryGetAssemblies(form.Assemblies);
            StringBuilder text = new StringBuilder();
            text.Append(
                $"Metrics Collection System version (version of MetricsSenderApplication): {form.ApplicationAssembly.GetName().Version}\n\n{mainDivisor}\n\n");
            foreach (var assembly in assemblies)
                text.Append($"Assembly: {assembly.GetName()}\n{divisor}\n");
            richTextBox.Text = text.ToString();

            var appSettings = ConfigHelper.GetAppSettings("MetricsSenderApplication.exe.config");
            textBoxAuthorizationUri.Text = appSettings["AuthorizationUri"];
            textBoxSendDataUri.Text = appSettings["SendDataUri"];
            textBoxUpdateXmlUri.Text = appSettings["UpdateXmlUri"];
            textBoxAssemblies.Text = appSettings["Assemblies"];
        }

        private void buttonCheckUpdate_Click(object sender, EventArgs e)
        {
            if (mainForm.UpdateXmlUri == null)
            {
                MessageBox.Show("Update is impossible, the update XML hasn't been obtained from config file.");
            }
            else
            {
                mainForm.Updater.DoUpdate();
                Close();
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            ConfigHelper.UpdateConfig("MetricsSenderApplication.exe.config", "AuthorizationUri", textBoxAuthorizationUri.Text);
            ConfigHelper.UpdateConfig("MetricsSenderApplication.exe.config", "SendDataUri", textBoxSendDataUri.Text);
            ConfigHelper.UpdateConfig("MetricsSenderApplication.exe.config", "UpdateXmlUri", textBoxUpdateXmlUri.Text);
            ConfigHelper.UpdateConfig("MetricsSenderApplication.exe.config", "Assemblies", textBoxAssemblies.Text);
            Application.Exit();
        }

        private Assembly[] TryGetAssemblies(string[] paths)
        {
            List<Assembly> assemblies = new List<Assembly>();
            foreach (string path in paths)
            {
                try
                {
                    assemblies.Add(Assembly.LoadFrom(path));
                }
                catch (Exception) { }
            }
            return assemblies.ToArray();
        }
    }
}
