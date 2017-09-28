using System.Windows;

namespace Dialog
{
    /// <summary>
    ///     DialogConfirm.xaml 的交互逻辑
    /// </summary>
    public partial class DialogConfirm
    {
        public DialogConfirm(string message)
        {
            InitializeComponent();
            DataContext = new DialogConfirmVm(message);
        }
    }
}