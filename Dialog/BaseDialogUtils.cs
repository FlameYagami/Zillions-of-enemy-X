using System;
using System.Threading.Tasks;
using Dialog.View;
using MaterialDesignThemes.Wpf;

namespace Dialog
{
    public class BaseDialogUtils
    {
        /// <summary>
        ///     二级信息提示窗口
        /// </summary>
        /// <param name="message">文本信息</param>
        public static async void ShowDialogAuto(string message)
        {
            await DialogHost.Show(new DialogAuto(message), (sender, eventArgs) =>
            {
                Task.Delay(TimeSpan.FromSeconds(1))
                    .ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                        TaskScheduler.FromCurrentSynchronizationContext());
            }, (sender, eventArgs) => { });
        }

        /// <summary>
        ///     二级信息提示窗口
        /// </summary>
        /// <param name="message">文本信息</param>
        /// <param name="second">自动关闭时间（秒）</param>
        public static async void ShowDialogAuto(string message, int second)
        {
            await DialogHost.Show(new DialogAuto(message), (sender, eventArgs) =>
            {
                Task.Delay(TimeSpan.FromSeconds(second))
                    .ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                        TaskScheduler.FromCurrentSynchronizationContext());
            }, (sender, eventArgs) => { });
        }

        /// <summary>
        ///     信息提示窗口，需要用户点击确认
        /// </summary>
        /// <param name="message">文本信息</param>
        public static async void ShowDialogOk(string message)
        {
            await DialogHost.Show(new DialogOk(message));
        }

        /// <summary>
        ///     信息确认窗口，需要用户点击确认
        /// </summary>
        /// <param name="message">文本信息</param>
        /// <returns>true|false</returns>
        public static async Task<bool> ShowDialogConfirm(string message)
        {
            var result = await DialogHost.Show(new DialogConfirm(message));
            return result.ToString().Equals(true.ToString());
        }

        /// <summary>
        ///     信息编辑窗口
        /// </summary>
        /// <param name="message">文本信息</param>
        /// <returns>true|false</returns>
        public static async Task<string> ShowDialogEditor(string message)
        {
            var result = await DialogHost.Show(new DialogEditor(message), DialogEditorClosingEventHandler);
            return result.ToString().Equals("DialogCancel") ? string.Empty : result.ToString();
        }

        /// <summary>
        ///     信息编辑窗口
        /// </summary>
        /// <param name="message">文本信息</param>
        /// <param name="hint">文本提示</param>
        /// <returns>true|false</returns>
        public static async Task<string> ShowDialogEditor(string message, string hint)
        {
            var result = await DialogHost.Show(new DialogEditor(message, hint), DialogEditorClosingEventHandler);
            return result.ToString().Equals("DialogCancel") ? string.Empty : result.ToString();
        }

        private static void DialogEditorClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (string.IsNullOrEmpty(eventArgs.Parameter.ToString()))
                eventArgs.Cancel();
        }
    }
}