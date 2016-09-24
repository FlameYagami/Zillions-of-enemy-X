using System;
using System.Data;
using System.IO;
using System.Windows.Controls;
using DeckEditor.Constant;
using DeckEditor.Model;
using DeckEditor.Utils;
using DeckEditor.View;
using Dialog;

namespace DeckEditor.Presenter
{
    public interface IPresenter
    {
        void ExitClick();
        void ResetClick();
        void QueryClick(StringConst.PreviewOrderType previewOrderType);
        void SaveClick(string deckName);
        void ResaveClick(string deckName);
        void DeleteClick(string deckName);
        void DeckNameChanged(string deckName);
        void ValueOrder();
        void RandomOrder();
        void ClearClick();
        void Init();
        void ShowAbilityDetail();
        void PreivewListViewChanged(int selectIndex);
        void OrderClick(StringConst.PreviewOrderType previewOrderType);
        void PreviewMouseRightClick(Grid grid, int selectedIndex);
        void ImgAreaMouseRightClick(Grid grid);
        void ImgAreaMouseLeftClick(Grid grid);
        void PictureMouseRightClick(Image image);
        void CampChanged(string camp);
        void ShowAllDeckName();
        void DeckStatisticalClick();
        void ImgAreaMouseDoubleClick(Grid grid);
    }

    internal class Presenter : IPresenter
    {
        private readonly IDeck _deck;
        private readonly IQuery _query;
        private readonly IView _view;

        public Presenter(IView view)
        {
            _view = view;
            _query = new Query();
            _deck = new Deck();
        }

        public void Init()
        {
            if (SqliteUtils.FillDataToDataSet(SqliteConst.QueryAllSql, DataCache.DsAllCache))
            {
                _view.Init();
                if (!Directory.Exists(Const.DeckFolderPath))
                    Directory.CreateDirectory(Const.DeckFolderPath);
            }
            else
                BaseDialogUtils.ShowDlgOk(StringConst.DbOpenError);
        }

        public void ExitClick()
        {
            Environment.Exit(0);
        }

        public void ResetClick()
        {
            _view.Reset();
        }

        public void QueryClick(StringConst.PreviewOrderType previewOrderType)
        {
            var cardModel = _view.GetCardModel();
            var querySql = _query.GetQuerySql(cardModel, previewOrderType);
            UpdateCacheAndUi(querySql);
        }

        public void OrderClick(StringConst.PreviewOrderType previewOrderType)
        {
            var memorySql = _query.QuerySqlMemory;
            if (memorySql.Equals(string.Empty)) return;
            UpdateCacheAndUi(memorySql +
                             (previewOrderType.Equals(StringConst.PreviewOrderType.Number)
                                 ? SqliteConst.OrderNumberSql
                                 : SqliteConst.OrderValueSql));
        }

        public void ResaveClick(string deckName)
        {
            var isResaved = _deck.Resave(deckName);
            if (!isResaved) return;
            BaseDialogUtils.ShowDlg(StringConst.ResaveSucceed);
        }

        public void DeleteClick(string deckName)
        {
            var isDeleted = _deck.Delete(deckName);
            if (!isDeleted) return;
            ClearClick();
            _view.SetDeckName(string.Empty);
            BaseDialogUtils.ShowDlg(StringConst.DeleteSucceed);
        }

        public void ValueOrder()
        {
            _deck.Order(StringConst.DeckOrderType.Value);
            UpdateDeckUi();
        }

        public void RandomOrder()
        {
            _deck.Order(StringConst.DeckOrderType.Random);
            UpdateDeckUi();
        }

        public void ClearClick()
        {
            DataCache.PlColl.Clear();
            DataCache.IgColl.Clear();
            DataCache.UgColl.Clear();
            DataCache.ExColl.Clear();
            UpdateDeckUi();
        }

        public void ShowAbilityDetail()
        {
            DialogUtils.ShowAbilityDetail();
        }

        public void PreivewListViewChanged(int selectIndex)
        {
            if (selectIndex.Equals(-1)) return;
            var infoMode = DataCache.InfoColl[selectIndex];
            var number = infoMode.Number;
            var cardmodel = CardUtils.GetCardModel(number);
            var numberList = CardUtils.GetNumberList(number);
            var picturePathList = CardUtils.GetPicturePathList(number);
            _view.SetCardModel(cardmodel);
            _view.SetPicture(numberList, picturePathList);
        }

        public void PreviewMouseRightClick(Grid grid, int selectIndex)
        {
            var label = (Label) grid?.FindName(StringConst.LblPreviewNumber);
            if (label == null) return;
            var number = label.Content.ToString();
            var thumbnailPath = _deck.GetAddThumbnailPath(number, selectIndex);

            AddCard(number, thumbnailPath);
        }

        public void ImgAreaMouseRightClick(Grid grid)
        {
            var label = (Label) grid?.FindName(StringConst.LblAreaNumber);
            if (label == null) return;
            var number = label.Content.ToString();

            var areaType = CardUtils.GetAreaType(number);
            // 删除非玩家卡
            switch (areaType)
            {
                case StringConst.AreaType.Pl:
                    _deck.DeleteEntityFromColl(number, DataCache.PlColl);
                    _view.UpdateDeckListView(areaType, DataCache.PlColl);
                    break;
                case StringConst.AreaType.Ig:
                    _deck.DeleteEntityFromColl(number, DataCache.IgColl);
                    _view.UpdateDeckListView(areaType, DataCache.IgColl);
                    _view.UpdateStartAndLifeAndVoid(CardUtils.GetStartAndLifeAndVoidCount());
                    break;
                case StringConst.AreaType.Ug:
                    _deck.DeleteEntityFromColl(number, DataCache.UgColl);
                    _view.UpdateDeckListView(areaType, DataCache.UgColl);
                    _view.UpdateStartAndLifeAndVoid(CardUtils.GetStartAndLifeAndVoidCount());
                    break;
                case StringConst.AreaType.Ex:
                    _deck.DeleteEntityFromColl(number, DataCache.ExColl);
                    _view.UpdateDeckListView(areaType, DataCache.ExColl);
                    break;
            }
        }

        public void ImgAreaMouseLeftClick(Grid grid)
        {
            var label = (Label) grid?.FindName(StringConst.LblAreaNumber);
            if (label == null) return;
            var number = label.Content.ToString();
            var cardmodel = CardUtils.GetCardModel(number);
            var numberList = CardUtils.GetNumberList(number);
            var picturePathList = CardUtils.GetPicturePathList(number);
            _view.SetCardModel(cardmodel);
            _view.SetPicture(numberList, picturePathList);
        }

        public void PictureMouseRightClick(Image image)
        {
            var number = image.Tag.ToString();
            var thumbnailPath = Const.ThumbnailPath + number + StringConst.ImageExtension;
            AddCard(number, thumbnailPath);
        }

        public void CampChanged(string camp)
        {
            _view.SetRaceItems(camp.Equals(StringConst.NotApplicable) ? null : CardUtils.GetPartRace(camp));
        }

        public void SaveClick(string deckName)
        {
            var isSaved = _deck.Save(deckName);
            BaseDialogUtils.ShowDlg(isSaved ? StringConst.SaveSucceed : StringConst.DeckNameNone);
        }

        public void DeckNameChanged(string deckName)
        {
            if (deckName.Equals(string.Empty)) return;
            ClearClick();
            _deck.Load(deckName);
            UpdateDeckUi();
        }

        public void ShowAllDeckName()
        {
            var deckNameList = _deck.GetDeckNameList();
            _view.SetDeckName(deckNameList);
        }

        public void DeckStatisticalClick()
        {
            if (DataCache.IgColl.Count.Equals(0) && DataCache.UgColl.Count.Equals(0))
                return;
            var dekcStatisticalDic = _deck.DekcStatistical();
            DialogUtils.ShowDekcStatistical(dekcStatisticalDic);
        }

        public void ImgAreaMouseDoubleClick(Grid grid)
        {
            var label = (Label) grid?.FindName(StringConst.LblAreaNumber);
            if (label == null) return;
            var image = (Image) grid?.FindName(StringConst.ImgAreaThumbnail);
            if (image == null) return;

            var number = label.Content.ToString();
            var thumbnailPath = image.Source.ToString();
            AddCard(number, thumbnailPath);
        }

        private void AddCard(string number, string thumbnailPath)
        {
            var areaType = CardUtils.GetAreaType(number);
            // 添加卡
            if (StringConst.AreaType.None.Equals(_deck.AddCard(areaType, number, thumbnailPath))) return;
            // 添加成功则更新该区域
            switch (areaType)
            {
                case StringConst.AreaType.Pl:
                    _view.UpdateDeckListView(areaType, DataCache.PlColl);
                    break;
                case StringConst.AreaType.Ig:
                    _view.UpdateDeckListView(areaType, DataCache.IgColl);
                    _view.UpdateStartAndLifeAndVoid(CardUtils.GetStartAndLifeAndVoidCount());
                    break;
                case StringConst.AreaType.Ug:
                    _view.UpdateDeckListView(areaType, DataCache.UgColl);
                    _view.UpdateStartAndLifeAndVoid(CardUtils.GetStartAndLifeAndVoidCount());
                    break;
                case StringConst.AreaType.Ex:
                    _view.UpdateDeckListView(areaType, DataCache.ExColl);
                    break;
            }
        }

        /// <summary>
        ///     更新全部数据集合以及ListView
        /// </summary>
        /// <param name="sql">查询语句</param>
        private void UpdateCacheAndUi(string sql)
        {
            var dsPartCache = new DataSet(); // 部分数据
            SqliteUtils.FillDataToDataSet(sql, dsPartCache);
            _query.SetCardList(dsPartCache);
            _view.UpdateCardPreviewListView(DataCache.InfoColl);
        }

        private void UpdateDeckUi()
        {
            _view.UpdateDeckListView(StringConst.AreaType.Pl, DataCache.PlColl);
            _view.UpdateDeckListView(StringConst.AreaType.Ig, DataCache.IgColl);
            _view.UpdateDeckListView(StringConst.AreaType.Ug, DataCache.UgColl);
            _view.UpdateDeckListView(StringConst.AreaType.Ex, DataCache.ExColl);
            _view.UpdateStartAndLifeAndVoid(CardUtils.GetStartAndLifeAndVoidCount());
        }
    }
}