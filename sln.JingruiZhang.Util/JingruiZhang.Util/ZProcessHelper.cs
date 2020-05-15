using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace JingruiZhang.Util
{
    /// <summary>
    /// 执行进程辅助类
    /// </summary>
    public class ZProcessHelper
    {
        /// <summary>
        /// 测试某端口是否处理监听状态
        /// </summary>
        /// <param name="port">端口号</param>
        /// <returns></returns>
        public static bool TestPortInUse(int port)
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
            var ep = ipEndPoints.FirstOrDefault(x => x.Port == port);
            return ep != null;
        }

        /// <summary>
        /// 后台执行java -jar 命令的辅助方法（起源于接口服务中静默转换Office文档为pdf。）
        /// </summary>
        /// <param name="javaExeFullPath">java.exe的全物理路径（暂不考虑是否已编辑过环境变量）</param>
        /// <param name="argumentsInclude_Jar">java.exe 后面跟的参数，如：-jar E:/Softwares/jodconvert2/jodconverter-cli-2.2.2.jar -p 8100 D:\00_a测试文件素材\office\旧版PPT.ppt D:\00_a测试文件素材\office\黑窗口已不存在1732.pdf</param>
        public static void RunJavaJarAtBackground(
            string javaExeFullPath = @"C:\jdk1.8.0_71\bin\java.exe"
            , string argumentsInclude_Jar = @"-jar E:/jodconverter-cli-2.2.2.jar -p 8100 D:\旧版PPT.ppt D:\32.pdf")
        {
            var psi = new ProcessStartInfo
            {
                FileName = javaExeFullPath // 必须带上路径，这里没有加载环境变量
            };
            psi.Arguments = argumentsInclude_Jar;

            //设置不在新窗口中启动新的进程
            psi.CreateNoWindow = true;

            //不使用操作系统使用的shell启动进程
            psi.UseShellExecute = false;

            //将输出信息重定向
            psi.RedirectStandardOutput = true;
            Process process = Process.Start(psi);
            process.WaitForExit();
            if (process != null)
            {
                process.Close();
                process.Dispose();
                process = null;
            }
        }
    }
}
