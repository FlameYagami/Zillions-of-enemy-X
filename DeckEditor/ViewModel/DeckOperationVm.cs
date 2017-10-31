using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using DeckEditor.Model;
using DeckEditor.View;
using Dialog;
using Dialog.View;
using MaterialDesignThemes.Wpf;
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
        private readonly PlayerVm _playerVm;

        private DeckManager _deckManager;
        private string _deckName;

        public DeckOperationVm(DeckExVm deckExVm, PlayerVm playerVm, DeckStatsVm deckStatsVm)
        {
            _playerVm = playerVm;
            _deckExVm = deckExVm;
            _deckStatsVm = deckStatsVm;
            _deckManager = new DeckManager();
            DeckName = _deckManager.DeckName;

            CmdRename = new DelegateCommand {ExecuteCommand = Rename_Click};
            CmdSave = new DelegateCommand {ExecuteCommand = Save_Click};
            CmdResave = new DelegateCommand {ExecuteCommand = Resave_Click};
            CmdClear = new DelegateCommand {ExecuteCommand = Clear_Click};
            CmdDelete = new DelegateCommand {ExecuteCommand = Delete_Click};
            CmdDeckStats = new DelegateCommand {ExecuteCommand = DeckStats_Click};
            CmdDeckPreview = new DelegateCommand {ExecuteCommand = DeckPreview_Click};
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

        public DelegateCommand CmdRename { get; set; }
        public DelegateCommand CmdSave { get; set; }
        public DelegateCommand CmdResave { get; set; }
        public DelegateCommand CmdClear { get; set; }
        public DelegateCommand CmdDelete { get; set; }
        public DelegateCommand CmdDeckStats { get; set; }
        public DelegateCommand CmdDeckPreview { get; set; }

        /// <summary>
        ///     卡组更名事件
        /// </summary>
        /// <param name="obj"></param>
        public async void Rename_Click(object obj)
        {
            var newDeckName = await BaseDialogUtils.ShowDialogEditor(DeckName, "请输入卡组名称");
            var newDeckPath = CardUtils.GetDeckPath(newDeckName);
            var canRename = DeckName.Equals(newDeckName) || !File.Exists(newDeckName);
            if (!canRename)
            {
                BaseDialogUtils.ShowDialogOk(StringConst.DeckNameExist);
                return;
            }

            var fileInfo = new FileInfo(CardUtils.GetDeckPath(DeckName));
            fileInfo.MoveTo(newDeckPath);
            DeckName = newDeckName;
        }

        /// <summary>
        ///     卡组删除事件
        /// </summary>
        public async void Delete_Click(object obj)
        {
            if (DeckName.Equals(string.Empty)) return;
            if (!await BaseDialogUtils.ShowDialogConfirm(StringConst.DeleteHint + "\n" + DeckName)) return;
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
        public async void Resave_Click(object obj)
        {
            var result = await BaseDialogUtils.ShowDialogEditor(DeckName, "请输入卡组名称");
            var deckPath = CardUtils.GetDeckPath(result);
            var canResave = DeckName.Equals(result) || !File.Exists(deckPath);
            if (canResave) Save();
            BaseDialogUtils.ShowDialogOk(StringConst.DeckNameExist);
        }

        /// <summary>
        ///     卡组存储事件
        /// </summary>
        public async void Save_Click(object obj)
        {
            if (!DeckName.Equals(string.Empty))
            {
                Save();
                return;
            }

            var result = await BaseDialogUtils.ShowDialogEditor(DeckName, "请输入卡组名称");
            if (result.Equals(string.Empty)) return;

            DeckName = result;
            Save();
        }

        /// <summary>
        ///     卡组清空
        /// </summary>
        public void ClearDeck()
        {
            _deckManager = new DeckManager();
            DeckName = _deckManager.DeckName;
            _playerVm.UpdatePlayerModels(_deckManager.PlayerModels);
            _deckExVm.UpdateAllDeckExModels(_deckManager);
            _deckStatsVm.UpdateDeckStatsModel(_deckManager.DeckStatsModel);
        }

        /// <summary>
        ///     卡组预览事件
        /// </summary>
        public async void DeckPreview_Click(object obj)
        {
            var model =
                await
                    DialogHost.Show(new DialogProgress(),
                        (sender, eventArgs) =>
                        {
                            Task.Run(() => GetDeckPreviewModels())
                                .ToObservable()
                                .ObserveOnDispatcher()
                                .Subscribe(result => { eventArgs.Session.UpdateContent(new DeckPreviewDialog(result)); });
                        }, (sender, eventArgs) =>{});
            if (model.ToString().Equals(string.Empty)) return;

            // 卡组载入
            await DialogHost.Show(new DialogProgress("载入中..."), (sender, eventArgs) =>
                {
                    Task.Run(() =>
                    {
                        var deckPreviewModel = (DeckPreviewModel) model;
                        _deckManager.Load(deckPreviewModel.DeckName, deckPreviewModel.NumberExList);
                    }).ToObservable().ObserveOnDispatcher().Subscribe(result =>
                    {
                        DeckName = _deckManager.DeckName;
                        _playerVm.UpdatePlayerModels(_deckManager.PlayerModels);
                        _deckExVm.UpdateAllDeckExModels(_deckManager);
                        _deckStatsVm.UpdateDeckStatsModel(_deckManager.DeckStatsModel);
                        eventArgs.Session.Close();
                    });
                },
                (sender, eventArgs) => { });
        }

        /// <summary>
        ///     获取卡组预览集合
        /// </summary>
        /// <returns>卡组预览集合</returns>
        public static List<DeckPreviewModel> GetDeckPreviewModels()
        {
            var deckPathList = new List<string>();
            deckPathList.AddRange(Directory.GetFiles(PathManager.DeckFolderPath, $"*{StringConst.DeckExtension}"));
            return (from deckPath in deckPathList
                let deckName = Path.GetFileNameWithoutExtension(deckPath)
                let numberListJson = FileUtils.GetFileContent(deckPath)
                let numberExList = JsonUtils.Deserialize<List<string>>(numberListJson)
                let models = CardUtils.GetCardModels(numberExList)
                let playerPath = 0 == models.Count? PathManager.PictureUnknownPath: CardUtils.GetPlayerPath(models)
                let startPath = CardUtils.GetStartPath(models)
                let statusMain = CardUtils.GetMainCount(models) == 50 ? "1" : "-1"
                let statusExtra = CardUtils.GetExtraCount(models) == 10 ? "1" : "-1"
                select new DeckPreviewModel
                {
                    DeckName = deckName,
                    StatusMain = statusMain,
                    StatusExtra = statusExtra,
                    PlayerPath = playerPath,
                    StartPath = startPath,
                    NumberExList = numberExList
                }).ToList();
        }

        public async void DeckStats_Click(object obj)
        {
            var dekcStatisticalDic = new Dictionary<int, int>();
            var costIgList = _deckManager.IgModels.Select(deckModel => deckModel.Cost);
            var costUgList = _deckManager.UgModels.Select(deckModel => deckModel.Cost);
            var costDeckList = new List<int>();
            costDeckList.AddRange(costIgList);
            costDeckList.AddRange(costUgList);
            if (0 == costDeckList.Count) return;
            var costMax = costDeckList.Max();
            for (var i = 0; i != costMax + 1; i++)
                dekcStatisticalDic.Add(i + 1, costDeckList.Count(cost => cost.Equals(i + 1)));
            await DialogHost.Show(new DeckStatisticalDialog(dekcStatisticalDic));
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
            deckNumberList.AddRange(_playerVm.PlayerModels.Select(deckModel => deckModel.NumberEx).ToList());
            deckNumberList.AddRange(_deckManager.IgModels.Select(deckModel => deckModel.NumberEx).ToList());
            deckNumberList.AddRange(_deckManager.UgModels.Select(deckModel => deckModel.NumberEx).ToList());
            deckNumberList.AddRange(_deckManager.ExModels.Select(deckModel => deckModel.NumberEx).ToList());
            deckBuilder.Append(JsonUtils.Serializer(deckNumberList));
            var isSave = FileUtils.SaveFile(deckPath, deckBuilder.ToString());
            BaseDialogUtils.ShowDialogAuto(isSave ? StringConst.SaveSucceed : StringConst.SaveFailed);
        }

        /// <summary>
        ///     添加卡片到组卡区
        /// </summary>
        /// <param name="numberEx">卡编</param>
        public void AddCard(string numberEx)
        {
            var returnAreaType = _deckManager.AddCard(numberEx);
            UpdateSingleDeckArea(returnAreaType);
        }

        /// <summary>
        ///     从组卡区删除卡牌
        /// </summary>
        /// <param name="numberEx"></param>
        public void DeleteCard(string numberEx)
        {
            var returnAreaType = _deckManager.DeleteCard(numberEx);
            UpdateSingleDeckArea(returnAreaType);
        }

        private void UpdateSingleDeckArea(Enums.AreaType areaType)
        {
            if (Enums.AreaType.None.Equals(areaType))
                return;
            // 添加成功则更新该区域
            if (areaType.Equals(Enums.AreaType.Player))
            {
                _playerVm.UpdatePlayerModels(_deckManager.PlayerModels);
            }
            else if (areaType.Equals(Enums.AreaType.Ig) || areaType.Equals(Enums.AreaType.Ug) ||
                     areaType.Equals(Enums.AreaType.Ex))
            {
                _deckExVm.UpdateDeckExModels(_deckManager, areaType);
                _deckStatsVm.UpdateDeckStatsModel(_deckManager.DeckStatsModel);
            }
        }
    }
}