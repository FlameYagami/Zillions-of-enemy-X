using DeckEditor.ViewModel;

namespace DeckEditor.View
{
    /// <summary>
    ///     DeckStatisticalDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DeckDetailDialog
    {
        public DeckDetailDialog(DeckManager deckManager)
        {
            InitializeComponent();
            DataContext = new DeckDetailVm(this, deckManager);
        }
    }
}