namespace CardEditor.Entity
{
    public class CardEntity
    {
        public string Type { get; set; }
        public string Camp { get; set; }
        public string Race { get; set; }
        public string Sign { get; set; }
        public string Rare { get; set; }
        public string Pack { get; set; }
        public string Restrict { get; set; }
        public string CName { get; set; }
        public string JName { get; set; }
        public string Illust { get; set; }
        public string Number { get; set; }
        public string Cost { get; set; }
        public string Power { get; set; }
        public string Ability { get; set; }
        public string Lines { get; set; }
        public string Faq { get; set; }
        public string AbilityType { get; set; }
        public string AbilityDetail { get; set; }
        public AbilityDetialEntity AbilityDetialEntity { get; set; }
    }
}