using System;
using System.Data;
using System.IO;
using System.Windows.Controls;
using DeckEditor.Constant;
using DeckEditor.Entity;
using DeckEditor.Model;
using DeckEditor.Utils;
using DeckEditor.View;
using Dialog;
using Common;
using System.Collections.Generic;

namespace DeckEditor.Presenter
{
    public interface IPresenter
    {
        void ExitClick();
        void ResetClick();
        void QueryClick(CardEntity cardEntity, string order);
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
        void OrderClick(string oder);
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
            if (SqliteUtils.FillDataToDataSet(SqlUtils.GetQueryAllSql(), DataCache.DsAllCache))
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

        public void QueryClick(CardEntity cardEntity, string order)
        {
            var querySql = _query.GetQuerySql(cardEntity, CardUtils.GetPreviewOrderType(order));
            UpdateCacheAndUi(querySql);
        }

        public void OrderClick(string order)
        {
            var memorySql = _query.QuerySqlMemory;
            if (memorySql.Equals(string.Empty)) return;
            UpdateCacheAndUi(memorySql + SqlUtils.GetFooterSql(CardUtils.GetPreviewOrderType(order)));
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
            UpdateDeckListView();
        }

        public void RandomOrder()
        {
            _deck.Order(StringConst.DeckOrderType.Random);
            UpdateDeckListView();
        }

        public void ClearClick()
        {
            DataCache.PlColl.Clear();
            DataCache.IgColl.Clear();
            DataCache.UgColl.Clear();
            DataCache.ExColl.Clear();
            UpdateDeckListView();
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
            var cardmodel = CardUtils.GetCardEntity(number);
            var numberExList = CardUtils.GetNumberExList(infoMode.ImageJson);
            var picturePathList = CardUtils.GetPicturePathList(infoMode.ImageJson);
            _view.SetCardModel(cardmodel);
            _view.SetPicture(numberExList, picturePathList);
        }

        public void PreviewMouseRightClick(Grid grid, int selectIndex)
        {
            var lblImageJson = (Label)grid?.FindName(StringConst.LblPreviewImageJson);
            if (lblImageJson == null) return;
            var imageJson = lblImageJson.Content.ToString();
            var numberEx = JsonUtils.JsonDeserialize<List<string>>(imageJson)[selectIndex].Replace("/","").Replace(StringConst.ImageExtension,"");
            var thumbnailPath = $"{Const.ThumbnailPath}/{ numberEx}{StringConst.ImageExtension}";
            BaseAdd(numberEx, thumbnailPath);
        }

        public void ImgAreaMouseRightClick(Grid grid)
        {
            var label = (Label) grid?.FindName(StringConst.LblAreaNumberEx);
            if (label == null) return;
            var numberEx = label.Content.ToString();
            var areaType = CardUtils.GetAreaType(numberEx);
            
            // 删除非玩家卡
            switch (areaType)
            {
                case StringConst.AreaType.Pl:
                    _deck.DeleteEntityFromColl(numberEx, DataCache.PlColl);
                    _view.UpdateDeckListView(areaType, DataCache.PlColl);
                    break;
                case StringConst.AreaType.Ig:
                    _deck.DeleteEntityFromColl(numberEx, DataCache.IgColl);
                    _view.UpdateDeckListView(areaType, DataCache.IgColl);
                    _view.UpdateStartAndLifeAndVoid(CardUtils.GetStartAndLifeAndVoidCount());
                    break;
                case StringConst.AreaType.Ug:
                    _deck.DeleteEntityFromColl(numberEx, DataCache.UgColl);
                    _view.UpdateDeckListView(areaType, DataCache.UgColl);
                    _view.UpdateStartAndLifeAndVoid(CardUtils.GetStartAndLifeAndVoidCount());
                    break;
                case StringConst.AreaType.Ex:
                    _deck.DeleteEntityFromColl(numberEx, DataCache.ExColl);
                    _view.UpdateDeckListView(areaType, DataCache.ExColl);
                    break;
            }
        }

        public void ImgAreaMouseLeftClick(Grid grid)
        {
            var label = (Label) grid?.FindName(StringConst.LblAreaNumberEx);
            var lblAreaImageJson = (Label)grid?.FindName(StringConst.LblAreaImageJson);
            if (label == null || lblAreaImageJson == null) return;
            var numberEx = label.Content.ToString();
            var imageJson = lblAreaImageJson.Content.ToString();
            var cardmodel = CardUtils.GetCardEntity(numberEx);
            var numberExList = CardUtils.GetNumberExList(imageJson);
            var picturePathList = CardUtils.GetPicturePathList(imageJson);
            _view.SetCardModel(cardmodel);
            _view.SetPicture(numberExList, picturePathList);
        }

        public void PictureMouseRightClick(Image image)
        {
            var number = image.Tag.ToString();
            var thumbnailPath = Const.ThumbnailPath + number + StringConst.ImageExtension;
            BaseAdd(number, thumbnailPath);
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
            UpdateDeckListView();
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
            var label = (Label) grid?.FindName(StringConst.LblAreaNumberEx);
            var image = (Image)grid?.FindName(StringConst.ImgAreaThumbnail);
            if (label == null  || image == null) return;

            var number = label.Content.ToString();
            var thumbnailPath = image.Source.ToString();
            BaseAdd(number, thumbnailPath);
        }

        private void BaseAdd(string number,string thumbnailPath)
        {
            var areaType = CardUtils.GetAreaType(number);
            // 添加卡
            var isAddSucceed = _deck.AddCard(areaType, number, thumbnailPath);
            if (StringConst.AreaType.None.Equals(isAddSucceed)) return;
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
            var dataSet = new DataSet();
            SqliteUtils.FillDataToDataSet(sql, dataSet);
            _query.SetCardList(dataSet);
            _view.UpdatePreviewListView(DataCache.InfoColl);
        }

        private void UpdateDeckListView()
        {
            _view.UpdateDeckListView(StringConst.AreaType.Pl, DataCache.PlColl);
            _view.UpdateDeckListView(StringConst.AreaType.Ig, DataCache.IgColl);
            _view.UpdateDeckListView(StringConst.AreaType.Ug, DataCache.UgColl);
            _view.UpdateDeckListView(StringConst.AreaType.Ex, DataCache.ExColl);
            _view.UpdateStartAndLifeAndVoid(CardUtils.GetStartAndLifeAndVoidCount());
        }
    }
}