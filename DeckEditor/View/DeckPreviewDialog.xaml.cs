using System.Collections.Generic;
using DeckEditor.Model;
using DeckEditor.ViewModel;

namespace DeckEditor.View
{
    /// <summary>
    ///     DeckPreviewDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DeckPreviewDialog
    {
        public DeckPreviewDialog(List<DeckPreviewModel> deckPreviewModel)
        {
            InitializeComponent();
            DataContext = new DeckPreviewVm(deckPreviewModel);
        }
    }
}