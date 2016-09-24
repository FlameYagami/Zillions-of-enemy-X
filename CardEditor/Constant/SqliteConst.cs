namespace CardEditor.Constant
{
    public class SqliteConst
    {
        public const string DatabaseName = "Data.db";
        public const string DatabasePassword = "DatabasePassword";
        public const string TableName = "TableCard";
        public const string QueryAllSql = "SELECT * FROM " + TableName + " ORDER BY " + Number + " ASC";
        public const string QueryBaseSql = "SELECT * FROM " + TableName + " WHERE 1=1";

        /// <summary>卡编排序方式</summary>
        public const string NumberOrderSql = " ORDER BY " + Number + " ASC";

        /// <summary>数值排序方式</summary>
        public const string ValueOrderSql =
            " ORDER BY " + Camp + " DESC," + Race + " ASC," + Cost + " DESC," + Power + " DESC," + JName + " DESC";

        public const string Type = "Type";
        public const string Camp = "Camp";
        public const string Race = "Race";
        public const string Sign = "Sign";
        public const string Rare = "Rare";
        public const string Pack = "Pack";
        public const string Restrict = "Restrict";
        public const string CName = "CName";
        public const string JName = "JName";
        public const string Illust = "Illust";
        public const string Number = "Number";
        public const string Cost = "Cost";
        public const string Power = "Power";
        public const string Lines = "Lines";
        public const string Faq = "Faq";

        public const string Ability = "Ability";
        public const string AbilityDetail = "AbilityDetail";

        public const string ColumnCard =
            " (" + Type + "," + Camp + "," + Race + "," + Sign + "," + Rare + "," + Pack + "," + Restrict + ","
            + CName + "," + JName + "," + Illust + "," + Number + "," + Cost + "," + Power + "," + Ability + "," +
            Lines + "," + Faq + "," + AbilityDetail + ")";

        public static string DatabasePath = $"Data Source='{Const.RootPath + DatabaseName}'";
    }
}