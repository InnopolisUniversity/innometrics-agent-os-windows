using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Update;

namespace MetricsSenderApplication
{
    public partial class DetailsForm : Form
    {
        private readonly MetricsSenderApplicationMainForm mainForm;

        public DetailsForm(MetricsSenderApplicationMainForm form)
        {
            InitializeComponent();

            mainForm = form;
            Assembly[] assemblies = TryGetAssemblies(form.Assemblies);

            string divisor = "----------";
            string mainDivisor = "**********";

            StringBuilder text = new StringBuilder();
            text.Append(
                $"Metrics Collection System version (version of MetricsSenderApplication): {form.ApplicationAssembly.GetName().Version}\n\n{mainDivisor}\n\n");

            foreach (var assembly in assemblies)
                text.Append($"Assembly: {assembly.GetName()}\n{divisor}\n");

            richTextBox.Text = text.ToString();
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
