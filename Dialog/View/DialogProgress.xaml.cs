using System.Windows.Controls;

namespace Dialog
{
    /// <summary>
    ///     DialogProgress.xaml 的交互逻辑
    /// </summary>
    public partial class DialogProgress : UserControl
    {
        public DialogProgress()
        {
            InitializeComponent();
//            DataContext = new DialogProgressVm();
        }

        public DialogProgress(string message)
        {
            InitializeComponent();
//            DataContext = new DialogProgressVm(message);
        }
    }
}