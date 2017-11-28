using System.Collections.Generic;

namespace DeckEditor.Model
{
    public class DeckPreviewModel
    {
        public string DeckName { get; set; }
        public string StatusMain { get; set; }
        public string StatusExtra { get; set; }
        public string PlayerPath { get; set; }
        public string StartPath { get; set; }
        public List<string> NumberExList { get; set; }
    }
}