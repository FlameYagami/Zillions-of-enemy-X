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
    }
}