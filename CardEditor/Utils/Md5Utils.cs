using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CardEditor.Utils
{
    public class Md5Utils
    {
        public static string GetMd5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            //将字符串转换为字节数组  
            var fromData = Encoding.Unicode.GetBytes(str);
            //计算字节数组的哈希值  
            var toData = md5.ComputeHash(fromData);
            var byteStr = toData.Aggregate("", (current, t) => current + t.ToString("x"));
            return byteStr.Substring(0, 8);
        }
    }
}
