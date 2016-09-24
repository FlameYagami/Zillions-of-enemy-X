using System;
using System.Windows.Threading;

namespace Dialog
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
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Close();
        }
    }
}