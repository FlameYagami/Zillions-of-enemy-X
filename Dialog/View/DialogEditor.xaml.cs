using Dialog.ViewModel;

namespace Dialog.View
{
    /// <summary>
    ///     DialogConfirm.xaml 的交互逻辑
    /// </summary>
    public partial class DialogEditor
    {
        /// <summary>
        ///     一级构造函数
        /// </summary>
        /// <param name="message">文本内容</param>
        public DialogEditor(string message)
        {
            InitializeComponent();
            DataContext = new DialogEditorVm(message, "");
        }

        /// <summary>
        ///     二级构造函数
        /// </summary>
        /// <param name="message">文本内容</param>
        /// <param name="hint">文本提示</param>
        public DialogEditor(string message, string hint)
        {
            InitializeComponent();
            DataContext = new DialogEditorVm(message, hint);
        }
    }
}