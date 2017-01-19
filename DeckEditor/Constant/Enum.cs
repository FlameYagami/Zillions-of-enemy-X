using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckEditor.Constant
{
    public class Enum
    {
        public enum AreaType
        {
            None = -1,
            Pl = 0,
            Ig = 1,
            Ug = 2,
            Ex = 3
        }

        public enum DeckOrderType
        {
            Value = 0,
            Random = 1
        }

        public enum IgType
        {
            Life = 0,
            Void = 1,
            Normal = 2
        }

        public enum PreviewOrderType
        {
            Number = 0,
            Value = 1
        }
    }
}
