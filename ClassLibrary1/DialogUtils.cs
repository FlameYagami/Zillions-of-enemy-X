using System.Collections.Generic;
using System.Windows;
using DeckEditor.View;

namespace DeckEditor.Utils
{
    public class DialogUtils
    {
        /// <summary>提示窗口窗口，自动关闭</summary>
        public static void ShowDlg(string value)
        {
            var dlg = new Dlg(value) {Owner = Application.Current.MainWindow};
            dlg.ShowDialog();
        }

        /// <summary>确认窗口，需要用户确认信息</summary>
        public static void ShowDlgOk(string value)
        {
            var dlg = new DlgOk(value) {Owner = Application.Current.MainWindow};
            dlg.ShowDialog();
        }

        /// <summary>信息确认窗口，返回BOOL类型</summary>
        public static bool ShowDlgOkCancel(string value)
        {
            var dlg = new DlgOkCancel(value) {Owner = Application.Current.MainWindow};
            var showDialog = dlg.ShowDialog();
            return (showDialog != null) && showDialog.Value;
        }

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