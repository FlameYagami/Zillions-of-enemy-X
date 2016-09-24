using System.Collections.Generic;
using System.Windows;
using DeckEditor.View;

namespace DeckEditor.Utils
{
    public class DialogUtils
    {
        /// <summary>能力分类窗口</summary>
        public static void ShowAbilityDetail()
        {
            var dlg = new AbilityDetail {Owner = Application.Current.MainWindow};
            var showDialog = dlg.ShowDialog();
        }

        /// <summary>卡组统计窗口</summary>
        public static void ShowDekcStatistical(Dictionary<int, int> deckStatistical)
        {
            var dlg = new DekcStatistical(deckStatistical) {Owner = Application.Current.MainWindow};
            var showDialog = dlg.ShowDialog();
        }
    }
}