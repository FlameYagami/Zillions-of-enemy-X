using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using DeckEditor.Constant;
using DeckEditor.Entity;
using DeckEditor.Utils;

namespace DeckEditor.Model
{
    internal interface IDeck
    {
        string GetAddThumbnailPath(string number, int selectIndex);
        StringConst.AreaType AddCard(StringConst.AreaType areaType, string number, string thumbnailPath);
        void DeleteEntityFromColl(string number, ICollection<DeckEntity> collection);
        void Order(StringConst.DeckOrderType value);
        bool Save(string deckName);
        bool Delete(string deckName);
        bool Resave(string deckName);
        void Load(string deckName);
        List<string> GetDeckNameList();
        Dictionary<int,int> DekcStatistical();
    }

    internal class Deck : SqliteConst, IDeck
    {
        /// <summary>
        ///     添加卡片到卡组
        /// </summary>
        public StringConst.AreaType AddCard(StringConst.AreaType areaType, string number, string thumbnailPath)
        {
            switch (areaType)
            {
                case StringConst.AreaType.Pl:
                    DataCache.PlColl.Clear();
                    AddEntityToColl(number, thumbnailPath, DataCache.PlColl);
                    return StringConst.AreaType.Pl;
                case StringConst.AreaType.Ig:
                    if (CheckAreaIg(number))
                    {
                        AddEntityToColl(number, thumbnailPath, DataCache.IgColl);
                        return StringConst.AreaType.Ig;
                    }
                    break;
                case StringConst.AreaType.Ug:
                    if (CheckAreaUg(number))
                    {
                        AddEntityToColl(number, thumbnailPath, DataCache.UgColl);
                        return StringConst.AreaType.Ug;
                    }
                    break;
                case StringConst.AreaType.Ex:
                    if (CheckAreaEx(number))
                    {
                        AddEntityToColl(number, thumbnailPath, DataCache.ExColl);
                        return StringConst.AreaType.Ex;
                    }
                    break;
            }
            return StringConst.AreaType.None;
        }

        public void DeleteEntityFromColl(string number, ICollection<DeckEntity> collection)
        {
            var deckEntity = collection.AsParallel()
                .First(tempDeckEntity => tempDeckEntity.Number.Equals(number));
            collection.Remove(deckEntity);
        }

        public void Order(StringConst.DeckOrderType value)
        {
            switch (value)
            {
                case StringConst.DeckOrderType.Value:
                    Value(DataCache.IgColl);
                    Value(DataCache.UgColl);
                    Value(DataCache.ExColl);
                    break;
                case StringConst.DeckOrderType.Random:
                    Random(DataCache.IgColl);
                    Random(DataCache.UgColl);
                    Random(DataCache.ExColl);
                    break;
            }
        }

        /// <summary>
        ///     获取TcImg选中项中的缩略图路径
        /// </summary>
        /// <param name="number">卡编</param>
        /// <param name="selectIndex">TcImg选中索引</param>
        /// <returns>缩略图路径</returns>
        public string GetAddThumbnailPath(string number, int selectIndex)
        {
            var thumbnailFilePathList = CardUtils.GetThumbnailFilePathList();
            return CardUtils.GetThumbnailPathList(number, thumbnailFilePathList)[selectIndex];
        }

        public bool Save(string deckName)
        {
            if (deckName.Equals(string.Empty))
            {
                DialogUtils.ShowDlg(StringConst.DeckNameNone);
                return false;
            }
            var deckPath = CardUtils.GetDeckPath(deckName);
            var fs = new FileStream(deckPath, FileMode.Create);
            var sw = new StreamWriter(fs);
            var deckBuilder = new StringBuilder();
            var deckNumberList = new List<string>();
            deckNumberList.AddRange(DataCache.PlColl.Select(deckEntity => deckEntity.Number).ToList());
            deckNumberList.AddRange(DataCache.IgColl.Select(deckEntity => deckEntity.Number).ToList());
            deckNumberList.AddRange(DataCache.UgColl.Select(deckEntity => deckEntity.Number).ToList());
            deckNumberList.AddRange(DataCache.ExColl.Select(deckEntity => deckEntity.Number).ToList());
            deckBuilder.Append(JsonUtils.JsonSerializer(deckNumberList));
            sw.Write(deckBuilder.ToString());
            sw.Close();
            fs.Close();
            fs.Dispose();
            return true;
        }

        public bool Delete(string deckName)
        {
            if (deckName.Equals(string.Empty)) return false;
            if (!DialogUtils.ShowDlgOkCancel(StringConst.DeleteHint)) return false;
            var deckPath = CardUtils.GetDeckPath(deckName);
            if (!File.Exists(deckPath)) return false;
            File.Delete(deckPath);
            return true;
        }

        public bool Resave(string deckName)
        {
            if (deckName.Equals(string.Empty)) return false;
            var deckPath = CardUtils.GetDeckPath(deckName);
            if (!File.Exists(deckPath)) return Save(deckName);
            DialogUtils.ShowDlg(StringConst.DeckNameExist);
            return false;
        }

        public void Load(string deckName)
        {
            var deckPath = CardUtils.GetDeckPath(deckName);
            try
            {
                var sr = File.OpenText(deckPath);
                var numberListString = sr.ReadToEnd().Trim();
                sr.Close();
                var numberList = JsonUtils.JsonDeserialize<List<string>>(numberListString);
                foreach (var number in numberList)
                {
                    var areaType = CardUtils.GetAreaType(number);
                    var thumbnailPath = CardUtils.GetThumbnailPath(number);
                    if (!File.Exists(thumbnailPath)) continue;
                    AddCard(areaType, number, thumbnailPath);
                }
            }
            catch (Exception exception)
            {
                DialogUtils.ShowDlg(exception.Message);
            }
        }

        public List<string> GetDeckNameList()
        {
            var deckFolder = new DirectoryInfo(Const.DeckFolderPath);
            var deckFiles = deckFolder.GetFiles(); //遍历文件
            return deckFiles
                .Where(deckFile => StringConst.DeckExtension.Equals(deckFile.Extension))
                .Select(deckName => Path.GetFileNameWithoutExtension(deckName.FullName))
                .ToList();
        }

        public Dictionary<int, int> DekcStatistical()
        {
            var dekcStatisticalDic = new Dictionary<int, int>();
            var costIgList = DataCache.IgColl.Select(deckEntity => int.Parse(deckEntity.Cost));
            var costUgList = DataCache.UgColl.Select(deckEntity => int.Parse(deckEntity.Cost));
            var costDeckList = new List<int>();
            costDeckList.AddRange(costIgList);
            costDeckList.AddRange(costUgList);
            var costMax = costDeckList.Max();
            for (var i = 0; i != costMax + 1; i++)
            {
                dekcStatisticalDic.Add(i + 1, costDeckList.Count(cost => cost.Equals(i + 1)));
            }
            return dekcStatisticalDic;
        }

        private static void AddEntityToColl(string number, string thumbnailPath, ICollection<DeckEntity> collection)
        {
            var row = DataCache.DsAllCache.Tables[TableCard].Rows.Cast<DataRow>().AsParallel()
                .First(tempRow => number.Contains(tempRow[ColumnNumber].ToString()));
            var camp = row[ColumnCamp].ToString();
            var cost = row[ColumnCost].ToString();
            var power = row[ColumnPower].ToString();
            var limit = row[ColumnLimit].ToString();
            var restrictPath = CardUtils.GetRestrictPath(limit);
            var deckEntity = new DeckEntity
            {
                Camp = camp,
                Cost = cost.Equals(string.Empty) ? StringConst.Hyphen : cost,
                Power = power.Equals(string.Empty) ? StringConst.Hyphen : power,
                Number = number,
                ImagePath = thumbnailPath,
                Restrict = restrictPath
            };
            collection.Add(deckEntity);
        }

        private static void Value(List<DeckEntity> deckEntityList)
        {
            var tempDeckEntityList = deckEntityList.OrderBy(tempDeckEntity => tempDeckEntity.Camp)
                .OrderByDescending(tempDeckEntity => tempDeckEntity.Cost)
                .OrderByDescending(tempDeckEntity => tempDeckEntity.Power)
                .ThenBy(tempDeckEntity => tempDeckEntity.Number)
                .ToList();
            deckEntityList.Clear();
            deckEntityList.AddRange(tempDeckEntityList);
        }

        private static void Random(List<DeckEntity> deckEntityList)
        {
            var tempEntityList = new List<DeckEntity>();
            var random = new Random();
            deckEntityList.ForEach(deckEntity => tempEntityList.Insert(random.Next(tempEntityList.Count + 1), deckEntity));
            deckEntityList.Clear();
            deckEntityList.AddRange(tempEntityList);
        }

        /// <summary>
        ///     返回卡编是否具有添加到额外区域的权限
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns>true|false</returns>
        private static bool CheckAreaEx(string number)
        {
            return (DataCache.ExColl.AsParallel().Count(deckEntity => number.Contains(deckEntity.Number)) <
                    CardUtils.GetMaxCount(number)) && (DataCache.ExColl.Count < 10);
        }

        /// <summary>
        ///     返回卡编是否具有添加到非点燃区域的权限
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns>true|false</returns>
        private static bool CheckAreaUg(string number)
        {
            return (DataCache.UgColl.AsParallel().Count(deckEntity => number.Contains(deckEntity.Number)) <
                    CardUtils.GetMaxCount(number)) && (DataCache.UgColl.Count < 30);
        }

        /// <summary>
        ///     返回卡编是否具有添加到点燃区域的权限
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns>true|false</returns>
        private static bool CheckAreaIg(string number)
        {
            // 根据卡编获取卡片在点燃区的枚举类型
            var igType = CardUtils.GetIgType(number);
            // 判断卡片是否超出自身添加数量以及点燃区总数量
            var canAdd = (DataCache.IgColl.AsParallel().Count(deckEntity => number.Contains(deckEntity.Number)) <
                          CardUtils.GetMaxCount(number)) && (DataCache.IgColl.Count < 20);
            switch (igType)
            {
                case StringConst.IgType.Life:
                    canAdd = canAdd &&
                             (DataCache.IgColl.AsParallel().Count(deckEntity => CardUtils.IsLife(deckEntity.Number)) < 4);
                    break;
                case StringConst.IgType.Void:
                    canAdd = canAdd &&
                             (DataCache.IgColl.AsParallel().Count(deckEntity => CardUtils.IsVoid(deckEntity.Number)) < 4);
                    break;
                case StringConst.IgType.Normal:
                    break;
            }
            return canAdd;
        }
    }
}