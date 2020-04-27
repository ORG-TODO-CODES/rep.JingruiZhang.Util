using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace JingruiZhang.Util
{
    /// <summary>
    /// 极光推送
    /// </summary>
    public class ZJPushHelper
    {
        /// <summary>
        /// 按别名推送消息
        /// </summary>
        /// <param name="appKey">appKey</param>
        /// <param name="masterSecret">masterSecret</param>
        /// <param name="alias">多个别名</param>
        /// <param name="allalert">推送的内容：【2020-04-27 11:39:42】【XXX】于项目【XXX】中添加了问题【sss  】</param>
        /// <param name="androidtitle">问题模块</param>
        /// <param name="ios_extraobj">额外的数据对象</param>
        /// <param name="apns_production">是否为生产环境</param>
        /// <param name="jiguangPostUrl">极光推送api地址</param>
        /// <returns></returns>
        public static string Push(
            string appKey
            , string masterSecret
            , List<string> alias
            , string allalert
            , string androidtitle
            , object ios_extraobj
            , bool apns_production
            , string jiguangPostUrl = "https://api.jpush.cn/v3/push")
        {
            /*
             appKey
"9b3e0bc5xxxxf45dd"
masterSecret
"8cc785xxxx1be5077d6"
alias.Count
1
alias[0]
"9d9cx069+axxa+4564+9xx9+591xxx560332"
allalert
"【2020-04-27 11:39:42】【xxx】于项目【xxx】中添加了问题【sss  】"
androidtitle
"xxxx模块"
ios_extraobj
{ OrganizeId = "7xxxx1e0-4f4a-xxx2-6xx4-f59xxxxxxx8", Object = {ProBIM.Application.Entity.ProjectManage.msg_msgEntity} }
    Object: {ProBIM.Application.Entity.ProjectManage.msg_msgEntity}
    OrganizeId: "7xxxx1e0-4f4a-xxx2-6xx4-f59xxxxxxx8"
apns_production
false
jiguangPostUrl
"https://api.jpush.cn/v3/push"
             */

            // 纠正 alias
            if (alias != null && alias.Count > 0)
            {
                for (int i = 0; i < alias.Count; i++)
                {
                    var ali = alias[i];
                    alias[i] = ali.Replace("-", "+");
                }
            }

            // 拼装请求头
            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(appKey + ":" + masterSecret));

            //
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", String.Concat("Basic ", auth));
            //JavaScriptSerializer jser = new JavaScriptSerializer();
            #region pushobj
            var pushobj = new
            {

                platform = new List<string>() { "android", "ios" },
                audience = new
                {
                    alias = alias
                },
                notification = new
                {
                    alert = allalert,
                    android = new
                    {
                        alert = allalert,
                        title = androidtitle
                    },
                    ios = new
                    {
                        alert = allalert,
                        badge = "+1",
                        extras = ios_extraobj
                    },
                },
                message = new
                {
                    msg_content = "message content",
                    title = "title",
                    extras = new
                    {
                        key1 = "value1"
                    }
                },
                options = new
                {
                    apns_production = apns_production//false
                }
            };
            #endregion
            var datastr = ObjectExtension.Serialize(pushobj);
            //string datastr = jser.Serialize(pushobj);
            HttpContent httpContent = new StringContent(datastr, Encoding.UTF8);

            Task<HttpResponseMessage> task = httpClient.PostAsync(jiguangPostUrl, httpContent);
            task.Wait();
            Task<string> task2 = task.Result.Content.ReadAsStringAsync();
            task2.Wait();

            return task2.Result;
        }
    }
}
