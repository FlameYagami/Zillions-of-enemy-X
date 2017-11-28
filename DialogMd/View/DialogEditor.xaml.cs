using Dialog.ViewModel;

namespace Dialog.View
{
    /// <summary>
    ///     DialogConfirm.xaml 的交互逻辑
    /// </summary>
    public partial class DialogEditor
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="message">文本内容</param>
        /// <param name="hint">文本提示</param>
        public DialogEditor(string message , string hint = null)
        {
            InitializeComponent();
            DataContext = new DialogEditorVm(message, hint);
        }
    }
}