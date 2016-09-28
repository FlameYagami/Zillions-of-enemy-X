namespace CardEditor.Constant
{
    public class StringConst
    {
        public enum AbilityType
        {
            None = -1,
            Ig = 1,
            Event = 2,
            Start = 3,
            Extra = 4
        }

        public enum PreviewOrderType
        {
            Number = 0,
            Value = 1
        }

        public enum ModeType
        {
            Query = 0,
            Editor = 1,
            Develop = 2
        }

        public const string DbOpenError = "打开数据库->错误";
        public const string AddSucceed = "添加->成功";
        public const string AddFailed = "添加->失败";
        public const string AddConfirm = "添加->确认";
        public const string UpdateSucceed = "更新->成功";
        public const string UpdateFailed = "更新->失败";
        public const string UpdateConfirm = "更新->确认";
        public const string DeleteSucceed = "删除->成功";
        public const string DeleteFailed = "删除->失败";
        public const string DeleteConfirm = "删除->确认";
        public const string CardChioceNone = "卡牌->未选择";
        public const string CardIsExitst = "卡牌->已存在";
        public const string PackChoiceNone = "卡包->未选择";
        public const string PasswordNone = "密码->未输入";
        public const string EncryptSucced = "加密->成功";
        public const string EncryptFailed = "加密->失败";
        public const string DncryptSucced = "解密->成功";
        public const string DncryptFailed = "解密->失败";
        public const string ExportSucceed = "导出->成功";
        public const string ExportFailed = "导出->失败";
        public const string OrderNumber = "编号";
        public const string ModeQuery = "检索";
        public const string ModeEditor = "编辑";
        public const string ModeDevelop = "开发";
        public const string NotApplicable = "(N/A)";
        public const string RarePr = "PR";
        public const string TypeZx = "Z/X";
        public const string TypePlayer = "玩家";
        public const string TypeEvent = "事件";
        public const string TypeZxEx = "Z/X EX";
        public const string Hyphen = "-";
        public const string Series = "系列";
        public const string QueryResult = "检索结果: ";
        public const string SignIg = "点燃";
    }
}