using System.Collections.Generic;
using System.Data;
using Wrapper.Model;

namespace Wrapper
{
    public class DataCache
    {
        /// <summary>总数据缓存</summary>
        public static DataSet DsAllCache = new DataSet();

        /// <summary>初始化能力分类模型</summary>
        public static AbilityDetialModel AbilityDetialModel = new AbilityDetialModel(); 
    }
}