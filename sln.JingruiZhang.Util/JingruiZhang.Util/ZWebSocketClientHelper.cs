using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JingruiZhang.Util
{
    /// <summary>
    /// WebSocket 客户端帮助类
    /// </summary>
    public class ZWebSocketClientHelper
    {
        /// <summary>
        /// 连接url，发送广播消息，并断开
        /// </summary>
        /// <param name="wsOrWssUrl"></param>
        /// <param name="data"></param>
        public static async void SendMsg(string wsOrWssUrl, string data)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(data);
                ClientWebSocket webSocket = new ClientWebSocket();
                await webSocket.ConnectAsync(new Uri(wsOrWssUrl), CancellationToken.None);
                await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Binary, true, CancellationToken.None);
                await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "1"
                    , CancellationToken.None);
                webSocket.Dispose();
            }
            catch (Exception ex)
            {

            }

        }
    }
}
