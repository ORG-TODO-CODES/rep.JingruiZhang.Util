using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JingruiZhang.Util.FleckNet45ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var clientObj = new Fleck.WebSocketConnection(listenUrl);

            var ws = new ClientWebSocket();
            await ws.ConnectAsync(new Uri("ws://127.0.0.1:8080"), CancellationToken.None);
        }
    }
}
