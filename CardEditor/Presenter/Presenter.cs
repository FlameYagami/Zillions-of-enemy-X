using System;
using System.Data;
using System.Windows;
using CardEditor.Model;
using CardEditor.Utils;
using CardEditor.View;
using Dialog;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;
using Enum = Wrapper.Constant.Enum;

namespace CardEditor.Presenter
{
    public interface IPresenter
    {
        void PackChanged(string pack, string number);
        void TypeChanged(string type);
        void CampChanged(string camp);
        void PreOrderChanged(string mode, string preOrder);
        void PreviewChanged(object selectItem);
        void ExitClick();
        void ResetClick(string mode);
        void AddClick(CardEditorModel cardEditorModel);
        void DeleteClick(CardEditorModel cardEditorModel, object selectItem);
        void UpdateClick(CardEditorModel cardEditorModel, object selectItem);
        void QueryClick(CardEditorModel cardEditorModel);
        void EncryptDatabaseClick(string password);
        void DecryptDatabaseClick(string password);
        void Init();
        void ExportClick(string pack);
        void AbilityChanged(string ability);
        void ModeChanged(CardEditorModel cardEditorModel);
        void Md5Click();
        void PackCoverClick();
    }

    internal class Presenter : IPresenter
    {
        private readonly ICardEditor _cardEditor;
        private readonly IView _view;

        public Presenter(IView view)
        {
            _view = view;
            _cardEditor = new Model.CardEditor();
        }

        public void Init()
        {
            if (SqliteUtils.FillDataToDataSet(SqlUtils.GetQueryAllSql(), DataCache.DsAllCache))
            {
                _view.SetPackItems(CardUtils.GetAllPack());
            }
            else
            {
                _view.SetPasswordVisibility(true, false);
                BaseDialogUtils.ShowDlgOk(StringConst.DbOpenError);
            }
        }

        public void ExportClick(string pack)
        {
            if (pack.Equals(StringConst.NotApplicable) || pack.Contains(StringConst.Series))
            {
                BaseDialogUtils.ShowDlgOk(StringConst.PackChoiceNone);
                return;
            }
            var exportPath = DialogUtils.ShowExport(pack);
            if (exportPath.Equals(string.Empty)) return;

            var sql = SqlUtils.GetExportSql(pack);
            var dataSet = new DataSet();
            if (!SqliteUtils.FillDataToDataSet(sql, dataSet)) return;

            var isExport = ExcelHelper.ExportPackToExcel(exportPath, dataSet);
            BaseDialogUtils.ShowDlg(isExport ? StringConst.ExportSucceed : StringConst.ExportFailed);
        }

        public void PackChanged(string pack, string number)
        {
            var packNumber = CardUtils.GetPackNumber(pack);
            if (number.Contains(StringConst.Hyphen))
                packNumber += number.Substring(number.IndexOf(StringConst.Hyphen, StringComparison.Ordinal) + 1);
            _view.UpdatePackLinkage(packNumber);
        }

        public void TypeChanged(string type)
        {
            _view.UpdateTypeLinkage(type);
        }

        public void CampChanged(string camp)
        {
            _view.UpdateCampLinkage(CardUtils.GetPartRace(camp));
        }

        public void AbilityChanged(string ability)
        {
            var abilityType = _cardEditor.AnalysisAbility(ability);
            _view.UpdateAbilityLinkage(abilityType);
        }

        public void PreviewChanged(object selectItem)
        {
            var cardPreviewModel = selectItem as CardPreviewModel;
            if (null == cardPreviewModel) return;
            var cardModel = CardUtils.GetCardModel(cardPreviewModel.Number);
            var picturePathList = CardUtils.GetPicturePathList(cardPreviewModel.ImageJson);
            _cardEditor.MemoryNumber = cardPreviewModel.Number;
            _view.SetCardModel(cardModel);
            _view.SetPicture(picturePathList);
        }

        /// <summary>重置</summary>
        public void ResetClick(string mode)
        {
            var modeType = CardUtils.GetModeType(mode);
            _cardEditor.MemoryNumber = string.Empty;
            _view.Reset(modeType);
        }


        /// <summary>添加</summary>
        public void AddClick(CardEditorModel cardEditorModel)
        {
            // 卡编是否重复判断
            if (CardUtils.IsNumberExist(cardEditorModel.Number))
            {
                BaseDialogUtils.ShowDlg(StringConst.CardIsExitst);
                return;
            }
            // 添加确认
            if (!BaseDialogUtils.ShowDlgOkCancel(StringConst.AddConfirm)) return;

            var isAdd = _cardEditor.AddCard(cardEditorModel);
            if (isAdd) UpdateCacheAndUi(_cardEditor.MemoryEditorCardModel);
            BaseDialogUtils.ShowDlg(isAdd ? StringConst.AddSucceed : StringConst.AddFailed);
        }

        /// <summary>删除</summary>
        public void DeleteClick(CardEditorModel cardEditorModel, object selectItem)
        {
            var previewModel = selectItem as CardPreviewModel;
            if (null == previewModel)
            {
                BaseDialogUtils.ShowDlg(StringConst.CardChioceNone);
                return;
            }
            if (!BaseDialogUtils.ShowDlgOkCancel(StringConst.DeleteConfirm)) return;

            var isDelete = _cardEditor.DeleteCard(previewModel.Number);
            if (isDelete) UpdateCacheAndUi(_cardEditor.MemoryEditorCardModel);
            BaseDialogUtils.ShowDlg(isDelete ? StringConst.DeleteSucceed : StringConst.DeleteFailed);
        }

        /// <summary>更新</summary>
        public void UpdateClick(CardEditorModel cardEditorModel,object selectItem)
        {
            var previewModel = selectItem as CardPreviewModel;
            if (null == previewModel)
            {
                BaseDialogUtils.ShowDlg(StringConst.CardChioceNone);
                return;
            }
            if (!BaseDialogUtils.ShowDlgOkCancel(StringConst.UpdateConfirm)) return;

            var isUpdate = _cardEditor.UpdateCard(cardEditorModel, previewModel.Number);
            if (isUpdate) UpdateCacheAndUi(_cardEditor.MemoryEditorCardModel);
            BaseDialogUtils.ShowDlg(isUpdate ? StringConst.UpdateSucceed  : StringConst.UpdateFailed);
        }

        /// <summary>排序发生变化</summary>
        public void PreOrderChanged(string mode, string preOrder)
        {
            var memoryEditorCardModel = _cardEditor.MemoryEditorCardModel;
            if (memoryEditorCardModel.Equals(null)) return;
            UpdateCacheAndUi(memoryEditorCardModel);
        }

        public void ExitClick()
        {
            Environment.Exit(0);
        }

        public void EncryptDatabaseClick(string password)
        {
            if (password.Equals(string.Empty))
            {
                BaseDialogUtils.ShowDlgOk(StringConst.PasswordNone);
                return;
            }
            if (SqliteUtils.Encrypt(DataCache.DsAllCache))
            {
                _view.SetPasswordVisibility(false, true);
                BaseDialogUtils.ShowDlg(StringConst.EncryptSucced);
                return;
            }
            BaseDialogUtils.ShowDlgOk(StringConst.EncryptFailed);
        }

        public void DecryptDatabaseClick(string password)
        {
            if (password.Equals(string.Empty))
            {
                BaseDialogUtils.ShowDlgOk(StringConst.PasswordNone);
                return;
            }
            if (SqliteUtils.Decrypt())
            {
                _view.SetPasswordVisibility(true, false);
                BaseDialogUtils.ShowDlg(StringConst.DncryptSucced);
                return;
            }
            BaseDialogUtils.ShowDlgOk(StringConst.DncryptFailed);
        }

        public void ModeChanged(CardEditorModel cardEditorModel)
        {
            var modeType = CardUtils.GetModeType(cardEditorModel.Mode);
            _cardEditor.MemoryEditorCardModel = cardEditorModel;
            switch (modeType)
            {
                case Enum.ModeType.Editor:
                {
                    if (!cardEditorModel.Pack.Equals(string.Empty))
                        UpdateCacheAndUi(cardEditorModel);
                    break;
                }
            }
        }

        public void Md5Click()
        {
            var sqlList = SqlUtils.GetMd5SqlList();
            var succeed = SqliteUtils.Execute(sqlList);
            BaseDialogUtils.ShowDlg(succeed ? StringConst.UpdateSucceed : StringConst.UpdateFailed);
        }

        public void PackCoverClick()
        {
            DialogUtils.ShowPackCover();
        }

        /// <summary>检索</summary>
        public void QueryClick(CardEditorModel cardEditorModel)
        {
            UpdateCacheAndUi(cardEditorModel);
        }

        /// <summary>
        ///     更新全部数据集合以及ListView
        /// </summary>
        private void UpdateCacheAndUi(CardEditorModel cardEditorModel)
        {
            var modeType = CardUtils.GetModeType(cardEditorModel.Mode);
            var sql = string.Empty;
            switch (modeType)
            {
                case Enum.ModeType.Query:
                    sql = _cardEditor.GetQuerySql(cardEditorModel);
                    break;
                case Enum.ModeType.Editor:
                    if (cardEditorModel.Pack.Equals(string.Empty)) return;
                    sql = _cardEditor.GetEditorSql(cardEditorModel);
                    break;
                case Enum.ModeType.Develop:
                    break;
            }
            var previewModelList = _cardEditor.GetCardPreviewList(sql, cardEditorModel.Restrict);
            _view.UpdatePreListView(previewModelList, _cardEditor.MemoryNumber);
        }
    }
}