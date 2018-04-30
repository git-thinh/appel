using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace app_sys
{
    public partial class fNotificationMsg : Form
    {
        private static List<fNotificationMsg> lsNoti = new List<fNotificationMsg>() { };
        private System.Windows.Forms.Timer lifeTimer;
        private int duration = 3000; 

        public fNotificationMsg()
        {
            InitializeComponent();
            this.Text = app.app_name;
        }

        public fNotificationMsg(string body, int duration_ = 3000)
        {
            InitializeComponent();
            this.Text = app.app_name;

            lblBody.Text = body;
            duration = duration_; 
        }

        public fNotificationMsg(string title, string body, int duration_ = 3000)
        {
            InitializeComponent();

            lblTitle.Text = title;
            lblBody.Text = body;
            duration = duration_; 
        }

        private void fNoti_Load(object sender, EventArgs e)
        {
            //lblTitle.Text = lblTitle.Text + lsNoti.Count.ToString();//

            this.FormClosed+=fNoti_FormClosed;
            lblBody.Click += label_Click;
            lblTitle.Click += label_Click;

            // Display the form just above the system tray.
            Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - Width,
                                      Screen.PrimaryScreen.WorkingArea.Height - Height);

            // Move each open form upwards to make room for this one
            foreach (fNotificationMsg openForm in lsNoti)
            {
                openForm.Top -= Height + 7;
            }

            lsNoti.Add(this);

            if (duration > 0)
            {
                this.lifeTimer = new System.Windows.Forms.Timer();
                this.lifeTimer.Tick += lifeTimer_Tick;
                lifeTimer.Interval = duration;
                lifeTimer.Start();
            }
        }

        private void lifeTimer_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fNoti_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Move down any open forms above this one
            foreach (fNotificationMsg openForm in lsNoti)
            {
                if (openForm == this)
                {
                    // Remaining forms are below this one
                    break;
                }
                openForm.Top += Height + 7;
            }

            lsNoti.Remove(this);
        }

        private void label_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
