using System.Collections.ObjectModel;
using Wrapper.Model;

namespace DeckEditor.ViewModel
{
    public class PlayerVm
    {
        public PlayerVm()
        {
            PlayerModels = new ObservableCollection<DeckModel>();
        }

        /// <summary>玩家数据缓存</summary>
        public ObservableCollection<DeckModel> PlayerModels { get; set; }
    }
}