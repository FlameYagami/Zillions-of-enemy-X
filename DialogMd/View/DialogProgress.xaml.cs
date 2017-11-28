using Dialog.ViewModel;

namespace Dialog.View
{
    /// <summary>
    ///     DialogProgress.xaml 的交互逻辑
    /// </summary>
    public partial class DialogProgress
    {
        public DialogProgress()
        {
            InitializeComponent();
            DataContext = new DialogProgressVm("请稍后...");
        }

        public DialogProgress(string message)
        {
            InitializeComponent();
            DataContext = new DialogProgressVm(message);
        }
    }
}