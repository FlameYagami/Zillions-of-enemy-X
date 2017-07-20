using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using Wrapper.Constant;
using Wrapper.Entity;
using Enum = CardEditor.Constant.Enum;

namespace Wrapper.Utils
{
    public class CardUtils : SqliteConst
    {
        /// <summary>
        ///     获取排序的枚举类型
        /// </summary>
        /// <param name="order">排序方式</param>
        /// <returns></returns>
        public static Enum.PreviewOrderType GetPreOrderType(string order)
        {
            return order.Equals(StringConst.OrderNumber)
                ? Enum.PreviewOrderType.Number
                : Enum.PreviewOrderType.Value;
        }

        /// <summary>
        ///     获取模式的枚举类型
        /// </summary>
        /// <param name="mode">模式</param>
        /// <returns></returns>
        public static Enum.ModeType GetModeType(string mode)
        {
            return mode.Equals(StringConst.ModeQuery)
                ? Enum.ModeType.Query
                : mode.Equals(StringConst.ModeEditor) ? Enum.ModeType.Editor : Enum.ModeType.Develop;
        }

        public static CardEntity GetCardEntity(string number)
        {
            var row = DataCache.DsAllCache.Tables[TableName].Rows
                .Cast<DataRow>()
                .AsParallel()
                .First(column => column[ColumnNumber].Equals(number));
            return new CardEntity
            {
                Md5 = row[ColumnMd5].ToString(),
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
                AbilityDetailJson = row[ColumnAbilityDetail].ToString(),
                ImageJson = row[ColumnImage].ToString(),
                Restrict = RestrictUtils.GetRestrict(row[ColumnMd5].ToString()).ToString()
            };
        }

        /// <summary>
        ///     获取卡编相关的大图路径集合
        /// </summary>
        /// <param name="imageJson">图片Json</param>
        /// <returns></returns>
        public static List<string> GetPicturePathList(string imageJson)
        {
            var imageExList = JsonUtils.JsonDeserialize<List<string>>(imageJson);
            return imageExList.AsParallel()
                .Select(imageEx => PathManager.PicturePath + imageEx)
                .ToList();
        }

        /// <summary>
        ///     获取卡编是否重复的布尔值
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns></returns>
        public static bool IsNumberExist(string number)
        {
            return
                DataCache.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
                    .Any(row => row[ColumnNumber].ToString().Equals(number));
        }

        /// <summary>
        /// 获取卡包集合
        /// </summary>
        /// <returns></returns>
        public static List<object> GetAllPack()
        {
            var packlist = new List<object> { StringConst.NotApplicable };
            packlist.AddRange(GetPartPack("B"));
            packlist.AddRange(GetPartPack("C"));
            packlist.AddRange(GetPartPack("CP"));
            packlist.AddRange(GetPartPack("E"));
            packlist.AddRange(GetPartPack("P"));
            packlist.AddRange(GetPartPack("L"));
            packlist.AddRange(GetPartPack("M"));
            packlist.AddRange(GetPartPack("I"));
            packlist.AddRange(GetPartPack("V"));
            return packlist;
        }

        /// <summary>
        /// 获取部分卡包集合
        /// </summary>
        /// <param name="packType">卡包系列</param>
        /// <returns></returns>
        private static IEnumerable<object> GetPartPack(string packType)
        {
            var packlist = new List<object> {packType + StringConst.Series};
            var tempList = DataCache.DsAllCache.Tables[TableName].AsEnumerable()
                .Select(column => column[ColumnPack])
                .Distinct()
                .Where(value => value.ToString().Contains(packType) && StringUtils.IsNumber(value.ToString().Substring(packType.Length, 1)))
                .OrderBy(value => value)
                .ToList();
            packlist.AddRange(tempList);
            return packlist;
        }

        /// <summary>
        /// 获取部分种族集合
        /// </summary>
        /// <param name="camp">阵营</param>
        /// <returns></returns>
        public static List<object> GetPartRace(string camp)
        {
            var packlist = new List<object> { StringConst.NotApplicable };
            if (camp.Equals(StringConst.NotApplicable))
            {
                return packlist;
            }
            var tempList = (from row in DataCache.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
                            where row[ColumnCamp].Equals(camp)
                            select row[ColumnRace])
                .ToList()
                .Distinct()
                .OrderBy(value => value.ToString().Length)
                .ToList();
            packlist.AddRange(tempList);
            return packlist;
        }

        /// <summary>
        /// 获取卡编
        /// </summary>
        /// <param name="pack">卡包</param>
        /// <returns></returns>
        public static string GetPackNumber(string pack)
        {
            if (pack.Contains(StringConst.Series))
                return pack.Substring(0, pack.IndexOf(StringConst.Series, StringComparison.Ordinal));
            if (pack.Contains("CP"))
                return pack.Substring(0,4) + StringConst.Hyphen;
            if (pack.Length >= 3 && !pack.Contains(StringConst.NotApplicable))
                return pack.Substring(0, 3) + StringConst.Hyphen;
            return string.Empty;
        }

        /// <summary>
        ///     获取卡编相关卡编集合
        /// </summary>
        /// <param name="imageJson">图片Json</param>
        /// <returns></returns>
        public static List<string> GetNumberExList(string imageJson)
        {
            var imageExList = JsonUtils.JsonDeserialize<List<string>>(imageJson);
            return imageExList.AsParallel()
                .Select(imageEx => imageEx.Replace("/", "").Replace(StringConst.ImageExtension, ""))
                .ToList();
        }

        /// <summary>
        ///     获取卡编相关的小图路径集合
        /// </summary>
        /// <param name="imageJson">图片Json</param>
        /// <returns></returns>
        public static List<string> GetThumbnailPathList(string imageJson)
        {
            var imageExList = JsonUtils.JsonDeserialize<List<string>>(imageJson);
            return imageExList.AsParallel()
                .Select(
                    imageEx =>
                        File.Exists(PathManager.ThumbnailPath + imageEx)
                            ? PathManager.ThumbnailPath + imageEx
                            : PathManager.ThumbnailUnknownPath)
                .ToList();
        }

        /// <summary>
        ///     获取限制的图标路径
        /// </summary>
        /// <param name="restrict">制限数量</param>
        /// <returns></returns>
        public static string GetRestrictPath(int restrict)
        {
            foreach (var item in Dictionary.ImgRestrictPathDic)
                if (restrict.Equals(item.Key))
                    return item.Value;
            return string.Empty;
        }

        /// <summary>
        /// 获取画师集合
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        public static List<object> GetIllust()
        {
            var packlist = new List<object> { StringConst.NotApplicable };
            var tempList = DataCache.DsAllCache.Tables[TableName].AsEnumerable().AsParallel()
                .Select(column => column[ColumnIllust])
                .Distinct()
                .OrderBy(value => value.ToString().Length)
                .ToList();
            packlist.AddRange(tempList);
            return packlist;
        }

        /// <summary>
        ///     获取标记的图片地址
        /// </summary>
        /// <param name="sign">标记类型</param>
        /// <returns></returns>
        public static string GetSignPath(string sign)
        {
            var signUri = string.Empty;
            foreach (var signItem in Dictionary.ImgSignPathDic)
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
                foreach (var campItem in Dictionary.ImgCampPathDic)
                    if (campItem.Key.Equals(tempcamp))
                    {
                        campUriList.Add(campItem.Value);
                        break;
                    }
            while (campUriList.Count < 5)
                campUriList.Add(string.Empty);
            return campUriList;
        }

        /// <summary>
        ///     获取卡片进入的区域
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static Enum.AreaType GetAreaType(string number)
        {
            var row = DataCache.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
                .First(tempRow => number.Contains(tempRow[ColumnNumber].ToString()));
            if (StringConst.SignIg.Equals(row[ColumnSign].ToString()))
                return Enum.AreaType.Ig;
            if (StringConst.TypeZxEx.Equals(row[ColumnType].ToString()))
                return Enum.AreaType.Ex;
            if (StringConst.TypePlayer.Equals(row[ColumnType].ToString()))
                return Enum.AreaType.Pl;
            return Enum.AreaType.Ug;
        }

        /// <summary>
        ///     获取卡片的最大数量
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns></returns>
        public static int GetMaxCount(string number)
        {
            var row = DataCache.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
                .First(tempRow => number.Contains(tempRow[ColumnNumber].ToString()));
            return RestrictUtils.GetRestrict(row[ColumnMd5].ToString());
        }

        /// <summary>
        ///     判断卡片是否为起始卡
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns>Ture|Flase</returns>
        public static bool IsStart(string number)
        {
            var row = DataCache.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
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
            var row = DataCache.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
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
            var row = DataCache.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
                .First(tempRow => number.Contains(tempRow[ColumnNumber].ToString()));
            return row[ColumnAbility].ToString().Contains(StringConst.AbilityVoid);
        }

        /// <summary>
        ///     获取卡片在点燃区的枚举类型
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns>Life|Void|Normal</returns>
        public static Enum.IgType GetIgType(string number)
        {
            var ability = DataCache.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
                .First(tempRow => number.Contains(tempRow[ColumnNumber].ToString()))[ColumnAbility].ToString();
            if (ability.Contains(StringConst.AbilityLife)) return Enum.IgType.Life;
            if (ability.Contains(StringConst.AbilityVoid)) return Enum.IgType.Void;
            return Enum.IgType.Normal;
        }

        /// <summary>
        ///     获取卡组中生命恢复和虚空使者总数的集合
        /// </summary>
        /// <returns></returns>
        public static List<int> GetStartAndLifeAndVoidCount()
        {
            var list = new List<int>
            {
                DataCache.UgColl.AsParallel().Count(deckEntity => IsStart(deckEntity.NumberEx)),
                DataCache.IgColl.AsParallel().Count(deckEntity => IsLife(deckEntity.NumberEx)),
                DataCache.IgColl.AsParallel().Count(deckEntity => IsVoid(deckEntity.NumberEx))
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
            return PathManager.DeckFolderPath + deckName + StringConst.DeckExtension;
        }

        /// <summary>
        ///     获取缩略图路径
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns>缩略图路径</returns>
        public static string GetThumbnailPath(string number)
        {
            return PathManager.ThumbnailPath + number + StringConst.ImageExtension;
        }

        /// <summary>
        ///     获取卡名
        /// </summary>
        /// <param name="number"></param>
        /// <returns>卡名</returns>
        public static string GetName(string number)
        {
            var row = DataCache.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
                .First(tempRow => number.Contains(tempRow[ColumnNumber].ToString()));
            return row[ColumnCName].ToString();
        }
    }
}