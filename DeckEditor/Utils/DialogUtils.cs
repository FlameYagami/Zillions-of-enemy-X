using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using DeckEditor.View;
using Dialog;
using Wrapper.Model;

namespace DeckEditor.Utils
{
    public class DialogUtils
    {
        //从Handle中获取Window对象
        protected static Window GetWindowFromHwnd(IntPtr hwnd)
        {
            return (Window)HwndSource.FromHwnd(hwnd).RootVisual;
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

        /// <summary>能力分类窗口</summary>
        public static void ShowAbilityDetail(ObservableCollection<AbilityModel> abilityDetailModels)
        {
            var dlg = new AbilityDetailWindow(abilityDetailModels) {Owner = GetTopWindow()};
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