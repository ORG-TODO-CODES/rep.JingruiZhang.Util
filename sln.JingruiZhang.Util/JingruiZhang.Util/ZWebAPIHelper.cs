using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace JingruiZhang.Util
{
    /// <summary>
    /// WebAPI 帮助类
    /// </summary>
    public class ZWebAPIHelper
    {

        /// <summary>
        /// 创建 ExcelPackage 内存流，并使用特定方式填充内容
        /// </summary>
        /// <param name="saveAction">package.SaveAs(memorystream);</param>
        /// <returns></returns>
        public static MemoryStream GetExcelPackageStream(Action<MemoryStream> saveAction)
        {
            MemoryStream memorystream = new MemoryStream();
            saveAction(memorystream);
            memorystream.Seek(0, SeekOrigin.Begin);//没这句话就格式错
            return memorystream;
        }

        /// <summary>
        /// 创建文件下载的响应消息，调用示例：CreateDownloadResponse(packageMemoryStream, "示例.xlsx", "application/octet-stream")
        /// </summary>
        /// <param name="stream">内存流</param>
        /// <param name="originfilename">文件名称（未EnCode）</param>
        /// <param name="contenttype">application/octet-stream</param>
        /// <param name="statuscode">服务器状态码</param>
        /// <param name="ContentDisposition">响应头Disposition的值</param>
        /// <returns></returns>
        public static HttpResponseMessage CreateDownloadResponse(Stream stream, string originfilename,
            string contenttype = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", HttpStatusCode statuscode = HttpStatusCode.OK, string ContentDisposition = "attachment")
        {
            HttpResponseMessage response = new HttpResponseMessage(statuscode);
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(contenttype);//application/octet-stream
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue(ContentDisposition);
            response.Content.Headers.ContentDisposition.FileName = HttpUtility.UrlEncode(originfilename);//file.xls用ContentType：application/vnd.ms-excel   
            return response;
        }

        /// <summary>
        /// 返回数据和服务器处理状态
        /// </summary>
        /// <param name="code">服务器状态码</param>
        /// <param name="data">返回业务数据</param>
        public static HttpResponseMessage CreateDataResponse(HttpStatusCode code, object data = null)
        {
            HttpResponseMessage res = new HttpResponseMessage();
            res.Content = new StringContent( ObjectExtension.Serialize(data));
            res.StatusCode = code;
            return res;
        }

        /// <summary>
        /// 返回数据和服务器处理状态
        /// </summary>
        /// <param name="code">服务器状态码</param>
        /// <param name="Data">返回业务数据</param>
        /// <param name="Ret">返回1或负数</param>
        /// <param name="Msg">OK或错误信息</param>
        [Obsolete("定制化代码")]
        public static HttpResponseMessage CreateDataResponse(HttpStatusCode code, object Data = null, int Ret = 1, string Msg = "OK")
        {
            return CreateDataResponse(HttpStatusCode.OK, new { Data, Ret, Msg});
        }

        /// <summary>
        /// 返回数据和服务器处理状态
        /// </summary>
        /// <param name="ex">异常对象</param>
        [Obsolete("定制化代码")]
        public static HttpResponseMessage CreateDataResponse_Exception(Exception ex)
        {
            return CreateDataResponse(HttpStatusCode.OK, null, -1, ex.Message);
        }

        /// <summary>
        /// 返回数据和服务器处理状态
        /// </summary>
        /// <param name="data">业务数据对象</param>
        [Obsolete("定制化代码")]
        public static HttpResponseMessage CreateDataResponse_Data(object data)
        {
            return CreateDataResponse(HttpStatusCode.OK, data, 1, "OK");
        }
    }
}
