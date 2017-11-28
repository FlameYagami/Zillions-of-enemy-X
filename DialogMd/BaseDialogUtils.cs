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
        /// <param name="identifier">容器标识符</param>
        /// <param name="second">自动关闭时间（秒）</param>
        public static async void ShowDialogAuto(string message, string identifier = null, int second = 1)
        {
            if (second <= 0) throw new ArgumentOutOfRangeException(nameof(second));
            await DialogHost.Show(new DialogAuto(message), identifier, (object sender, DialogClosingEventArgs eventArgs) =>
            {
                Task.Delay(TimeSpan.FromSeconds(second))
                    .ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                        TaskScheduler.FromCurrentSynchronizationContext());
            });
        }

        /// <summary>
        ///     信息提示窗口，需要用户点击确认
        /// </summary>
        /// <param name="message">文本信息</param>
        /// <param name="identifier">容器标识符</param>
        public static async void ShowDialogOk(string message , string identifier = null)
        {
            await DialogHost.Show(new DialogOk(message), identifier);
        }

        /// <summary>
        ///     信息确认窗口，需要用户点击确认
        /// </summary>
        /// <param name="message">文本信息</param>
        /// <param name="identifier">容器标识符</param>
        /// <returns>true|false</returns>
        public static async Task<bool> ShowDialogConfirm(string message, string identifier = null)
        {
            var result = await DialogHost.Show(new DialogConfirm(message), identifier);
            return result.ToString().Equals(true.ToString());
        }

        /// <summary>
        ///     信息编辑窗口
        /// </summary>
        /// <param name="message">文本信息</param>
        /// <param name="hint">文本提示</param>
        /// <param name="identifier">容器标识符</param>
        /// <returns>true|false</returns>
        public static async Task<string> ShowDialogEditor(string message, string hint, string identifier = null)
        {
            var result = await DialogHost.Show(new DialogEditor(message, hint), identifier,
                (sender, eventArgs) =>
                {
                    if (string.IsNullOrEmpty(eventArgs.Parameter.ToString()))
                        eventArgs.Cancel();
                });
            return result.ToString().Equals("DialogCancel") ? string.Empty : result.ToString();
        }
    }
}