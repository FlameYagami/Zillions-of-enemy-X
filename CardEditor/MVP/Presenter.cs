using System;
using CardEditor.Constant;
using CardEditor.Utils;
using CardEditor.Utils.Dialog;

namespace CardEditor.MVP
{
    internal class Presenter : Constract.IPresenter
    {
        private readonly Constract.IData _data;
        private readonly Constract.IView _view;

        public Presenter(Constract.IView view)
        {
            _view = view;
            _data = new Data();
        }

        public void Init()
        {
            if (SqliteUtils.FillDataToDataSet(SqliteConst.QueryAllSql, Data.DsAllCache))
            {
                _view.SetPackItems(CardUtils.GetAllPack());
            }
            else
            {
                _view.SetPasswordVisibility(true, false);
                DialogUtils.ShowDlgOk(StringConst.DbOpenError);
            }
            //_view.SetBackground();
        }

        public void PackChanged()
        {
            var pack = _view.GetPack();
            var packNumber = CardUtils.GetPackNumber(pack);
            _view.SetNumber(packNumber);
            if (pack.Contains("P"))
                _view.SetRare(StringConst.RarePr);
        }

        public void TypeChanged()
        {
            var type = _view.GetType();
            switch (type)
            {
                case StringConst.NotApplicable:
                case StringConst.TypeZx:
                case StringConst.TypeZxEx:
                {
                    _view.SetRaceEnabled(true);
                    _view.SetSignEnabled(true);
                    _view.SetCostEnabled(true);
                    _view.SetPowerEnabled(true);
                    _view.SetRace(StringConst.NotApplicable);
                    _view.SetSign(StringConst.NotApplicable);
                    _view.SetCost(string.Empty);
                    _view.SetPower(string.Empty);
                    break;
                }
                case StringConst.TypePlayer:
                {
                    _view.SetRaceEnabled(false);
                    _view.SetSignEnabled(false);
                    _view.SetCostEnabled(false);
                    _view.SetPowerEnabled(false);
                    _view.SetRace(StringConst.Hyphen);
                    _view.SetSign(StringConst.Hyphen);
                    _view.SetCost(string.Empty);
                    _view.SetPower(string.Empty);
                    break;
                }
                case StringConst.TypeEvent:
                {
                    _view.SetRaceEnabled(false);
                    _view.SetSignEnabled(true);
                    _view.SetCostEnabled(false);
                    _view.SetPowerEnabled(true);
                    _view.SetRace(StringConst.Hyphen);
                    _view.SetPower(string.Empty);
                    break;
                }
            }
        }

        public void CampChanged()
        {
            var camp = _view.GetCamp();
            _view.SetRaceEnabled(!camp.Equals(StringConst.NotApplicable));
            if (!camp.Equals(StringConst.NotApplicable))
                _view.SetRaceItems(CardUtils.GetPartRace(camp));
        }

        public void CardPreviewChanged()
        {
            if (0 > _view.GetSelectIndex()) return;
            var listViewMode = Data.CardList[_view.GetSelectIndex()];
            var cardmodel = CardUtils.GetCardModel(listViewMode.Number);
            var imageListUri = CardUtils.GetImageUriList(listViewMode.Number);
            _view.SetCardEntity(cardmodel);
            _view.SetImage(imageListUri);
        }

        /// <summary>检索</summary>
        public void Query()
        {
            var mode = _view.GetMode();
            var order = _view.GetOrder();
            var cardModel = _view.GetCardEntity();
            if (mode.Equals(StringConst.ModeQuery))
            {
                UpdateCacheAndUi(_data.GetQuerySql(cardModel, order));
            }
            else if (mode.Equals(StringConst.ModeEditor))
            {
                if (_view.GetPack().Equals(string.Empty))
                {
                    _view.ShowDialog(StringConst.PackSeleteNone);
                    return;
                }
                UpdateCacheAndUi(_data.GetEditorSql(cardModel, order));
            }
        }

        /// <summary>添加</summary>
        public void AddCard()
        {
            var cardModel = _view.GetCardEntity();
            if (CardUtils.IsNumberExist(cardModel.Number))
            {
                _view.ShowDialog(StringConst.CardIsExitst);
                return;
            }
            if (!DialogUtils.ShowDlgOkCancel(StringConst.AddConfirm)) return;

            var sql = _data.GetAddSql(cardModel);
            if (SqliteUtils.Execute(sql))
            {
                SqliteUtils.FillDataToDataSet(SqliteConst.QueryAllSql, Data.DsAllCache);
                UpdateCacheAndUi(Data.MemoryQuerySql +
                                 (_view.GetOrder().Equals(StringConst.OrderNumber)
                                     ? SqliteConst.NumberOrderSql
                                     : SqliteConst.ValueOrderSql));
                _view.ShowDialog(StringConst.AddSucceed);
            }
            else
            {
                _view.ShowDialog(StringConst.AddFailed);
            }
        }

        /// <summary>重置</summary>
        public void Reset()
        {
            _view.SetType(StringConst.NotApplicable);
            _view.SetCamp(StringConst.NotApplicable);
            _view.SetRace(StringConst.NotApplicable);
            _view.SetSign(StringConst.NotApplicable);
            _view.SetRare(StringConst.NotApplicable);
            _view.SetPack(StringConst.NotApplicable);
            _view.SetLimit(StringConst.NotApplicable);
            _view.SetCName(string.Empty);
            _view.SetJName(string.Empty);
            _view.SetJName(string.Empty);
            _view.SetIllust(string.Empty);
            _view.SetNumber(string.Empty);
            _view.SetCost(string.Empty);
            _view.SetPower(string.Empty);
            _view.SetAbility(string.Empty);
            _view.SetLines(string.Empty);
            _view.SetFaq(string.Empty);
            _view.ResetAbility();
        }

        /// <summary>更新</summary>
        public void UpdateCard()
        {
            var selectIndex = _view.GetSelectIndex();
            if (-1 == selectIndex)
            {
                _view.ShowDialog(StringConst.CardSeleteNone);
                return;
            }
            if (!DialogUtils.ShowDlgOkCancel(StringConst.UpdateConfirm)) return;

            var number = Data.CardList[selectIndex].Number;
            var updateSql = _data.GetUpdateSql(_view.GetCardEntity(), number);
            if (SqliteUtils.Execute(updateSql))
            {
                SqliteUtils.FillDataToDataSet(SqliteConst.QueryAllSql, Data.DsAllCache);
                UpdateCacheAndUi(Data.MemoryQuerySql +
                                 (_view.GetOrder().Equals(StringConst.OrderNumber)
                                     ? SqliteConst.NumberOrderSql
                                     : SqliteConst.ValueOrderSql));
                _view.ShowDialog(StringConst.UpdateSucceed);
            }
            else
            {
                _view.ShowDialog(StringConst.UpdateFailed);
            }
        }

        /// <summary>删除</summary>
        public void Delete()
        {
            var selectIndex = _view.GetSelectIndex();
            if (-1 == selectIndex)
            {
                _view.ShowDialog(StringConst.CardSeleteNone);
                return;
            }
            if (!DialogUtils.ShowDlgOkCancel(StringConst.DeleteConfirm)) return;

            var number = Data.CardList[selectIndex].Number;
            var deleteSql = _data.GetDeleteSql(number);
            if (SqliteUtils.Execute(deleteSql))
            {
                SqliteUtils.FillDataToDataSet(SqliteConst.QueryAllSql, Data.DsAllCache);
                UpdateCacheAndUi(Data.MemoryQuerySql +
                                 (_view.GetOrder().Equals(StringConst.OrderNumber)
                                     ? SqliteConst.NumberOrderSql
                                     : SqliteConst.ValueOrderSql));
                _view.ShowDialog(StringConst.DeleteSucceed);
            }
            else
            {
                _view.ShowDialog(StringConst.DeleteFailed);
            }
        }

        /// <summary>排序发生变化</summary>
        public void Order()
        {
            var memorySql = Data.MemoryQuerySql;
            if (!memorySql.Equals(string.Empty))
                UpdateCacheAndUi(memorySql +
                                 (_view.GetOrder().Equals(StringConst.OrderNumber)
                                     ? SqliteConst.NumberOrderSql
                                     : SqliteConst.ValueOrderSql));
        }

        public void Exit()
        {
            Environment.Exit(0);
        }

        public void EncryptDatabase()
        {
            var password = _view.GetPassword();
            if (password.Equals(string.Empty))
            {
                DialogUtils.ShowDlgOk(StringConst.PasswordHint);
                return;
            }
            if (SqliteUtils.Encrypt(Data.DsAllCache))
            {
                _view.SetPasswordVisibility(false, true);
                DialogUtils.ShowDlg(StringConst.EncryptSucced);
            }
            else
            {
                DialogUtils.ShowDlgOk(StringConst.EncryptFailed);
            }
        }

        public void DecryptDatabase()
        {
            var password = _view.GetPassword();
            if (password.Equals(string.Empty))
            {
                DialogUtils.ShowDlgOk(StringConst.PasswordHint);
                return;
            }
            if (SqliteUtils.Decrypt())
            {
                _view.SetPasswordVisibility(true, false);
                DialogUtils.ShowDlg(StringConst.DncryptSucced);
            }
            else
            {
                DialogUtils.ShowDlgOk(StringConst.DncryptFailed);
            }
        }

        /// <summary>
        ///     更新全部数据集合以及ListView
        /// </summary>
        /// <param name="sql">查询语句</param>
        private void UpdateCacheAndUi(string sql)
        {
            SqliteUtils.FillDataToDataSet(sql, Data.DsPartCache);
            _data.SetCardList();
            _view.UpdateListView(Data.CardList);
        }
    }
}