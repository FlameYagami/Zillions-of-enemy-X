namespace CardEditor.MVP
{
    public class CardModel
    {
        public CardModel(string type, string camp, string race, string sign, string rare, string pack,
            string limit, string cname, string jname, string illust, string number, string cost, string power,
            string ability, string lines, string faq, string abilityType, string abilityDetail)
        {
            Type = type;
            Camp = camp;
            Race = race;
            Sign = sign;
            Rare = rare;
            Cname = cname;
            Jname = jname;
            Illust = illust;
            Pack = pack;
            Number = number;
            Cost = cost;
            Power = power;
            Ability = ability;
            Lines = lines;
            Faq = faq;
            Limit = limit;
            AbilityType = abilityType;
            AbilityDetail = abilityDetail;
        }

        public string Type { get; set; }
        public string Camp { get; set; }
        public string Race { get; set; }
        public string Sign { get; set; }
        public string Rare { get; set; }
        public string Pack { get; set; }
        public string Limit { get; set; }
        public string Cname { get; set; }
        public string Jname { get; set; }
        public string Illust { get; set; }
        public string Number { get; set; }
        public string Cost { get; set; }
        public string Power { get; set; }
        public string Ability { get; set; }
        public string Lines { get; set; }
        public string Faq { get; set; }
        public string AbilityType { get; set; }
        public string AbilityDetail { get; set; }
    }
}