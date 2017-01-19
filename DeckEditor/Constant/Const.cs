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
        public static string PictureUnknownPath = RootPath + "picture\\Unknown.jpg";
        public static string ThumbnailPath = RootPath + "thumbnail\\";
        public static string ThumbnailUnknownPath = RootPath + "thumbnail\\Unknown.jpg";
        public static string DeckFolderPath = RootPath + "deck\\";
        public static string TexturesPath = RootPath + "textures\\";
        public static string BackgroundPath = TexturesPath + "Background.jpg";
    }
}