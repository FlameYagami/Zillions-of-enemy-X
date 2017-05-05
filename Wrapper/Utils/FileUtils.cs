using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrapper.Constant;

namespace Wrapper.Utils
{
    public class FileUtils
    {
        public static string GetFileContent(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception(StringConst.FileNotExits);
            }
            var content = new StringBuilder();
            content.Append(File.ReadAllText(filePath));
            return content.ToString();
        }
    }
}
