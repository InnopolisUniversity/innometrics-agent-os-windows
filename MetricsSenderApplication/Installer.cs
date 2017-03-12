using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace MetricsSenderApplication
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        // from here
        // http://www.c-sharpcorner.com/UploadFile/27c648/create-a-custom-setup-for-change-app-onfig/

        public Installer()
        {
            InitializeComponent();
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }

    }
}


/*
    public override void Commit(IDictionary savedState)
    {
        base.Commit(savedState);

        try
        {
            AddConnectionStringToConfig();
        }
        catch (AddingOfConnectionStringException e)
        {
            MessageBox.Show(e.Message);
        }
        catch (NoMSSQLServerInstancesFoundException e)
        {
            var res = MessageBox.Show(e.Message +
                                        " Press OK to continue installation, you'll have to install MSSQL Server" +
                                        " and add a connection string to .config file manually. " +
                                        "Otherwise intallation will be aborted.", "Warning", MessageBoxButtons.OKCancel);
            if (res == DialogResult.Cancel)
            {
                base.Rollback(savedState);
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
            base.Rollback(savedState);
        }


        try
        {
            AddConnectionStringToConfig();
        }
        catch (AddingOfConnectionStringException e)
        {
            MessageBox.Show(e.Message);
        }
        catch (FailToObtainInstanceNameException)
        {
            MessageBox.Show("Server name was obtained.");
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
            base.Rollback(savedState);
        }
    }







    /// <returns>NULL if not found</returns>
    private string GetMsSqlServiceName()
    {
        // possible mssql server names
        string servicename = "MSSQL";
        string servicename2 = "SQLAgent";
        string servicename3 = "SQL Server";
        string servicename4 = "msftesql";

        string mssqlservicename = null;
        ServiceController[] services = ServiceController.GetServices();
        foreach (ServiceController service in services)
        {
            if (service == null)
                continue;
            if (service.ServiceName.Contains(servicename) ||
                service.ServiceName.Contains(servicename3))
            {
                try
                {
                    var parts = service.ServiceName.Split('$');
                    mssqlservicename = parts.Last();
                    break;
                }
                catch (ArgumentOutOfRangeException)
                {
                    continue;
                }
            }
        }

        return mssqlservicename;
    }






    /// <returns>NULL if not found</returns>
    private string GetMsSqlServiceName()
    {
        // possible mssql server names
        string servicename = "MSSQL";
        string servicename2 = "SQLAgent";
        string servicename3 = "SQL Server";
        string servicename4 = "msftesql";

        string mssqlservicename = null;
        ServiceController[] services = ServiceController.GetServices();
        foreach (ServiceController service in services)
        {
            if (service == null)
                continue;
            if (service.ServiceName.Contains(servicename) ||
                service.ServiceName.Contains(servicename3))
            {
                try
                {
                    var parts = service.ServiceName.Split('$');
                    mssqlservicename = parts.Last();
                    break;
                }
                catch (ArgumentOutOfRangeException)
                {
                    continue;
                }
            }
        }

        return mssqlservicename;
    }







    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="FailToObtainInstanceNameException"></exception>
    /// <exception cref="AddingOfConnectionStringException"></exception>
    private void AddConnectionStringToConfig()
    {
        string sqlServer;
        try
        {
            sqlServer = Context.Parameters["MSSQLSERVERINSTANCENAME"];
        }
        catch (Exception)
        {
            throw new FailToObtainInstanceNameException();
        }
        if (sqlServer != null)
        {
            try
            {
                // Get the path to the executable file that is being installed on the target computer  
                string assemblypath = Context.Parameters["assemblypath"];
                string appConfigPath = assemblypath + ".config";

                // Write the path to the app.config file  
                XmlDocument doc = new XmlDocument();
                doc.Load(appConfigPath);

                XmlNode configuration = null;
                foreach (XmlNode node in doc.ChildNodes)
                    if (node.Name == "configuration")
                        configuration = node;

                if (configuration != null)
                {
                    XmlNode connectionStringsNode = null;
                    foreach (XmlNode node in configuration.ChildNodes)
                    {
                        if (node.Name == "connectionStrings")
                            connectionStringsNode = node;
                    }

                    if (connectionStringsNode != null)
                    {
                        XmlElement elem = doc.CreateElement("add");

                        XmlAttribute nameAttr = doc.CreateAttribute("name");
                        nameAttr.Value = "DefaultConnection";

                        XmlAttribute connStrAttr = doc.CreateAttribute("connectionString");
                        connStrAttr.Value =
                            $@"Data Source={sqlServer};Initial Catalog=WindowsMetrics;Integrated Security=True";

                        XmlAttribute providerNameAttr = doc.CreateAttribute("providerName");
                        providerNameAttr.Value = $@"System.Data.SqlClient";

                        elem.Attributes.Append(nameAttr);
                        elem.Attributes.Append(connStrAttr);
                        elem.Attributes.Append(providerNameAttr);

                        connectionStringsNode.AppendChild(elem);
                    }
                }
                doc.Save(appConfigPath);
            }
            catch
            {
                throw new AddingOfConnectionStringException
                ("Sorry, automatic adding of a connection string to .config file failed. " +
                    "Please, add connection string manually.");
            }
        }
    }






    private class AddingOfConnectionStringException : Exception
    {
        public AddingOfConnectionStringException(string message) : base(message) { }
    }

    private class NoMSSQLServerInstancesFoundException : Exception
    {
        public NoMSSQLServerInstancesFoundException(string message) : base(message) { }
    }

    private class FailToObtainInstanceNameException : Exception
    {

    }
 */
