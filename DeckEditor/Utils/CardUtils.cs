using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using DeckEditor.Constant;
using DeckEditor.Entity;
using DeckEditor.Model;

namespace DeckEditor.Utils
{
    internal class CardUtils : SqliteConst
    {
        public static CardEntity GetCardModel(string number)
        {
            var row = DataCache.DsAllCache.Tables[TableCard].Rows.AsParallel()
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

        public static AbilityDetialEntity GetAbilityDetialModel(ListBox listBox)
        {
            var abilityDetialModel = new AbilityDetialEntity();
            abilityDetialModel.SetAbilityDetailDic(listBox.Items.Cast<CheckBox>());
            return abilityDetialModel;
        }

        /// <summary>
        ///     获取卡编相关的大图路径集合
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns>../B01-001.JPG && ../B01-001SP.JPG</returns>
        public static List<string> GetPicturePathList(string number)
        {
            var theFolder = new DirectoryInfo(Const.PicturePath);
            var fileInfo = theFolder.GetFiles();
            return
                (from nextFile in fileInfo.AsParallel() where nextFile.Name.Contains(number) select nextFile.FullName)
                    .OrderBy(path => path.Length).ToList();
        }

        /// <summary>
        ///     获取卡编相关卡编集合
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns></returns>
        public static List<string> GetNumberList(string number)
        {
            var theFolder = new DirectoryInfo(Const.PicturePath);
            var fileInfo = theFolder.GetFiles();
            return
                (from nextFile in fileInfo.AsParallel() where nextFile.Name.Contains(number) select nextFile.Name)
                    .OrderBy(path => path.Length).ToList();
        }

        /// <summary>
        ///     获取小图路径集合
        /// </summary>
        /// <returns></returns>
        public static List<string> GetThumbnailFilePathList()
        {
            var theFolder = new DirectoryInfo(Const.ThumbnailPath);
            return theFolder.GetFiles().AsParallel().Select(file => file.FullName).ToList();
        }

        /// <summary>
        ///     获取卡编相关的小图路径集合
        /// </summary>
        /// <param name="number">卡编</param>
        /// <param name="thumbnailFilePathList">小图路径集合</param>
        /// <returns></returns>
        public static List<string> GetThumbnailPathList(string number, List<string> thumbnailFilePathList)
        {
            var tempList = thumbnailFilePathList.AsParallel()
                .Where(nextInfoPath => nextInfoPath.Contains(number))
                .OrderBy(path => path.Length)
                .ToList();
            if (tempList.Count.Equals(0))
                tempList.Add(Const.ThumbnailUnknownPath);
            return tempList;
        }

        /// <summary>
        ///     获取限制的图标路径
        /// </summary>
        /// <param name="limit">数量</param>
        /// <returns></returns>
        public static string GetRestrictPath(string limit)
        {
            foreach (var item in Const.ImgRestrictPathDic)
                if (limit.Equals(item.Key))
                    return item.Value;
            return string.Empty;
        }

        /// <summary>
        ///     获取卡编是否重复的布尔值
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns></returns>
        public static bool IsNumberExist(string number)
        {
            return
                DataCache.DsAllCache.Tables[TableCard].Rows.Cast<DataRow>().AsParallel()
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

        public static List<object> GetIllust()
        {
            var packlist = new List<object> {StringConst.NotApplicable};
            var tempList = DataCache.DsAllCache.Tables[TableCard].AsEnumerable().AsParallel()
                .Select(column => column[ColumnIllust])
                .Distinct()
                .OrderBy(value => value.ToString().Length)
                .ToList();
            packlist.AddRange(tempList);
            return packlist;
        }

        private static IEnumerable<object> GetPartPack(string packType)
        {
            var packlist = new List<object> {packType + StringConst.Series};
            var tempList = DataCache.DsAllCache.Tables[TableCard].AsEnumerable().AsParallel()
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
            var tempList = (from row in DataCache.DsAllCache.Tables[TableCard].Rows.Cast<DataRow>().AsParallel()
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

        /// <summary>
        ///     获取标记的图片地址
        /// </summary>
        /// <param name="sign">标记类型</param>
        /// <returns></returns>
        public static string GetSignPath(string sign)
        {
            var signUri = string.Empty;
            foreach (var signItem in Const.ImgSignPathDic)
                if (signItem.Key.Equals(sign))
                {
                    signUri = signItem.Value;
                    break;
                }
            return signUri;
        }

        /// <summary>
        ///     获取阵营的图片地址
        /// </summary>
        /// <param name="camp">阵营类型</param>
        /// <returns></returns>
        public static List<string> GetCampPathList(string camp)
        {
            var campUriList = new List<string>();
            var campArray = camp.Split('/');
            foreach (var tempcamp in campArray)
                foreach (var campItem in Const.ImgCampPathDic)
                    if (campItem.Key.Equals(tempcamp))
                    {
                        campUriList.Add(campItem.Value);
                        break;
                    }
            if (!camp.Contains('/')) campUriList.Add(string.Empty);
            return campUriList;
        }

        /// <summary>
        ///     获取卡片进入的区域
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static StringConst.AreaType GetAreaType(string number)
        {
            var row = DataCache.DsAllCache.Tables[TableCard].Rows.Cast<DataRow>().AsParallel()
                .First(tempRow => number.Contains(tempRow[ColumnNumber].ToString()));
            if (StringConst.SignIg.Equals(row[ColumnSign].ToString()))
                return StringConst.AreaType.Ig;
            if (StringConst.TypeZxEx.Equals(row[ColumnType].ToString()))
                return StringConst.AreaType.Ex;
            if (StringConst.TypePlayer.Equals(row[ColumnType].ToString()))
                return StringConst.AreaType.Pl;
            return StringConst.AreaType.Ug;
        }

        /// <summary>
        ///     获取卡片的最大数量
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns></returns>
        public static int GetMaxCount(string number)
        {
            var row = DataCache.DsAllCache.Tables[TableCard].Rows.Cast<DataRow>().AsParallel()
                .First(tempRow => number.Contains(tempRow[ColumnNumber].ToString()));
            return row[ColumnRestrict].ToString().Equals(string.Empty) ? 4 : int.Parse(row[ColumnRestrict].ToString());
        }

        /// <summary>
        ///     判断卡片是否为起始卡
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns>Ture|Flase</returns>
        public static bool IsStart(string number)
        {
            var row = DataCache.DsAllCache.Tables[TableCard].Rows.Cast<DataRow>().AsParallel()
                .First(tempRow => number.Contains(tempRow[ColumnNumber].ToString()));
            return row[ColumnAbility].ToString().Contains(StringConst.AbilityStart);
        }

        /// <summary>
        ///     判断卡片是否为生命恢复
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns>Ture|Flase</returns>
        public static bool IsLife(string number)
        {
            var row = DataCache.DsAllCache.Tables[TableCard].Rows.Cast<DataRow>().AsParallel()
                .First(tempRow => number.Contains(tempRow[ColumnNumber].ToString()));
            return row[ColumnAbility].ToString().Contains(StringConst.AbilityLife);
        }

        /// <summary>
        ///     判断卡片是否为虚空使者
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns>Ture|Flase</returns>
        public static bool IsVoid(string number)
        {
            var row = DataCache.DsAllCache.Tables[TableCard].Rows.Cast<DataRow>().AsParallel()
                .First(tempRow => number.Contains(tempRow[ColumnNumber].ToString()));
            return row[ColumnAbility].ToString().Contains(StringConst.AbilityVoid);
        }

        /// <summary>
        ///     获取卡片在点燃区的枚举类型
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns>Life|Void|Normal</returns>
        public static StringConst.IgType GetIgType(string number)
        {
            var ability = DataCache.DsAllCache.Tables[TableCard].Rows.Cast<DataRow>().AsParallel()
                .First(tempRow => number.Contains(tempRow[ColumnNumber].ToString()))[ColumnAbility].ToString();
            if (ability.Contains(StringConst.AbilityLife)) return StringConst.IgType.Life;
            if (ability.Contains(StringConst.AbilityVoid)) return StringConst.IgType.Void;
            return StringConst.IgType.Normal;
        }

        /// <summary>
        ///     获取卡组中生命恢复和虚空使者总数的集合
        /// </summary>
        /// <returns></returns>
        public static List<int> GetStartAndLifeAndVoidCount()
        {
            var list = new List<int>
            {
                DataCache.UgColl.AsParallel().Count(deckEntity => IsStart(deckEntity.Number)),
                DataCache.IgColl.AsParallel().Count(deckEntity => IsLife(deckEntity.Number)),
                DataCache.IgColl.AsParallel().Count(deckEntity => IsVoid(deckEntity.Number))
            };
            return list;
        }

        /// <summary>
        ///     获取卡组路径
        /// </summary>
        /// <param name="deckName">卡组名称</param>
        /// <returns>卡组路径</returns>
        public static string GetDeckPath(string deckName)
        {
            return Const.DeckFolderPath + deckName + StringConst.DeckExtension;
        }

        public static string GetThumbnailPath(string number)
        {
            return Const.ThumbnailPath + number + StringConst.ImageExtension;
        }
    }
}