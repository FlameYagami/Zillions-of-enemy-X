using System;
using System.Windows.Forms;

namespace CardEditor.View
{
    /// <summary>
    ///     Dlg.xaml 的交互逻辑
    /// </summary>
    public partial class Dlg
    {
        public Dlg(string value)
        {
            InitializeComponent();
            StartKiller();
            LblHint.Content = value;
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