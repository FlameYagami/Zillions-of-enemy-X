using System.Collections.Generic;
using System.Data;
using System.Linq;
using DeckEditor.Model;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;

namespace DeckEditor
{
    public class DeckManager
    {
        /// <summary>
        ///     初始化
        /// </summary>
        public DeckManager()
        {
            DeckName = string.Empty;
            Md5List = new List<string>();
            NumberExList = new List<string>();
            PlayerModels = new List<DeckModel>();
            StartModels = new List<DeckModel>();
            IgModels = new List<DeckModel>();
            UgModels = new List<DeckModel>();
            ExModels = new List<DeckModel>();
            IgExModels = new List<DeckExModel>();
            UgExModels = new List<DeckExModel>();
            ExExModels = new List<DeckExModel>();
            DeckStatsModel = new DeckStatsModel();
        }

        // 卡组名称
        public string DeckName { get; set; }

        // 卡组Md5集合
        public List<string> Md5List { get; set; }

        // 卡组扩展名集合
        public List<string> NumberExList { get; set; }

        // 玩家卡数据缓存
        public List<DeckModel> PlayerModels { get; set; }

        // 起始卡缓存
        public List<DeckModel> StartModels { get; set; }

        // 点燃数据缓存
        public List<DeckModel> IgModels { get; set; }

        // 非点燃数据缓存
        public List<DeckModel> UgModels { get; set; }

        // 额外数据缓存
        public List<DeckModel> ExModels { get; set; }

        // 点燃数据缓存
        public List<DeckExModel> IgExModels { get; set; }

        // 非点燃数据缓存
        public List<DeckExModel> UgExModels { get; set; }

        // 额外数据缓存
        public List<DeckExModel> ExExModels { get; set; }

        public DeckStatsModel DeckStatsModel { get; set; }

        /// <summary>
        ///     初始化
        /// </summary>
        /// <param name="deckName">卡组名称</param>
        /// <param name="numberExList">编号扩展集合(B01-001A、B01-001B)</param>
        public void Load(string deckName, List<string> numberExList)
        {
            DeckName = deckName;
            NumberExList = numberExList;
            ClearDeck();
            LoadDeck();
        }

        /// <summary>
        ///     添加卡片到组卡区
        /// </summary>
        /// <param name="numberEx">卡编</param>
        /// <returns>枚举类型</returns>
        public Enums.AreaType AddCard(string numberEx)
        {
            var deckModel = GetDeckModel(numberEx);
            var areaType = AddCard(deckModel);
            if (areaType.Equals(Enums.AreaType.None)) return areaType;
            SortDeckExModels(areaType);
            UpdateDeckExModels(areaType);
            UpdateDeckStatsModel();
            return areaType;
        }

        /// <summary>
        ///     添加卡片到组卡区
        /// </summary>
        /// <param name="deckModel">卡牌数据模型</param>
        /// <returns>枚举类型</returns>
        public Enums.AreaType AddCard(DeckModel deckModel)
        {
            var areaType = CardUtils.GetAreaType(deckModel.NumberEx);
            if (!CheckArea(areaType, deckModel)) return Enums.AreaType.None;

            Md5List.Add(CardUtils.GetMd5(deckModel.NumberEx));
            switch (areaType)
            {
                case Enums.AreaType.Ig:
                    IgModels.Add(deckModel);
                    break;
                case Enums.AreaType.Ug:
                    UgModels.Add(deckModel);
                    if (CardUtils.GetUgType(deckModel.NumberEx).Equals(Enums.UgType.Start))
                        StartModels.Add(deckModel);
                    break;
                case Enums.AreaType.Ex:
                    ExModels.Add(deckModel);
                    break;
                case Enums.AreaType.Player:
                    PlayerModels.Clear();
                    PlayerModels.Add(deckModel);
                    break;
            }
            return areaType;
        }

        /// <summary>
        ///     从组卡区删除卡牌
        /// </summary>
        /// <param name="numberEx">卡编</param>
        /// <returns>枚举类型</returns>
        public Enums.AreaType DeleteCard(string numberEx)
        {
            var areaType = CardUtils.GetAreaType(numberEx);
            Md5List.Remove(CardUtils.GetMd5(numberEx));
            switch (areaType)
            {
                case Enums.AreaType.Ig:
                    IgModels.Remove(IgModels.First(model => model.NumberEx.Equals(numberEx)));
                    break;
                case Enums.AreaType.Ug:
                    UgModels.Remove(UgModels.First(model => model.NumberEx.Equals(numberEx)));
                    if (CardUtils.GetUgType(numberEx).Equals(Enums.UgType.Start))
                        StartModels.Remove(StartModels.First(model => model.NumberEx.Equals(numberEx)));
                    break;
                case Enums.AreaType.Ex:
                    ExModels.Remove(ExModels.First(model => model.NumberEx.Equals(numberEx)));
                    break;
                case Enums.AreaType.Player:
                    PlayerModels.Clear();
                    break;
            }
            UpdateDeckExModels(areaType);
            UpdateDeckStatsModel();
            return areaType;
        }

        /// <summary>
        ///     卡组数据模型重置
        /// </summary>
        private void ClearDeck()
        {
            Md5List.Clear();
            PlayerModels.Clear();
            StartModels.Clear();
            IgModels.Clear();
            UgModels.Clear();
            ExModels.Clear();
            IgExModels.Clear();
            UgExModels.Clear();
            ExExModels.Clear();
            DeckStatsModel = new DeckStatsModel();
        }

        /// <summary>
        ///     获取卡组中生命恢复和虚空使者总数的集合
        /// </summary>
        /// <returns></returns>
        private void UpdateDeckStatsModel()
        {
            var startCount = UgModels.AsParallel().Count(deckModel => CardUtils.IsStart(deckModel.NumberEx));
            var lifeCount = IgModels.AsParallel().Count(deckModel => CardUtils.IsLife(deckModel.NumberEx));
            var voidCount = IgModels.AsParallel().Count(deckModel => CardUtils.IsVoid(deckModel.NumberEx));
            DeckStatsModel = new DeckStatsModel
            {
                StartCountValue = startCount.ToString(),
                LifeCountValue = lifeCount.ToString(),
                VoidCountValue = voidCount.ToString(),
                IgCountValue = IgModels.Count.ToString(),
                UgCountValue = UgModels.Count.ToString(),
                ExCountValue = ExModels.Count.ToString()
            };
        }

        private static List<DeckExModel> GetDeckExModels(IReadOnlyCollection<DeckModel> deckList)
        {
            // 对象去重
            var numberExList = deckList.Select(deck => deck.NumberEx).Distinct().ToList();
            var tempDeckList = new List<DeckModel>();
            tempDeckList.AddRange(
                numberExList.Select(numberEx => deckList.First(deck => deck.NumberEx.Equals(numberEx))).ToList());
            // 重新生成对象
            var tempdDeckExList = new List<DeckExModel>();
            tempdDeckExList.AddRange(tempDeckList
                .Select(deck => new DeckExModel
                {
                    DeckModel = deck,
                    Count = deckList.Count(temp => temp.NumberEx.Equals(deck.NumberEx))
                })
                .ToList());
            return tempdDeckExList;
        }

        private void LoadDeck()
        {
            NumberExList.Select(GetDeckModel).ToList().ForEach(deck => { AddCard(deck); });
            SortDeckExModels(Enums.AreaType.Ig);
            SortDeckExModels(Enums.AreaType.Ug);
            SortDeckExModels(Enums.AreaType.Ex);
            UpdateDeckExModels(Enums.AreaType.Ig);
            UpdateDeckExModels(Enums.AreaType.Ug);
            UpdateDeckExModels(Enums.AreaType.Ex);
            UpdateDeckStatsModel();
        }

        private void SortDeckExModels(Enums.AreaType areaType)
        {
            switch (areaType)
            {
                case Enums.AreaType.Ig:
                    IgModels = GetSortDeckModels(IgModels);
                    break;
                case Enums.AreaType.Ug:
                    UgModels = GetSortDeckModels(UgModels);
                    break;
                case Enums.AreaType.Ex:
                    ExModels = GetSortDeckModels(ExModels);
                    break;
            }
        }

        private void UpdateDeckExModels(Enums.AreaType areaType)
        {
            switch (areaType)
            {
                case Enums.AreaType.Ig:
                    IgExModels = GetDeckExModels(IgModels);
                    break;
                case Enums.AreaType.Ug:
                    UgExModels = GetDeckExModels(UgModels);
                    break;
                case Enums.AreaType.Ex:
                    ExExModels = GetDeckExModels(ExModels);
                    break;
            }
        }

        private static DeckModel GetDeckModel(string numberEx)
        {
            var thumbnailPath = CardUtils.GetThumbnailPath(numberEx);
            var row = DataManager.DsAllCache.Tables[SqliteConst.TableName].Rows.Cast<DataRow>()
                .AsEnumerable()
                .AsParallel()
                .First(tempRow => numberEx.Contains(tempRow[SqliteConst.ColumnNumber].ToString()));
            var md5 = row[SqliteConst.ColumnMd5].ToString();
            var name = row[SqliteConst.ColumnCName].ToString();
            var camp = row[SqliteConst.ColumnCamp].ToString();
            var cost = row[SqliteConst.ColumnCost].ToString();
            var power = row[SqliteConst.ColumnPower].ToString();
            var restrictPath = RestrictUtils.GetRestrictPath(md5);
            var deckModel = new DeckModel
            {
                NumberEx = numberEx,
                Camp = camp,
                CName = name,
                Cost = cost.Equals(string.Empty) || cost.Equals("0") ? 0 : int.Parse(cost),
                Power = power.Equals(string.Empty) || power.Equals("0") ? 0 : int.Parse(power),
                ImagePath = thumbnailPath,
                RestrictPath = restrictPath
            };
            return deckModel;
        }

        private static List<DeckModel> GetSortDeckModels(ICollection<DeckModel> deckModelList)
        {
            if (0 == deckModelList.Count) return new List<DeckModel>();
            return deckModelList
                .OrderBy(tempDeckEntity => tempDeckEntity.Camp)
                .ThenByDescending(tempDeckEntity => tempDeckEntity.Cost)
                .ThenByDescending(tempDeckEntity => tempDeckEntity.Power)
                .ThenBy(tempDeckEntity => tempDeckEntity.NumberEx)
                .ToList();
        }

        private bool CheckArea(Enums.AreaType areaType, DeckModel deckModel)
        {
            switch (areaType)
            {
                case Enums.AreaType.Ig:
                    return CheckAreaIg(deckModel.NumberEx);
                case Enums.AreaType.Ug:
                    return CheckAreaUg(deckModel.NumberEx);
                case Enums.AreaType.Ex:
                    return CheckAreaEx(deckModel.NumberEx);
                case Enums.AreaType.Player:
                    return true;
            }
            return false;
        }

        /// <summary>
        ///     返回卡编是否具有添加到额外区域的权限
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns>true|false</returns>
        private bool CheckAreaEx(string number)
        {
            var name = CardUtils.GetName(number);
            return (ExModels.AsParallel().Count(model => name.Equals(model.CName)) <
                    CardUtils.GetMaxCount(number)) && (ExModels.Count < 10);
        }

        /// <summary>
        ///     返回卡编是否具有添加到非点燃区域的权限
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns>true|false</returns>
        private bool CheckAreaUg(string number)
        {
            var name = CardUtils.GetName(number);
            return (UgModels.AsParallel().Count(model => name.Equals(model.CName)) <
                    CardUtils.GetMaxCount(number)) && (UgModels.Count < 30);
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
            var canAdd = (IgModels.AsParallel().Count(deckEntity => name.Equals(deckEntity.CName)) <
                          CardUtils.GetMaxCount(number)) && (IgModels.Count < 20);
            switch (igType)
            {
                case Enums.IgType.Life:
                    canAdd = canAdd &&
                             (IgModels.AsParallel().Count(model => CardUtils.IsLife(model.NumberEx)) < 4);
                    break;
                case Enums.IgType.Void:
                    canAdd = canAdd &&
                             (IgModels.AsParallel().Count(model => CardUtils.IsVoid(model.NumberEx)) < 4);
                    break;
                case Enums.IgType.Normal:
                    break;
            }
            return canAdd;
        }
    }
}