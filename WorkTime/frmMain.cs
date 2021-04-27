using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkTime
{
    public partial class frmMain : Form
    {
        //System.Threading.Timer timer;

        public frmMain()
        {
            InitializeComponent();
            

            btnCancel.Click += BtnCancel_Click;
            btnStart.Click += BtnStart_Click;
        }

        private void MnuSettings_Click(object sender, EventArgs e)
        {

        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
           //timer = new System.Threading.Timer(TimerCallbackFunc,
           //     null, 5000, 0);

        }


        private void TimerCallbackFunc(object args)
        {
            MessageBox.Show("Time is over", "Work Time");
            //timer.Dispose();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ContextMenuNotify_Opening(object sender, CancelEventArgs e)
        {

        }
    }
}
