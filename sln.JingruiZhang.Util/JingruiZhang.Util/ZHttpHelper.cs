using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Security.Authentication;

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
        [Obsolete("未使用Cookie")]
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
        [Obsolete("未使用Cookie")]
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
        [Obsolete("未使用Cookie")]
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
        [Obsolete("未使用Cookie")]
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
        /// <param name="boundary">可不传</param>        
        [Obsolete("未使用Cookie")]
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

        /// <summary>
        /// 发送 post 请求，适用于地址带有 QueryString 的地址，且需要使用 Cookie 字符串
        /// </summary>
        /// <param name="Url">请求地址</param>
        /// <param name="postQueryPara">请参考可选参数值</param>
        /// <param name="cookieStrLikePostman">请参考可选参数值</param>
        /// <returns></returns>
        public static string Post_Query_Cookie(string Url, string postQueryPara = "Key1=EncodedVal&key2=EncodedValue2", string cookieStrLikePostman = "X-LENOVO-SESS-ID=270b8fd36a6d8; path=/; domain=.filen;")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            if (cookieStrLikePostman != null)
            {
                request.Headers.Add("Cookie", cookieStrLikePostman);
            }

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postQueryPara.Length;
            StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
            writer.Write(postQueryPara);
            writer.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8"; //默认编码  
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
            string retString = reader.ReadToEnd();
            return retString;
        }
        
        /// <summary>
        /// 参考 Post_Query_Cookie 的测试方法
        /// </summary>
        public static string Post_WithFile_Cookie(string url, Dictionary<string, string> datas
            , Dictionary<string, string> filenameAndPaths, string boundary = "ceshi", string cookieStrLikePostman = "")
        {
            string Enter = "\r\n";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            if (!String.IsNullOrWhiteSpace(cookieStrLikePostman))
            {
                request.Headers.Add("Cookie", cookieStrLikePostman);
            }
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

        /// <summary>
        /// 参考 Post_Query_Cookie 的测试方法
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static string Get_Cookie(string Url, string cookies)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            if (cookies != null)
                request.Headers.Add("Cookie", cookies);
            request.ContentType = "application/x-www-form-urlencoded";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8"; //默认编码  
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
            string retString = reader.ReadToEnd();
            return retString;
        }

        /// <summary>
        /// get 请求（携带cookiestr）下载并获取 byte[]（配合 CreateDownloadBytesResponse 使用）
        /// </summary>
        /// <param name="Url">下载地址</param>
        /// <param name="cookies">cookie字符串（参考postman）</param>
        /// <param name="contentType">默认为二进制文件</param>
        public static byte[] Get_Bytes_Cookie(string Url, string cookies
            , string contentType = "application/octet-stream")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            if (cookies != null)
            {
                request.Headers.Add("Cookie", cookies);
            }
            request.ContentType = contentType;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8"; //默认编码  
            }
            Stream ResponseStream = response.GetResponseStream();
            MemoryStream ms = new MemoryStream();
            ResponseStream.CopyTo(ms);
            var Allbuffer = new byte[ms.Length];
            ms.Seek(0, SeekOrigin.Begin);
            ms.Read(Allbuffer, 0, Allbuffer.Length);
            ms.Close();
            ResponseStream.Close();
            response.Close();
            return Allbuffer;
        }
    }
}
