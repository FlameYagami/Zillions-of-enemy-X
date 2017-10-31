namespace Wrapper.Constant
{
    public class Enums
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
            Player = 0,
            Ig = 1,
            Ug = 2,
            Ex = 3
        }

        public enum IgType
        {
            Normal = 0,
            Life = 1,
            Void = 2
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

        public enum UgType
        {
            Normal = 0,
            Start = 4
        }
    }
}