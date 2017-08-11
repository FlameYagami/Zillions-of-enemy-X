using System.Collections.Generic;
using System.Linq;

namespace Wrapper.Model
{
    public class AbilityDetialModel
    {
        private Dictionary<string, int> _abilityDetailDic = new Dictionary<string, int>
        {
            {"��Դ����", 0},
            {"��������", 0},
            {"��������", 0},
            {"��Ƭ�ǳ�", 0},
            {"��Ƭ�ƻ�", 0},
            {"��Ƭ����", 0},
            {"��������", 0},
            {"���ؿ���", 0},
            {"��Դ����", 0},
            {"�����û�", 0},
            {"���ܷ���", 0},
            {"��������", 0},
            {"��������", 0},
            {"�鿨����", 0},
            {"�������", 0},
            {"��������", 0},
            {"������", 0},
            {"�������", 0},
            {"�������", 0},
            {"�������", 0},
            {"�˺����", 0},
            {"ԭ�����", 0},
            {"������", 0},
            {"�������", 0},
            {"����ʤ��", 0}
        };

        public AbilityDetialModel()
        {
        }

        /// <summary>
        ///     ������ϸ����ʵ��
        /// </summary>
        /// <param name="checkboxDic">CheckBox���Թ����������ֵ�</param>
        /// <param name="isModle">�Ƿ���Ҫ�õ�˽�����Թ���ģ��</param>
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

        // �����������
        public int ��Դ���� { get; set; }
        public int �������� { get; set; }
        public int �������� { get; set; }
        public int ��Ƭ�ǳ� { get; set; }
        public int ��Ƭ�ƻ� { get; set; }
        public int ��Ƭ���� { get; set; }
        public int �������� { get; set; }
        public int ���ؿ��� { get; set; }
        public int ��Դ���� { get; set; }
        public int �����û� { get; set; }
        public int ���ܷ��� { get; set; }
        public int �������� { get; set; }
        public int �������� { get; set; }
        public int �鿨���� { get; set; }
        public int ������� { get; set; }
        public int �������� { get; set; }
        public int ������ { get; set; }
        public int ������� { get; set; }
        public int ������� { get; set; }
        public int ������� { get; set; }
        public int �˺���� { get; set; }
        public int ԭ����� { get; set; }
        public int ������ { get; set; }
        public int ������� { get; set; }
        public int ����ʤ�� { get; set; }

        /// <summary>
        ///     ��ȡ��ϸ���������ֵ�
        /// </summary>
        /// <param name="isModel">�Ƿ���Ҫ�õ�˽�����Թ���ģ��</param>
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
        ///     ��ȡ��ϸ���������ֵ�
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, bool> GetAbilityDetailExDic()
        {
            return GetAbilityDetailDic(true).ToDictionary(pair => pair.Key, pair => pair.Value == 1);
        }

        /// <summary>
        ///     ������ϸ���������ֵ�
        /// </summary>
        public void ResetAbilityDetailDic()
        {
            var tempAbilityDetailDic = _abilityDetailDic.ToDictionary(abilityDetail => abilityDetail.Key,
                abilityDetail => 0);
            _abilityDetailDic = tempAbilityDetailDic;
        }
    }
}