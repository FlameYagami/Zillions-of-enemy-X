using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CardEditor.Model;
using Common;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;
using Dialog;

namespace CardEditor.ViewModel
{
    public class CardSearchVm : BaseModel
    {
        private readonly CardPictureVm _cardPictureVm;
        private readonly CardPreviewVm _cardPreviewVm;
        private readonly CardSearchExVm _externOpertaionVm;

        public CardSearchVm(CardSearchExVm externOpertaionVm, CardPreviewVm cardPreviewVm,
            CardPictureVm cardPictureVm)
        {
            _externOpertaionVm = externOpertaionVm;
            _cardPreviewVm = cardPreviewVm;
            _cardPictureVm = cardPictureVm;

            CmdQuery = new DelegateCommand {ExecuteCommand = Query_Click};
            CmdReset = new DelegateCommand {ExecuteCommand = Reset_Click};
            CmdAdd = new DelegateCommand {ExecuteCommand = Add_Click};
            CmdUpdate = new DelegateCommand {ExecuteCommand = Update_Click};
            CmdDelete = new DelegateCommand {ExecuteCommand = Delete_Click};
            CmdExport = new DelegateCommand {ExecuteCommand = Export_Click};

            SearchModel = new CeSearchModel();
            SearchSourceModel = new SearchSourceModel();
        }

        public DelegateCommand CmdQuery { get; set; }
        public DelegateCommand CmdReset { get; set; }
        public DelegateCommand CmdAdd { get; set; }
        public DelegateCommand CmdUpdate { get; set; }
        public DelegateCommand CmdDelete { get; set; }
        public DelegateCommand CmdExport { get; set; }

        public CeSearchModel SearchModel { get; set; }
        public SearchSourceModel SearchSourceModel { get; set; }

        public void Export_Click(object obj)
        {
            var pack = SearchModel.Pack;
            if (pack.Equals(StringConst.NotApplicable) || pack.Contains(StringConst.Series))
            {
                BaseDialogUtils.ShowDialogOk(StringConst.PackChoiceNone);
                return;
            }

            var sfd = new SaveFileDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Filter = @"xls文件(*.xls)|*.xls",
                FileName = pack
            };
            var exportPath =  sfd.ShowDialog() != DialogResult.OK ? string.Empty : sfd.FileName;
            if (exportPath.Equals(string.Empty)) return;

            var sql = SqlUtils.GetExportSql(pack);
            var dataSet = new DataSet();
            if (!DataManager.FillDataToDataSet(dataSet, sql)) return;

            var isExport = ExcelHelper.ExportPackToExcel(exportPath, dataSet);
            BaseDialogUtils.ShowDialogAuto(isExport ? StringConst.ExportSucceed : StringConst.ExportFailed);
        }

        /// <summary>
        ///     卡牌添加事件
        /// </summary>
        public async void Add_Click(object obj)
        {
            // 卡编是否重复判断
            if (CardUtils.IsNumberExist(SearchModel.Number))
            {
                BaseDialogUtils.ShowDialogAuto(StringConst.CardIsExitst);
                return;
            }
            // 添加确认
            if (!await BaseDialogUtils.ShowDialogConfirm(StringConst.AddConfirm)) return;
            // 数据库添加
            var addSql = CeSqlUtils.GetAddSql(SearchModel);
            var isAdd = DataManager.Execute(addSql);
            BaseDialogUtils.ShowDialogAuto(isAdd ? StringConst.AddSucceed : StringConst.AddFailed);
            // 数据库更新
            if (!isAdd) return;
            DataManager.DsAllCache.Clear();
            DataManager.FillDataToDataSet(DataManager.DsAllCache,SqlUtils.GetQueryAllSql());
            if (null == _cardPreviewVm.MemoryQueryModel)
                _cardPreviewVm.MemoryQueryModel = GetCardQueryMdoel();
            _cardPreviewVm.UpdateCardPreviewList(_cardPreviewVm.MemoryQueryModel);
        }

        /// <summary>
        ///     卡牌删除事件
        /// </summary>
        public async void Delete_Click(object obj)
        {
            // 选择判空
            var selectedItem = _cardPreviewVm.SelectedItem;
            if (null == selectedItem)
            {
                BaseDialogUtils.ShowDialogAuto(StringConst.CardChioceNone);
                return;
            }
            // 删除确认
            if (!await BaseDialogUtils.ShowDialogConfirm(StringConst.DeleteConfirm)) return;
            // 数据库删除
            var deleteSql = CeSqlUtils.GetDeleteSql(selectedItem.Number);
            var isDelete = DataManager.Execute(deleteSql);
            BaseDialogUtils.ShowDialogAuto(isDelete ? StringConst.DeleteSucceed : StringConst.DeleteFailed);
            // 数据库更新
            if (!isDelete) return;
            DataManager.DsAllCache.Clear();
            DataManager.FillDataToDataSet(DataManager.DsAllCache,SqlUtils.GetQueryAllSql());
            if (null == _cardPreviewVm.MemoryQueryModel)
                _cardPreviewVm.MemoryQueryModel = GetCardQueryMdoel();
            _cardPreviewVm.UpdateCardPreviewList(_cardPreviewVm.MemoryQueryModel);
        }

        /// <summary>
        ///     卡牌更新事件
        /// </summary>
        public async void Update_Click(object obj)
        {
            // 选择判空
            var selectedItem = _cardPreviewVm.SelectedItem;
            if (null == selectedItem)
            {
                BaseDialogUtils.ShowDialogAuto(StringConst.CardChioceNone);
                return;
            }
            // 卡编是否重复判断
            var checkNumber = selectedItem.Number.Equals(SearchModel.Number) ||
                              !CardUtils.IsNumberExist(SearchModel.Number);
            if (!checkNumber)
            {
                BaseDialogUtils.ShowDialogAuto(StringConst.CardIsExitst);
                return;
            }
            // 修改确认
            if (
                !await
                    BaseDialogUtils.ShowDialogConfirm(StringConst.UpdateConfirm + "\n" +
                                                      CardUtils.GetMd5(selectedItem.Number))) return;
            // 数据库修改
            var updateSql = CeSqlUtils.GetUpdateSql(SearchModel, selectedItem.Number);
            var udpateSameSql = CeSqlUtils.GetUpdateSql(SearchModel);
            var isUpdate = DataManager.Execute(new List<string> {updateSql, udpateSameSql});
            BaseDialogUtils.ShowDialogAuto(isUpdate ? StringConst.UpdateSucceed : StringConst.UpdateFailed);
            // 数据库更新
            if (!isUpdate) return;
            DataManager.DsAllCache.Clear();
            DataManager.FillDataToDataSet(DataManager.DsAllCache,SqlUtils.GetQueryAllSql());
            _cardPreviewVm.UpdateCardPreviewList(_cardPreviewVm.MemoryQueryModel);
        }

        /// <summary>
        ///     重置事件
        /// </summary>
        public void Reset_Click(object obj)
        {
            SearchModel = new CeSearchModel();
            OnPropertyChanged(nameof(SearchModel));
            var mode = _externOpertaionVm.ModeValue;
            if (!CardUtils.GetModeType(mode).Equals(Enums.ModeType.Editor)) return;
            SearchModel.Pack = _cardPreviewVm.MemoryQueryModel.CardEditorModel.Pack;
            SearchModel.Number = _cardPreviewVm.MemoryQueryModel.CardEditorModel.Number;
        }

        /// <summary>
        ///     查询事件
        /// </summary>
        public void Query_Click(object obj)
        {
            _cardPreviewVm.UpdateCardPreviewList(GetCardQueryMdoel());
        }

        private CeSearchExModel GetCardQueryMdoel()
        {
            // 深拷贝查询模型
            var cardEditorModel = JsonUtils.Deserialize<CeSearchModel>(JsonUtils.Serializer(SearchModel));
            var mode = _externOpertaionVm.ModeValue;
            var restrict = _externOpertaionVm.RestrictValue.Equals(StringConst.NotApplicable)
                ? -1
                : int.Parse(_externOpertaionVm.RestrictValue);
            return new CeSearchExModel
            {
                CardEditorModel = cardEditorModel,
                Restrict = restrict,
                ModeValue = mode
            };
        }

        /// <summary>
        ///     阵营、种族联动事件
        /// </summary>
        public void UpdateRaceList()
        {
            if (_cardPreviewVm.IsPreviewChanged) return;
            // 类型判断,玩家、事件不改变种族默认值（此时种族处于不可编辑状态）
            if (SearchModel.Type.Equals(StringConst.TypePlayer) ||
                SearchModel.Type.Equals(StringConst.TypeEvent)) return;
            SearchSourceModel.UpdateRaceList(SearchModel.Camp);
            SearchModel.Race = StringConst.NotApplicable;
            OnPropertyChanged(nameof(SearchModel));
        }

        /// <summary>
        ///     种类改变联动事件
        /// </summary>
        public void UpdateTypeLinkage()
        {
            if (_cardPreviewVm.IsPreviewChanged) return;
            switch (SearchModel.Type)
            {
                case StringConst.NotApplicable:
                case StringConst.TypeZx:
                {
                    SearchModel.CostEnabled = true;
                    SearchModel.PowerEnabled = true;
                    SearchModel.RaceEnabled = true;
                    SearchModel.SignEnabled = true;
                    break;
                }
                case StringConst.TypeZxEx:
                {
                    SearchModel.CostEnabled = true;
                    SearchModel.PowerEnabled = true;
                    SearchModel.RaceEnabled = true;
                    SearchModel.SignEnabled = false;
                    SearchModel.Sign = StringConst.Hyphen;
                    break;
                }
                case StringConst.TypePlayer:
                {
                    SearchModel.CostValue = "-1";
                    SearchModel.PowerValue = "-1";
                    SearchModel.CostEnabled = false;
                    SearchModel.PowerEnabled = false;
                    SearchModel.RaceEnabled = false;
                    SearchModel.SignEnabled = false;
                    SearchModel.Race = StringConst.Hyphen;
                    SearchModel.Sign = StringConst.Hyphen;
                    break;
                }
                case StringConst.TypeEvent:
                {
                    SearchModel.CostEnabled = true;
                    SearchModel.PowerEnabled = false;
                    SearchModel.RaceEnabled = false;
                    SearchModel.SignEnabled = true;
                    SearchModel.Race = StringConst.Hyphen;
                    SearchModel.PowerValue = "-1";
                    break;
                }
            }
            OnPropertyChanged(nameof(SearchModel));
        }

        public void UpdateCardEditorModel(CardModel cardModel)
        {
            _cardPreviewVm.IsPreviewChanged = true;

            SearchModel.Md5 = cardModel.Md5;
            SearchModel.Type = cardModel.Type;
            SearchModel.Camp = cardModel.Camp;
            SearchModel.Race = cardModel.Race;
            SearchModel.Sign = cardModel.Sign;
            SearchModel.Rare = cardModel.Rare;
            SearchModel.Pack = cardModel.Pack;
            SearchModel.CName = cardModel.CName;
            SearchModel.JName = cardModel.JName;
            SearchModel.Number = cardModel.Number;
            SearchModel.Illust = cardModel.Illust;
            SearchModel.CostValue = cardModel.Cost.ToString();
            SearchModel.PowerValue = cardModel.Power.ToString();
            SearchModel.Ability = cardModel.Ability;
            SearchModel.Lines = cardModel.Lines;

            for (var i = 0; i != SearchModel.AbilityTypeModels.Count; i++)
            {
                var model = SearchModel.AbilityTypeModels[i];
                SearchModel.AbilityTypeModels[i] = new AbilityModel
                {
                    Checked = cardModel.Ability.Contains(model.Name),
                    Name = model.Name
                };
            }

            var abilityDetailModelList = JsonUtils.Deserialize<List<List<int>>>(cardModel.AbilityDetailJson);
            foreach (var pair in abilityDetailModelList)
                for (var i = 0; i != SearchModel.AbilityDetailModels.Count; i++)
                {
                    var model = SearchModel.AbilityDetailModels[i];
                    if (!model.Code.Equals(pair[0])) continue;
                    SearchModel.AbilityDetailModels[i] = new AbilityModel
                    {
                        Checked = pair[1] == 1,
                        Name = model.Name,
                        Code = pair[0]
                    };
                    break;
                }
            OnPropertyChanged(nameof(SearchModel));
            _externOpertaionVm.Md5Value = cardModel.Md5;
            _externOpertaionVm.UpdateRestrictValue(cardModel.Restrict);
            _cardPictureVm.UpdatePicture(cardModel);

            _cardPreviewVm.IsPreviewChanged = false;
        }

        /// <summary>
        ///     卡包改变联动事件
        /// </summary>
        public void UpdatePackLinkage()
        {
            if (_cardPreviewVm.IsPreviewChanged) return;
            var packNumber = CardUtils.GetPackNumber(SearchModel.Pack);
            if (SearchModel.Number.Contains(StringConst.Hyphen))
                packNumber +=
                    SearchModel.Number.Substring(
                        SearchModel.Number.IndexOf(StringConst.Hyphen, StringComparison.Ordinal) + 1);
            SearchModel.Number = packNumber;
            if (packNumber.IndexOf("P", StringComparison.Ordinal) == 0)
                SearchModel.Rare = StringConst.RarePr;
        }

        /// <summary>
        ///     能力改变联动事件
        /// </summary>
        public void UpdateAbilityLinkage()
        {
            if (_cardPreviewVm.IsPreviewChanged) return;
            var ability = SearchModel.Ability;
            if (ability.Contains("降临条件") || ability.Contains("觉醒条件"))
            {
                SearchModel.Type = StringConst.TypeZxEx;
                SearchModel.Sign = StringConst.Hyphen;
            }
            if (ability.Contains("【★】"))
            {
                SearchModel.Type = StringConst.TypeEvent;
                SearchModel.Race = StringConst.Hyphen;
                SearchModel.PowerValue = string.Empty;
            }
            if (ability.Contains("【常】生命恢复") || ability.Contains("【常】虚空使者"))
            {
                SearchModel.Type = StringConst.TypeZx;
                SearchModel.Sign = StringConst.SignIg;
            }
            if (ability.Contains("【常】起始卡"))
            {
                SearchModel.Type = StringConst.TypeZx;
                SearchModel.Sign = StringConst.Hyphen;
            }
        }
    }
}