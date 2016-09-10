using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using DeckEditor.Entity;

namespace DeckEditor.Model
{
    public class DataCache
    {
        /// <summary>总数据缓存</summary>
        public static DataSet DsAllCache = new DataSet();

        /// <summary>列表数据缓存</summary>
        public static List<PreviewEntity> InfoColl = new List<PreviewEntity>();

        /// <summary>能力分类缓存</summary>
        public static AbilityDetialEntity AbilityDetialEntity = new AbilityDetialEntity(); // 初始化能力分类模型

        /// <summary>玩家数据缓存</summary>
        public static List<DeckEntity> PlColl = new List<DeckEntity>();

        /// <summary>点燃数据缓存</summary>
        public static List<DeckEntity> IgColl = new List<DeckEntity>();

        /// <summary>非点燃数据缓存</summary>
        public static List<DeckEntity> UgColl = new List<DeckEntity>();

        /// <summary>额外数据缓存</summary>
        public static List<DeckEntity> ExColl = new List<DeckEntity>();
    }
}