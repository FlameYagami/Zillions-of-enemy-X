using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Common;
using Dialog;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;

namespace CardEditor.ViewModel
{
    public class CardQueryVm
    {
        private readonly CardPictureVm _cardPictureVm;
        private readonly CardPreviewVm _cardPreviewVm;
        private readonly CardQueryExVm _cardQueryExVm;

        public CardQueryVm(CardQueryExVm cardQueryExVm, CardPreviewVm cardPreviewVm,
            CardPictureVm cardPictureVm)
        {
            _cardQueryExVm = cardQueryExVm;
            _cardPreviewVm = cardPreviewVm;
            _cardPictureVm = cardPictureVm;

            CmdQuery = new DelegateCommand {ExecuteCommand = Query_Click};
            CmdReset = new DelegateCommand {ExecuteCommand = Reset_Click};
            CmdAdd = new DelegateCommand {ExecuteCommand = Add_Click};
            CmdUpdate = new DelegateCommand {ExecuteCommand = Update_Click};
            CmdDelete = new DelegateCommand {ExecuteCommand = Delete_Click};
            CmdExport = new DelegateCommand {ExecuteCommand = Export_Click};

            CardQueryModel = new CeQueryModel();
            QuerySourceModel = new QuerySourceModel();
        }

        public DelegateCommand CmdQuery { get; set; }
        public DelegateCommand CmdReset { get; set; }
        public DelegateCommand CmdAdd { get; set; }
        public DelegateCommand CmdUpdate { get; set; }
        public DelegateCommand CmdDelete { get; set; }
        public DelegateCommand CmdExport { get; set; }

        public CeQueryModel CardQueryModel { get; set; }
        public QuerySourceModel QuerySourceModel { get; set; }

        public void Export_Click(object obj)
        {
            var pack = CardQueryModel.Pack;
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
            var exportPath = sfd.ShowDialog() != DialogResult.OK ? string.Empty : sfd.FileName;
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
            if (CardUtils.IsNumberExist(CardQueryModel.Number))
            {
                BaseDialogUtils.ShowDialogAuto(StringConst.CardIsExitst);
                return;
            }
            // 添加确认
            if (!await BaseDialogUtils.ShowDialogConfirm(StringConst.AddConfirm)) return;
            // 数据库添加
            var addSql = CeSqlUtils.GetAddSql(CardQueryModel);
            var isAdd = DataManager.Execute(addSql);
            BaseDialogUtils.ShowDialogAuto(isAdd ? StringConst.AddSucceed : StringConst.AddFailed);
            // 数据库更新
            if (!isAdd) return;
            DataManager.FillDataToDataSet();
            if (null == _cardPreviewVm.CeQueryExModel)
                _cardPreviewVm.CeQueryExModel = GetCardQueryExMdoel();
            _cardPreviewVm.UpdateCardPreviewList(_cardPreviewVm.CeQueryExModel);
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
            DataManager.FillDataToDataSet();
            if (null == _cardPreviewVm.CeQueryExModel)
                _cardPreviewVm.CeQueryExModel = GetCardQueryExMdoel();
            _cardPreviewVm.UpdateCardPreviewList(_cardPreviewVm.CeQueryExModel);
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
            var checkNumber = selectedItem.Number.Equals(CardQueryModel.Number) ||
                              !CardUtils.IsNumberExist(CardQueryModel.Number);
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
            var updateSql = CeSqlUtils.GetUpdateSql(CardQueryModel, selectedItem.Number);
            var udpateSameSql = CeSqlUtils.GetUpdateSql(CardQueryModel);
            var isUpdate = DataManager.Execute(new List<string> {updateSql, udpateSameSql});
            BaseDialogUtils.ShowDialogAuto(isUpdate ? StringConst.UpdateSucceed : StringConst.UpdateFailed);
            // 数据库更新
            if (!isUpdate) return;
            DataManager.FillDataToDataSet();
            _cardPreviewVm.UpdateCardPreviewList(_cardPreviewVm.CeQueryExModel);
        }

        /// <summary>
        ///     重置事件
        /// </summary>
        public void Reset_Click(object obj)
        {
            CardQueryModel = new CeQueryModel();
            var mode = _cardQueryExVm.ModeValue;
            if (!CardUtils.GetModeType(mode).Equals(Enums.ModeType.Editor)) return;
            CardQueryModel.Pack = _cardPreviewVm.CeQueryExModel.CeQueryModel.Pack;
            CardQueryModel.Number = _cardPreviewVm.CeQueryExModel.CeQueryModel.Number;
        }

        /// <summary>
        ///     查询事件
        /// </summary>
        public void Query_Click(object obj)
        {
            _cardPreviewVm.UpdateCardPreviewList(GetCardQueryExMdoel());
        }

        private CeQueryExModel GetCardQueryExMdoel()
        {
            // 深拷贝查询模型
            var cardEditorModel = JsonUtils.Deserialize<CeQueryModel>(JsonUtils.Serializer(CardQueryModel));
            var mode = _cardQueryExVm.ModeValue;
            var restrict = _cardQueryExVm.RestrictValue.Equals(StringConst.NotApplicable)
                ? -1
                : int.Parse(_cardQueryExVm.RestrictValue);
            return new CeQueryExModel
            {
                CeQueryModel = cardEditorModel,
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
            if (CardQueryModel.Type.Equals(StringConst.TypePlayer) ||
                CardQueryModel.Type.Equals(StringConst.TypeEvent)) return;
            QuerySourceModel.UpdateRaceList(CardQueryModel.Camp);
            CardQueryModel.Race = StringConst.NotApplicable;
        }

        /// <summary>
        ///     种类改变联动事件
        /// </summary>
        public void UpdateTypeLinkage()
        {
            if (_cardPreviewVm.IsPreviewChanged) return;
            CardQueryModel.UpdateTypeLinkage();
        }

        public void UpdateCardEditorModel(CardModel cardModel)
        {
            _cardPreviewVm.IsPreviewChanged = true;

            CardQueryModel.UpdateBaseProperty(cardModel);
            CardQueryModel.UpdateAbilityTypeModels(cardModel);
            CardQueryModel.UpdateAbilityDetailModel(cardModel);

            _cardQueryExVm.Md5Value = cardModel.Md5;
            _cardQueryExVm.UpdateRestrictValue(cardModel.Restrict);
            _cardPictureVm.UpdatePicture(cardModel);

            _cardPreviewVm.IsPreviewChanged = false;
        }

        /// <summary>
        ///     卡包改变联动事件
        /// </summary>
        public void UpdatePackLinkage()
        {
            if (_cardPreviewVm.IsPreviewChanged) return;
            CardQueryModel.UpdatePackLinkage();
        }

        /// <summary>
        ///     能力改变联动事件
        /// </summary>
        public void UpdateAbilityLinkage()
        {
            if (_cardPreviewVm.IsPreviewChanged) return;
            CardQueryModel.UpdateAbilityLinkage();
        }
    }
}