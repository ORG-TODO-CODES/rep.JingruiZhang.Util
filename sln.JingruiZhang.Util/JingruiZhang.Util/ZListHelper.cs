using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JingruiZhang.Util
{
    public class ZListHelper
    {
        /// <summary>
        /// 使用分隔符拼接字符串集合为一个新字符串（拼接后末尾无分隔符）
        /// </summary>
        /// <param name="strList">被拼接的字符串集合</param>
        /// <param name="splitter">分隔符</param>
        /// <returns></returns>
        public static string ConcatToString(List<string> strList, char splitter)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < strList.Count; i++)
            {
                sb.AppendFormat("{0}{1}", strList[i], i == strList.Count - 1 ? "" : splitter.ToString());
            }
            string ret = sb.ToString();
            return ret;
        }
    }
}
