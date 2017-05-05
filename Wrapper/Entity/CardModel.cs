using System.Windows.Controls;

namespace CardEditor.MVP
{
    public class ControlModel
    {
        public ControlModel(ComboBox type, ComboBox camp, ComboBox race, ComboBox sign, ComboBox rare, ComboBox pack,
            ComboBox restrict, TextBox cname, TextBox jname, TextBox illust, TextBox number, TextBox cost, TextBox power,
            TextBox ability, TextBox lines, TextBox faq, ListBox abilityType, ListBox abilityDetail)
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
            Restrict = restrict;
            AbilityType = abilityType;
            AbilityDetail = abilityDetail;
        }

        public ComboBox Type { get; set; }
        public ComboBox Camp { get; set; }
        public ComboBox Race { get; set; }
        public ComboBox Sign { get; set; }
        public ComboBox Rare { get; set; }
        public ComboBox Pack { get; set; }
        public ComboBox Restrict { get; set; }
        public TextBox Cname { get; set; }
        public TextBox Jname { get; set; }
        public TextBox Illust { get; set; }
        public TextBox Number { get; set; }
        public TextBox Cost { get; set; }
        public TextBox Power { get; set; }
        public TextBox Ability { get; set; }
        public TextBox Lines { get; set; }
        public TextBox Faq { get; set; }
        public ListBox AbilityType { get; set; }
        public ListBox AbilityDetail { get; set; }
    }
}