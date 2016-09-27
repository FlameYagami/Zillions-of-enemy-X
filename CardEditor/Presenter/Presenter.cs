using System;
using System.Data;
using System.Windows.Forms;
using CardEditor.Constant;
using CardEditor.Model;
using CardEditor.Utils;
using CardEditor.View;
using Dialog;

namespace CardEditor.Presenter
{
    public interface IPresenter
    {
        void PackChanged(string pack);
        void TypeChanged(string type);
        void CampChanged(string camp);
        void OrderChanged(string order);
        void PreviewChanged(int selectIndex);
        void QueryClick(string mode, string order, string pack);
        void ExitClick();
        void ResetClick();
        void AddClick(string order);
        void DeleteClick(int selectIndex, string order);
        void UpdateClick(int selectIndex, string order);
        void EncryptDatabaseClick(string password);
        void DecryptDatabaseClick(string password);
        void Init();
        void ExportClick(string pack);
        void AbilityChanged(string ability);
    }

    internal class Presenter : IPresenter
    {
        private readonly IQuery _query;
        private readonly IView _view;

        public Presenter(IView view)
        {
            _view = view;
            _query = new Query();
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

        public void PackChanged(string pack)
        {
            var packNumber = CardUtils.GetPackNumber(pack);
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

        public void PreviewChanged(int selectIndex)
        {
            if (selectIndex.Equals(-1)) return;
            var previewEntity = Query.PreviewList[selectIndex];
            var cardEntity = CardUtils.GetCardEntity(previewEntity.Number);
            var picturePathList = CardUtils.GetPicturePathList(previewEntity.Number);
            Query.MemoryNumber = previewEntity.Number;
            _view.SetCardEntity(cardEntity);
            _view.SetPicture(picturePathList);
        }

        /// <summary>检索</summary>
        public void QueryClick(string mode, string order, string pack)
        {
            var cardModel = _view.GetCardEntity();
            if (mode.Equals(StringConst.ModeQuery))
            {
                UpdateCacheAndUi(_query.GetQuerySql(cardModel, CardUtils.GetPreviewOrderType(order)));
            }
            else if (mode.Equals(StringConst.ModeEditor))
            {
                if (pack.Equals(string.Empty))
                {
                    BaseDialogUtils.ShowDlg(StringConst.PackChoiceNone);
                    return;
                }
                UpdateCacheAndUi(_query.GetEditorSql(cardModel, CardUtils.GetPreviewOrderType(order)));
            }
        }

        /// <summary>添加</summary>
        public void AddClick(string order)
        {
            var cardModel = _view.GetCardEntity();
            if (CardUtils.IsNumberExist(cardModel.Number))
            {
                BaseDialogUtils.ShowDlg(StringConst.CardIsExitst);
                return;
            }
            if (!BaseDialogUtils.ShowDlgOkCancel(StringConst.AddConfirm)) return;

            var sql = _query.GetAddSql(cardModel);
            if (SqliteUtils.Execute(sql))
            {
                SqliteUtils.FillDataToDataSet(SqlUtils.GetQueryAllSql(), DataCache.DsAllCache);
                if (Query.MemoryQuerySql.Equals(string.Empty))
                {
                    var cardEntity = _view.GetCardEntity();
                    sql = _query.GetQuerySql(cardEntity, CardUtils.GetPreviewOrderType(order));
                    UpdateCacheAndUi(sql);
                }
                else
                {
                    sql = Query.MemoryQuerySql + SqlUtils.GetFooterSql(CardUtils.GetPreviewOrderType(order));
                    UpdateCacheAndUi(sql);
                }
                BaseDialogUtils.ShowDlg(StringConst.AddSucceed);
                return;
            }
            BaseDialogUtils.ShowDlg(StringConst.AddFailed);
        }

        /// <summary>重置</summary>
        public void ResetClick()
        {
            _view.Reset();
        }

        /// <summary>更新</summary>
        public void UpdateClick(int selectIndex, string order)
        {
            if (-1 == selectIndex)
            {
                BaseDialogUtils.ShowDlg(StringConst.CardChioceNone);
                return;
            }
            if (!BaseDialogUtils.ShowDlgOkCancel(StringConst.UpdateConfirm)) return;

            var number = Query.PreviewList[selectIndex].Number;
            var updateSql = _query.GetUpdateSql(_view.GetCardEntity(), number);
            MessageBox.Show(updateSql);
            if (SqliteUtils.Execute(updateSql))
            {
                SqliteUtils.FillDataToDataSet(SqlUtils.GetQueryAllSql(), DataCache.DsAllCache);
                UpdateCacheAndUi(Query.MemoryQuerySql + SqlUtils.GetFooterSql(CardUtils.GetPreviewOrderType(order)));
                BaseDialogUtils.ShowDlg(StringConst.UpdateSucceed);
                return;
            }
            BaseDialogUtils.ShowDlg(StringConst.UpdateFailed);
        }

        /// <summary>删除</summary>
        public void DeleteClick(int selectIndex, string order)
        {
            if (-1 == selectIndex)
            {
                BaseDialogUtils.ShowDlg(StringConst.CardChioceNone);
                return;
            }
            if (!BaseDialogUtils.ShowDlgOkCancel(StringConst.DeleteConfirm)) return;

            var number = Query.PreviewList[selectIndex].Number;
            var deleteSql = _query.GetDeleteSql(number);
            if (SqliteUtils.Execute(deleteSql))
            {
                SqliteUtils.FillDataToDataSet(SqlUtils.GetQueryAllSql(), DataCache.DsAllCache);
                UpdateCacheAndUi(Query.MemoryQuerySql + SqlUtils.GetFooterSql(CardUtils.GetPreviewOrderType(order)));
                BaseDialogUtils.ShowDlg(StringConst.DeleteSucceed);
                return;
            }
            BaseDialogUtils.ShowDlg(StringConst.DeleteFailed);
        }

        /// <summary>排序发生变化</summary>
        public void OrderChanged(string order)
        {
            var memorySql = Query.MemoryQuerySql;
            if (memorySql.Equals(string.Empty)) return;
            UpdateCacheAndUi(memorySql + SqlUtils.GetFooterSql(CardUtils.GetPreviewOrderType(order)));
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

        public void AbilityChanged(string ability)
        {
            var abilityType = _query.AnalysisAbility(ability);
            _view.UpdateAbilityLinkage(abilityType);
        }

        /// <summary>
        ///     更新全部数据集合以及ListView
        /// </summary>
        /// <param name="sql">查询语句</param>
        private void UpdateCacheAndUi(string sql)
        {
            SqliteUtils.FillDataToDataSet(sql, DataCache.DsPartCache);
            _query.SetCardList();
            _view.UpdateListView(Query.PreviewList, Query.MemoryNumber);
        }
    }
}