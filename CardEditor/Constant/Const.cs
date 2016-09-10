using System;
using System.Collections.Generic;

namespace CardEditor.Constant
{
    public class Const
    {
        public const string CardEditor = "CardEditor";
        public static string BasePath = AppDomain.CurrentDomain.BaseDirectory;
        public static string RootPath = BasePath.Substring(0, BasePath.LastIndexOf(CardEditor, StringComparison.Ordinal));

        public static string PicturePath = RootPath + "picture\\";
        public static string ThumbnailPath = RootPath + "thumbnail\\";
        public static string DeckPath = RootPath + "deck\\";
        public static string TexturesPath = RootPath + "textures\\";
        public static string BackgroundPath = TexturesPath + "Background.jpg";

        public static Dictionary<string, string> AbilityTypeDic = new Dictionary<string, string>
        {
            {"Lv", "Lv"},
            {"射程", "【常】射程"},
            {"绝界", "【常】"},
            {"起始卡", "【常】起始卡"},
            {"生命恢复", "【常】生命恢复"},
            {"虚空使者", "【常】虚空使者"},
            {"进化原力", "【自】进化原力"},
            {"零点优化", "【※】零点优化"}
        };
    }
}