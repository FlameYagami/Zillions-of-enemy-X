using System.Windows;

namespace DeckEditor.View
{
    /// <summary>
    ///     DlgOkCancel.xaml 的交互逻辑
    /// </summary>
    public partial class DlgOkCancel : Window
    {
        public DlgOkCancel(string message)
        {
            InitializeComponent();
            LblHint.Content = message;
        }

        private void BtnCacncel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}