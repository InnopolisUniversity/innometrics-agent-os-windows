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
    public partial class SendingProgressForm : Form
    {
        private readonly int _percentageToAddAtOneStep;

        /// <param name="itemsCount">
        /// This whole number of items to send
        /// </param>
        public SendingProgressForm(int itemsCount)
        {
            InitializeComponent();
            this._percentageToAddAtOneStep = 100 / itemsCount;
        }

        public void Increment()
        {
            progressBar.Increment(_percentageToAddAtOneStep);
        }

        public void To100()
        {
            progressBar.Value = 100;
        }

        public void SetCancelClickAction(EventHandler handler)
        {
            buttonCancel.Click += handler;
        }



        // to disable X button
        // from http://stackoverflow.com/questions/7301825/windows-forms-how-to-hide-close-x-button
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
    }
}
