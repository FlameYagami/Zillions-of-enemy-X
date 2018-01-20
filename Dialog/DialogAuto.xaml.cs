using System;
using System.Windows.Threading;

namespace Dialog
{
    /// <summary>
    ///     Dlg.xaml 的交互逻辑
    /// </summary>
    public partial class DialogAuto
    {
        public DialogAuto(string value,int second)
        {
            InitializeComponent();
            StartKiller(second);
            LblHint.Content = value;
        }

        public void StartKiller(int second)
        {
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, second);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Close();
        }
    }
}