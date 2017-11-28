using Dialog.ViewModel;

namespace Dialog.View
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