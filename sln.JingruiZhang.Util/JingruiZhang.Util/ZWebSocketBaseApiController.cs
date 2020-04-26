using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
#if NET45
using System.Web.Http;
using System.Web.WebSockets;
#else
#endif

namespace JingruiZhang.Util
{
    /// <summary>
    /// 具有 WebSocket 功能的基类，目前正在测试 NET45 框架下的功能，DotNetCore 暂未测试；使用时请注意 ApiController 或 ControllerBase 所依赖类库的 nuget 包版本
    /// </summary>
    public class ZWebSocketBaseApiController
#if NET45
        : ApiController
#else
#endif
    {
        /// <summary>
        /// 记录的客户端
        /// </summary>
        protected static List<WebSocket> _sockets = new List<WebSocket>();

#if NET45

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="cname">客户端标识</param>
        /// <returns></returns>
        [HttpGet]
        public virtual HttpResponseMessage GetConnect(string cname)
        {

            HttpContext.Current.AcceptWebSocketRequest(ProcessRequest); //在服务器端接受Web Socket请求，传入的函数作为Web Socket的处理函数，待Web Socket建立后该函数会被调用，在该函数中可以对Web Socket进行消息收发
            return Request.CreateResponse(HttpStatusCode.SwitchingProtocols); //构造同意切换至Web Socket的Response.

        }

        /// <summary>
        /// 处理接收到的消息，并作响应，参考此库源码来编写重写
        /// </summary>
        public virtual void ProcessRequestAndResponse(ArraySegment<byte> buffer, WebSocketReceiveResult receivedResult)
        {
            string recvMsg = Encoding.UTF8.GetString(buffer.Array, 0
                , receivedResult.Count);

            // 一个简单处理：小写转大写
            // ------------------------
            //recvMsg = recvMsg.ToUpper();

            // ArraySegment 提高性能？
            // ----------------------
            var recvBytes = Encoding.UTF8.GetBytes(recvMsg);
            var sendBuffer = new ArraySegment<byte>(recvBytes);

            // 遍历当前正在连接的所有客户端，并返回转为大写后的字符串
            // ------------------------------------------------------
            for (int i = 0; i < _sockets.Count; i++)
            {
                _sockets[i].SendAsync(sendBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        /// <summary>
        /// 接受信息和发送信息
        /// </summary>
        public virtual async Task ProcessRequest(AspNetWebSocketContext context)
        {
            try
            {
                // 只要有连接过来，就会进入到这里，socket 相当于这一个客户端。
                // ----------------------------------------------------------
                var socket = context.WebSocket;

                // 将当前的客户端加入所有客户端列表
                // --------------------------------
                _sockets.Add(socket);

                // 进入当前客户端的消息循环，当客户端断开时，也会发送消息，并使 ReceiveAsync 结束阻塞并执行下一句
                // --------------------------------
                while (true)
                {
                    var buffer = new ArraySegment<byte>(new byte[1024]);
                    var receivedResult = await socket.ReceiveAsync(buffer, CancellationToken.None);//对web socket进行异步接收数据

                    // 客户端消息到达之后，会停到此行代码
                    // 客户端断开时，也会停到此行代码
                    // ------------------------------
                    if (receivedResult.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);//如果client发起close请求，对client进行ack
                        _sockets.Remove(socket);
                        break;
                    }

                    if (socket.State == WebSocketState.Open)
                    {
                        ProcessRequestAndResponse(buffer, receivedResult);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
#else
#endif

    }
}
