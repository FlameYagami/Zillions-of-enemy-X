using System.Collections.ObjectModel;
using DeckEditor.ViewModel;
using Wrapper.Model;

namespace DeckEditor.View
{
    /// <summary>
    ///     ColumnAbilityDetail.xaml 的交互逻辑
    /// </summary>
    public partial class AbilityDetailDialog
    {
        public AbilityDetailDialog(ObservableCollection<AbilityModel> abilityDetailModels)
        {
            InitializeComponent();
            var abilityDetailVm = new AbilityDetailVm(abilityDetailModels);
            DataContext = abilityDetailVm;
        }
    }
}