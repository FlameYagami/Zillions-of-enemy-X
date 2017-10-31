using System.Collections.ObjectModel;
using Wrapper.Constant;
using Wrapper.Model;

namespace DeckEditor.ViewModel
{
    public class DeckExVm
    {
        public DeckExVm()
        {
            IgExModels = new ObservableCollection<DeckExModel>();
            UgExModels = new ObservableCollection<DeckExModel>();
            ExExModels = new ObservableCollection<DeckExModel>();
        }

        /// <summary>点燃数据缓存</summary>
        public ObservableCollection<DeckExModel> IgExModels { get; set; }

        /// <summary>非点燃数据缓存</summary>
        public ObservableCollection<DeckExModel> UgExModels { get; set; }

        /// <summary>额外数据缓存</summary>
        public ObservableCollection<DeckExModel> ExExModels { get; set; }

        public void UpdateDeckExModels(DeckManager deckManager, Enums.AreaType areaType)
        {
            switch (areaType)
            {
                case Enums.AreaType.Ig:
                    IgExModels.Clear();
                    deckManager.IgExModels.ForEach(IgExModels.Add);
                    break;
                case Enums.AreaType.Ug:
                    UgExModels.Clear();
                    deckManager.UgExModels.ForEach(UgExModels.Add);
                    break;
                case Enums.AreaType.Ex:
                    ExExModels.Clear();
                    deckManager.ExExModels.ForEach(ExExModels.Add);
                    break;
            }
        }

        public void UpdateAllDeckExModels(DeckManager deckManager)
        {
            Clear();
            deckManager.IgExModels.ForEach(IgExModels.Add);
            deckManager.UgExModels.ForEach(UgExModels.Add);
            deckManager.ExExModels.ForEach(ExExModels.Add);
        }

        public void Clear()
        {
            IgExModels.Clear();
            UgExModels.Clear();
            ExExModels.Clear();
        }
    }
}