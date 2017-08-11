using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Wrapper;
using Wrapper.Model;

namespace DeckEditor.View
{
    /// <summary>
    ///     ColumnAbilityDetail.xaml 的交互逻辑
    /// </summary>
    public partial class AbilityDetail
    {
        public AbilityDetail()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            var checkboxListDic = LstAbilityDetail.Items.Cast<CheckBox>()
                .ToDictionary(checkbox => checkbox.Content.ToString(),
                    checkbox => checkbox.IsChecked != null && (bool)checkbox.IsChecked);
            DataCache.AbilityDetialModel = new AbilityDetialModel(checkboxListDic);
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var abilityDetialDic = DataCache.AbilityDetialModel.GetAbilityDetailDic();
            foreach (var checkbox in LstAbilityDetail.Items.Cast<CheckBox>()) // 根据能力分类模型勾选对应的能力
                foreach (var abilityDetailItem in abilityDetialDic)
                {
                    if (!abilityDetailItem.Key.Equals(checkbox.Content)) continue;
                    checkbox.IsChecked = abilityDetailItem.Value.Equals(1);
                    break;
                }
        }
    }
}