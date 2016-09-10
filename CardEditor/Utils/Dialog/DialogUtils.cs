using System.Windows.Forms;

namespace CardEditor.Utils.Dialog
{
    public class DialogUtils
    {
        /// <summary>提示窗口窗口，自动关闭</summary>
        public static void ShowDlg(string value)
        {
            new Dlg(value).ShowDialog();
        }

        /// <summary>确认窗口，需要用户确认信息</summary>
        public static void ShowDlgOk(string value)
        {
            new DlgOK(value).ShowDialog();
        }

        /// <summary>信息确认窗口，返回BOOL类型</summary>
        public static bool ShowDlgOkCancel(string value)
        {
            var dlg = new DlgOKCANCEL(value);
            return DialogResult.OK == dlg.ShowDialog();
        }
    }
}