using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrapper.Model
{
    public class CardQueryModel : CardModel
    {
        public string Key { get; set; }
        public new string Cost { get; set; }
        public new string Power { get; set; }
        public string Order { get; set; }
    }
}
