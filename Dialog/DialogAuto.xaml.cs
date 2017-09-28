using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Dialog
{
    /// <summary>
    ///     DialogConfirm.xaml 的交互逻辑
    /// </summary>
    public partial class DialogAuto : UserControl
    {
        public DialogAuto(string message)
        {
            InitializeComponent();
            DataContext = new DialogAutoVm(message);
        }
    }
}