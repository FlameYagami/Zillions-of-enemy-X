using System.Collections.Generic;
using System.Linq;

namespace Wrapper.Model
{
    public class AbilityDetialModel
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

        public AbilityDetialModel()
        {
        }

        /// <summary>
        ///     构建详细能力实例
        /// </summary>
        /// <param name="checkboxDic">CheckBox属性构建的数据字典</param>
        /// <param name="isModle">是否需要用到私有属性构建模型</param>
        public AbilityDetialModel(Dictionary<string, bool> checkboxDic, bool isModle = false)
        {
            if (!isModle)
            {
                var tempAbilityDetailDic = new Dictionary<string, int>();
                foreach (var abilityDetail in _abilityDetailDic)
                    foreach (var entity in checkboxDic)
                        if (abilityDetail.Key.Equals(entity.Key))
                        {
                            tempAbilityDetailDic.Add(abilityDetail.Key, entity.Value ? 1 : 0);
                            break;
                        }
                _abilityDetailDic = tempAbilityDetailDic;
            }
            else
            {
                foreach (var entity in checkboxDic)
                    foreach (var properties in GetType().GetProperties())
                    {
                        if (!properties.Name.ToLower().Equals(entity.Key.ToLower())) continue;
                        properties.SetValue(this, entity.Value ? 1 : 0);
                        break;
                    }
            }
        }

        // 反向解析属性
        public int 资源联动 { get; set; }
        public int 方阵联动 { get; set; }
        public int 充能联动 { get; set; }
        public int 卡片登场 { get; set; }
        public int 卡片破坏 { get; set; }
        public int 卡片除外 { get; set; }
        public int 返回手牌 { get; set; }
        public int 返回卡组 { get; set; }
        public int 资源放置 { get; set; }
        public int 方阵置换 { get; set; }
        public int 充能放置 { get; set; }
        public int 废弃放置 { get; set; }
        public int 重启休眠 { get; set; }
        public int 抽卡辅助 { get; set; }
        public int 卡组检索 { get; set; }
        public int 充能上限 { get; set; }
        public int 玩家相关 { get; set; }
        public int 费用相关 { get; set; }
        public int 力量相关 { get; set; }
        public int 种族相关 { get; set; }
        public int 伤害相关 { get; set; }
        public int 原力相关 { get; set; }
        public int 标记相关 { get; set; }
        public int 生命相关 { get; set; }
        public int 特殊胜利 { get; set; }

        /// <summary>
        ///     获取详细能力数据字典
        /// </summary>
        /// <param name="isModel">是否需要用到私有属性构建模型</param>
        /// <returns></returns>
        public Dictionary<string, int> GetAbilityDetailDic(bool isModel = false)
        {
            if (!isModel) return _abilityDetailDic;
            var tempAbilityDetailDic = new Dictionary<string, int>();
            foreach (var abilityDetailItem in _abilityDetailDic)
                foreach (var properties in GetType().GetProperties())
                {
                    if (!properties.Name.ToLower().Equals(abilityDetailItem.Key)) continue;
                    tempAbilityDetailDic.Add(abilityDetailItem.Key, (int) properties.GetValue(this));
                    break;
                }
            _abilityDetailDic = tempAbilityDetailDic;
            return _abilityDetailDic;
        }

        /// <summary>
        ///     获取详细能力数据字典
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, bool> GetAbilityDetailExDic()
        {
            return GetAbilityDetailDic(true).ToDictionary(pair => pair.Key, pair => pair.Value == 1);
        }

        /// <summary>
        ///     重置详细能力数据字典
        /// </summary>
        public void ResetAbilityDetailDic()
        {
            var tempAbilityDetailDic = _abilityDetailDic.ToDictionary(abilityDetail => abilityDetail.Key,
                abilityDetail => 0);
            _abilityDetailDic = tempAbilityDetailDic;
        }
    }
}