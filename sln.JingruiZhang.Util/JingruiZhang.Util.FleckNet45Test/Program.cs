using Fleck;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            // 配置项读取
            // ----------
            string BIND_port = ConfigurationManager.AppSettings["BIND_port"];
            string BIND_useSSL = ConfigurationManager.AppSettings["BIND_useSSL"];
            string BIND_pfxFileName = ConfigurationManager.AppSettings["BIND_pfxFileName"];
            string BIND_pfxFilePassword = ConfigurationManager.AppSettings["BIND_pfxFilePassword"];

            // 监听地址准备
            // ------------
            string listenUrl = String.Format("{0}://0.0.0.0:{1}", BIND_useSSL == "1" ? "wss" : "ws", BIND_port);

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
            if (BIND_useSSL == "1")
            {
                // pfx 文件路径准备
                // ----------------
                string assemblyPath = Assembly.GetExecutingAssembly().Location;
                string assemblyDirPath = Path.GetDirectoryName(assemblyPath);
                string pfxPath = Path.Combine(assemblyDirPath, "ResourceFiles", BIND_pfxFileName);

                serverObj.Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(pfxPath, BIND_pfxFilePassword, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            }

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
