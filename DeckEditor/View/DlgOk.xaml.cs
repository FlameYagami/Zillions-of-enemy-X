using System.Windows;

namespace DeckEditor.View
{
    /// <summary>
    ///     DlgOK.xaml 的交互逻辑
    /// </summary>
    public partial class DlgOk
    {
        public DlgOk(string message)
        {
            InitializeComponent();
            label.Content = label;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}