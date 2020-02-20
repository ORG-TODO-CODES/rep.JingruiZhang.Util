using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JingruiZhang.Util
{
    /// <summary>
    /// 文件操作帮助类
    /// </summary>
    public class ZFileHelper
    {
        /// <summary>
        /// 将指定 bytes 写入到新文件
        /// </summary>
        /// <param name="newfilePath">指定新文件的路径（文件会自动创建）</param>
        /// <param name="bytes">文件内容</param>
        /// <returns></returns>
        public static void WriteToNewPath(string newfilePath, byte[] bytes, FileMode mode, FileAccess accesscfg)
        {
            using (FileStream writeFs = new FileStream(newfilePath, mode, accesscfg))
            {
                using (BinaryWriter bw = new BinaryWriter(writeFs))
                {
                    bw.Write(bytes);
                }
            }
        }

        /// <summary>
        /// 读取指定路径的文件内容到 byte[] 中
        /// </summary>
        public static byte[] ReadFromPath(string path)
        {
            FileInfo fi = new FileInfo(path);
            if (fi.Length >= int.MaxValue)
            {
                throw new Exception("文件太大");
            }
            byte[] input;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                // fs 流的读取器
                // -------------
                using (BinaryReader br = new BinaryReader(fs))
                {
                    input = br.ReadBytes((int)fi.Length);
                }
            }
            return input;
        }

        /// <summary>
        /// 分割大文件
        /// </summary>
        /// <param name="srcLocalPath">被分割源文件的物理路径</param>
        /// <param name="targetPathFormat">分割生成每个文件片的路径格式字符串，如：F:\A_{0}.txt</param>
        /// <param name="sectionAct">生成每个文件时的回调</param>
        /// <param name="finishAct">完成时的回调</param>
        /// <param name="splitFileSize">分隔的每个文件大小</param>
        /// <returns></returns>
        public static List<string> SplitLargeFile(
string srcLocalPath, string targetPathFormat, Action<string> sectionAct, Action finishAct, int splitFileSize = 1024 * 1024 * 100)
        {
            List<string> paths = new List<string>();

            // 用于获取文件大小
            // ----------------
            FileInfo fileInfo = new FileInfo(srcLocalPath);

            using (FileStream fs = new FileStream(srcLocalPath, FileMode.Open, FileAccess.Read))
            {
                // fs 流的读取器
                // -------------
                using (BinaryReader br = new BinaryReader(fs))
                {
                    int i = 1;
                    bool isReadingComplete = false;
                    while (!isReadingComplete)
                    {
                        // 单个文件的文件名
                        // ----------------
                        string filePath = String.Format(targetPathFormat, i);

                        // 边读边进行位移。
                        // ---------------
                        byte[] input = br.ReadBytes(splitFileSize);

                        WriteToNewPath(filePath, input, FileMode.Create, FileAccess.ReadWrite);
                        paths.Add(filePath);

                        // 如果此次读取的总大小与分割单元的不等，视为即将分割完毕
                        // ------------------------------------------------------
                        isReadingComplete = (input.Length != splitFileSize);
                        if (!isReadingComplete)
                        {
                            i += 1;
                        }

                        if (sectionAct != null)
                        {
                            sectionAct.Invoke(filePath);
                        }
                    }
                }
            }

            if (finishAct != null)
            {
                finishAct.Invoke();
            }
            return paths;
        }

        /// <summary>
        /// 合并被分割的多个文件
        /// </summary>
        /// <param name="splitFiles">被分割的多个文件</param>
        /// <param name="targetPath">合并后的物理路径</param>
        /// <param name="finishAct">完成时的回调</param>
        /// <returns></returns>
        public static void MergeSplitFiles(List<string> splitFiles, string targetPath, Action finishAct)
        {
            using (FileStream fs = new FileStream(targetPath, FileMode.Create, FileAccess.ReadWrite))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    int has = 0;
                    for (int i = 0; i < splitFiles.Count; i++)
                    {
                        var bytes = ReadFromPath(splitFiles[i]);

                        // 将 bytes 写入新的大文件
                        bw.Write(bytes, 0, bytes.Length);
                        has += bytes.Length;
                        bw.Seek(has, SeekOrigin.Begin);
                    }
                }
            }
            if (finishAct != null)
            {
                finishAct.Invoke();
            }
        }
    }
}
