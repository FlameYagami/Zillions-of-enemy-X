using System.Collections.ObjectModel;
using System.Windows;
using DeckEditor.ViewModel;
using Wrapper.Model;

namespace DeckEditor.View
{
    /// <summary>
    ///     ColumnAbilityDetail.xaml 的交互逻辑
    /// </summary>
    public partial class AbilityDetail
    {
        private readonly AbilityDetailVm _abilityDetailVm;

        public AbilityDetail(ObservableCollection<AbilityModel> abilityDetailModels)
        {
            InitializeComponent();
            _abilityDetailVm = new AbilityDetailVm(abilityDetailModels);
            DataContext = _abilityDetailVm;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            _abilityDetailVm.Update();
            Close();
        }
    }
}