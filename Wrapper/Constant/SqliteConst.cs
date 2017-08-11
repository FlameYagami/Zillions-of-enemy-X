namespace Wrapper.Constant
{
    public class SqliteConst
    {
        // Common
        public const string TableName = "TableCard";
        public const string ColumnMd5 = "md5";
        public const string ColumnType = "Type";
        public const string ColumnCamp = "Camp";
        public const string ColumnRace = "Race";
        public const string ColumnSign = "Sign";
        public const string ColumnRare = "Rare";
        public const string ColumnPack = "Pack";
        public const string ColumnCName = "CName";
        public const string ColumnJName = "JName";
        public const string ColumnIllust = "Illust";
        public const string ColumnNumber = "Number";
        public const string ColumnCost = "Cost";
        public const string ColumnPower = "Power";
        public const string ColumnLines = "Lines";
        public const string ColumnImage = "Image";
        public const string ColumnAbility = "Ability";
        public const string ColumnAbilityDetail = "AbilityDetail"; // 存放Json数据的字段

        // CardeDitor
        public const string ColumnCard =
            " (" + ColumnMd5 + "," + ColumnType + "," + ColumnCamp + "," + ColumnRace + "," + ColumnSign + "," +
            ColumnRare + "," + ColumnPack + "," + ColumnCName + "," + ColumnJName + "," +
            ColumnIllust + "," + ColumnNumber + "," + ColumnCost + "," + ColumnPower + "," + ColumnAbility + "," +
            ColumnLines + ","  + ColumnImage + "," + ColumnAbilityDetail + ")";

        //DeckEditor
        public static string[] ColumKeyArray =
        {
            ColumnCName, ColumnType, ColumnCamp, ColumnRace, ColumnSign, ColumnRare, ColumnIllust, ColumnNumber,
            ColumnCost, ColumnPower, ColumnAbility
        };
    }
}