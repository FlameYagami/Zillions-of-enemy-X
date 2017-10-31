using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
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

        //从Handle中获取Window对象
        protected static Window GetWindowFromHwnd(IntPtr hwnd)
        {
            return (Window) HwndSource.FromHwnd(hwnd).RootVisual;
        }

        //GetForegroundWindow API
        [DllImport("user32.dll")]
        protected static extern IntPtr GetForegroundWindow();

        //调用GetForegroundWindow然后调用GetWindowFromHwnd
        protected static Window GetTopWindow()
        {
            var hwnd = GetForegroundWindow();
            return GetWindowFromHwnd(hwnd);
        }
    }
}