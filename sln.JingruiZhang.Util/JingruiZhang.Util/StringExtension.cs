using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
#if NET45
using System.Web.Script.Serialization;
#else
using Newtonsoft.Json;
#endif

namespace JingruiZhang.Util
{
    /// <summary>
    /// String 扩展方法
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// AES 加密字符串
        /// </summary>
        /// <param name="input">被加密的字符串</param>
        /// <param name="key">加密key，如果长度小于16则自身叠加</param>
        /// <param name="AES_IV">AES 算法中指定的 AES_IV</param>
        /// <returns></returns>
        public static string EncryptByAES(this string input, string key, string AES_IV)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (AES_IV == null)
            {
                throw new ArgumentNullException("AES_IV");
            }
            if (key.Length == 0)
            {
                throw new Exception("key 长度至少为1");
            }
            if (AES_IV.Length < 16)
            {
                throw new Exception("AES_IV 长度至少为16");
            }

            while (key.Length < 16)
            {
                key += key;
            }
            byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0));
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = keyBytes;
                aesAlg.IV = Encoding.UTF8.GetBytes(AES_IV.Substring(0, 16));

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(input);
                        }
                        byte[] bytes = msEncrypt.ToArray();
                        return ByteArrayToHexString(bytes);
                    }
                }
            }
        }

        /// <summary>
        /// AES 字符串解密
        /// </summary>
        /// <param name="input">加密后的字符串</param>
        /// <param name="key">加密key，如果长度小于16则自身叠加</param>
        /// <param name="AES_IV">AES 算法中指定的 AES_IV</param>
        /// <returns></returns>
        public static string DecryptByAES(this string input, string key, string AES_IV)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (AES_IV == null)
            {
                throw new ArgumentNullException("AES_IV");
            }
            if (key.Length == 0)
            {
                throw new Exception("key 长度至少为1");
            }
            if (AES_IV.Length < 16)
            {
                throw new Exception("AES_IV 长度至少为16");
            }

            while (key.Length < 16)
            {
                key += key;
            }

            byte[] inputBytes = HexStringToByteArray(input);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0));
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = keyBytes;
                aesAlg.IV = Encoding.UTF8.GetBytes(AES_IV.Substring(0, 16));

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream(inputBytes))
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srEncrypt = new StreamReader(csEncrypt))
                        {
                            return srEncrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 字符串反序列化为对象。.netFramework 及 .netstandard 分别进行实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string input)
            where T:class
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
#if NET45
            JavaScriptSerializer jser = new JavaScriptSerializer();
            return jser.Deserialize(input, typeof(T)) as T;
#else
            T retv = JsonConvert.DeserializeObject(input) as T;
            return retv;
#endif
        }

        /// <summary>
        /// 字符串反序列化为对象。.netFramework 及 .netstandard 分别进行实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string input, int _MaxJsonLength)
         where T : class
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
#if NET45
            JavaScriptSerializer jser = new JavaScriptSerializer() { MaxJsonLength = _MaxJsonLength };
            return jser.Deserialize(input, typeof(T)) as T;
#else
            T retv = JsonConvert.DeserializeObject(input) as T;
            return retv;
#endif
        }

        /// <summary>
        /// 将字符串转为JObject。转换后可以使用中括号进行索引
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static JObject ConvertToJObject(this string input)
        {
            JObject jo = input == null ? JObject.Parse("{}") : JObject.Parse(input.Replace("&nbsp;", ""));
            return jo;
        }

        /// <summary>
        /// 字符串转 Int32
        /// </summary>
        /// <param name="input">输入的字符串，可以为null</param>
        /// <param name="defaultValue">当字符串不能成功地转为 Int32 时，指定的值</param>
        /// <returns></returns>
        public static int ToInt32(this string input, int defaultValue)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                return defaultValue;
            }
            int tempi;
            if (!int.TryParse(input, out tempi))
            {
                return defaultValue;
            }
            return tempi;
        }

        /// <summary>
        /// 字符串转 double
        /// </summary>
        /// <param name="input">输入的字符串，可以为null</param>
        /// <param name="defaultValue">当字符串不能成功地转为 double 时，指定的值</param>
        /// <returns></returns>
        public static double ToDouble(this string input, double defaultValue)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                return defaultValue;
            }
            double tempi;
            if (!double.TryParse(input, out tempi))
            {
                return defaultValue;
            }
            return tempi;
        }

        /// <summary>
        /// 字符串转 Guid
        /// </summary>
        /// <param name="input">输入的字符串，可以为null</param>
        /// <param name="defaultValue">当字符串不能成功地转为 double 时，指定的值</param>
        /// <returns></returns>
        public static Guid ToGuid(this string input, Guid defaultValue)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                return defaultValue;
            }
            Guid tempi;
            if (!Guid.TryParse(input, out tempi))
            {
                return defaultValue;
            }
            return tempi;
        }

        /// <summary>
        /// 字符串转 DateTime
        /// </summary>
        /// <param name="input">输入的字符串，可以为null</param>
        /// <param name="defaultValue">当字符串不能成功地转为 double 时，指定的值</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string input, DateTime defaultValue)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                return defaultValue;
            }
            DateTime tempi;
            if (!DateTime.TryParse(input, out tempi))
            {
                return defaultValue;
            }
            return tempi;
        }

        // privates
        #region ...
        /// <summary>
        /// 将一个byte数组转换成一个格式化的16进制字符串
        /// </summary>
        /// <param name="data">byte数组</param>
        /// <returns>格式化的16进制字符串</returns>
        private static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
            {
                //16进制数字
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
                //16进制数字之间以空格隔开
                //sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            }
            return sb.ToString().ToUpper();
        }
        /// <summary>
        /// 将指定的16进制字符串转换为byte数组
        /// </summary>
        /// <param name="s">16进制字符串(如：“7F 2C 4A”或“7F2C4A”都可以)</param>
        /// <returns>16进制字符串对应的byte数组</returns>
        public static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }
        #endregion
    }
}
