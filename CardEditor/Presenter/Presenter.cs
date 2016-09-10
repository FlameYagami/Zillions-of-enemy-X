using System;
using CardEditor.Constant;
using CardEditor.Model;
using CardEditor.Utils;
using CardEditor.View;

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
            if (SqliteUtils.FillDataToDataSet(SqliteConst.QueryAllSql, DataCache.DsAllCache))
            {
                _view.SetPackItems(CardUtils.GetAllPack());
            }
            else
            {
                _view.SetPasswordVisibility(true, false);
                DialogUtils.ShowDlgOk(StringConst.DbOpenError);
            }
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
            if (!camp.Equals(StringConst.NotApplicable))
                _view.UpdateCampLinkage(CardUtils.GetPartRace(camp));
        }

        public void PreviewChanged(int selectIndex)
        {
            if (0 > selectIndex) return;
            var listViewMode = Query.CardList[selectIndex];
            var cardmodel = CardUtils.GetCardModel(listViewMode.Number);
            var imageListUri = CardUtils.GetImageUriList(listViewMode.Number);
            _view.SetCardEntity(cardmodel);
            _view.SetPicture(imageListUri);
        }

        /// <summary>检索</summary>
        public void QueryClick(string mode, string order, string pack)
        {
            var cardModel = _view.GetCardEntity();
            if (mode.Equals(StringConst.ModeQuery))
            {
                UpdateCacheAndUi(_query.GetQuerySql(cardModel, order));
            }
            else if (mode.Equals(StringConst.ModeEditor))
            {
                if (pack.Equals(string.Empty))
                {
                    DialogUtils.ShowDlg(StringConst.PackSeleteNone);
                    return;
                }
                UpdateCacheAndUi(_query.GetEditorSql(cardModel, order));
            }
        }

        /// <summary>添加</summary>
        public void AddClick(string order)
        {
            var cardModel = _view.GetCardEntity();
            if (CardUtils.IsNumberExist(cardModel.Number))
            {
                DialogUtils.ShowDlg(StringConst.CardIsExitst);
                return;
            }
            if (!DialogUtils.ShowDlgOkCancel(StringConst.AddConfirm)) return;

            var sql = _query.GetAddSql(cardModel);
            if (SqliteUtils.Execute(sql))
            {
                SqliteUtils.FillDataToDataSet(SqliteConst.QueryAllSql, DataCache.DsAllCache);
                UpdateCacheAndUi(Query.MemoryQuerySql +
                                 (order.Equals(StringConst.OrderNumber)
                                     ? SqliteConst.NumberOrderSql
                                     : SqliteConst.ValueOrderSql));
                DialogUtils.ShowDlg(StringConst.AddSucceed);
                return;
            }
            DialogUtils.ShowDlg(StringConst.AddFailed);
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
                DialogUtils.ShowDlg(StringConst.CardSeleteNone);
                return;
            }
            if (!DialogUtils.ShowDlgOkCancel(StringConst.UpdateConfirm)) return;

            var number = Query.CardList[selectIndex].Number;
            var updateSql = _query.GetUpdateSql(_view.GetCardEntity(), number);
            if (SqliteUtils.Execute(updateSql))
            {
                SqliteUtils.FillDataToDataSet(SqliteConst.QueryAllSql, DataCache.DsAllCache);
                UpdateCacheAndUi(Query.MemoryQuerySql +
                                 (order.Equals(StringConst.OrderNumber)
                                     ? SqliteConst.NumberOrderSql
                                     : SqliteConst.ValueOrderSql));
                DialogUtils.ShowDlg(StringConst.UpdateSucceed);
                return;
            }
            DialogUtils.ShowDlg(StringConst.UpdateFailed);
        }

        /// <summary>删除</summary>
        public void DeleteClick(int selectIndex, string order)
        {
            if (-1 == selectIndex)
            {
                DialogUtils.ShowDlg(StringConst.CardSeleteNone);
                return;
            }
            if (!DialogUtils.ShowDlgOkCancel(StringConst.DeleteConfirm)) return;

            var number = Query.CardList[selectIndex].Number;
            var deleteSql = _query.GetDeleteSql(number);
            if (SqliteUtils.Execute(deleteSql))
            {
                SqliteUtils.FillDataToDataSet(SqliteConst.QueryAllSql, DataCache.DsAllCache);
                UpdateCacheAndUi(Query.MemoryQuerySql +
                                 (order.Equals(StringConst.OrderNumber)
                                     ? SqliteConst.NumberOrderSql
                                     : SqliteConst.ValueOrderSql));
                DialogUtils.ShowDlg(StringConst.DeleteSucceed);
                return;
            }
            DialogUtils.ShowDlg(StringConst.DeleteFailed);
        }

        /// <summary>排序发生变化</summary>
        public void OrderChanged(string order)
        {
            var memorySql = Query.MemoryQuerySql;
            if (!memorySql.Equals(string.Empty))
                UpdateCacheAndUi(memorySql +
                                 (order.Equals(StringConst.OrderNumber)
                                     ? SqliteConst.NumberOrderSql
                                     : SqliteConst.ValueOrderSql));
        }

        public void ExitClick()
        {
            Environment.Exit(0);
        }

        public void EncryptDatabaseClick(string password)
        {
            if (password.Equals(string.Empty))
            {
                DialogUtils.ShowDlgOk(StringConst.PasswordHint);
                return;
            }
            if (SqliteUtils.Encrypt(DataCache.DsAllCache))
            {
                _view.SetPasswordVisibility(false, true);
                DialogUtils.ShowDlg(StringConst.EncryptSucced);
                return;
            }
            DialogUtils.ShowDlgOk(StringConst.EncryptFailed);
        }

        public void DecryptDatabaseClick(string password)
        {
            if (password.Equals(string.Empty))
            {
                DialogUtils.ShowDlgOk(StringConst.PasswordHint);
                return;
            }
            if (SqliteUtils.Decrypt())
            {
                _view.SetPasswordVisibility(true, false);
                DialogUtils.ShowDlg(StringConst.DncryptSucced);
                return;
            }
            DialogUtils.ShowDlgOk(StringConst.DncryptFailed);
        }

        /// <summary>
        ///     更新全部数据集合以及ListView
        /// </summary>
        /// <param name="sql">查询语句</param>
        private void UpdateCacheAndUi(string sql)
        {
            SqliteUtils.FillDataToDataSet(sql, DataCache.DsPartCache);
            _query.SetCardList();
            _view.UpdateListView(Query.CardList);
        }
    }
}