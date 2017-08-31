using System.Collections.Generic;

namespace Wrapper.Constant
{
    public class Dic
    {
        // Common
        public static Dictionary<string, string> AbilityTypeDic = new Dictionary<string, string>
        {
            {"Lv", StringConst.AbilityLv},
            {"射程", StringConst.AbilityRange},
            {"绝界", StringConst.AbilityInsulateWards},
            {"起始卡", StringConst.AbilityStart},
            {"生命恢复", StringConst.AbilityLife},
            {"虚空使者", StringConst.AbilityVoid},
            {"进化原力", StringConst.AbilityEvo},
            {"零点优化", StringConst.AbilityZeroOptima}
        };

        public static Dictionary<string, int> AbilityDetailDic = new Dictionary<string, int>
        {
            {"卡牌登场", 10},
            {"卡牌移位", 11},
            {"卡牌破坏", 12},
            {"卡牌除外", 13},
            {"卡牌抽取", 14},
            {"卡牌检索", 15},
            {"资源放置", 20},
            {"废弃放置", 21},
            {"充能放置", 22},
            {"生命放置", 23},
            {"返回手牌", 24},
            {"返回卡组", 25},
            {"阵营相关", 30},
            {"种族相关", 31},
            {"标记相关", 32},
            {"费用相关", 33},
            {"力量相关", 34},
            {"原力相关", 35},
            {"伤害相关", 40},
            {"玩家相关", 41},
            {"联动相关", 42},
            {"规则相关", 43},
            {"状态调整", 50},
            {"牌库调整", 51}
        };

        public static Dictionary<string, int> PackSeriesDic = new Dictionary<string, int>
        {
            {"AC", 0},
            {"B", 0},
            {"C", 0},
            {"CP", 0},
            {"D", 0},
            {"E", 0},
            {"P", 0},
            {"L", 0},
            {"M", 0},
            {"I", 0},
            {"V", 0}
        };

        // DeckEditor
        public static Dictionary<string, string> ImgSignPathDic = new Dictionary<string, string>
        {
            {StringConst.Hyphen, string.Empty},
            {StringConst.SignIg, PathManager.ImageIgPath},
            {StringConst.SignEl, PathManager.ImageElPath}
        };

        public static Dictionary<string, string> ImgRarePathDic = new Dictionary<string, string>
        {
            {"CVR", PathManager.ImageRareCvr},
            {"IGR", PathManager.ImageRareIgr},
            {"ZX/R", PathManager.ImageRareZxr},
            {"HR", PathManager.ImageRareHr},
            {"DR", PathManager.ImageRareDr},
            {"UR", PathManager.ImageRareUc},
            {"SR", PathManager.ImageRareSr},
            {"R", PathManager.ImageRareR},
            {"N", PathManager.ImageRareN},
            {"PR", PathManager.ImageRarePr},
            {"UC", PathManager.ImageRareUc},
            {"C", PathManager.ImageRareC},
            {"日本一R", PathManager.ImageRareNhir}
        };

        public static Dictionary<int, string> ImgRestrictPathDic = new Dictionary<int, string>
        {
            {0, PathManager.ImageRestrictPath},
            {4, string.Empty},
            {20, string.Empty},
            {30, string.Empty}
        };

        public static Dictionary<string, string> ImgCampPathDic = new Dictionary<string, string>
        {
            {StringConst.CampRed, PathManager.ImageCampRedPath},
            {StringConst.CampBlue, PathManager.ImageCampBluePath},
            {StringConst.CampWhite, PathManager.ImageCampWhitePath},
            {StringConst.CampBlack, PathManager.ImageCampBlackPath},
            {StringConst.CampGreen, PathManager.ImageCampGreenPath},
            {StringConst.CampVoid, PathManager.ImageCampVoidPath}
        };

        public static Dictionary<Enums.PreviewOrderType, string> PreviewOrderDic = new Dictionary
            <Enums.PreviewOrderType, string>
            {
                {Enums.PreviewOrderType.Value, StringConst.OrderValue},
                {Enums.PreviewOrderType.Number, StringConst.OrderNumber}
            };

        public static Dictionary<Enums.ModeType, string> ModeDic = new Dictionary<Enums.ModeType, string>
        {
            {Enums.ModeType.Query, StringConst.ModeQuery},
            {Enums.ModeType.Editor, StringConst.ModeEditor},
            {Enums.ModeType.Develop, StringConst.ModeDevelop}
        };
    }
}