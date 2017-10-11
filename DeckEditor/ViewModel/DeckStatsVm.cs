using System.Windows.Media;
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

        public void UpdateView(int igCount, int ugCount, int exCount, int startCount, int lifeCount, int voidCount)
        {
            DeckStatsModel.StartCountValue = $"{startCount} / 1";
            DeckStatsModel.LifeCountValue = $"{lifeCount} / 4";
            DeckStatsModel.VoidCountValue = $"{voidCount} / 4";
            DeckStatsModel.IgCountValue = $"{igCount} / 20";
            DeckStatsModel.UgCountValue = $"{ugCount} / 30";
            DeckStatsModel.ExCountValue = $"{exCount} / 10";

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