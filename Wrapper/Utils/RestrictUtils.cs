using System.Collections.Generic;
using System.Linq;
using Common;
using Wrapper.Constant;
using Wrapper.Model;

namespace Wrapper.Utils
{
    public class RestrictUtils
    {
        private static List<RestricModel> _mRestrictList = new List<RestricModel>();

        /// <summary>
        ///     获取制限数量
        /// </summary>
        /// <param name="md5">卡牌Md5</param>
        /// <returns></returns>
        public static int GetRestrict(string md5)
        {
            return (GetRestrictList().FirstOrDefault(entity => entity.md5.Equals(md5)) ?? new RestricModel()).restrict;
        }

        /// <summary>
        ///     获取制限枚举类型
        /// </summary>
        /// <param name="restrict">制限数量字符串</param>
        /// <returns>限制枚举类型</returns>
        private static RestrictType GetRestrictType(int restrict)
        {
            switch (restrict)
            {
                case 0:
                    return RestrictType.Restrict0;
                case 4:
                    return RestrictType.Restrict4;
                case 20:
                    return RestrictType.Restrict20;
                case 30:
                    return RestrictType.Restrict30;
                default:
                    return RestrictType.None;
            }
        }

        /// <summary>
        ///     获取制限列表
        /// </summary>
        /// <returns>限制列表</returns>
        private static IEnumerable<RestricModel> GetRestrictList()
        {
            if (_mRestrictList.Count != 0) return _mRestrictList;
            var restrictJson = FileUtils.GetFileContent(PathManager.RestrictPath);
            _mRestrictList = JsonUtils.Deserialize<List<RestricModel>>(restrictJson);
            return _mRestrictList;
        }

        /// <summary>
        ///     获取制限后的卡牌集合
        /// </summary>
        /// <param name="preList">预览卡牌集合</param>
        /// <param name="restrict">制限数量</param>
        /// <returns>制限后的卡牌集合</returns>
        public static List<CardPreviewModel> GetRestrictCardList(List<CardPreviewModel> preList, int restrict)
        {
            var mRestrictType = GetRestrictType(restrict);
            if (mRestrictType.Equals(RestrictType.None))
                return preList;
            var restrictCardList = new List<CardPreviewModel>();
            var tempCardList = preList.Where(entity => GetRestrictType(entity.Restrict).Equals(mRestrictType)).ToList();
            restrictCardList.AddRange(tempCardList);
            return restrictCardList;
        }

        /// <summary>
        ///     获取限制的图标路径
        /// </summary>
        /// <param name="md5">制限数量</param>
        /// <returns></returns>
        public static string GetRestrictPath(string md5)
        {
            var restrict = GetRestrict(md5);
            foreach (var item in Dic.ImgRestrictPathDic)
                if (restrict.Equals(item.Key))
                    return item.Value;
            return string.Empty;
        }

        private enum RestrictType
        {
            None,
            Restrict0,
            Restrict4,
            Restrict20,
            Restrict30
        }
    }
}