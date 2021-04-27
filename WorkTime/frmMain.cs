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
        public string ConfirmPassword
        {
            get
            {
                return this.txtConfirmPassword.Text;
            }
            set
            {
                this.txtConfirmPassword.Text = value;
            }
        }
        public int Hours
        {
            get
            {
                return (int)this.numHours.Value;
            }
            set
            {
                this.numHours.Value = value;
            }
        }
        public int Minutes
        {
            get
            {
                return (int)this.numMinutes.Value;
            }
            set
            {
                this.numMinutes.Value = value;
            }
        }

        public frmMain()
        {
            InitializeComponent();
            btnCancel.Click += BtnCancel_Click;
            checkBoxMore.CheckedChanged += CheckBoxMore_CheckedChanged;
        }

        private void CheckBoxMore_CheckedChanged(object sender, EventArgs e)
        {
            if(sender is CheckBox cb)
            {
                if (cb.Checked == true)
                {
                    Size newSize = new Size(this.Width * 2, this.Height);
                    this.MaximumSize = newSize;
                    this.Size = newSize;
                }
                else
                {
                    Size newSize = new Size(this.Width / 2, this.Height);
                    this.MaximumSize = newSize;
                    this.Size = newSize;
                }
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            
        }
    }
}
