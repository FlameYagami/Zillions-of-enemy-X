using System.Collections.ObjectModel;
using System.Linq;
using Wrapper.Model;

namespace DeckEditor.ViewModel
{
    public class DeckVm
    {
        public DeckVm()
        {
            IgModels = new ObservableCollection<DeckModel>();
            UgModels = new ObservableCollection<DeckModel>();
            ExModels = new ObservableCollection<DeckModel>();
        }

        /// <summary>点燃数据缓存</summary>
        public ObservableCollection<DeckModel> IgModels { get; set; }

        /// <summary>非点燃数据缓存</summary>
        public ObservableCollection<DeckModel> UgModels { get; set; }

        /// <summary>额外数据缓存</summary>
        public ObservableCollection<DeckModel> ExModels { get; set; }
    }
}