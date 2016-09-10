using System.Windows;
using System.Windows.Controls;

namespace DeckEditor.Utils
{
    internal class PointUtils
    {
        public static bool IsMousePointInControl(Canvas canvas, Point mousePoint, ListView listView)
        {
            var point = listView.TranslatePoint(new Point(), canvas);
            //MessageBox.Show("X区间：" + mousePoint.X + "<=" + listView.Width);
            return (mousePoint.X >= point.X) && (mousePoint.X <= point.X + listView.Width)
                   && (mousePoint.Y >= point.Y) && (mousePoint.Y <= point.Y + listView.Height);
        }
    }
}