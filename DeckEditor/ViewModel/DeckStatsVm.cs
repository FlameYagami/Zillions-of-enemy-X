using System.Windows.Media;
using Common;
using DeckEditor.Model;
using Wrapper.Model;

namespace DeckEditor.ViewModel
{
    public class DeckStatsVm : BaseModel
    {
        public DeckStatsVm()
        {
            DeckStatsModel = new DeckStatsModel();
        }

        public DeckStatsModel DeckStatsModel { get; set; }

        public void UpdateView(int startCount, int lifeCount, int voidCount)
        {
            DeckStatsModel.StartCount = startCount;
            DeckStatsModel.LifeCount = lifeCount;
            DeckStatsModel.VoidCount = voidCount;

            DeckStatsModel.StartForeground = startCount == 0
                ? new SolidColorBrush(Colors.Red)
                : startCount == 1
                    ? new SolidColorBrush(Colors.Lime)
                    : new SolidColorBrush(Colors.Yellow);
            DeckStatsModel.LifeForeground = (lifeCount == 0) || (lifeCount == 1)
                ? new SolidColorBrush(Colors.Red)
                : lifeCount == 2
                    ? new SolidColorBrush(Colors.Yellow)
                    : new SolidColorBrush(Colors.Lime);
            DeckStatsModel.VoidForeground = (voidCount == 0) || (voidCount == 1)
                ? new SolidColorBrush(Colors.Red)
                : voidCount == 2
                    ? new SolidColorBrush(Colors.Yellow)
                    : new SolidColorBrush(Colors.Lime);
            OnPropertyChanged(nameof(DeckStatsModel));
        }
    }
}