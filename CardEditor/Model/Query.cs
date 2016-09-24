using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CardEditor.Constant;
using CardEditor.Entity;
using CardEditor.Utils;
using JsonLib;

namespace CardEditor.Model
{
    public interface IQuery
    {
        void SetCardList();
        string GetQuerySql(CardEntity cardEntity, string order);
        string GetEditorSql(CardEntity cardEntity, string order);
        string GetUpdateSql(CardEntity cardEntity, string number);
        string GetAddSql(CardEntity cardEntity);
        string GetDeleteSql(string number);
        StringConst.AbilityType AnalysisAbility(string ability);
    }

    internal class Query : SqliteConst, IQuery
    {
        public Query()
        {
            MemoryQuerySql = string.Empty;
            CardList = new List<PreviewEntity>();
        }

        /// <summary>ListView数据缓存</summary>
        public static List<PreviewEntity> CardList { get; set; }

        /// <summary>记忆中的查询语句</summary>
        public static string MemoryQuerySql { get; set; }

        public string GetQuerySql(CardEntity cardEntity, string order)
        {
            var builder = new StringBuilder();
            builder.Append(QueryBaseSql);
            builder.Append(SqlUtils.GetBaseSql(cardEntity.Type, Type)); // 种类
            builder.Append(SqlUtils.GetBaseSql(cardEntity.Camp, Camp)); // 阵营
            builder.Append(SqlUtils.GetBaseSql(cardEntity.Race, Race)); // 种族
            builder.Append(SqlUtils.GetBaseSql(cardEntity.Sign, Sign)); // 标记
            builder.Append(SqlUtils.GetBaseSql(cardEntity.Rare, Rare)); // 罕贵
            builder.Append(SqlUtils.GetSimilarSql(cardEntity.CName, CName)); // 卡名
            builder.Append(SqlUtils.GetSimilarSql(cardEntity.JName, JName)); // 日名
            builder.Append(SqlUtils.GetSimilarSql(cardEntity.Illust, Illust)); // 画师
            builder.Append(SqlUtils.GetPackSql(cardEntity.Pack, Pack)); // 卡包
            builder.Append(SqlUtils.GetSimilarSql(cardEntity.Number, Number)); // 卡编
            builder.Append(SqlUtils.GetIntervalSql(cardEntity.Cost, Cost)); // 费用
            builder.Append(SqlUtils.GetIntervalSql(cardEntity.Power, Power)); // 力量
            builder.Append(SqlUtils.GetBaseSql(cardEntity.Restrict, Restrict)); // 限制
            builder.Append(cardEntity.AbilityType); // 
            builder.Append(cardEntity.AbilityDetail); //
            MemoryQuerySql = builder.ToString(); // 除排序外的查询语句
            builder.Append(order.Equals(StringConst.OrderNumber) ? NumberOrderSql : ValueOrderSql); // 完整的查询语句
            return builder.ToString();
        }

        public string GetEditorSql(CardEntity cardEntity, string order)
        {
            var builder = new StringBuilder();
            builder.Append(QueryBaseSql);
            builder.Append(SqlUtils.GetPackSql(cardEntity.Pack, Pack)); // 卡包
            MemoryQuerySql = builder.ToString(); // 除排序外的查询语句
            builder.Append(order.Equals(StringConst.OrderNumber) ? NumberOrderSql : ValueOrderSql); // 完整的查询语句
            return builder.ToString();
        }

        public string GetAddSql(CardEntity cardEntity)
        {
            var builder = new StringBuilder();
            builder.Append("INSERT INTO " + TableName);
            builder.Append(ColumnCard);
            builder.Append("VALUES(");
            builder.Append("'" + SqlUtils.GetBaseValue(cardEntity.Type) + "',");
            builder.Append("'" + SqlUtils.GetBaseValue(cardEntity.Camp) + "',");
            builder.Append("'" + SqlUtils.GetBaseValue(cardEntity.Race) + "',");
            builder.Append("'" + SqlUtils.GetBaseValue(cardEntity.Sign) + "',");
            builder.Append("'" + SqlUtils.GetBaseValue(cardEntity.Rare) + "',");
            builder.Append("'" + SqlUtils.GetBaseValue(cardEntity.Pack) + "',");
            builder.Append("'" + (cardEntity.Restrict.Equals(StringConst.Ban) ? "0" : "4") + "',");

            builder.Append("'" + cardEntity.CName + "',");
            builder.Append("'" + cardEntity.JName + "',");
            builder.Append("'" + cardEntity.Illust + "',");
            builder.Append("'" + cardEntity.Number + "',");
            builder.Append("'" + cardEntity.Cost + "',");
            builder.Append("'" + cardEntity.Power + "',");
            builder.Append("'" + cardEntity.Ability + "',");
            builder.Append("'" + cardEntity.Lines + "',");
            builder.Append("'" + cardEntity.Faq + "',");
            builder.Append("'" + JsonUtils.JsonSerializer(cardEntity.AbilityDetialEntity) + "'"); // 详细能力处理
            builder.Append(")");
            return builder.ToString();
        }

        public void SetCardList()
        {
            if (null == CardList)
                CardList = new List<PreviewEntity>();
            else
                CardList.Clear();
            foreach (var row in DataCache.DsPartCache.Tables[TableName].Rows.Cast<DataRow>())
                CardList.Add(new PreviewEntity
                {
                    CName = row[CName].ToString(),
                    Power = row[Power].ToString().Equals(string.Empty) ? StringConst.Hyphen : row[Power].ToString(),
                    Cost = row[Cost].ToString().Equals(string.Empty) ? StringConst.Hyphen : row[Cost].ToString(),
                    Number = row[Number].ToString()
                });
        }

        /// <summary>
        ///     返回删除语句
        /// </summary>
        /// <param name="number">卡片编号</param>
        /// <returns></returns>
        public string GetDeleteSql(string number)
        {
            var builder = new StringBuilder();
            builder.Append("DELETE FROM " + TableName);
            builder.Append(" WHERE " + Number);
            builder.Append("= '" + number + "'");
            return builder.ToString();
        }

        /// <summary>
        ///     返回更新语句
        /// </summary>
        /// <param name="cardEntity"></param>
        /// <param name="number">卡片编号</param>
        /// <returns></returns>
        public string GetUpdateSql(CardEntity cardEntity, string number)
        {
            var builder = new StringBuilder();
            builder.Append("UPDATE " + TableName + " SET ");
            builder.Append(Type + "= '" + cardEntity.Type + "',");
            builder.Append(Camp + "= '" + cardEntity.Camp + "',");
            builder.Append(Race + "= '" + cardEntity.Race + "',");
            builder.Append(Sign + "= '" + cardEntity.Sign + "',");
            builder.Append(Rare + "= '" + cardEntity.Rare + "',");
            builder.Append(Pack + "= '" + cardEntity.Pack + "',");
            builder.Append(Restrict + "='" + (cardEntity.Restrict.Equals(StringConst.Ban) ? "0" : "4") + "',");

            builder.Append(CName + "= '" + cardEntity.CName + "',");
            builder.Append(JName + "= '" + cardEntity.JName + "',");
            builder.Append(Illust + "= '" + cardEntity.Illust + "',");
            builder.Append(Number + "= '" + cardEntity.Number + "',");
            builder.Append(Cost + "= '" + cardEntity.Cost + "',");
            builder.Append(Power + "= '" + cardEntity.Power + "',");
            builder.Append(Ability + "= '" + cardEntity.Ability + "',");
            builder.Append(Lines + "= '" + cardEntity.Lines + "',");
            builder.Append(Faq + "= '" + cardEntity.Faq + "',");
            builder.Append(AbilityDetail + "= '" + JsonUtils.JsonSerializer(cardEntity.AbilityDetialEntity) + "'");
            // 详细能力处理

            builder.Append(" WHERE " + Number);
            builder.Append("= '" + number + "'");
            return builder.ToString();
        }

        public StringConst.AbilityType AnalysisAbility(string ability)
        {
            if (ability.Contains("降临条件") || ability.Contains("觉醒条件"))
                return StringConst.AbilityType.Extra;
            if (ability.Contains("【★】"))
                return StringConst.AbilityType.Event;
            if (ability.Contains(StringConst.AbilityLife) || ability.Contains(StringConst.AbilityVoid))
                return StringConst.AbilityType.Ig;
            if (ability.Contains(StringConst.AbilityStart))
                return StringConst.AbilityType.Start;
            return StringConst.AbilityType.None;
        }

        public List<PreviewEntity> GetCardList()
        {
            return CardList;
        }
    }
}