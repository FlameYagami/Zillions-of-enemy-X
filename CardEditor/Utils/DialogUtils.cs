using System.Windows.Forms;
using CardEditor.Utils.Dialog;

namespace CardEditor.Utils
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

        public static string ShowExport(string pack)
        {
            var sfd = new SaveFileDialog
            {
                InitialDirectory = Application.StartupPath,
                Filter = @"xls文件(*.xls)|*.xls",
                FileName = pack
            };
            return sfd.ShowDialog() != DialogResult.OK ? string.Empty : sfd.FileName;
        }
    }
}