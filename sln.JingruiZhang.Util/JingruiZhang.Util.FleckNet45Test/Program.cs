using Fleck;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace JingruiZhang.Util.FleckNet45Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // pfx 文件路径准备
            // ----------------
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string assemblyDirPath = Path.GetDirectoryName(assemblyPath);
            string pfxPath = Path.Combine(assemblyDirPath, "ResourceFiles", "probim_cn_2021_12_0000.pfx");

            // 监听地址准备
            // ------------
            string listenUrl = "wss://0.0.0.0:8083";

            //// 测试：判断文件是否存在
            //// -----------------------
            //bool ifFileExists = File.Exists(pfxPath);

            // 所有客户端的代理
            // ----------------
            var allClients = new List<IWebSocketConnection>();

            // 创建 server 实例
            // ----------------
            var serverObj = new WebSocketServer(listenUrl);

            // 添加 SSL 证书
            // -------------
            serverObj.Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(pfxPath, "0000", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

            serverObj.Start((cfg) =>
            {
                // OnOpen 在连接后会触发？
                // ----------------------
                cfg.OnOpen = () =>
                {
                    Console.WriteLine(String.Format("【OnOpen 触发】{0}", listenUrl));
                    allClients.Add(cfg);
                };

                cfg.OnClose = () =>
                {
                    Console.WriteLine(String.Format("【OnClose 触发】"));
                    allClients.Remove(cfg);
                };

                cfg.OnMessage = (str) =>
                {
                    Console.WriteLine(String.Format("【OnMessage 触发】{0}", str));
                    for (int i = 0; i < allClients.Count; i++)
                    {
                        allClients[i].Send(str);
                    }
                };

            });


            // intervene
            // {"Title":"【系统通知】","Content":"这是一条系统通知","Receiver":"all"}
            // ---------
            Console.WriteLine("【服务器就绪】");
            var input = Console.ReadLine();
            while (input != "exit")
            {
                foreach (var socket in allClients.ToList())
                {
                    socket.Send(input);
                }
                input = Console.ReadLine();
            }

        }
    }
}
