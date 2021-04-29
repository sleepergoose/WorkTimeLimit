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
        static string sysFilePath = "sys.set";

        [MTAThread]
        static void Main()
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Start();
            DisplayNotify();
            Application.Run();
        }

        static void Start()
        {
            if(File.Exists("sys.set") == true)
            {
                ReadFile();

                if (stoper.FinalTime >= DateTime.Now)
                    timer = new System.Threading.Timer(TimerCallbackFunc, null, 0, 60000);
                else if (stoper.FinalTime.Day < DateTime.Now.Day)
                {
                    stoper.FinalTime = DateTime.Now + stoper.TimeSpan;
                    SaveFile();
                }
                else
                    PCShutdown();
            }
            else
            {
                // Push app to register here
                stoper = new Stoper()
                {
                    Password = "",
                    Login = "",
                    TimeSpan = TimeSpan.FromMilliseconds(0),
                    FinalTime = DateTime.Parse("1/1/0")
                };
                SaveFile();
            }
        }

        static void SaveFile()
        {
            using (FileStream fs = new FileStream(sysFilePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, stoper);
            }
        }

        static void ReadFile()
        {
            using (FileStream fs = new FileStream("sys.set", FileMode.OpenOrCreate, FileAccess.Read))
            {
                BinaryFormatter bf = new BinaryFormatter();
                stoper = (Stoper)bf.Deserialize(fs);
            }
        }

        static void PCShutdown()
        {
            MessageBox.Show("The End");
        }

        static void DisplayNotify()
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
                    Visible = true,
                    ContextMenuStrip = NotifyContextMenu
                };               
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
            ReadFile();
            frmInput LoginDialog = new frmInput();
            if (LoginDialog.ShowDialog() == DialogResult.OK 
                && LoginDialog.Login == stoper.Login 
                && LoginDialog.Password == stoper.Password)
            {
                timer = new System.Threading.Timer(TimerCallbackFunc, null, 0, 60000);
            }     
        }

        static void TimerCallbackFunc(object args)
        {
            TimeSpan timeSpan = stoper.FinalTime - DateTime.Now;
            MessageBox.Show($"Info: \n" +
                $"FinalTime: {stoper.FinalTime.ToString()}\n" +
                $"TimeSpan: {stoper.TimeSpan.ToString()}\n" +
                $"Remaining: {timeSpan.Hours}:{timeSpan.Minutes}", "Work Time", MessageBoxButtons.OK, MessageBoxIcon.Information );
            if (timeSpan.TotalMinutes <= 10)
            {
                notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon.BalloonTipText = $"In {(int)timeSpan.TotalMinutes} mins this PC will be turn off. \n" +
                    $"Please, save your work and close all the applications";
                notifyIcon.ShowBalloonTip(1000);
            }
            else if(stoper.FinalTime <= DateTime.Now)
            {
                PCShutdown();
            }
        }

        /// <summary>
        /// Shows settings menu 
        /// </summary>
        static void mnuSettings_Click(object sender, EventArgs e)
        {
            Task task = new Task(() => {
                // 
                frmInput LoginDialog = new frmInput();
                LoginDialog.ShowDialog();

                ReadFile();
                frmMain form = new frmMain();
                (form.Hours, form.Minutes) = stoper.GetTimeSeparate();
                (form.Login, form.Password, form.ConfirmPassword) = stoper.GetLoginAndPassword();

                if (LoginDialog.DialogResult == DialogResult.OK
                    && LoginDialog.Login == stoper.Login
                    && LoginDialog.Password == stoper.Password)
                {
                    
                    form.ShowDialog();

                    if (form.DialogResult == DialogResult.OK)
                    {
                        if(form.Password == form.ConfirmPassword)
                        {
                            stoper.Parse((int)form.Hours, (int)form.Minutes);
                            stoper.Login = form.Login;
                            stoper.Password = form.Password;
                            SaveFile();
                        }
                    }
                }
            });
            task.Start();
        }
    }

    /// <summary>
    /// Model class
    /// </summary>
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
