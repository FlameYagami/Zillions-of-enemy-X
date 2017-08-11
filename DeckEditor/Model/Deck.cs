using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Dialog;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;
using Enum = Wrapper.Constant.Enum;

namespace DeckEditor.Model
{
    internal interface IDeck
    {
        Enum.AreaType AddCard(Enum.AreaType areaType, string number, string thumbnailPath);
        void DeleteCard(string number, Enum.AreaType areaType);
        void Order(Enum.DeckOrderType value);
        bool Save(string deckName);
        bool Delete(string deckName);
        bool Resave(string deckName);
        void Load(string deckName);
        void ClearDeck();
        List<string> GetDeckNameList();
        Dictionary<int, int> DekcStatistical();
        List<DeckModel> GetDeckModelList(Enum.AreaType areaType);
        List<int> GetStartAndLifeAndVoidCount();
    }

    internal class Deck : SqliteConst, IDeck
    {
        public Deck()
        {
            PlayerList = new List<DeckModel>();
            IgList = new List<DeckModel>();
            UgList = new List<DeckModel>();
            ExList = new List<DeckModel>();
        }

        /// <summary>玩家数据缓存</summary>
        private List<DeckModel> PlayerList { get; }

        /// <summary>点燃数据缓存</summary>
        private List<DeckModel> IgList { get; }

        /// <summary>非点燃数据缓存</summary>
        private List<DeckModel> UgList { get; }

        /// <summary>额外数据缓存</summary>
        private List<DeckModel> ExList { get; }

        public void ClearDeck()
        {
            PlayerList.Clear();
            IgList.Clear();
            UgList.Clear();
            ExList.Clear();
        }

        public List<DeckModel> GetDeckModelList(Enum.AreaType areaType)
        {
            var tempDeckModelList = new List<DeckModel>();
            switch (areaType)
            {
                case Enum.AreaType.Pl:
                    tempDeckModelList.AddRange(PlayerList);
                    break;
                case Enum.AreaType.Ig:
                    tempDeckModelList.AddRange(IgList);
                    break;
                case Enum.AreaType.Ug:
                    tempDeckModelList.AddRange(UgList);
                    break;
                case Enum.AreaType.Ex:
                    tempDeckModelList.AddRange(ExList);
                    break;
            }
            return tempDeckModelList;
        }

        /// <summary>
        ///     添加卡片到组卡区
        /// </summary>
        /// <param name="areaType">卡片添加区域枚举类型</param>
        /// <param name="number">卡编</param>
        /// <param name="thumbnailPath">缩略图路径</param>
        /// <returns></returns>
        public Enum.AreaType AddCard(Enum.AreaType areaType, string number, string thumbnailPath)
        {
            switch (areaType)
            {
                case Enum.AreaType.Pl:
                    PlayerList.Clear();
                    AddDeckModel(number, thumbnailPath, PlayerList);
                    return Enum.AreaType.Pl;
                case Enum.AreaType.Ig:
                    if (CheckAreaIg(number))
                    {
                        AddDeckModel(number, thumbnailPath, IgList);
                        return Enum.AreaType.Ig;
                    }
                    break;
                case Enum.AreaType.Ug:
                    if (CheckAreaUg(number))
                    {
                        AddDeckModel(number, thumbnailPath, UgList);
                        return Enum.AreaType.Ug;
                    }
                    break;
                case Enum.AreaType.Ex:
                    if (CheckAreaEx(number))
                    {
                        AddDeckModel(number, thumbnailPath, ExList);
                        return Enum.AreaType.Ex;
                    }
                    break;
            }
            return Enum.AreaType.None;
        }

        public void DeleteCard(string numberEx, Enum.AreaType areaType)
        {
            var deckModelList = GetDeckModelList(areaType);
            var deckEntity = deckModelList.AsParallel()
                .First(tempDeckEntity => tempDeckEntity.NumberEx.Equals(numberEx));
            deckModelList.Remove(deckEntity);
        }

        public void Order(Enum.DeckOrderType value)
        {
            switch (value)
            {
                case Enum.DeckOrderType.Value:
                    Value(IgList);
                    Value(UgList);
                    Value(ExList);
                    break;
                case Enum.DeckOrderType.Random:
                    Random(IgList);
                    Random(UgList);
                    Random(ExList);
                    break;
            }
        }

        public bool Save(string deckName)
        {
            if (deckName.Equals(string.Empty))
            {
                BaseDialogUtils.ShowDlg(StringConst.DeckNameNone);
                return false;
            }
            var deckPath = CardUtils.GetDeckPath(deckName);
            var deckBuilder = new StringBuilder();
            var deckNumberList = new List<string>();
            deckNumberList.AddRange(PlayerList.Select(deckEntity => deckEntity.NumberEx).ToList());
            deckNumberList.AddRange(IgList.Select(deckEntity => deckEntity.NumberEx).ToList());
            deckNumberList.AddRange(UgList.Select(deckEntity => deckEntity.NumberEx).ToList());
            deckNumberList.AddRange(ExList.Select(deckEntity => deckEntity.NumberEx).ToList());
            deckBuilder.Append(JsonUtils.JsonSerializer(deckNumberList));
            return FileUtils.SaveFile(deckPath, deckBuilder.ToString());
        }

        public bool Delete(string deckName)
        {
            if (deckName.Equals(string.Empty)) return false;
            if (!BaseDialogUtils.ShowDlgOkCancel(StringConst.DeleteHint)) return false;
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
            BaseDialogUtils.ShowDlg(StringConst.DeckNameExist);
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
                BaseDialogUtils.ShowDlg(exception.Message);
            }
        }

        public List<string> GetDeckNameList()
        {
            var deckFolder = new DirectoryInfo(PathManager.DeckFolderPath);
            var deckFiles = deckFolder.GetFiles(); //遍历文件
            return deckFiles
                .Where(deckFile => StringConst.DeckExtension.Equals(deckFile.Extension))
                .Select(deckName => Path.GetFileNameWithoutExtension(deckName.FullName))
                .ToList();
        }

        public Dictionary<int, int> DekcStatistical()
        {
            var dekcStatisticalDic = new Dictionary<int, int>();
            var costIgList = IgList.Select(deckEntity => deckEntity.Cost);
            var costUgList = UgList.Select(deckEntity => deckEntity.Cost);
            var costDeckList = new List<int>();
            costDeckList.AddRange(costIgList);
            costDeckList.AddRange(costUgList);
            var costMax = costDeckList.Max();
            for (var i = 0; i != costMax + 1; i++)
                dekcStatisticalDic.Add(i + 1, costDeckList.Count(cost => cost.Equals(i + 1)));
            return dekcStatisticalDic;
        }

        /// <summary>
        ///     获取卡组中生命恢复和虚空使者总数的集合
        /// </summary>
        /// <returns></returns>
        public List<int> GetStartAndLifeAndVoidCount()
        {
            var list = new List<int>
            {
                UgList.AsParallel().Count(deckEntity => CardUtils.IsStart(deckEntity.NumberEx)),
                IgList.AsParallel().Count(deckEntity => CardUtils.IsLife(deckEntity.NumberEx)),
                IgList.AsParallel().Count(deckEntity => CardUtils.IsVoid(deckEntity.NumberEx))
            };
            return list;
        }

        private static void AddDeckModel(string numberEx, string thumbnailPath, ICollection<DeckModel> collection)
        {
            var row = DataCache.DsAllCache.Tables[TableName].Rows.Cast<DataRow>().AsParallel()
                .First(tempRow => numberEx.Contains(tempRow[ColumnNumber].ToString()));
            var md5 = row[ColumnMd5].ToString();
            var name = row[ColumnCName].ToString();
            var camp = row[ColumnCamp].ToString();
            var cost = row[ColumnCost].ToString();
            var power = row[ColumnPower].ToString();
            var restrict = RestrictUtils.GetRestrict(md5);
            var restrictPath = CardUtils.GetRestrictPath(restrict);
            var deckEntity = new DeckModel
            {
                Camp = camp,
                Cost = cost.Equals(string.Empty) ? 0 : int.Parse(cost),
                Power = power.Equals(string.Empty) ? 0 : int.Parse(power),
                NumberEx = numberEx,
                CName = name,
                ImagePath = thumbnailPath,
                RestrictPath = restrictPath
            };
            collection.Add(deckEntity);
        }

        private static void Value(List<DeckModel> deckEntityList)
        {
            var tempDeckEntityList = deckEntityList
                .OrderBy(tempDeckEntity => tempDeckEntity.Camp)
                .ThenByDescending(tempDeckEntity => tempDeckEntity.Cost)
                .ThenByDescending(tempDeckEntity => tempDeckEntity.Power)
                .ThenBy(tempDeckEntity => tempDeckEntity.NumberEx)
                .ToList();
            deckEntityList.Clear();
            deckEntityList.AddRange(tempDeckEntityList);
        }

        private static void Random(List<DeckModel> deckEntityList)
        {
            var tempEntityList = new List<DeckModel>();
            var random = new Random();
            deckEntityList.ForEach(
                deckEntity => tempEntityList.Insert(random.Next(tempEntityList.Count + 1), deckEntity));
            deckEntityList.Clear();
            deckEntityList.AddRange(tempEntityList);
        }

        /// <summary>
        ///     返回卡编是否具有添加到额外区域的权限
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns>true|false</returns>
        private bool CheckAreaEx(string number)
        {
            var name = CardUtils.GetName(number);
            return (ExList.AsParallel().Count(deckEntity => name.Equals(deckEntity.CName)) <
                    CardUtils.GetMaxCount(number)) && (ExList.Count < 10);
        }

        /// <summary>
        ///     返回卡编是否具有添加到非点燃区域的权限
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns>true|false</returns>
        private bool CheckAreaUg(string number)
        {
            var name = CardUtils.GetName(number);
            return (UgList.AsParallel().Count(deckEntity => name.Equals(deckEntity.CName)) <
                    CardUtils.GetMaxCount(number)) && (UgList.Count < 30);
        }

        /// <summary>
        ///     返回卡编是否具有添加到点燃区域的权限
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns>true|false</returns>
        private bool CheckAreaIg(string number)
        {
            var name = CardUtils.GetName(number);
            // 根据卡编获取卡片在点燃区的枚举类型
            var igType = CardUtils.GetIgType(number);
            // 判断卡片是否超出自身添加数量以及点燃区总数量
            var canAdd = (IgList.AsParallel().Count(deckEntity => name.Equals(deckEntity.CName)) <
                          CardUtils.GetMaxCount(number)) && (IgList.Count < 20);
            switch (igType)
            {
                case Enum.IgType.Life:
                    canAdd = canAdd &&
                             (IgList.AsParallel().Count(deckEntity => CardUtils.IsLife(deckEntity.NumberEx)) <
                              4);
                    break;
                case Enum.IgType.Void:
                    canAdd = canAdd &&
                             (IgList.AsParallel().Count(deckEntity => CardUtils.IsVoid(deckEntity.NumberEx)) <
                              4);
                    break;
                case Enum.IgType.Normal:
                    break;
            }
            return canAdd;
        }
    }
}