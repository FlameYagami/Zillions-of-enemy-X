using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Wrapper.Utils
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
            return toData.Aggregate("", (current, t) => current + t.ToString("X2"));
        }
    }
}