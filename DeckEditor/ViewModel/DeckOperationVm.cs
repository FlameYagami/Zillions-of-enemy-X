using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using DeckEditor.Utils;
using Dialog;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;

namespace DeckEditor.ViewModel
{
    public class DeckOperationVm : BaseModel
    {
        private readonly DeckExVm _deckExVm;
        private readonly DeckStatsVm _deckStatsVm;
        private readonly DeckVm _deckVm;
        private readonly PlayerVm _playerVm;

        private string _deckName;

        public DeckOperationVm(DeckExVm deckExVm, PlayerVm playerVm, DeckStatsVm deckStatsVm)
        {
            _deckVm = deckExVm.GetDeckVm();
            _playerVm = playerVm;
            _deckExVm = deckExVm;
            _deckStatsVm = deckStatsVm;

            DeckName = string.Empty;
            DeckNameList = new ObservableCollection<string>();

            CmdSave = new DelegateCommand {ExecuteCommand = Save_Click};
            CmdResave = new DelegateCommand {ExecuteCommand = Resave_Click};
            CmdClear = new DelegateCommand {ExecuteCommand = Clear_Click};
            CmdDelete = new DelegateCommand {ExecuteCommand = Delete_Click};
            CmdDeckStats = new DelegateCommand {ExecuteCommand = DeckStats_Click};
        }

        public string DeckName
        {
            get { return _deckName; }
            set
            {
                _deckName = value;
                OnPropertyChanged(nameof(DeckName));
            }
        }

        public DelegateCommand CmdSave { get; set; }
        public DelegateCommand CmdResave { get; set; }
        public DelegateCommand CmdClear { get; set; }
        public DelegateCommand CmdDelete { get; set; }
        public DelegateCommand CmdDeckStats { get; set; }
        public ObservableCollection<string> DeckNameList { get; set; }

        /// <summary>
        ///     卡组删除事件
        /// </summary>
        public async void Delete_Click(object obj)
        {
            if (DeckName.Equals(string.Empty)) return;
            if (!await BaseDialogUtils.ShowDialogConfirm(StringConst.DeleteHint)) return;
            var deckPath = CardUtils.GetDeckPath(DeckName);
            if (!File.Exists(deckPath)) return;
            File.Delete(deckPath);
            ClearDeck();
            DeckName = string.Empty;
            BaseDialogUtils.ShowDialogAuto(StringConst.DeleteSucceed);
        }

        /// <summary>
        ///     卡组清空事件
        /// </summary>
        public void Clear_Click(object obj)
        {
            ClearDeck();
        }

        /// <summary>
        ///     卡组另存事件
        /// </summary>
        public void Resave_Click(object obj)
        {
            var deckPath = CardUtils.GetDeckPath(DeckName);
            if (!File.Exists(deckPath)) Save();
            BaseDialogUtils.ShowDialogAuto(StringConst.DeckNameExist);
        }

        /// <summary>
        ///     卡组存储事件
        /// </summary>
        public void Save_Click(object obj)
        {
            Save();
        }

        /// <summary>
        ///     卡组清空
        /// </summary>
        public void ClearDeck()
        {
            _playerVm.PlayerModels.Clear();
            _deckVm.IgModels.Clear();
            _deckVm.UgModels.Clear();
            _deckVm.ExModels.Clear();
            _deckExVm.Clear();
            UpdateDeckStatsView();
        }

        /// <summary>
        ///     卡组载入
        /// </summary>
        public void LoadDeck()
        {
            ClearDeck();
            var deckPath = CardUtils.GetDeckPath(DeckName);
            if (!File.Exists(deckPath)) return;
            var numberListString = FileUtils.GetFileContent(deckPath);
            var numberExList = JsonUtils.Deserialize<List<string>>(numberListString);
            foreach (var numberEx in numberExList)
                AddCard(numberEx);
        }

        public void DeckStats_Click(object obj)
        {
            var dekcStatisticalDic = new Dictionary<int, int>();
            var costIgList = _deckVm.IgModels.Select(deckEntity => deckEntity.Cost);
            var costUgList = _deckVm.UgModels.Select(deckEntity => deckEntity.Cost);
            var costDeckList = new List<int>();
            costDeckList.AddRange(costIgList);
            costDeckList.AddRange(costUgList);
            var costMax = costDeckList.Max();
            for (var i = 0; i != costMax + 1; i++)
                dekcStatisticalDic.Add(i + 1, costDeckList.Count(cost => cost.Equals(i + 1)));
            DialogUtils.ShowDekcStatistical(dekcStatisticalDic);
        }

        public void UpdateDeckNameList()
        {
            DeckNameList.Clear();
            var deckFolder = new DirectoryInfo(PathManager.DeckFolderPath);
            var deckFiles = deckFolder.GetFiles(); //遍历文件
            deckFiles
                .Where(deckFile => StringConst.DeckExtension.Equals(deckFile.Extension))
                .Select(deckName => Path.GetFileNameWithoutExtension(deckName.FullName))
                .ToList()
                .ForEach(DeckNameList.Add);
        }

        private void Save()
        {
            if (DeckName.Equals(string.Empty))
            {
                BaseDialogUtils.ShowDialogOk(StringConst.DeckNameNone);
                return;
            }
            var deckPath = CardUtils.GetDeckPath(DeckName);
            var deckBuilder = new StringBuilder();
            var deckNumberList = new List<string>();
            deckNumberList.AddRange(_playerVm.PlayerModels.Select(deckEntity => deckEntity.NumberEx).ToList());
            deckNumberList.AddRange(_deckVm.IgModels.Select(deckEntity => deckEntity.NumberEx).ToList());
            deckNumberList.AddRange(_deckVm.UgModels.Select(deckEntity => deckEntity.NumberEx).ToList());
            deckNumberList.AddRange(_deckVm.ExModels.Select(deckEntity => deckEntity.NumberEx).ToList());
            deckBuilder.Append(JsonUtils.Serializer(deckNumberList));
            var isSave = FileUtils.SaveFile(deckPath, deckBuilder.ToString());
            BaseDialogUtils.ShowDialogAuto(isSave ? StringConst.SaveSucceed : StringConst.SaveFailed);
        }

        /// <summary>
        ///     获取卡组中生命恢复和虚空使者总数的集合
        /// </summary>
        /// <returns></returns>
        public void UpdateDeckStatsView()
        {
            var startCount = _deckVm.UgModels.AsParallel().Count(deckEntity => CardUtils.IsStart(deckEntity.NumberEx));
            var lifeCount = _deckVm.IgModels.AsParallel().Count(deckEntity => CardUtils.IsLife(deckEntity.NumberEx));
            var voidCount = _deckVm.IgModels.AsParallel().Count(deckEntity => CardUtils.IsVoid(deckEntity.NumberEx));
            _deckStatsVm.UpdateView(_deckVm.IgModels.Count, _deckVm.UgModels.Count, _deckVm.ExModels.Count, startCount,
                lifeCount, voidCount);
        }

        /// <summary>
        ///     添加卡片到组卡区
        /// </summary>
        /// <param name="numberEx">卡编</param>
        public void AddCard(string numberEx)
        {
            var areaType = CardUtils.GetAreaType(numberEx);
            switch (areaType)
            {
                case Enums.AreaType.Pl:
                    _playerVm.PlayerModels.Clear();
                    AddDeckModel(numberEx, _playerVm.PlayerModels);
                    break;
                case Enums.AreaType.Ig:
                    if (CheckAreaIg(numberEx))
                    {
                        AddDeckModel(numberEx, _deckVm.IgModels);
                        _deckExVm.UpdateDeckExModels(areaType);
                    }
                    break;
                case Enums.AreaType.Ug:
                    if (CheckAreaUg(numberEx))
                    {
                        AddDeckModel(numberEx, _deckVm.UgModels);
                        _deckExVm.UpdateDeckExModels(areaType);
                    }
                    break;
                case Enums.AreaType.Ex:
                    if (CheckAreaEx(numberEx))
                    {
                        AddDeckModel(numberEx, _deckVm.ExModels);
                        _deckExVm.UpdateDeckExModels(areaType);
                    }
                    break;
            }
        }

        /// <summary>
        ///     从组卡区删除卡牌
        /// </summary>
        /// <param name="numberEx"></param>
        public void DeleteCard(string numberEx)
        {
            var areaType = CardUtils.GetAreaType(numberEx);
            var deckModelList = GetDeckModelList(areaType);
            var deckModel = deckModelList.AsParallel()
                .First(model => model.NumberEx.Equals(numberEx));
            deckModelList.Remove(deckModel);
            _deckExVm.UpdateDeckExModels(areaType);
        }

        private void AddDeckModel(string numberEx, ObservableCollection<DeckModel> deckModelList)
        {
            var thumbnailPath = CardUtils.GetThumbnailPath(numberEx);
            var row = DataCache.DsAllCache.Tables[SqliteConst.TableName].Rows.Cast<DataRow>()
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
            deckModelList.Add(deckModel);
        }

        private ObservableCollection<DeckModel> GetDeckModelList(Enums.AreaType areaType)
        {
            switch (areaType)
            {
                case Enums.AreaType.Pl:
                    return _playerVm.PlayerModels;
                case Enums.AreaType.Ig:
                    return _deckVm.IgModels;
                case Enums.AreaType.Ug:
                    return _deckVm.UgModels;
                case Enums.AreaType.Ex:
                    return _deckVm.ExModels;
            }
            return new ObservableCollection<DeckModel>();
        }

        /// <summary>
        ///     返回卡编是否具有添加到额外区域的权限
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns>true|false</returns>
        private bool CheckAreaEx(string number)
        {
            var name = CardUtils.GetName(number);
            return (_deckVm.ExModels.AsParallel().Count(deckEntity => name.Equals(deckEntity.CName)) <
                    CardUtils.GetMaxCount(number)) && (_deckVm.ExModels.Count < 10);
        }

        /// <summary>
        ///     返回卡编是否具有添加到非点燃区域的权限
        /// </summary>
        /// <param name="number">卡编</param>
        /// <returns>true|false</returns>
        private bool CheckAreaUg(string number)
        {
            var name = CardUtils.GetName(number);
            return (_deckVm.UgModels.AsParallel().Count(deckEntity => name.Equals(deckEntity.CName)) <
                    CardUtils.GetMaxCount(number)) && (_deckVm.UgModels.Count < 30);
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
            var canAdd = (_deckVm.IgModels.AsParallel().Count(deckEntity => name.Equals(deckEntity.CName)) <
                          CardUtils.GetMaxCount(number)) && (_deckVm.IgModels.Count < 20);
            switch (igType)
            {
                case Enums.IgType.Life:
                    canAdd = canAdd &&
                             (_deckVm.IgModels.AsParallel().Count(deckEntity => CardUtils.IsLife(deckEntity.NumberEx)) <
                              4);
                    break;
                case Enums.IgType.Void:
                    canAdd = canAdd &&
                             (_deckVm.IgModels.AsParallel().Count(deckEntity => CardUtils.IsVoid(deckEntity.NumberEx)) <
                              4);
                    break;
                case Enums.IgType.Normal:
                    break;
            }
            return canAdd;
        }
    }
}