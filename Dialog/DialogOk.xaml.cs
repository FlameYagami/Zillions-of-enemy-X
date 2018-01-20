using System.Windows;

namespace Dialog
{
    /// <summary>
    ///     DlgOK.xaml 的交互逻辑
    /// </summary>
    public partial class DialogOk
    {
        public DialogOk(string message)
        {
            InitializeComponent();
            LblHint.Content = message;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}