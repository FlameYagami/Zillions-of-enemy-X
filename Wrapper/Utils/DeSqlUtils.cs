using System;
using System.Linq;
using System.Text;
using Common;
using Wrapper.Model;

namespace Wrapper.Utils
{
    public class DeSqlUtils : SqlUtils
    {
        public static string GetQuerySql(DeQueryModel card, string cardPreviewOrder)
        {
            var previewOrderType = CardUtils.GetPreOrderType(cardPreviewOrder);
            var builder = new StringBuilder();
            builder.Append(GetHeaderSql()); // 基础查询语句
            builder.Append(GetAllKeySql(card.Key)); // 关键字
            builder.Append(GetAccurateSql(card.Type, ColumnType)); // 种类
            builder.Append(GetAccurateSql(card.Camp, ColumnCamp)); // 阵营
            builder.Append(GetAccurateSql(card.Race, ColumnRace)); // 种族
            builder.Append(GetAccurateSql(card.Sign, ColumnSign)); // 标记
            builder.Append(GetAccurateSql(card.Rare, ColumnRare)); // 罕贵
            builder.Append(GetAccurateSql(card.Illust, ColumnIllust)); // 画师
            builder.Append(GetPackSql(card.Pack, ColumnPack)); // 卡包
            builder.Append(GetIntervalSql(card.CostValue, ColumnCost)); // 费用
            builder.Append(GetIntervalSql(card.PowerValue, ColumnPower)); // 力量
            builder.Append(GetAbilityTypeSql(card.AbilityTypeModels.ToList())); //  能力类型
            builder.Append(GetAbilityDetailSql(card.AbilityDetailModels.ToList())); // 详细能力
            builder.Append(GetFooterSql(previewOrderType)); // 排序
            return builder.ToString(); // 完整的查询语句
        }
    }
}