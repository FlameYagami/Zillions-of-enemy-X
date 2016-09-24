using System.Windows;

namespace Dialog
{
    public class BaseDialogUtils
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
    }
}