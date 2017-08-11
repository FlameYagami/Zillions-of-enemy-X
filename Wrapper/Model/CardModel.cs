using System.Collections.Generic;

namespace Wrapper.Model
{
    public class CardModel
    {
        public string Md5 { get; set; }
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
        public int Cost { get; set; }
        public int Power { get; set; }
        public string Ability { get; set; }
        public string Lines { get; set; }
        public string ImageJson { get; set; }
        public Dictionary<string, bool> AbilityTypeDic { get; set; }
        public Dictionary<string, bool> AbilityDetailDic { get; set; }
    }
}