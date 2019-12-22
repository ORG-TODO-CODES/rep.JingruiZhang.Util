using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NET45
using System.Web.Script.Serialization;
#else
using Newtonsoft.Json;
#endif

namespace JingruiZhang.Util
{
    /// <summary>
    /// Object 扩展方法
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// 对象序列化为字符串。.netFramework 及 .netstandard 分别进行实现
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Serialize(this object input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
#if NET45
            JavaScriptSerializer jser = new JavaScriptSerializer();
            return jser.Serialize(input);
#else
            return JsonConvert.SerializeObject(input);
#endif
        }

        /// <summary>
        /// object 转 Int32
        /// </summary>
        public static int ToInt32(this object input, int defaultValue)
        {
            if (input==null)
            {
                return defaultValue;
            }
            return StringExtension.ToInt32(input.ToString(), defaultValue);
        }

        /// <summary>
        /// object 转 double
        /// </summary>
        public static double ToDouble(this object input, double defaultValue)
        {
            if (input == null)
            {
                return defaultValue;
            }
            return StringExtension.ToDouble(input.ToString(), defaultValue);
        }

        /// <summary>
        /// object 转 Guid
        /// </summary>
        public static Guid ToGuid(this object input, Guid defaultValue)
        {
            if (input == null)
            {
                return defaultValue;
            }
            return StringExtension.ToGuid(input.ToString(), defaultValue);
        }

        /// <summary>
        /// object 转 DateTime
        /// </summary>
        public static DateTime ToDateTime(this object input, DateTime defaultValue)
        {
            if (input == null)
            {
                return defaultValue;
            }
            return StringExtension.ToDateTime(input.ToString(), defaultValue);
        }
    }
}
