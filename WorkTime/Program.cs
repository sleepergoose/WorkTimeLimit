using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

/* 
 * TODO: New check system
 * TODO: Add commentaries
 */


namespace WorkTime
{
    static class Program
    {
        static System.Threading.Timer timer;
        static Stoper stoper;
        static NotifyIcon notifyIcon;
        static ContextMenuStrip NotifyContextMenu;

        [MTAThread]
        static void Main()
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            StartSettings();
            Displaynotify();
            Application.Run();
        }

        static void StartSettings()
        {
            using (FileStream fs = new FileStream("sys.set", FileMode.OpenOrCreate, FileAccess.Read))
            {
                BinaryFormatter bf = new BinaryFormatter();
                stoper = (Stoper)bf.Deserialize(fs);
            }
            if(stoper.FinalTime < DateTime.Now)
            {
                stoper.FinalTime = DateTime.Now.AddMinutes(stoper.TimeSpan.TotalMinutes);
                using (FileStream fs = new FileStream("sys.set", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, stoper);
                }
            }
            timer = new System.Threading.Timer(TimerCallbackFunc, null, stoper.TimeSpan, TimeSpan.FromMilliseconds(0));
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
                //notifyIcon.ShowBalloonTip(100);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        static void mnuExit_Click(object sender, EventArgs e)
        {
            Task task = new Task(() => {
                frmInput LoginDialog = new frmInput();
                LoginDialog.ShowDialog();

                if (LoginDialog.DialogResult == DialogResult.OK
                    && LoginDialog.Login == stoper.Login
                    && LoginDialog.Password == stoper.Password)
                {
                    Application.Exit();
                }
            });
            task.Start();
        }
        static void mnuStart_Click(object sender, EventArgs e)
        {
            using (FileStream fs = new FileStream("sys.set", FileMode.OpenOrCreate, FileAccess.Read))
            {
                BinaryFormatter bf = new BinaryFormatter();
                stoper = (Stoper)bf.Deserialize(fs);
            }
            frmInput LoginDialog = new frmInput();
            if (LoginDialog.ShowDialog() == DialogResult.OK 
                && LoginDialog.Login == stoper.Login 
                && LoginDialog.Password == stoper.Password)
            {
                timer = new System.Threading.Timer(TimerCallbackFunc, null, stoper.TimeSpan, TimeSpan.FromMilliseconds(0));
            }     
        }

        static void TimerCallbackFunc(object args)
        {
            MessageBox.Show("Time is over", "Work Time");
            timer.Dispose();
        }
        static void mnuSettings_Click(object sender, EventArgs e)
        {
            Task task = new Task(() => {
                frmInput LoginDialog = new frmInput();
                LoginDialog.ShowDialog();

                frmMain form = new frmMain();
                using (FileStream fs = new FileStream("sys.set", FileMode.OpenOrCreate, FileAccess.Read))
                {
                    try
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        stoper = (Stoper)bf.Deserialize(fs);
                        (form.Hours, form.Minutes) = stoper.GetTimeSeparate();
                        (form.Login, form.Password, form.ConfirmPassword) =
                            stoper.GetLoginAndPassword();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"It was impossible to read data from file\n {ex.Message}");
                    }
                }

                if (LoginDialog.DialogResult == DialogResult.OK
                    && LoginDialog.Login == stoper.Login
                    && LoginDialog.Password == stoper.Password)
                {
                    form.ShowDialog();

                    if (form.DialogResult == DialogResult.OK)
                    {
                        if(form.Password == form.ConfirmPassword)
                        {
                            using (FileStream fs = new FileStream("sys.set", FileMode.OpenOrCreate, FileAccess.Write))
                            {
                                stoper.Parse((int)form.Hours, (int)form.Minutes);
                                stoper.Login = form.Login;
                                stoper.Password = form.Password;
                                BinaryFormatter bf = new BinaryFormatter();
                                bf.Serialize(fs, stoper);
                            }
                        }
                    }
                }
            });
            task.Start();
        }
    }

    [Serializable]
    class Stoper
    {
        public TimeSpan TimeSpan { get; set; }
        public DateTime FinalTime { get; set; }
        public string Login { get; set; } = "";
        public string Password { get; set; } = "";

        public Stoper() {}

        public Stoper(string login, string password)
        {
            this.Login = login;
            this.Password = password;
        }

        public TimeSpan Parse(int hours, int minutes)
        {
            TimeSpan = TimeSpan.FromMinutes(hours * 60d + minutes);
            FinalTime = DateTime.Now.AddMinutes(TimeSpan.TotalMinutes);
            return TimeSpan;
        }

        public (int, int) GetTimeSeparate()
        {
            return ((int)TimeSpan.Hours, (int)(TimeSpan.Minutes));
        }

        public (string, string, string) GetLoginAndPassword()
        {
            return (Login, Password, Password);
        }
    }
}
