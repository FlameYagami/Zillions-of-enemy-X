using System;
using System.IO;
using System.Text;

namespace Common
{
    public class FileUtils
    {
        public static string GetFileContent(string filePath)
        {
            if (!File.Exists(filePath))
                return string.Empty;
            var content = new StringBuilder();
            content.Append(File.ReadAllText(filePath));
            return content.ToString();
        }

        public static bool SaveFile(string filePath, string content)
        {
            try
            {
                var fs = new FileStream(filePath, FileMode.Create);
                var sw = new StreamWriter(fs);
                sw.Write(content);
                sw.Close();
                fs.Close();
                fs.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}