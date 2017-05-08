using System;
using System.Data;
using CardEditor.Model;
using CardEditor.Utils;
using CardEditor.View;
using Dialog;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Entity;
using Wrapper.Utils;
using Enum = CardEditor.Constant.Enum;

namespace CardEditor.Presenter
{
    public interface IPresenter
    {
        void PackChanged(string pack);
        void TypeChanged(string type);
        void CampChanged(string camp);
        void PreOrderChanged(string preOrder);
        void PreviewChanged(int selectIndex);
        void QueryClick(string mode, string preOrder);
        void ExitClick();
        void ResetClick(string mode);
        void AddClick(string preOrder);
        void DeleteClick(int selectIndex, string preOrder);
        void UpdateClick(int selectIndex, string preOrder);
        void EncryptDatabaseClick(string password);
        void DecryptDatabaseClick(string password);
        void Init();
        void ExportClick(string pack);
        void AbilityChanged(string ability);
        void ModeChanged(string mode, string preOrder);
        void Md5Click();
        void PackCoverClick();
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

        public void AbilityChanged(string ability)
        {
            var abilityType = _query.AnalysisAbility(ability);
            _view.UpdateAbilityLinkage(abilityType);
        }

        public void PreviewChanged(int selectIndex)
        {
            if (selectIndex.Equals(-1)) return;
            var previewEntity = DataCache.PreEntityList[selectIndex];
            var cardEntity = CardUtils.GetCardEntity(previewEntity.Number);
            var picturePathList = CardUtils.GetPicturePathList(previewEntity.ImageJson);
            _query.MemoryNumber = previewEntity.Number;
            _view.SetCardEntity(cardEntity);
            _view.SetPicture(picturePathList);
        }

        /// <summary>检索</summary>
        public void QueryClick(string mode, string preOrder)
        {
            var modeType = CardUtils.GetModeType(mode);
            var cardEntity = _view.GetCardEntity();
            var restrictQuery = cardEntity.Restrict;
            switch (modeType)
            {
                case Enum.ModeType.Query:
                    UpdateCacheAndUi(cardEntity, preOrder);
                    break;
                case Enum.ModeType.Editor:
                    if (!cardEntity.Pack.Equals(string.Empty))
                    {
                        var sql = _query.GetEditorSql(cardEntity, preOrder);
                        UpdateCacheAndUi(sql, restrictQuery);
                        return;
                    }
                    BaseDialogUtils.ShowDlg(StringConst.PackChoiceNone);
                    break;
                case Enum.ModeType.Develop:
                    break;
            }
        }

        /// <summary>添加</summary>
        public void AddClick(string preOrder)
        {
            var cardEntity = _view.GetCardEntity();
            // 卡编是否重复判断
            if (CardUtils.IsNumberExist(cardEntity.Number))
            {
                BaseDialogUtils.ShowDlg(StringConst.CardIsExitst);
                return;
            }
            // 添加确认
            if (!BaseDialogUtils.ShowDlgOkCancel(StringConst.AddConfirm)) return;
            // 获取添加的Sql
            var sql = _query.GetAddSql(cardEntity);

            // 添加数据是否成功
            if (SqliteUtils.Execute(sql))
            {
                SqliteUtils.FillDataToDataSet(SqlUtils.GetQueryAllSql(), DataCache.DsAllCache); // 刷新总数据缓存
                _query.MemoryNumber = cardEntity.Number; // 添加成功后记录添加的编号，以便显示位置
                var memoryCardEntity = _query.MemoryCardEntity;
                UpdateCacheAndUi(memoryCardEntity.Equals(null) ? cardEntity : memoryCardEntity, preOrder);
                BaseDialogUtils.ShowDlg(StringConst.AddSucceed);
                return;
            }
            BaseDialogUtils.ShowDlg(StringConst.AddFailed);
        }

        /// <summary>重置</summary>
        public void ResetClick(string mode)
        {
            var modeType = CardUtils.GetModeType(mode);
            _query.MemoryNumber = string.Empty;
            _view.Reset(modeType);
        }

        /// <summary>更新</summary>
        public void UpdateClick(int selectIndex, string preOrder)
        {
            if (-1 == selectIndex)
            {
                BaseDialogUtils.ShowDlg(StringConst.CardChioceNone);
                return;
            }
            if (!BaseDialogUtils.ShowDlgOkCancel(StringConst.UpdateConfirm)) return;

            var number = DataCache.PreEntityList[selectIndex].Number;
            var updateSql = _query.GetUpdateSql(_view.GetCardEntity(), number);
            if (SqliteUtils.Execute(updateSql))
            {
                SqliteUtils.FillDataToDataSet(SqlUtils.GetQueryAllSql(), DataCache.DsAllCache);
                var cardEntity = _query.MemoryCardEntity;
                UpdateCacheAndUi(cardEntity , preOrder);
                BaseDialogUtils.ShowDlg(StringConst.UpdateSucceed);
                return;
            }
            BaseDialogUtils.ShowDlg(StringConst.UpdateFailed);
        }

        /// <summary>删除</summary>
        public void DeleteClick(int selectIndex, string preOrder)
        {
            if (-1 == selectIndex)
            {
                BaseDialogUtils.ShowDlg(StringConst.CardChioceNone);
                return;
            }
            if (!BaseDialogUtils.ShowDlgOkCancel(StringConst.DeleteConfirm)) return;

            var number = DataCache.PreEntityList[selectIndex].Number;
            var deleteSql = _query.GetDeleteSql(number);
            if (SqliteUtils.Execute(deleteSql))
            {
                SqliteUtils.FillDataToDataSet(SqlUtils.GetQueryAllSql(), DataCache.DsAllCache);
                _query.MemoryNumber = string.Empty; // 删除成功后清空记录的编号，不显示位置
                UpdateCacheAndUi(_query.MemoryCardEntity, preOrder);
                BaseDialogUtils.ShowDlg(StringConst.DeleteSucceed);
                return;
            }
            BaseDialogUtils.ShowDlg(StringConst.DeleteFailed);
        }

        /// <summary>排序发生变化</summary>
        public void PreOrderChanged(string preOrder)
        {
            var memoryCardEntity = _query.MemoryCardEntity;
            if(memoryCardEntity.Equals(null)) return;
            UpdateCacheAndUi(memoryCardEntity ,preOrder);
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

        public void ModeChanged(string mode, string preOrder)
        {
            var modeType = CardUtils.GetModeType(mode);
            var cardEntity = _view.GetCardEntity();
            _query.MemoryCardEntity = cardEntity;
            switch (modeType)
            {
                case Enum.ModeType.Editor:
                {
                    if (!cardEntity.Pack.Equals(string.Empty))
                        UpdateCacheAndUi(cardEntity, preOrder);
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

        /// <summary>
        ///     更新全部数据集合以及ListView
        /// </summary>
        /// <param name="sql">查询的Sql</param>
        /// <param name="queryRestrict">查询的制限</param>
        private void UpdateCacheAndUi(string sql, string queryRestrict)
        {
            var dataSet = new DataSet();
            SqliteUtils.FillDataToDataSet(sql, dataSet);
            _query.SetPreCardList(dataSet, queryRestrict);
            var memoryNumber = _query.MemoryNumber == null ? string.Empty : _query.MemoryNumber;
            _view.UpdatePreListView(DataCache.PreEntityList, _query.MemoryNumber);
        }

        /// <summary>
        ///     更新全部数据集合以及ListView
        /// </summary>
        /// <param name="cardEntity">查询的实例</param>
        /// <param name="preOrder">预览排序</param>
        private void UpdateCacheAndUi(CardEntity cardEntity, string preOrder)
        {
            var sql = _query.GetQuerySql(cardEntity, preOrder);
            UpdateCacheAndUi(sql, cardEntity.Restrict);
        }
    }
}