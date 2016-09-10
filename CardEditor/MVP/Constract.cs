using System.Collections.Generic;
using CardEditor.Entity;

namespace CardEditor.MVP
{
    public class Constract
    {
        public interface IData
        {
            void SetCardList();
            string GetQuerySql(CardEntity cardEntity, string order);
            string GetEditorSql(CardEntity cardEntity, string order);
            string GetUpdateSql(CardEntity cardEntity, string number);
            string GetAddSql(CardEntity cardEntity);
            string GetDeleteSql(string number);
        }

        public interface IView
        {
            void SetBackground();
            void SetType(string message);
            void SetCamp(string message);
            void SetRace(string message);
            void SetSign(string message);
            void SetRare(string message);
            void SetCName(string message);
            void SetJName(string message);
            void SetIllust(string message);
            void SetPack(string message);
            void SetNumber(string message);
            void SetCost(string message);
            void SetPower(string message);
            void SetCampEnabled(bool isEnabled);
            void SetRaceEnabled(bool isEnabled);
            void SetSignEnabled(bool isEnabled);
            void SetCostEnabled(bool isEnabled);
            void SetPowerEnabled(bool isEnabled);
            void SetRaceItems(List<object> itemList);
            void SetPackItems(List<object> itemList);
            void UpdateListView(List<PreviewEntity> cardList);
            void SetCardEntity(CardEntity cardmodel);
            void SetPasswordVisibility(bool isEncryptVisible, bool isDecryptVisible);
            void ShowDialog(string v);
            void SetAbility(string message);
            void SetLimit(string message);
            void SetLines(string message);
            void SetFaq(string message);
            int GetSelectIndex();
            string GetPack();
            string GetType();
            string GetCamp();
            string GetOrder();
            string GetPassword();
            string GetMode();
            CardEntity GetCardEntity();
            void SetImage(List<string> imageListUri);
            void ResetAbility();
        }

        public interface IPresenter
        {
            void Query();
            void AddCard();
            void PackChanged();
            void TypeChanged();
            void Exit();
            void Reset();
            void CampChanged();
            void CardPreviewChanged();
            void Delete();
            void UpdateCard();
            void EncryptDatabase();
            void DecryptDatabase();
            void Order();
            void Init();
        }
    }
}