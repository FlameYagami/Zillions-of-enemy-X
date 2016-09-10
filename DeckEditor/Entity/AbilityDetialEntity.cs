using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DeckEditor.Entity
{
    public class AbilityDetialEntity
    {
        private Dictionary<string, int> _abilityDetailDic = new Dictionary<string, int>
        {
            {"资源联动", 0},
            {"方阵联动", 0},
            {"充能联动", 0},
            {"卡片登场", 0},
            {"卡片破坏", 0},
            {"卡片除外", 0},
            {"返回手牌", 0},
            {"返回卡组", 0},
            {"资源放置", 0},
            {"方阵置换", 0},
            {"充能放置", 0},
            {"废弃放置", 0},
            {"重启休眠", 0},
            {"抽卡辅助", 0},
            {"卡组检索", 0},
            {"充能上限", 0},
            {"玩家相关", 0},
            {"费用相关", 0},
            {"力量相关", 0},
            {"种族相关", 0},
            {"伤害相关", 0},
            {"原力相关", 0},
            {"标记相关", 0},
            {"生命相关", 0},
            {"特殊胜利", 0}
        };

        public Dictionary<string, int> GetAbilityDetailDic()
        {
            return _abilityDetailDic;
        }

        public void SetAbilityDetailDic(IEnumerable<CheckBox> checkboxIEnumerable)
        {
            var tempAbilityDetailDic = new Dictionary<string, int>();
            foreach (var abilityDetail in _abilityDetailDic)
                foreach (var checkbox in checkboxIEnumerable)
                    if (abilityDetail.Key.Equals(checkbox.Content))
                    {
                        tempAbilityDetailDic.Add(abilityDetail.Key, (bool) checkbox.IsChecked ? 1 : 0);
                        break;
                    }
            _abilityDetailDic = tempAbilityDetailDic;
        }

        public void ResetAbilityDetailDic()
        {
            var tempAbilityDetailDic = _abilityDetailDic.ToDictionary(abilityDetail => abilityDetail.Key,
                abilityDetail => 0);
            _abilityDetailDic = tempAbilityDetailDic;
        }
    }
}