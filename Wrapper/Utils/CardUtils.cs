using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Common;
using Wrapper.Constant;
using Wrapper.Model;

namespace Wrapper.Utils
{
    public class CardUtils : SqliteConst
    {
        /// <summary>
        ///     获取排序的枚举类型
        /// </summary>
        /// <param name="order">排序方式</param>
        /// <returns></returns>
        public static Enums.PreviewOrderType GetPreOrderType(string order)
        {
            return order.Equals(StringConst.OrderNumber)
                ? Enums.PreviewOrderType.Number
                : Enums.PreviewOrderType.Value;
        }

        /// <summary>
        ///     获取模式的枚举类型
        /// </summary>
        /// <param name="mode">模式</param>
        /// <returns></returns>
        public static Enums.ModeType GetModeType(string mode)
        {
            return mode.Equals(StringConst.ModeQuery)
                ? Enums.ModeType.Query
                : mode.Equals(StringConst.ModeEditor) ? Enums.ModeType.Editor : Enums.ModeType.Develop;
        }

        public static CardModel GetCardModel(string numberEx)
        {
            var row = DataManager.DsAllCache.Tables[TableName].Rows
                .Cast<DataRow>()
                .AsParallel()
                .First(column => numberEx.Contains(column[ColumnNumber].ToString()));
            var cost = row[ColumnCost].ToString().Equals(StringConst.CostValueNotApplicable)
                ? -1
                : int.Parse(row[ColumnCost].ToString());
            var power = row[ColumnPower].ToString().Equals(StringConst.PowerValueNotApplicable)
                ? -1
                : int.Parse(row[ColumnPower].ToString());
            var restrict = RestrictUtils.GetRestrict(row[ColumnMd5].ToString());
            return new CardModel
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
                Number = row[ColumnNumber].ToString(),
                Ability = row[ColumnAbility].ToString(),
                Lines = row[ColumnLines].ToString(),
                ImageJson = row[ColumnImage].ToString(),
                Cost = cost,
                Power = power,
                AbilityDetailJson = row[ColumnAbilityDetail].ToString(),
                Restrict = restrict
            };
        }

        public static List<CardPreviewModel> GetCardPreviewModels(DataSet dataSet)
        {
            return dataSet.Tables[TableName].Rows.Cast<DataRow>().Select(row =>
            {
                var md5 = row[ColumnMd5].ToString();
                var name = row[ColumnCName].ToString();
                var number = row[ColumnNumber].ToString();
                var cost = row[ColumnCost].ToString();
                var power = row[ColumnPower].ToString();
                var race = row[ColumnRace].ToString();
                var camp = row[ColumnCamp].ToString();
                var imageJson = row[ColumnImage].ToString();
                var imagePath = GetThumbnailPathList(imageJson)[0];
                var restrict = RestrictUtils.GetRestrict(md5);
                var restrictPath = RestrictUtils.GetRestrictPath(md5);

                cost = cost.Equals(StringConst.CostValueNotApplicable) ? StringConst.Hyphen : cost;
                power = power.Equals(StringConst.PowerValueNotApplicable) ? StringConst.Hyphen : power;
                race = race.Equals(string.Empty) ? StringConst.Hyphen : race;

                return new CardPreviewModel
                {
                    CName = name,
                    Number = number,
                    CostValue = cost,
                    PowerValue = power,
                    Camp = camp,
                    Race = race,
                    ImagePath = imagePath,
                    ImageJson = imageJson,
                    Restrict = restrict,
                    RestrictPath = restrictPath
                };
            }).ToList();
        }

        /// <summary>
        ///     获取卡编相关的大图路径集合
        /// </summary>
        /// <param name="imageJson">图片Json</param>
        /// <returns></returns>
        public static List<string> GetPicturePathList(string imageJson)
        {
            var imageExList = JsonUtils.Deserialize<List<string>>(imageJson);
            return imageExList.AsParallel()
                .Select(imageEx => PathManager.PicturePath + imageEx + StringConst.ImageExtension)
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
                DataManager.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
                    .Any(row => row[ColumnNumber].ToString().Equals(number));
        }

        /// <summary>
        ///     获取卡包集合
        /// </summary>
        /// <returns></returns>
        public static List<string> GetPackList()
        {
            var list = new List<string> {StringConst.NotApplicable};
            Dic.PackSeriesDic.Keys.ToList().ForEach(series => { list.AddRange(GetPartPack(series)); });
            return list;
        }

        /// <summary>
        ///     获取制限集合
        /// </summary>
        /// <returns></returns>
        public static List<string> GetRestrictList()
        {
            var list = new List<string> {StringConst.NotApplicable};
            Dic.ImgRestrictPathDic.Keys.ToList().ForEach(restrict => { list.Add(restrict.ToString()); });
            return list;
        }

        /// <summary>
        ///     获取部分卡包集合
        /// </summary>
        /// <param name="packType">卡包系列</param>
        /// <returns></returns>
        private static IEnumerable<string> GetPartPack(string packType)
        {
            var list = new List<string> {packType + StringConst.Series};
            var tempList = DataManager.DsAllCache.Tables[TableName].AsEnumerable()
                .Select(column => column[ColumnPack].ToString())
                .Distinct()
                .Where(
                    value =>
                        value.ToString().Contains(packType) &&
                        StringUtils.IsNumber(value.ToString().Substring(packType.Length, 1)))
                .OrderBy(value => value)
                .ToList();
            list.AddRange(tempList);
            return list;
        }

        /// <summary>
        ///     获取部分种族集合
        /// </summary>
        /// <param name="camp">阵营</param>
        /// <returns></returns>
        public static List<string> GetPartRace(string camp)
        {
            var list = new List<string> {StringConst.NotApplicable};
            if (camp.Equals(StringConst.NotApplicable))
                return list;
            var tempList = (from row in DataManager.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
                    where row[ColumnCamp].Equals(camp)
                    select row[ColumnRace].ToString())
                .ToList()
                .Distinct()
                .OrderBy(value => value.ToString().Length)
                .ToList();
            list.AddRange(tempList);
            return list;
        }

        /// <summary>
        ///     获取卡编
        /// </summary>
        /// <param name="pack">卡包</param>
        /// <returns></returns>
        public static string GetPackNumber(string pack)
        {
            if (pack.Contains(StringConst.Series))
                return pack.Substring(0, pack.IndexOf(StringConst.Series, StringComparison.Ordinal));
            if (pack.Contains("AC") || pack.Contains("CP"))
                return pack.Substring(0, 4) + StringConst.Hyphen;
            if ((pack.Length >= 3) && !pack.Contains(StringConst.NotApplicable))
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
            return JsonUtils.Deserialize<List<string>>(imageJson);
        }

        /// <summary>
        ///     获取卡编相关的小图路径集合
        /// </summary>
        /// <param name="imageJson">图片Json</param>
        /// <returns></returns>
        public static List<string> GetThumbnailPathList(string imageJson)
        {
            var imageExList = JsonUtils.Deserialize<List<string>>(imageJson);
            return imageExList.AsParallel()
                .Select(
                    imageEx =>
                        File.Exists(PathManager.ThumbnailPath + imageEx + StringConst.ImageExtension)
                            ? PathManager.ThumbnailPath + imageEx + StringConst.ImageExtension
                            : PathManager.ThumbnailUnknownPath)
                .ToList();
        }

        /// <summary>
        ///     获取画师集合
        /// </summary>
        /// <returns></returns>
        public static List<string> GetIllustList()
        {
            var list = new List<string> {StringConst.NotApplicable};
            var tempList = DataManager.DsAllCache.Tables[TableName].AsEnumerable().AsParallel()
                .Select(column => column[ColumnIllust].ToString())
                .Distinct()
                .OrderBy(value => value.ToString().Length)
                .ToList();
            list.AddRange(tempList);
            return list;
        }

        /// <summary>
        ///     获取标记的图片地址
        /// </summary>
        /// <param name="sign">标记类型</param>
        /// <returns></returns>
        public static string GetSignPath(string sign)
        {
            var signUri = string.Empty;
            foreach (var signItem in Dic.ImgSignPathDic)
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
            var list = new List<string>();
            var campArray = camp.Split('/');
            foreach (var tempcamp in campArray)
                foreach (var campItem in Dic.ImgCampPathDic)
                    if (campItem.Key.Equals(tempcamp))
                    {
                        list.Add(campItem.Value);
                        break;
                    }
            while (list.Count < 5)
                list.Add(string.Empty);
            return list;
        }

        /// <summary>
        ///     获取卡片进入的区域
        /// </summary>
        /// <param name="numberEx">卡编</param>
        /// <returns></returns>
        public static Enums.AreaType GetAreaType(string numberEx)
        {
            var row = DataManager.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
                .FirstOrDefault(tempRow => numberEx.Contains(tempRow[ColumnNumber].ToString()));
            if (null == row) return Enums.AreaType.None;
            if (StringConst.SignIg.Equals(row[ColumnSign].ToString()))
                return Enums.AreaType.Ig;
            if (StringConst.TypeZxEx.Equals(row[ColumnType].ToString()))
                return Enums.AreaType.Ex;
            if (StringConst.TypePlayer.Equals(row[ColumnType].ToString()))
                return Enums.AreaType.Player;
            return Enums.AreaType.Ug;
        }

        /// <summary>
        ///     获取卡片的最大数量
        /// </summary>
        /// <param name="numberEx">卡编</param>
        /// <returns></returns>
        public static int GetMaxCount(string numberEx)
        {
            var row = DataManager.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
                .First(tempRow => numberEx.Contains(tempRow[ColumnNumber].ToString()));
            return RestrictUtils.GetRestrict(row[ColumnMd5].ToString());
        }

        /// <summary>
        ///     判断卡片是否为起始卡
        /// </summary>
        /// <param name="numberEx">卡编</param>
        /// <returns>Ture|Flase</returns>
        public static bool IsStart(string numberEx)
        {
            var row = DataManager.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
                .First(tempRow => numberEx.Contains(tempRow[ColumnNumber].ToString()));
            return row[ColumnAbility].ToString().Contains(StringConst.AbilityStart);
        }

        /// <summary>
        ///     判断卡片是否为生命恢复
        /// </summary>
        /// <param name="numberEx">卡编</param>
        /// <returns>Ture|Flase</returns>
        public static bool IsLife(string numberEx)
        {
            var row = DataManager.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
                .First(tempRow => numberEx.Contains(tempRow[ColumnNumber].ToString()));
            return row[ColumnAbility].ToString().Contains(StringConst.AbilityLife);
        }

        /// <summary>
        ///     判断卡片是否为虚空使者
        /// </summary>
        /// <param name="numberEx">卡编</param>
        /// <returns>Ture|Flase</returns>
        public static bool IsVoid(string numberEx)
        {
            var row = DataManager.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
                .First(tempRow => numberEx.Contains(tempRow[ColumnNumber].ToString()));
            return row[ColumnAbility].ToString().Contains(StringConst.AbilityVoid);
        }

        /// <summary>
        ///     获取卡片在点燃区的枚举类型
        /// </summary>
        /// <param name="numberEx">卡编</param>
        /// <returns>Life|Void|Normal</returns>
        public static Enums.IgType GetIgType(string numberEx)
        {
            var ability = DataManager.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
                .First(tempRow => numberEx.Contains(tempRow[ColumnNumber].ToString()))[ColumnAbility].ToString();
            if (ability.Contains(StringConst.AbilityLife)) return Enums.IgType.Life;
            if (ability.Contains(StringConst.AbilityVoid)) return Enums.IgType.Void;
            return Enums.IgType.Normal;
        }

        /// <summary>
        ///     获取卡片在非点燃区的枚举类型
        /// </summary>
        /// <param name="numberEx">卡编</param>
        /// <returns>Start|Normal</returns>
        public static Enums.UgType GetUgType(string numberEx)
        {
            return GetUgType(GetCardModel(numberEx));
        }

        /// <summary>
        ///     获取卡片在非点燃区的枚举类型
        /// </summary>
        /// <param name="cardModel">卡牌数据模型</param>
        /// <returns>Start|Normal</returns>
        public static Enums.UgType GetUgType(CardModel cardModel)
        {
            var ability = cardModel.Ability;
            return ability.StartsWith(StringConst.AbilityStart) ? Enums.UgType.Start : Enums.UgType.Normal;
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
        /// <param name="numberEx">卡编</param>
        /// <returns>缩略图路径</returns>
        public static string GetThumbnailPath(string numberEx)
        {
            return $"{PathManager.ThumbnailPath}{numberEx}{StringConst.ImageExtension}";
        }

        /// <summary>
        ///     获取大图路径
        /// </summary>
        /// <param name="numberEx">卡编</param>
        /// <returns>缩略图路径</returns>
        public static string GetPicturePath(string numberEx)
        {
            return $"{PathManager.PicturePath}{numberEx}{StringConst.ImageExtension}";
        }

        /// <summary>
        ///     获取卡名
        /// </summary>
        /// <param name="number"></param>
        /// <returns>卡名</returns>
        public static string GetName(string number)
        {
            var row = DataManager.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
                .First(tempRow => number.Contains(tempRow[ColumnNumber].ToString()));
            return row[ColumnCName].ToString();
        }

        /// <summary>
        ///     获取卡密
        /// </summary>
        /// <param name="number"></param>
        /// <returns>卡名</returns>
        public static string GetMd5(string number)
        {
            var row = DataManager.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
                .First(tempRow => number.Contains(tempRow[ColumnNumber].ToString()));
            return row[ColumnMd5].ToString();
        }

        /// <summary>
        ///     获取主卡组的数量
        /// </summary>
        /// <param name="cardModels">卡牌信息集合</param>
        /// <returns>数量</returns>
        public static int GetMainCount(IEnumerable<CardModel> cardModels)
        {
            return
                cardModels.Count(
                    card =>
                        GetAreaType(card.Number).Equals(Enums.AreaType.Ig) ||
                        GetAreaType(card.Number).Equals(Enums.AreaType.Ug));
        }

        /// <summary>
        ///     获取额外卡组的数量
        /// </summary>
        /// <param name="cardModels">卡牌信息集合</param>
        /// <returns>数量</returns>
        public static int GetExtraCount(IEnumerable<CardModel> cardModels)
        {
            if (cardModels == null) throw new ArgumentNullException(nameof(cardModels));
            return cardModels.Count(card => GetAreaType(card.Number).Equals(Enums.AreaType.Ex));
        }

        /// <summary>
        ///     获取卡牌信息集合
        /// </summary>
        /// <param name="numberList">卡编集合</param>
        /// <returns>卡牌信息集合</returns>
        public static List<CardModel> GetCardModels(List<string> numberList)
        {
            return numberList.Select(GetCardModel).ToList();
        }

        /// <summary>
        ///     获取玩家卡牌路径
        /// </summary>
        /// <param name="cardModel">卡牌信息集合</param>
        /// <returns>玩家卡牌路径</returns>
        public static string GetPlayerPath(List<CardModel> cardModel)
        {
            var tempModel = cardModel.FirstOrDefault(model => GetAreaType(model.Number).Equals(Enums.AreaType.Player));
            return null == tempModel ? PathManager.PictureUnknownPath : GetPicturePath(tempModel.Number);
        }

        /// <summary>
        ///     获取玩家卡牌路径
        /// </summary>
        /// <param name="cardModel">卡牌信息集合</param>
        /// <returns>玩家卡牌路径</returns>
        public static string GetStartPath(List<CardModel> cardModel)
        {
            var tempModel = cardModel.FirstOrDefault(model => IsStart(model.Number));
            return null == tempModel ? PathManager.PictureUnknownPath : GetPicturePath(tempModel.Number);
        }

        /// <summary>
        ///     获取数据库中卡牌全部编号
        /// </summary>
        /// <returns>卡牌编号集合</returns>
        public static List<string> GetAllNumberList()
        {
            return DataManager.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
                .Select(row => row[SqliteConst.ColumnNumber].ToString())
                .ToList();
        }
    }
}