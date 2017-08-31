using System.Collections.Generic;
using System.Collections.ObjectModel;
using DeckEditor.View;
using Dialog;
using Wrapper.Model;

namespace DeckEditor.Utils
{
    public class DialogUtils : BaseDialogUtils
    {
        /// <summary>能力分类窗口</summary>
        public static void ShowAbilityDetail(ObservableCollection<AbilityModel> abilityDetailModels)
        {
            var dlg = new AbilityDetail(abilityDetailModels) {Owner = GetTopWindow()};
            dlg.ShowDialog();
        }

        /// <summary>卡组统计窗口</summary>
        public static void ShowDekcStatistical(Dictionary<int, int> deckStatistical)
        {
            var dlg = new DekcStatistical(deckStatistical) {Owner = GetTopWindow()};
            dlg.ShowDialog();
        }
    }
}