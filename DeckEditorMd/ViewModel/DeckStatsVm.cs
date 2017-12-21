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

        public void UpdateDeckStatsModel(DeckStatsModel deckStatsModel)
        {
            DeckStatsModel.StartCountValue = $"{deckStatsModel.StartCountValue} / 1";
            DeckStatsModel.LifeCountValue = $"{deckStatsModel.LifeCountValue} / 4";
            DeckStatsModel.VoidCountValue = $"{deckStatsModel.VoidCountValue} / 4";
            DeckStatsModel.IgCountValue = $"{deckStatsModel.IgCountValue} / 20";
            DeckStatsModel.UgCountValue = $"{deckStatsModel.UgCountValue} / 30";
            DeckStatsModel.ExCountValue = $"{deckStatsModel.ExCountValue} / 10";

//            DeckStatsModel.StartForeground = startCount == 0
//                ? new SolidColorBrush(Colors.Red)
//                : startCount == 1
//                    ? new SolidColorBrush(Colors.Lime)
//                    : new SolidColorBrush(Colors.Yellow);
//            DeckStatsModel.LifeForeground = (lifeCount == 0) || (lifeCount == 1)
//                ? new SolidColorBrush(Colors.Red)
//                : lifeCount == 2
//                    ? new SolidColorBrush(Colors.Yellow)
//                    : new SolidColorBrush(Colors.Lime);
//            DeckStatsModel.VoidForeground = (voidCount == 0) || (voidCount == 1)
//                ? new SolidColorBrush(Colors.Red)
//                : voidCount == 2
//                    ? new SolidColorBrush(Colors.Yellow)
//                    : new SolidColorBrush(Colors.Lime);
//            OnPropertyChanged(nameof(DeckStatsModel));
        }
    }
}