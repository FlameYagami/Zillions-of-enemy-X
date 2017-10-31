using System.Collections.Generic;
using DeckEditor.Model;

namespace DeckEditor.ViewModel
{
    public class DeckPreviewVm
    {
        public DeckPreviewVm(List<DeckPreviewModel> deckPreviewModels)
        {
            DeckPreviewModels = deckPreviewModels;
        }

        public List<DeckPreviewModel> DeckPreviewModels { get; set; }
    }
}