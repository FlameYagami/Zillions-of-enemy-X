namespace DeckEditor.Constant
{
    public class StringConst
    {
        public enum AreaType
        {
            None = -1,
            Pl = 0,
            Ig = 1,
            Ug = 2,
            Ex = 3
        }

        public enum DeckOrderType
        {
            Value = 0,
            Random = 1
        }

        public enum IgType
        {
            Life = 0,
            Void = 1,
            Normal = 2
        }

        public enum PreviewOrderType
        {
            Number = 0,
            Value = 1
        }

        public const string DbOpenError = "打开数据库->错误";
        public const string DeckNameNone = "卡组名称->未输入";
        public const string DeckNameExist = "卡组名称->已存在";
        public const string DeleteHint = "删除->确认";
        public const string DeleteSucceed = "删除->成功";
        public const string CoverHint = "覆写->确认";
        public const string ResaveSucceed = "另存->成功";
        public const string SaveSucceed = "保存->成功";
        public const string OrderNumber = "编号";
        public const string SignIg = "点燃";
        public const string ModeQuery = "检索";
        public const string ModeDevelop = "开发";
        public const string NotApplicable = "(N/A)";
        public const string Ban = "禁止";
        public const string TypeZx = "Z/X";
        public const string TypePlayer = "玩家";
        public const string TypeEvent = "事件";
        public const string TypeZxEx = "Z/X EX";
        public const string Hyphen = "-";
        public const string Series = "系列";
        public const string QueryResult = "检索结果: ";
        public const string Life = "生命恢复:";
        public const string Void = "虚空使者:";
        public const string AbilityLife = "【常】生命恢复";
        public const string AbilityVoid = "【常】虚空使者";

        /// <summary>卡组扩展名</summary>
        public const string DeckExtension = ".zx";

        public const string ImageExtension = ".jpg";

        public const string LblPreviewNumber = "LblPreviewNumber";
        public const string LblAreaNumber = "LblAreaNumber";
        public const string ImgAreaThumbnail = "ImgAreaThumbnail";
    }
}