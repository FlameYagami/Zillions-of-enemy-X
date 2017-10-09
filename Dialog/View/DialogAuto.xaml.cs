using System.Windows.Controls;
using Dialog.ViewModel;

namespace Dialog.View
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