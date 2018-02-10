using System.Collections.Generic;
using System.Data;
using CardEditor.Utils;
using Common;
using Dialog;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;
using ExcelHelper = Common.ExcelHelper;

namespace CardEditor.ViewModel
{
    public sealed class CardQueryVm
    {
        private readonly AbilityTypeVm _abilityTypeVm;
        private readonly CardPictureVm _cardPictureVm;
        private readonly CardPreviewVm _cardPreviewVm;
        private readonly CardQueryExVm _cardQueryExVm;

        public CardQueryVm(AbilityTypeVm abilityTypeVm, CardQueryExVm cardQueryExVm, CardPreviewVm cardPreviewVm,
            CardPictureVm cardPictureVm)
        {
            _abilityTypeVm = abilityTypeVm;
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
            _abilityTypeVm.UpdateAbilityType(CardQueryModel.AbilityTypeModels);
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
            var exportPath = DialogUtils.ShowExport(pack);
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
        public void Add_Click(object obj)
        {
            // 卡编是否重复判断
            if (CardUtils.IsNumberExist(CardQueryModel.Number))
            {
                BaseDialogUtils.ShowDialogAuto(StringConst.CardIsExitst);
                return;
            }
            // 添加确认
            if (!BaseDialogUtils.ShowDialogConfirm(StringConst.AddConfirm)) return;
            // 数据库添加
            var addSql = CeSqlUtils.GetAddSql(CardQueryModel);
            var isAdd = DataManager.Execute(addSql);
            BaseDialogUtils.ShowDialogAuto(isAdd ? StringConst.AddSucceed : StringConst.AddFailed);
            if (!isAdd) return;
            // 数据库更新
            DataManager.FillDataToDataSet();
            // 跟踪历史
            if (null == _cardPreviewVm.MemoryQueryModel)
                _cardPreviewVm.MemoryQueryModel = GetCardQueryExMdoel();
            _cardPreviewVm.UpdateCardPreviewList(_cardPreviewVm.MemoryQueryModel);
        }

        /// <summary>
        ///     卡牌删除事件
        /// </summary>
        public void Delete_Click(object obj)
        {
            // 选择判空
            var selectedItem = _cardPreviewVm.SelectedItem;
            if (null == selectedItem)
            {
                BaseDialogUtils.ShowDialogAuto(StringConst.CardChioceNone);
                return;
            }
            // 删除确认
            if (!BaseDialogUtils.ShowDialogConfirm(StringConst.DeleteConfirm)) return;
            // 数据库删除
            var deleteSql = CeSqlUtils.GetDeleteSql(selectedItem.Number);
            var isDelete = DataManager.Execute(deleteSql);
            BaseDialogUtils.ShowDialogAuto(isDelete ? StringConst.DeleteSucceed : StringConst.DeleteFailed);
            if (!isDelete) return;
            // 数据库更新
            DataManager.FillDataToDataSet();
            // 跟踪历史
            if (null == _cardPreviewVm.MemoryQueryModel)
                _cardPreviewVm.MemoryQueryModel = GetCardQueryExMdoel();
            _cardPreviewVm.UpdateCardPreviewList(_cardPreviewVm.MemoryQueryModel);
        }

        /// <summary>
        ///     卡牌更新事件
        /// </summary>
        public void Update_Click(object obj)
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
            if (!BaseDialogUtils.ShowDialogConfirm(StringConst.UpdateConfirm)) return;
            // 数据库修改
            var updateSql = CeSqlUtils.GetUpdateSql(CardQueryModel, selectedItem.Number);
            var updateSqls = new List<string> {updateSql};
            if (_cardQueryExVm.ModeType.Equals(Enums.ModeType.Develop))
                updateSqls.Add(CeSqlUtils.GetUpdateSql(CardQueryModel));
            var isUpdate = DataManager.Execute(updateSqls);
            BaseDialogUtils.ShowDialogAuto(isUpdate ? StringConst.UpdateSucceed : StringConst.UpdateFailed);
            if (!isUpdate) return;
            // 数据库更新
            DataManager.FillDataToDataSet();
            _cardPreviewVm.UpdateCardPreviewList(_cardPreviewVm.MemoryQueryModel);
        }

        /// <summary>
        ///     获取额外的的查询模型（其中包含模式、规制）
        /// </summary>
        /// <returns></returns>
        private CeQueryExModel GetCardQueryExMdoel()
        {
            // 深拷贝查询模型
            var cardEditorModel = JsonUtils.Deserialize<CeQueryModel>(JsonUtils.Serializer(CardQueryModel));
            var restrict = _cardQueryExVm.RestrictValue.Equals(StringConst.NotApplicable)
                ? -1
                : int.Parse(_cardQueryExVm.RestrictValue);
            return new CeQueryExModel
            {
                CeQueryModel = cardEditorModel,
                Restrict = restrict,
                ModeType = _cardQueryExVm.ModeType
            };
        }

        /// <summary>
        ///     重置事件
        /// </summary>
        public void Reset_Click(object obj)
        {
            CardQueryModel.InitCeQueryModel();
            if (_cardQueryExVm.ModeType.Equals(Enums.ModeType.Editor))
            {
                CardQueryModel.Pack = _cardPreviewVm.MemoryQueryModel.CeQueryModel.Pack;
                CardQueryModel.Number = _cardPreviewVm.MemoryQueryModel.CeQueryModel.Number;
            }
            _abilityTypeVm.UpdateAbilityType(CardQueryModel.AbilityTypeModels);
        }

        /// <summary>
        ///     查询事件
        /// </summary>
        public void Query_Click(object obj)
        {
            _cardPreviewVm.UpdateCardPreviewList(GetCardQueryExMdoel());
        }

        public void UpdateCardQueryModel(CardModel cardModel)
        {
            _cardPreviewVm.IsPreviewChanged = true;
            CardQueryModel.UpdateBaseProperty(cardModel);
            CardQueryModel.UpdateAbilityTypeModels(cardModel);
            CardQueryModel.UpdateAbilityDetailModel(cardModel);
            _cardQueryExVm.Md5 = cardModel.Md5;
            _cardQueryExVm.UpdateRestrictValue(cardModel.Restrict);
            _cardPictureVm.UpdatePicture(cardModel);
            _cardPreviewVm.IsPreviewChanged = false;
        }

        /// <summary>
        ///     阵营、种族联动事件
        /// </summary>
        public void UpdateRaceLinkage()
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
            CardQueryModel.UpdateTypeLinkage();
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
        public void UpdateAbilityLinkage(string ability)
        {
            if (_cardPreviewVm.IsPreviewChanged) return;
            CardQueryModel.UpdateAbilityLinkage(ability);
        }
    }
}