using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Dialog
{
    public class BaseDialogUtils
    {
        /// <summary>提示窗口窗口，自动关闭</summary>
        public static void ShowDialogAuto(string value, int second = 1)
        {
            var dlg = new DialogAuto(value, second) {Owner = GetTopWindow()};
            dlg.ShowDialog();
        }

        /// <summary>确认窗口，需要用户确认信息</summary>
        public static void ShowDialogOk(string value)
        {
            var dlg = new DialogOk(value) {Owner = GetTopWindow()};
            dlg.ShowDialog();
        }

        /// <summary>信息确认窗口，返回BOOL类型</summary>
        public static bool ShowDialogConfirm(string value)
        {
            var dlg = new DialogConfirm(value) {Owner = GetTopWindow()};
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