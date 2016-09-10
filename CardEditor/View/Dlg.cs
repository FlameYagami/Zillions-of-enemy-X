using System;
using System.Windows.Forms;

namespace CardEditor.Utils.Dialog
{
    public partial class Dlg : Form
    {
        public Dlg(string value)
        {
            InitializeComponent();
            StartKiller();
            label.Text = value;
        }

        public void StartKiller()
        {
            var timer = new Timer {Interval = 1000};
            //0.5秒启动
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public void Timer_Tick(object sender, EventArgs e)
        {
            Close();
            ((Timer) sender).Stop();
        }
    }
}