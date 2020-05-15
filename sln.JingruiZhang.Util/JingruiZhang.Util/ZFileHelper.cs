
#if NET45
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
#else
#endif

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
        /// 根据流计算Header字符串
        /// </summary>
        /// <param name="stream">文件流或内存流等</param>
        /// <returns>Header字符串，配合GetHeaderByExtName使用</returns>
        public static string GetHeaderByBytes(Stream stream)
        {
            string fileclass = "";
            using (BinaryReader reader = new BinaryReader(stream))
            {
                for (int i = 0; i < 2; i++)
                {
                    fileclass += reader.ReadByte().ToString();
                }
            }
            return fileclass;
        }

        /// <summary>
        /// 根据扩展名（如：.jpg) 得到可能的Header字符串（用于根据内容判断实际类型）
        /// </summary>
        /// <param name="extname">（如：.jpg) </param>
        /// <returns>字符串集合，配合 GetHeaderByBytes 方法使用</returns>
        public static List<string> GetHeaderByExtName(string extname)
        {
            if (String.IsNullOrWhiteSpace(extname))
            {
                return null;
            }
            extname = extname.ToLower();
            switch (extname)
            {
                case ".jpg":
                    return new List<string>() { "255216" };
                case ".doc":
                case ".xls":
                case ".ppt":
                case ".wps":
                    return new List<string>() { "208207" };
                case ".docx":
                case ".pptx":
                case ".xlsx":
                case ".zip":
                case ".mmap":
                    return new List<string>() { "8075" };
                case ".txt":
                    return new List<string>() { "5150", "4946", "104116", "239187" };
                case ".rar":
                    return new List<string>() { "8297" };
                case ".pdf":
                    return new List<string>() { "3780" };
                case ".gif":
                    return new List<string>() { "7173" };
                case ".png":
                    return new List<string>() { "13780" };
                case ".bmp":
                    return new List<string>() { "6677" };
                case ".aspx":
                case ".asp":
                case ".sql":
                    return new List<string>() { "239187" };
                case ".xml":
                    return new List<string>() { "6063" };
                case ".htm":
                case ".html":
                    return new List<string>() { "6033" };
                case ".js":
                    return new List<string>() { "4742" };
                case ".accdb":
                case ".mdb":
                    return new List<string>() { "01" };
                case ".exe":
                case ".dll":
                    return new List<string>() { "7790" };
                case ".psd":
                    return new List<string>() { "5666" };
                case ".rdp":
                    return new List<string>() { "255254" };
                case ".torrent":
                    return new List<string>() { "10056" };

                case ".bat":
                    return new List<string>() { "64101" };
                case ".sgf":
                    return new List<string>() { "4059" };

                default:
                    return new List<string>();
            }
        }

#if NET45
        /// <summary>
        /// 压缩多个文件项（文件或文件夹）方法
        /// </summary>
        /// <param name="parentDirPath">当前多选项所处于的父目录（末尾需要确保有“\”）</param>
        /// <param name="multiSelectedDirOrFiles">多个文件或文件夹的物理路径，每一项末尾不要有“\”</param>
        /// <param name="GzipFileName">目标zip文件物理路径</param>
        [Obsolete("不支持DotNetCore")]
        public static void CompressSelected(string parentDirPath, List<string> multiSelectedDirOrFiles, string GzipFileName)
        {
            try
            {
                //创建文件流
                // ---------
                FileStream pCompressFile = new FileStream(GzipFileName, FileMode.Create);

                // 使用文件流创建zip输出流
                // -----------------------
                using (ZipOutputStream zipoutputstream = new ZipOutputStream(pCompressFile))
                {
                    // 获取 dirPath 目录下的所有文件
                    // -----------------------------
                    Crc32 crc = new Crc32();
                    Dictionary<string, DateTime> fileList = GetAllFiles(multiSelectedDirOrFiles);

                    // 遍历每个文件并进行 ZipEntry 的构造
                    // ----------------------------------
                    foreach (KeyValuePair<string, DateTime> item in fileList)
                    {
                        // 如果是文件
                        // ----------
                        if (File.Exists(item.Key))
                        {
                            // 将文件压缩到压缩包中
                            // --------------------
#region ...
                            FileStream fs = new FileStream(item.Key.ToString(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            // FileStream fs = File.OpenRead(item.Key.ToString());
                            byte[] buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, buffer.Length);
                            string thisEntryName = item.Key.Substring(parentDirPath.Length);
                            ZipEntry entry = new ZipEntry(thisEntryName);
                            entry.DateTime = item.Value;
                            entry.Size = fs.Length;
                            fs.Close();
                            crc.Reset();
                            crc.Update(buffer);
                            entry.Crc = crc.Value;
                            zipoutputstream.PutNextEntry(entry);
                            zipoutputstream.Write(buffer, 0, buffer.Length);
#endregion
                        }
                        else if (Directory.Exists(item.Key))
                        {
                            // 将文件夹压缩到压缩包中
                            // ----------------------
#region ...
                            var di = new DirectoryInfo(item.Key);
                            var difiles = di.GetFiles();
                            var didirs = di.GetDirectories();
                            if (didirs.Length == 0 && difiles.Length == 0)
                            {
                                string thisEntryName = item.Key.Substring(parentDirPath.Length);
                                if (!thisEntryName.EndsWith("/"))
                                {
                                    thisEntryName += "/";
                                }
                                ZipEntry entry = new ZipEntry(thisEntryName);
                                zipoutputstream.PutNextEntry(entry);
                            }
#endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(@"D:\Logs\zip.txt", ex.StackTrace);
            }
        }

        // 私有方法
        // --------
#region ...
        private static Dictionary<string, DateTime> GetAllFiles(List<string> filePathOrDirPath)
        {
            if (filePathOrDirPath == null)
            {
                return null;
            }
            Dictionary<string, DateTime> FilesList = new Dictionary<string, DateTime>();
            for (int i = 0; i < filePathOrDirPath.Count; i++)
            {
                if (File.Exists(filePathOrDirPath[i]))
                {
                    FileInfo file = new FileInfo(filePathOrDirPath[i]);
                    FilesList.Add(file.FullName, file.LastWriteTime);
                }
                else if (Directory.Exists(filePathOrDirPath[i]))
                {
                    DirectoryInfo dirinfo = new DirectoryInfo(filePathOrDirPath[i]);

                    // 额外把文件夹保存到 FilesList 中
                    FilesList.Add(filePathOrDirPath[i], dirinfo.LastWriteTime);

                    GetAllDirFiles(dirinfo, FilesList);
                    GetAllDirsFiles(dirinfo.GetDirectories(), FilesList);
                }
            }
            return FilesList;
        }

        private static void GetAllDirsFiles(DirectoryInfo[] dirs, Dictionary<string, DateTime> filesList)
        {
            foreach (DirectoryInfo dir in dirs)
            {
                foreach (FileInfo file in dir.GetFiles("."))
                {
                    filesList.Add(file.FullName, file.LastWriteTime);
                }
                GetAllDirsFiles(dir.GetDirectories(), filesList);
            }
        }

        private static void GetAllDirFiles(DirectoryInfo dir, Dictionary<string, DateTime> filesList)
        {
            foreach (FileInfo file in dir.GetFiles())
            {
                filesList.Add(file.FullName, file.LastWriteTime);
            }
        }
#endregion
#else
#endif




        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        /// <param name="resource">该目录是已存在的，如：C:\A\Dir1</param>
        /// <param name="dest">该目录是拷贝后才存在的，如:C:\B\Dir1</param>
        public static void CopyDir(string resource, string dest)
        {
            // 参数判空
            // --------
#region ...
            if (String.IsNullOrWhiteSpace(resource))
            {
                throw new ArgumentNullException(resource);
            }
            if (String.IsNullOrWhiteSpace(dest))
            {
                throw new ArgumentNullException(dest);
            }
#endregion

            // 判断文件夹是否存在
            // ------------------
#region ...
            if (!Directory.Exists(resource))
            {
                throw new Exception(String.Format("文件夹{0}不存在", resource));
            }
#endregion

            // 创建 dest 文件夹
            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
            }

            string[] childdirs = Directory.GetDirectories(resource);
            for (int i = 0; i < childdirs.Length; i++)
            {
                // 获取 childdirs 的名字，用于拼接
                // -------------------------------
                string thisChildName = Path.GetFileName(childdirs[i]);

                // C:\A\Dir1\c1 => C:\B\Dir1\[c1]
                CopyDir(childdirs[i], Path.Combine(dest, thisChildName));
            }

            string[] childFiles = Directory.GetFiles(resource);
            for (int i = 0; i < childFiles.Length; i++)
            {
                // C:\A\Dir1\1.txt => C:\B\Dir1\[1.txt]
                string thisChildName = Path.GetFileName(childFiles[i]);
                File.Copy(childFiles[i], Path.Combine(dest, thisChildName));
            }
        }

        /// <summary>
        /// 将指定 bytes 写入到新文件
        /// </summary>
        /// <param name="newfilePath">指定新文件的路径（文件会自动创建）</param>
        /// <param name="bytes">文件内容</param>
        /// <param name="mode">使用枚举值</param>
        /// <param name="accesscfg">使用枚举值</param>
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
