using System.Collections.Generic;
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
        public static CardEntity GetCardModel(string number)
        {
            var row = DataCache.DsAllCache.Tables[CardTable].Rows
                .Cast<DataRow>()
                .First(column => column[Number].Equals(number));
            return new CardEntity
            {
                Type = row[Type].ToString(),
                Camp = row[Camp].ToString(),
                Race = row[Race].ToString(),
                Sign = row[Sign].ToString(),
                Rare = row[Rare].ToString(),
                CName = row[CName].ToString(),
                JName = row[JName].ToString(),
                Illust = row[Illust].ToString(),
                Pack = row[Pack].ToString(),
                Cost = row[Cost].ToString(),
                Power = row[Power].ToString(),
                Number = row[Number].ToString(),
                Ability = row[Ability].ToString(),
                Lines = row[Lines].ToString(),
                Faq = row[Faq].ToString(),
                Restrict = row[Restrict].ToString(),
                AbilityDetail = row[AbilityDetail].ToString()
            };
        }

        public static AbilityDetialEntity GetAbilityDetialModel(ListBox listBox)
        {
            var abilityDetialEntity = new AbilityDetialEntity();
            abilityDetialEntity.SetAbilityDetailDic(listBox.Items.Cast<CheckBox>());
            return abilityDetialEntity;
        }

        public static List<string> GetImageUriList(string number)
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
                DataCache.DsAllCache.Tables[CardTable].Rows.Cast<DataRow>()
                    .Any(row => row[Number].ToString().Equals(number));
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
            var tempList = DataCache.DsAllCache.Tables[CardTable].AsEnumerable()
                .Select(column => column[Pack])
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
            var tempList =
                (from row in DataCache.DsAllCache.Tables[CardTable].Rows.Cast<DataRow>()
                        where row[Camp].Equals(camp)
                        select row[Race])
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