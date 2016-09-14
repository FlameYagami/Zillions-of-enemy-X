using System;
using System.Collections.Generic;

namespace DeckEditor.Constant
{
    public class Const
    {
        public const string DeckEditor = "DeckEditor";
        public static string BasePath = AppDomain.CurrentDomain.BaseDirectory;
        public static string RootPath = BasePath.Substring(0, BasePath.LastIndexOf(DeckEditor, StringComparison.Ordinal));

        public static string PicturePath = RootPath + "picture\\";
        public static string ThumbnailPath = RootPath + "thumbnail\\";
        public static string ThumbnailUnknownPath = RootPath + "thumbnail\\Unknown.jpg";
        public static string DeckFolderPath = RootPath + "deck\\";
        public static string TexturesPath = RootPath + "textures\\";
        public static string BackgroundPath = TexturesPath + "Background.JPG";

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

        public static Dictionary<string, string> ImgSignPathDic = new Dictionary<string, string>
        {
            {"-", string.Empty},
            {"点燃", TexturesPath + "Ig.png"},
            {"觉醒之种", TexturesPath + "El.png"}
        };

        public static Dictionary<string, string> ImgRestrictPathDic = new Dictionary<string, string>
        {
            {"0", TexturesPath + "Restrict.png"}
        };

        public static Dictionary<string, string> ImgCampPathDic = new Dictionary<string, string>
        {
            {"红", TexturesPath + "Red.png"},
            {"蓝", TexturesPath + "Blue.png"},
            {"白", TexturesPath + "White.png"},
            {"黑", TexturesPath + "Black.png"},
            {"绿", TexturesPath + "Green.png"},
            {"无", TexturesPath + "Void.png"}
        };
    }
}