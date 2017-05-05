using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Wrapper.Entity;

namespace Wrapper.Utils
{
    public class RestrictUtils
    {
        private static List<RestricEntity> _mRestrictList = new List<RestricEntity>();

        private enum RestrictType
        {
            None,
            Restrict0,
            Restrict4,
            Restrict20,
            Restrict30
        }

        /// <summary>
        /// 获取制限数量
        /// </summary>
        /// <param name="md5">卡牌Md5</param>
        /// <returns></returns>
        public static int GetRestrict(string md5)
        {
            return (GetRestrictList().FirstOrDefault(entity => entity.md5.Equals(md5)) ?? new RestricEntity()).restrict;
        }

        /// <summary>
        /// 获取制限枚举类型
        /// </summary>
        /// <param name="restrict">制限数量字符串</param>
        /// <returns>限制枚举类型</returns>
        private static RestrictType GetRestrictType(string restrict)
        {
            switch (restrict)
            {
                case "0":
                    return RestrictType.Restrict0;
                case "4":
                    return RestrictType.Restrict4;
                case "20":
                    return RestrictType.Restrict20;
                case "30":
                    return RestrictType.Restrict30;
                default:
                    return RestrictType.None;
            }
        }

        /// <summary>
        /// 获取制限列表
        /// </summary>
        /// <returns>限制列表</returns>
        private static IEnumerable<RestricEntity> GetRestrictList()
        {
            if (_mRestrictList.Count != 0) return _mRestrictList;
            var restrictJson = FileUtils.GetFileContent(PathManager.RestrictPath);
            _mRestrictList = JsonUtils.JsonDeserialize<List<RestricEntity>>(restrictJson);
            return _mRestrictList;
        }

        /// <summary>
        /// 获取制限后的卡牌集合
        /// </summary>
        /// <param name="preList">预览卡牌集合</param>
        /// <param name="restrict">制限数量</param>
        /// <returns>制限后的卡牌集合</returns>
        public static List<PreviewEntity> GetRestrictCardList(List<PreviewEntity> preList, string restrict)
        {
            var mRestrictType = GetRestrictType(restrict);
            if (mRestrictType.Equals(RestrictType.None))
            {
                return preList;
            }
            var restrictCardList = new List<PreviewEntity>();
            var tempCardList = preList.Where(entity => GetRestrictType(entity.Restrict).Equals(mRestrictType)).ToList();
            restrictCardList.AddRange(tempCardList);
            return restrictCardList;
        }
    }
}
