using Fleck;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace JingruiZhang.Util
{
    /// <summary>
    /// WebSocket 服务器类
    /// </summary>
    public class ZWebSocketServer
    {
        /// <summary>
        /// 由构造函数传入，监听的端口号
        /// </summary>
        protected string m_port;

        /// <summary>
        /// 由构件函数传入，是否使用SSL，即 wss 协议。如果为"1"：则为 wss协议。如果为其它值（比如"0"）：则为 ws 协议
        /// </summary>
        protected string m_useSSL;

        /// <summary>
        /// 导出的 pfx 证书文件（如iis的pfx导出文件）
        /// </summary>
        protected string m_pfxFilePath;

        /// <summary>
        /// 导出 pfx 证书文件时指定的密码
        /// </summary>
        protected string m_pfxFilePassword;

        /// <summary>
        /// 内部的 WebSocketServer 对象
        /// </summary>
        private WebSocketServer m_webSocketServer;

        /// <summary>
        /// 内部的 所有的客户端代理
        /// </summary>
        private List<IWebSocketConnection> m_webSocketClientList;

        /// <summary>
        /// 创建 ZWebSocketServer 的实例
        /// </summary>
        /// <param name="port">监听的端口号</param>
        /// <param name="useSSL">使用wss协议，且值为1，否则为0</param>
        /// <param name="pfxFilePath">当useSSL为1时必须指定，SSL证书物理路径</param>
        /// <param name="pfxFilePassword">当useSSL为1时必须指定，SSL证书密码</param>
        public ZWebSocketServer(string port
            , string useSSL = "0"
            , string pfxFilePath = null
            , string pfxFilePassword = null)
        {
            // 参数检查
            // --------
            #region ...
            if (String.IsNullOrWhiteSpace(port))
            {
                throw new ArgumentNullException("ZWebSocketServer::ZWebSocketServer.port");
            }
            if (String.IsNullOrWhiteSpace(useSSL))
            {
                throw new ArgumentNullException("ZWebSocketServer::ZWebSocketServer.useSSL");
            }
            if (useSSL == "1")
            {
                if (String.IsNullOrWhiteSpace(pfxFilePath))
                {
                    throw new ArgumentNullException("case useSSL 1: ZWebSocketServer::ZWebSocketServer.pfxFilePath");
                }
                if (String.IsNullOrWhiteSpace(pfxFilePassword))
                {
                    throw new ArgumentNullException("case useSSL 1: ZWebSocketServer::ZWebSocketServer.pfxFilePassword");
                }

                // 判断文件是否存在
                // ----------------
                if (!File.Exists(pfxFilePath))
                {
                    throw new FileNotFoundException(String.Format("文件{0}不存在", pfxFilePath));
                }
            }
            #endregion

            this.m_port = port;
            this.m_useSSL = useSSL;
            this.m_pfxFilePath = pfxFilePath;
            this.m_pfxFilePassword = pfxFilePassword;

        }

        /// <summary>
        /// 初始化工作
        /// </summary>
        /// <param name="OnErrorCallback">如果指定为null，则不绑定内部Server对象的OnError</param>
        /// <param name="OnOpenExtend">指定为null则不触发。触发时参数为当前内部Server监听的地址</param>
        /// <param name="OnCloseExtend">指定为null则不触发</param>
        /// <param name="OnMessageExtend">指定为null则不触发，触发时参数为当前内部Server收到的消息</param>
        public void Init(Action<Exception> OnErrorCallback
            , Action<string> OnOpenExtend
            , Action OnCloseExtend
            , Action<string> OnMessageExtend)

        {
            // 监听地址准备
            // ------------
            string listenUrl = String.Format("{0}://0.0.0.0:{1}", this.m_useSSL == "1" ? "wss" : "ws", this.m_port);

            // 所有客户端的代理
            // ----------------
            this.m_webSocketClientList = new List<IWebSocketConnection>();

            // 创建 server 实例
            // ----------------
            this.m_webSocketServer = new WebSocketServer(listenUrl);

            // 添加 SSL 证书
            // -------------
            if (this.m_useSSL == "1")
            {
                this.m_webSocketServer.Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(this.m_pfxFilePath, this.m_pfxFilePassword, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            }

            // 调用内部的 m_webSocketServer 的 Start
            // -------------------------------------
            this.m_webSocketServer.Start((cfg) =>
            {
                // OnOpen 在连接后会触发？
                // ----------------------
                cfg.OnOpen = () =>
                {
                    if (OnOpenExtend != null)
                    {
                        OnOpenExtend.Invoke(listenUrl);
                    }
                    this.m_webSocketClientList.Add(cfg);
                };
                cfg.OnClose = () =>
                {
                    if (OnCloseExtend != null)
                    {
                        OnCloseExtend.Invoke();
                    }
                    this.m_webSocketClientList.Remove(cfg);
                };
                cfg.OnMessage = (str) =>
                {
                    if (OnMessageExtend != null)
                    {
                        OnMessageExtend.Invoke(str);
                    }
                    for (int i = 0; i < this.m_webSocketClientList.Count; i++)
                    {
                        this.m_webSocketClientList[i].Send(str);
                    }
                };
                if (OnErrorCallback != null)
                {
                    cfg.OnError = OnErrorCallback;
                }

            });
        }

        /// <summary>
        /// 调用 Init 后调用此方法。（内部使用 ReadLine 读取，并判断输入的如果是 Exit 则直接退出）
        /// </summary>
        public virtual void Run()
        {
            Console.WriteLine("【服务器就绪】");
            var input = Console.ReadLine();
            while (input != "exit")
            {
                foreach (var socket in this.m_webSocketClientList.ToList())
                {
                    socket.Send(input);
                }
                input = Console.ReadLine();
            }
        }
    }
}
