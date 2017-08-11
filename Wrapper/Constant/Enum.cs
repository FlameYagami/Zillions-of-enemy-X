namespace Wrapper.Constant
{
    public class Enum
    {
        // CardEditor
        public enum AbilityType
        {
            None = -1,
            Ig = 1,
            Event = 2,
            Start = 3,
            Extra = 4
        }

        // DeckEditor
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

        public enum ModeType
        {
            Query = 0,
            Editor = 1,
            Develop = 2
        }

        // Common
        public enum PreviewOrderType
        {
            Number = 0,
            Value = 1
        }
    }
}