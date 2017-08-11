using System.Collections.Generic;

namespace Wrapper.Constant
{
    public class Dictionary
    {
        // Common
        public static Dictionary<string, string> AbilityTypeDic = new Dictionary<string, string>
        {
            {StringConst.Lv, StringConst.AbilityLv},
            {StringConst.Range, StringConst.AbilityRange},
            {StringConst.InsulateWards, StringConst.AbilityInsulateWards},
            {StringConst.Start, StringConst.AbilityStart},
            {StringConst.Life, StringConst.AbilityLife},
            {StringConst.Void, StringConst.AbilityVoid},
            {StringConst.Evo, StringConst.AbilityEvo},
            {StringConst.ZeroOptima, StringConst.AbilityZeroOptima}
        };

        // DeckEditor
        public static Dictionary<string, string> ImgSignPathDic = new Dictionary<string, string>
        {
            {StringConst.Hyphen, string.Empty},
            {StringConst.SignIg, PathManager.ImageIgPath},
            {StringConst.SignEl, PathManager.ImageElPath}
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
    }
}