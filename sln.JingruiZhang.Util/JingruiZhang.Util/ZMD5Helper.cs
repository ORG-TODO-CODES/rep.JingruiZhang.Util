using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JingruiZhang.Util
{
    /// <summary>
    /// 计算 MD5 帮助类
    /// </summary>
    public class ZMD5Helper
    {
        /// <summary>
        /// 获取文件 MD5 值
        /// </summary>
        /// <param name="fileName">文件路径，如：D:\1.txt </param>
        /// <returns>MD5值</returns>
        public static string GetFileMD5(string fileName)
        {
            MD5 md5 = MD5.Create();
            byte[] fileHashBytes;
            using (FileStream file = new FileStream(fileName, FileMode.Open))
            {
                fileHashBytes = md5.ComputeHash(file);
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < fileHashBytes.Length; i++)
            {
                sb.Append(fileHashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 得到字符串的 MD5 值
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5WithString(string input)
        {
            string str = "";
            byte[] data = Encoding.GetEncoding("utf-8").GetBytes(input);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(data);
            for (int i = 0; i<bytes.Length; i++)
            {
                str += bytes[i].ToString("x2");
            }
            return str;
        }
    }
}
