using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkTime
{
    static class Program
    {
        static System.Threading.Timer timer;
        static System.Windows.Forms.NotifyIcon notifyIcon;
        static System.Windows.Forms.ContextMenuStrip NotifyContextMenu;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Displaynotify();
            Application.Run();

        }


        static void Displaynotify()
        {
            try
            {
                NotifyContextMenu = new ContextMenuStrip()
                {
                    Items = { 
                        new ToolStripMenuItem() 
                        { 
                            Text = "Exit",
                            Name = "mnuExit",
                            Image = System.Drawing.Image.FromFile("sync.ico")
                        },
                        new ToolStripMenuItem()
                        {
                            Text = "Start",
                            Name = "mnuStart"
                        },
                        new ToolStripMenuItem()
                        {
                            Text = "Settings",
                            Name = "mnuSettings"
                        }
                    }  
                };

                NotifyContextMenu.Items["mnuExit"].Click += mnuExit_Click;
                NotifyContextMenu.Items["mnuStart"].Click += mnuStart_Click;
                NotifyContextMenu.Items["mnuSettings"].Click += mnuSettings_Click;

                notifyIcon = new NotifyIcon()
                {
                    Icon = new System.Drawing.Icon(Path.GetFullPath(@"sync.ico")),
                    Text = "Export Datatable Utlity",
                    Visible = true,
                    BalloonTipTitle = "Welcome Devesh omar to Datatable Export Utlity",
                    BalloonTipText = "Click Here to see details",
                    BalloonTipIcon = ToolTipIcon.Info,
                    ContextMenuStrip = NotifyContextMenu
                };
                notifyIcon.ShowBalloonTip(100);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        static void mnuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        static void mnuStart_Click(object sender, EventArgs e)
        {

        }
        static void mnuSettings_Click(object sender, EventArgs e)
        {
            Task task = new Task(() => {
                frmInput LoginDialog = new frmInput();
                LoginDialog.ShowDialog();

                if (LoginDialog.DialogResult == DialogResult.OK
                    && LoginDialog.txtPassword.Text == ""
                    && LoginDialog.txtLogin.Text == "")
                {
                    frmMain form = new frmMain();
                    using (FileStream fs = new FileStream("sys.set", FileMode.OpenOrCreate, FileAccess.Read))
                    {
                        byte[] buffer = new byte[2];
                        fs.Read(buffer, 0, 2);
                        form.numHours.Value = buffer[0];
                        form.numMinutes.Value = buffer[1];
                    }
                    form.ShowDialog();

                    if (form.DialogResult == DialogResult.OK)
                    {
                        using (FileStream fs = new FileStream("sys.set", FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            byte[] buffer = new byte[2];
                            buffer[0] = (byte)form.numHours.Value;
                            buffer[1] = (byte)form.numMinutes.Value;
                            fs.Write(buffer, 0, 2);
                        }
                    }
                }
            });
            task.Start();
        }
    }
}
