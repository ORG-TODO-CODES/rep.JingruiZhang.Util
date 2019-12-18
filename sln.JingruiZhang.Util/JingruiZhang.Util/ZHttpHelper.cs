using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JingruiZhang.Util
{
    /// <summary>
    /// 模拟 http 客户端发送请求帮助类
    /// </summary>
    public class ZHttpHelper
    {
        /// <summary>
        /// 发送 get 请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="statusCode">请求响应状态码</param>
        /// <returns>请求返回的数据</returns>
        public static string Get(string url, out string statusCode)
        {
            string result = string.Empty;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                statusCode = response.StatusCode.ToString();

                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }
            return result;
        }

        /// <summary>
        /// 发送 post 请求，在post中对应的 Content-Type 为：raw, Text。
        /// </summary>
        /// <param name="url">post地址</param>
        /// <param name="jsonstr">Text内容</param>
        /// <returns>请求返回的数据</returns>
        public static string Post_Text(string url, string jsonstr)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            //var postData = "thing1=hello";
            //postData += "&thing2=world";
            var data = Encoding.ASCII.GetBytes(jsonstr);
            request.Method = "POST";
            // request.ContentType = "application/x-www-form-urlencoded";
            request.ContentType = "text/plain";
            request.ContentLength = data.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            string msgPostRet;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    msgPostRet = sr.ReadToEnd();
                }
            }
            return msgPostRet;
        }

        /// <summary>
        /// 发送 post 请求，在post中对应的 Content-Type 为：raw, json。
        /// </summary>
        /// <param name="url">post地址</param>
        /// <param name="jsonstr">Text内容</param>
        /// <returns>请求返回的数据</returns>
        public static string Post_Json(string url, string jsonstr)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            //var postData = "thing1=hello";
            //postData += "&thing2=world";
            var data = Encoding.ASCII.GetBytes(jsonstr);
            request.Method = "POST";
            // request.ContentType = "application/x-www-form-urlencoded";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            string msgPostRet;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    msgPostRet = sr.ReadToEnd();
                }
            }
            return msgPostRet;
        }

        /// <summary>
        /// 发送 post 请求（不带文件），常用于form-data
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="datas">post的数据</param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> datas)
        {
            using (var httpClient = new HttpClient())
            {
                // 请求头设置
                httpClient.DefaultRequestHeaders.Add("ContentType", "multipart/form-data");//设置请求头

                //post
                var urlobj = new Uri(url);
                var body = new FormUrlEncodedContent(datas);
                // response
                var response = httpClient.PostAsync(urlobj, body).Result;
                var data = response.Content.ReadAsStringAsync().Result;
                return data;//接口调用成功数据
            }
        }

        /// <summary>
        /// 发送 post 请求（带文件）
        /// </summary>
        /// <param name="url">post 目标地址</param>
        /// <param name="datas">普通 name 及值</param>
        /// <param name="filenameAndPaths">文件的name及文件路径</param>        
        public static string Post_WithFile(string url, Dictionary<string, string> datas
            , Dictionary<string, string> filenameAndPaths, string boundary = "ceshi")
        {
            string Enter = "\r\n";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "multipart/form-data;boundary=" + boundary;
            Stream myRequestStream = request.GetRequestStream();//定义请求流
            #region 将流中写入keyvalue及文件

            #region 示例 formdata 数据
            /*
------WebKitFormBoundaryOIe2pJynlMDNGrnD
Content-Disposition: form-data; name="ProjectID"

c14075a2-6c91-1fdb-3fbe-76bf898c24cf
------WebKitFormBoundaryOIe2pJynlMDNGrnD
Content-Disposition: form-data; name="FolderID"

c14075a2-6c91-1fdb-3fbe-76bf898c24cf
------WebKitFormBoundaryOIe2pJynlMDNGrnD
Content-Disposition: form-data; name="CreateUserName"

System
------WebKitFormBoundaryOIe2pJynlMDNGrnD
Content-Disposition: form-data; name="CreateUserID"

System
------WebKitFormBoundaryOIe2pJynlMDNGrnD
Content-Disposition: form-data; name="NormalOrDrawings"

Normal
------WebKitFormBoundaryOIe2pJynlMDNGrnD
Content-Disposition: form-data; name="IsSaveVersion"

1
------WebKitFormBoundaryOIe2pJynlMDNGrnD
Content-Disposition: form-data; name="F_0_1558419830545"; filename="arrow-down_outline.svg"
Content-Type: image/svg+xml


------WebKitFormBoundaryOIe2pJynlMDNGrnD--
             */
            #endregion           

            #region 遍历每一个key
            foreach (var item in datas)
            {
                string itemstr = "--" + boundary + Enter
                    + "Content-Disposition: form-data; name=\"" + item.Key + "\"" + Enter + Enter
                    + item.Value + Enter;
                byte[] itembytes = Encoding.UTF8.GetBytes(itemstr);
                myRequestStream.Write(itembytes, 0, itembytes.Length);
            }
            #endregion

            #region 遍历每一个文件
            foreach (var filefd in filenameAndPaths)
            {
                string fileContentStr = "--" + boundary + Enter
                        + "Content-Type:application/octet-stream" + Enter
                        + "Content-Disposition: form-data; name=\"" + filefd.Key + "\"; filename=\"" +
                        Path.GetFileName(filefd.Value)
                        + "\"" + Enter + Enter;
                byte[] fileContentStrByte = Encoding.UTF8.GetBytes(fileContentStr);
                myRequestStream.Write(fileContentStrByte, 0, fileContentStrByte.Length);

                //myRequestStream.Write(fileContentByte, 0, fileContentByte.Length);
                FileStream fs = new FileStream(filefd.Value, FileMode.Open, FileAccess.Read);
                byte[] fileContentByte = new byte[fs.Length]; // 二进制文件
                fs.Read(fileContentByte, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                fs.Dispose();
                myRequestStream.Write(fileContentByte, 0, fileContentByte.Length);

                //Enter
                string EnterStr = Enter;
                byte[] enterBytes = Encoding.UTF8.GetBytes(EnterStr);
                myRequestStream.Write(enterBytes, 0, enterBytes.Length);
            }
            #endregion

            // 添加 end boundary
            string EndStr = "--" + boundary + "--";
            byte[] endBytes = Encoding.UTF8.GetBytes(EndStr);
            myRequestStream.Write(endBytes, 0, endBytes.Length);

            #endregion

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();//发送

            Stream myResponseStream = response.GetResponseStream();//获取返回值
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));

            string retString = myStreamReader.ReadToEnd();

            myStreamReader.Close();
            myResponseStream.Close();
            myStreamReader.Dispose();
            myResponseStream.Dispose();

            return retString;
        }
    }
}
