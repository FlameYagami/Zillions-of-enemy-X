using System.Collections.Generic;
using System.Collections.ObjectModel;
using Wrapper.Model;

namespace DeckEditor.ViewModel
{
    public class PlayerVm : BaseModel
    {
        public PlayerVm()
        {
            PlayerModels = new ObservableCollection<DeckModel>();
        }

        /// <summary>玩家数据缓存</summary>
        public ObservableCollection<DeckModel> PlayerModels { get; set; }

        public void UpdatePlayerModels(List<DeckModel> deckModels)
        {
            PlayerModels.Clear();
            if (0 != deckModels.Count)
                PlayerModels.Add(deckModels[0]);
        }
    }
}