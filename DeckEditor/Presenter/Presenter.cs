using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using DeckEditor.Model;
using DeckEditor.Utils;
using DeckEditor.View;
using Dialog;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;
using Enum = Wrapper.Constant.Enum;

namespace DeckEditor.Presenter
{
    public interface IPresenter
    {
        void ExitClick();
        void ResetClick();
        void QueryClick(CardQueryModel card);
        void SaveClick(string deckName);
        void ResaveClick(string deckName);
        void DeleteClick(string deckName);
        void DeckNameChanged(string deckName);
        void ValueOrder();
        void RandomOrder();
        void DeckClearClick();
        void Init();
        void ShowAbilityDetail();
        void PreivewListViewChanged(object selectItem);
        void OrderClick(string oder);
        void CardPreviewItemMouseRightClick(string imageJson,int selectedIndex);
        void DeckItemMouseRightClick(string numberEx);
        void DeckItemMouseLeftClick(string numberEx);
        void PictureMouseRightClick(string numberEx);
        void CampChanged(string camp);
        void ShowAllDeckName();
        void DeckStatisticalClick();
        void DeckItemMouseDoubleClick(string numberEx);
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
                if (!Directory.Exists(PathManager.DeckFolderPath))
                    Directory.CreateDirectory(PathManager.DeckFolderPath);
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

        public void QueryClick(CardQueryModel card)
        {
            UpdateCacheAndUi(card);
        }

        public void OrderClick(string order)
        {
            if (null == _query.MemoryCardQueryModel) return;
            UpdateCacheAndUi(_query.MemoryCardQueryModel);
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
            _deck.ClearDeck();
            _view.SetDeckName(string.Empty);
            BaseDialogUtils.ShowDlg(StringConst.DeleteSucceed);
        }

        public void ValueOrder()
        {
            _deck.Order(Enum.DeckOrderType.Value);
            UpdateDeckListView();
        }

        public void RandomOrder()
        {
            _deck.Order(Enum.DeckOrderType.Random);
            UpdateDeckListView();
        }

        public void DeckClearClick()
        {
            _deck.ClearDeck();
            UpdateDeckListView();
        }

        public void ShowAbilityDetail()
        {
            DialogUtils.ShowAbilityDetail();
        }

        public void PreivewListViewChanged(object selectItem)
        {
            var previewModel = selectItem as CardPreviewModel;
            if (null == previewModel) return;
            var cardmodel = CardUtils.GetCardModel(previewModel.Number);
            var numberExList = CardUtils.GetNumberExList(previewModel.ImageJson);
            var picturePathList = CardUtils.GetPicturePathList(previewModel.ImageJson);
            _view.SetCardModel(cardmodel);
            _view.SetPicture(numberExList, picturePathList);
        }

        public void CardPreviewItemMouseRightClick(string imageJson, int selectIndex)
        {
            var numberEx =
                JsonUtils.JsonDeserialize<List<string>>(imageJson)[selectIndex].Replace("/", "")
                    .Replace(StringConst.ImageExtension, "");
            BaseAdd(numberEx);
        }

        public void DeckItemMouseRightClick(string numberEx)
        {
            var areaType = CardUtils.GetAreaType(numberEx);

            if (Enum.AreaType.None == areaType) return;
            // 删除非玩家卡
            _deck.DeleteCard(numberEx, areaType);
            var deckModelList = _deck.GetDeckModelList(areaType);
            _view.UpdateDeckListView(areaType, deckModelList);
            if (areaType.Equals(Enum.AreaType.Ig) || areaType.Equals(Enum.AreaType.Ug))
                _view.UpdateStartAndLifeAndVoid(_deck.GetStartAndLifeAndVoidCount());
        }

        public void DeckItemMouseLeftClick(string numberEx)
        {
            var cardmodel = CardUtils.GetCardModel(numberEx);
            var numberExList = CardUtils.GetNumberExList(cardmodel.ImageJson);
            var picturePathList = CardUtils.GetPicturePathList(cardmodel.ImageJson);
            _view.SetCardModel(cardmodel);
            _view.SetPicture(numberExList, picturePathList);
        }

        public void PictureMouseRightClick(string numberEx)
        {
            BaseAdd(numberEx);
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
            _deck.ClearDeck();
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
            var dekcStatisticalDic = _deck.DekcStatistical();
            DialogUtils.ShowDekcStatistical(dekcStatisticalDic);
        }

        public void DeckItemMouseDoubleClick(string numberEx)
        {
            BaseAdd(numberEx);
        }

        private void BaseAdd(string numberEx)
        {
            var thumbnailPath = CardUtils.GetThumbnailPath(numberEx);
            var areaType = CardUtils.GetAreaType(numberEx);
            // 添加卡
            var isAddSucceed = _deck.AddCard(areaType, numberEx, thumbnailPath);
            if (Enum.AreaType.None.Equals(isAddSucceed)) return;
            // 添加成功则更新该区域
            var deckModelList = _deck.GetDeckModelList(areaType);
            _view.UpdateDeckListView(areaType, deckModelList);
            if (areaType.Equals(Enum.AreaType.Ig) || areaType.Equals(Enum.AreaType.Ug))
                _view.UpdateStartAndLifeAndVoid(_deck.GetStartAndLifeAndVoidCount());
        }

        /// <summary>
        ///     更新全部数据集合以及ListView
        /// </summary>
        /// <param name="card">查询的实例</param>
        private void UpdateCacheAndUi(CardQueryModel card)
        {
            var sql = _query.GetQuerySql(card);
            var previewCardList = _query.GetCardPreviewList(sql, card.Restrict);
            _view.UpdatePreviewListView(previewCardList);
        }

        private void UpdateDeckListView()
        {
            _view.UpdateDeckListView(Enum.AreaType.Pl, _deck.GetDeckModelList(Enum.AreaType.Pl));
            _view.UpdateDeckListView(Enum.AreaType.Ig, _deck.GetDeckModelList(Enum.AreaType.Ig));
            _view.UpdateDeckListView(Enum.AreaType.Ug, _deck.GetDeckModelList(Enum.AreaType.Ug));
            _view.UpdateDeckListView(Enum.AreaType.Ex, _deck.GetDeckModelList(Enum.AreaType.Ex));
            _view.UpdateStartAndLifeAndVoid(_deck.GetStartAndLifeAndVoidCount());
        }
    }
}