using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Dialog
{
    public class BaseDialogUtils
    {
        /// <summary>提示窗口窗口，自动关闭</summary>
        public static void ShowDlg(string value)
        {
            var dlg = new Dlg(value) {Owner = GetTopWindow()};
            dlg.ShowDialog();
        }

        /// <summary>确认窗口，需要用户确认信息</summary>
        public static void ShowDlgOk(string value)
        {
            var dlg = new DlgOk(value) {Owner = GetTopWindow()};
            dlg.ShowDialog();
        }

        /// <summary>信息确认窗口，返回BOOL类型</summary>
        public static bool ShowDlgOkCancel(string value)
        {
            var dlg = new DlgOkCancel(value) {Owner = GetTopWindow()};
            var showDialog = dlg.ShowDialog();
            return (showDialog != null) && showDialog.Value;
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