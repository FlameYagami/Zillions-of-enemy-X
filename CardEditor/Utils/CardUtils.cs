﻿using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using CardEditor.Constant;
using CardEditor.Entity;
using CardEditor.Model;

namespace CardEditor.Utils
{
    public class CardUtils : SqliteConst
    {
        /// <summary>
        ///     获取排序的枚举类型
        /// </summary>
        /// <param name="order">排序方式</param>
        /// <returns></returns>
        public static StringConst.PreviewOrderType GetPreviewOrderType(string order)
        {
            return order.Equals(StringConst.OrderNumber)
                ? StringConst.PreviewOrderType.Number
                : StringConst.PreviewOrderType.Value;
        }

        /// <summary>
        ///     获取模式的枚举类型
        /// </summary>
        /// <param name="mode">模式</param>
        /// <returns></returns>
        public static StringConst.ModeType GetModeType(string mode)
        {
            return mode.Equals(StringConst.ModeQuery)
                ? StringConst.ModeType.Query
                : mode.Equals(StringConst.ModeEditor) ? StringConst.ModeType.Editor : StringConst.ModeType.Develop;
        }

        public static CardEntity GetCardEntity(string number)
        {
            var row = DataCache.DsAllCache.Tables[TableName].Rows
                .Cast<DataRow>()
                .First(column => column[ColumnNumber].Equals(number));
            return new CardEntity
            {
                Type = row[ColumnType].ToString(),
                Camp = row[ColumnCamp].ToString(),
                Race = row[ColumnRace].ToString(),
                Sign = row[ColumnSign].ToString(),
                Rare = row[ColumnRare].ToString(),
                CName = row[ColumnCName].ToString(),
                JName = row[ColumnJName].ToString(),
                Illust = row[ColumnIllust].ToString(),
                Pack = row[ColumnPack].ToString(),
                Cost = row[ColumnCost].ToString(),
                Power = row[ColumnPower].ToString(),
                Number = row[ColumnNumber].ToString(),
                Ability = row[ColumnAbility].ToString(),
                Lines = row[ColumnLines].ToString(),
                Faq = row[ColumnFaq].ToString(),
                Restrict = row[ColumnRestrict].ToString(),
                AbilityDetail = row[ColumnAbilityDetail].ToString()
            };
        }

        public static AbilityDetialEntity GetAbilityDetialEntity(ListBox listBox)
        {
            var abilityDetialEntity = new AbilityDetialEntity();
            abilityDetialEntity.SetAbilityDetailDic(listBox.Items.Cast<CheckBox>());
            return abilityDetialEntity;
        }

        public static List<string> GetPicturePathList(string number)
        {
            var theFolder = new DirectoryInfo(Const.PicturePath);
            var fileInfo = theFolder.GetFiles();
            return
                (from nextFile in fileInfo where nextFile.Name.Contains(number) select nextFile.FullName).OrderBy(
                    path => path.Length).ToList();
        }

        public static bool IsNumberExist(string number)
        {
            return
                DataCache.DsAllCache.Tables[TableName].Rows.Cast<DataRow>()
                    .Any(row => row[ColumnNumber].ToString().Equals(number));
        }

        public static List<object> GetAllPack()
        {
            var packlist = new List<object> {StringConst.NotApplicable};
            packlist.AddRange(GetPartPack("B"));
            packlist.AddRange(GetPartPack("C"));
            packlist.AddRange(GetPartPack("E"));
            packlist.AddRange(GetPartPack("P"));
            packlist.AddRange(GetPartPack("L"));
            packlist.AddRange(GetPartPack("M"));
            packlist.AddRange(GetPartPack("I"));
            packlist.AddRange(GetPartPack("V"));
            return packlist;
        }

        private static IEnumerable<object> GetPartPack(string packType)
        {
            var packlist = new List<object> {packType + StringConst.Series};
            var tempList = DataCache.DsAllCache.Tables[TableName].AsEnumerable()
                .Select(column => column[ColumnPack])
                .Distinct()
                .Where(value => value.ToString().Contains(packType))
                .OrderBy(value => value)
                .ToList();
            packlist.AddRange(tempList);
            return packlist;
        }

        public static List<object> GetPartRace(string camp)
        {
            var packlist = new List<object> {StringConst.NotApplicable};
            if (camp.Equals(StringConst.NotApplicable)) return packlist;
            var tempList =
                (from row in DataCache.DsAllCache.Tables[TableName].Rows.Cast<DataRow>()
                        where row[ColumnCamp].Equals(camp)
                        select row[ColumnRace])
                    .ToList()
                    .Distinct()
                    .OrderBy(value => value.ToString().Length)
                    .ToList();
            packlist.AddRange(tempList);
            return packlist;
        }

        public static string GetPackNumber(string pack)
        {
            if (pack.Contains(StringConst.NotApplicable))
                return string.Empty;
            if (pack.Contains(StringConst.Series))
                return pack.Substring(0, 1) + "XX-";
            return pack.Substring(0, 3) + "-";
        }
    }
}