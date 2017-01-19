using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardEditor.Constant
{
    public class Enum
    {
        public enum AbilityType
        {
            None = -1,
            Ig = 1,
            Event = 2,
            Start = 3,
            Extra = 4
        }

        public enum PreviewOrderType
        {
            Number = 0,
            Value = 1
        }

        public enum ModeType
        {
            Query = 0,
            Editor = 1,
            Develop = 2
        }
    }
}
