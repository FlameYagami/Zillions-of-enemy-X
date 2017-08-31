using System;
using System.Windows.Forms;
using CardEditor.View;
using Dialog;

namespace CardEditor.Utils
{
    internal class DialogUtils : BaseDialogUtils
    {
        public static string ShowExport(string pack)
        {
            var sfd = new SaveFileDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Filter = @"xls文件(*.xls)|*.xls",
                FileName = pack
            };
            return sfd.ShowDialog() != DialogResult.OK ? string.Empty : sfd.FileName;
        }

        public static void ShowPackCover()
        {
            var dialog = new PackCoverWindow {Owner = GetTopWindow()};
            dialog.Show();
        }
    }
}