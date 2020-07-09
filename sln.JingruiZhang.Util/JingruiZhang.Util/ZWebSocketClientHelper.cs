using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SuperSocket.ClientEngine;
using WebSocket4Net;

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
        [Obsolete("此方法不稳定，不建议用")]
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

        #region 一次性发送 WebSocket 消息给服务器端
        WebSocket4Net.WebSocket websocket;
        string data;

        /// <summary>
        /// 连接 url，向服务器端发消息
        /// </summary>
        /// <param name="wsOrWssUrl">ws或wss地址</param>
        /// <param name="data">要发送的字符串</param>
        public void SendMsg2(string wsOrWssUrl, string data)
        {
            this.data = data;
            websocket = new WebSocket4Net.WebSocket(wsOrWssUrl);
            websocket.Opened += websocket_Opened;
            websocket.Closed += websocket_Closed;
            websocket.Error += Websocket_Error;
            websocket.MessageReceived += websocket_MessageReceived;
            websocket.Open();
        }

        private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
        }

        private void Websocket_Error(object sender, ErrorEventArgs e)
        {
        }

        private void websocket_Closed(object sender, EventArgs e)
        {
        }

        private void websocket_Opened(object sender, EventArgs e)
        {
            websocket.Send(this.data);
            websocket.Close();
        }
        #endregion
    }
}
