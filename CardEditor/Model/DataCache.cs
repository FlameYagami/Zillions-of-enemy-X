using System.Data;

namespace CardEditor.Model
{
    public class DataCache
    {
        /// <summary>总数据缓存</summary>
        public static DataSet DsAllCache = new DataSet();

        /// <summary>部分数据缓存</summary>
        public static DataSet DsPartCache = new DataSet();
    }
}