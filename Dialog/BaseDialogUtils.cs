using System;
using System.Threading.Tasks;
using Dialog.View;
using MaterialDesignThemes.Wpf;

namespace Dialog
{
    public class BaseDialogUtils
    {
        /// <summary>提示窗口窗口，自动关闭</summary>
        public static void ShowDialogAuto(string message)
        {
            var view = new DialogAuto(message);
            DialogHost.Show(view, (sender, eventArgs) =>
            {
                Task.Delay(TimeSpan.FromSeconds(1.5))
                    .ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                        TaskScheduler.FromCurrentSynchronizationContext());
            }, (sender, eventArgs) => { });
        }

        /// <summary>确认窗口，需要用户确认信息</summary>
        public static void ShowDialogOk(string message)
        {
            var view = new DialogOk(message);
            DialogHost.Show(view, (sender, eventArgs) => { },
                (sender, eventArgs) => { });
        }

        /// <summary>信息确认窗口，返回BOOL类型</summary>
        public static async Task<bool> ShowDialogConfirm(string message)
        {
            var view = new DialogConfirm(message);
            var result = await DialogHost.Show(view, (sender, eventArgs) => { },
                (sender, eventArgs) => { });
            return result.ToString().Equals(true.ToString());
        }
    }
}