﻿namespace CardEditor.Constant
{
    public class SqliteConst
    {
        public const string TableName = "TableCard";

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
        public const string ColumnLines = "Lines";
        public const string ColumnFaq = "Faq";
        public const string ColumnAbility = "Ability";
        public const string ColumnAbilityDetail = "AbilityDetail";

        public const string ColumnCard = " (" +
                                         ColumnType + "," +
                                         ColumnCamp + "," +
                                         ColumnRace + "," +
                                         ColumnSign + "," +
                                         ColumnRare + "," +
                                         ColumnPack + "," +
                                         ColumnRestrict + "," +
                                         ColumnCName + "," +
                                         ColumnJName + "," +
                                         ColumnIllust + "," +
                                         ColumnNumber + "," +
                                         ColumnCost + "," +
                                         ColumnPower + "," +
                                         ColumnAbility + "," +
                                         ColumnLines + "," +
                                         ColumnFaq + "," +
                                         ColumnAbilityDetail + ")";
    }
}