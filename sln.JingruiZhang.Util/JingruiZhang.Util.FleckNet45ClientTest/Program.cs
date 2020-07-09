using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;

namespace JingruiZhang.Util.FleckNet45ClientTest
{
    class Program
    {
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

        static void Main(string[] args)
        {
            string url = "wss://www.probim.cn:8083";
            string data = @"{""Title"":""【系统通知】"",""Content"":""这是一条系统通知"",""Receiver"":""all""}";

            Program p = new Program();
            p.SendMsg2(url, data);

            Console.ReadKey();
        }

      
    }
}
