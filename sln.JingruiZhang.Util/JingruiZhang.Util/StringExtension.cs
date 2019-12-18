using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace JingruiZhang.Util
{
    /// <summary>
    /// String 扩展方法
    /// </summary>
    public static class StringExtension
    {
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
    }
}
