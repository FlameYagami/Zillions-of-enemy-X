using System.Collections.Generic;

namespace DeckEditor.Model
{
    public class CardDetailModel
    {
        public string CName { get; set; }
        public List<string> CampPathList { get; set; }
        public string SignPath { get; set; }
        public string Type { get; set; }
        public string RarePath { get; set; }
        public string Number { get; set; }
        public string PowerValue { get; set; }
        public string CostValue { get; set; }
        public string Race { get; set; }
        public string Ability { get; set; }
        public string JName { get; set; }
        public string Pack { get; set; }
        public string Illust { get; set; }
        public string Lines { get; set; }
        public string OrigAbility { get; set; }
    }
}