namespace DeckEditor.Constant
{
    public class SqliteConst
    {
        public const string DatabaseName = "Data.db";
        public const string DatabasePassword = "DatabasePassword";
        public const string TableName = "TableCard";
        public const string QueryAllSql = "SELECT * FROM " + TableName + " ORDER BY " + ColumnNumber + " ASC";
        public const string QueryBaseSql = "SELECT * FROM " + TableName + " WHERE 1=1";

        /// <summary>卡编排序方式</summary>
        public const string OrderNumberSql = " ORDER BY " + ColumnNumber + " ASC";

        /// <summary>数值排序方式</summary>
        public const string OrderValueSql =
            " ORDER BY " + ColumnCamp + " DESC," + ColumnRace + " ASC," + ColumnCost + " DESC," + ColumnPower + " DESC," +
            ColumnJName + " DESC";

        public const string ColumnType = "Type";
        public const string ColumnCamp = "Camp";
        public const string ColumnRace = "Race";
        public const string ColumnSign = "Sign";
        public const string ColumnRare = "Rare";
        public const string ColumnPack = "Pack";
        public const string ColumnRestrict = "Restrict";
        public const string ColumnCName = "CName";
        public const string ColumnJName = "JName";
        public const string ColumnIllust = "Illust";
        public const string ColumnNumber = "Number";
        public const string ColumnCost = "Cost";
        public const string ColumnPower = "Power";
        public const string ColumnLimit = "Restrict";
        public const string ColumnLines = "Lines";
        public const string ColumnFaq = "Faq";

        public const string ColumnAbility = "Ability";
        public const string ColumnAbilityDetail = "AbilityDetail";

        public const string ColumnCard =
            " (" + ColumnType + "," + ColumnCamp + "," + ColumnRace + "," + ColumnSign + "," + ColumnRare + "," +
            ColumnPack + "," + ColumnRestrict + ","
            + ColumnCName + "," + ColumnJName + "," + ColumnIllust + "," + ColumnNumber + "," + ColumnCost + "," +
            ColumnPower + "," + ColumnAbility + "," +
            ColumnLines + "," + ColumnFaq + "," + ColumnAbilityDetail + ")";

        public static string[] ColumKeyArray =
        {
            ColumnCName,
            ColumnType,
            ColumnCamp,
            ColumnRace,
            ColumnSign,
            ColumnRare,
            ColumnIllust,
            ColumnNumber,
            ColumnCost,
            ColumnPower,
            ColumnAbility
        };

        public static string DatabasePath = $"Data Source='{Const.RootPath + DatabaseName}'";
    }
}