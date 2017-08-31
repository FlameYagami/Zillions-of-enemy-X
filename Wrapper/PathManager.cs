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

        public static string ImageRareC = $"{TexturesPath}r_c.png";
        public static string ImageRareCvr = $"{TexturesPath}r_cvr.png";
        public static string ImageRareDr = $"{TexturesPath}r_dr.png";
        public static string ImageRareHr = $"{TexturesPath}r_hr.png";
        public static string ImageRareIgr = $"{TexturesPath}r_igr.png";
        public static string ImageRareN = $"{TexturesPath}r_n.png";
        public static string ImageRareNhir = $"{TexturesPath}r_nhir.png";
        public static string ImageRarePr = $"{TexturesPath}r_pr.png";
        public static string ImageRareR = $"{TexturesPath}r_r.png";
        public static string ImageRareSr = $"{TexturesPath}r_sr.png";
        public static string ImageRareUc = $"{TexturesPath}r_uc.png";
        public static string ImageRareUr = $"{TexturesPath}r_ur.png";
        public static string ImageRareZxr = $"{TexturesPath}r_zxr.png";
    }
}