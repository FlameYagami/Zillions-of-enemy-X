using System;
using System.IO;

namespace Wrapper
{
    public class PathManager
    {
        public static string RootPath = Directory.GetParent(Environment.CurrentDirectory).FullName + "\\";

        public static string PicturePath = RootPath + "picture\\";
        public static string PictureUnknownPath = RootPath + "picture\\Unknown.jpg";
        public static string ThumbnailPath = RootPath + "thumbnail\\";
        public static string ThumbnailUnknownPath = RootPath + "thumbnail\\Unknown.jpg";
        public static string DeckFolderPath = RootPath + "deck\\";
        public static string TexturesPath = RootPath + "textures\\";
        public static string BackgroundPath = TexturesPath + "Background.jpg";
        public static string RestrictPath = RootPath + "restrict";

        public static string ImageRestrictPath = $"{TexturesPath}Restrict.png";
        public static string ImageIgPath = $"{TexturesPath}Ig.png";
        public static string ImageElPath = $"{TexturesPath}El.png";

        public static string ImageCampRedPath = $"{TexturesPath}Red.png";
        public static string ImageCampBluePath = $"{TexturesPath}Blue.png";
        public static string ImageCampWhitePath = $"{TexturesPath}White.png";
        public static string ImageCampBlackPath = $"{TexturesPath}Black.png";
        public static string ImageCampGreenPath = $"{TexturesPath}Green.png";
        public static string ImageCampVoidPath = $"{TexturesPath}Void.png";
    }
}