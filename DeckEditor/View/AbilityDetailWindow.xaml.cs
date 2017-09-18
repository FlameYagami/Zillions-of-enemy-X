using System.Collections.ObjectModel;
using System.Windows;
using DeckEditor.ViewModel;
using Wrapper.Model;

namespace DeckEditor.View
{
    /// <summary>
    ///     ColumnAbilityDetail.xaml 的交互逻辑
    /// </summary>
    public partial class AbilityDetailWindow
    {
        public AbilityDetailWindow(ObservableCollection<AbilityModel> abilityDetailModels)
        {
            InitializeComponent();
            var abilityDetailVm = new AbilityDetailVm(this, abilityDetailModels);
            DataContext = abilityDetailVm;
        }
    }
}