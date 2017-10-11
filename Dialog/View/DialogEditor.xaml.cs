using Dialog.ViewModel;

namespace Dialog
{
    /// <summary>
    ///     DialogConfirm.xaml 的交互逻辑
    /// </summary>
    public partial class DialogEditor
    {
        public DialogEditor(string message)
        {
            InitializeComponent();
            DataContext = new DialogEditorVm(message);
        }
    }
}