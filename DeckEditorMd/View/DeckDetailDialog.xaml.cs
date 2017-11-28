using System.Collections.Generic;
using System.Collections.ObjectModel;
using DeckEditor.ViewModel;
using Visifire.Charts;
using Wrapper.Model;

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