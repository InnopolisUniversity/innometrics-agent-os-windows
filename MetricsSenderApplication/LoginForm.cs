using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetricsSenderApplication
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        public void SetLoginClickAction(EventHandler handler)
        {
            btnLogin.Click += handler;
        }

        public void SetCloseAction(CancelEventHandler handler)
        {
            this.Closing += handler;
        }

        public string GetLogin()
        {
            return txbLogin.Text;
        }

        public string GetPassword()
        {
            return txbPassword.Text;
        }
    }
}
