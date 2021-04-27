using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WorkTime
{
    public partial class frmInput : Form
    {
        public frmInput()
        {
            InitializeComponent();
        }

        public string Login
        {
            get
            {
                return this.txtLogin.Text;
            }
            set
            {
                this.txtLogin.Text = value;
            }
        }

        public string Password
        {
            get
            {
                return this.txtPassword.Text;
            }
            set
            {
                this.txtPassword.Text = value;
            }
        }
    }
}
