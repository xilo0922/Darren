using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace OFM.FTP
{
    /// <summary>
    /// ftp方式文件下载上传
    /// </summary>
    public static class FileUpDownload
    {
        #region 变量属性

        /// <summary>
        /// Ftp服务器ip
        /// </summary>
        public static string FtpServerIp = string.Empty;

        /// <summary>
        /// Ftp 指定用户名
        /// </summary>
        public static string FtpUserId = string.Empty;

        /// <summary>
        /// Ftp 指定用户密码
        /// </summary>
        public static string FtpPassword = string.Empty;

        #endregion 变量属性

        #region 从FTP服务器下载文件，指定本地路径和本地文件名

        /// <summary>
        /// 从FTP服务器下载文件，指定本地路径和本地文件名（支持断点下载）
        /// </summary>
        /// <param name="remoteFileName">远程文件名</param>
        /// <param name="localFileName">保存本地的文件名（包含路径）</param>
        /// <param name="ifCredential">是否启用身份验证（false：表示允许用户匿名下载）</param>
        /// <param name="size">已下载文件流大小</param>
        /// <param name="updateProgress">报告进度的处理(第一个参数：总大小，第二个参数：当前进度)</param>
        /// <returns>是否下载成功</returns>
        public static bool FtpBrokenDownload(string remoteFileName, string localFileName, bool ifCredential, long size, Action<int, int> updateProgress = null)
        {
            Stream ftpStream = null;
            FtpWebResponse response = null;
            FileStream outputStream = null;
            try
            {
                outputStream = new FileStream(localFileName, FileMode.Append);
                if (FtpServerIp == null || FtpServerIp.Trim().Length == 0)
                {
                    throw new Exception("ftp下载目标服务器地址未设置！");
                }
                Uri uri = new Uri("ftp://" + FtpServerIp + "/" + remoteFileName);
                FtpWebRequest ftpsize = (FtpWebRequest)FtpWebRequest.Create(uri);
                ftpsize.UseBinary = true;
                ftpsize.ContentOffset = size;

                FtpWebRequest reqFtp = (FtpWebRequest)FtpWebRequest.Create(uri);
                reqFtp.UseBinary = true;
                reqFtp.KeepAlive = false;
                reqFtp.ContentOffset = size;
                if (ifCredential)//使用用户身份认证
                {
                    ftpsize.Credentials = new NetworkCredential(FtpUserId, FtpPassword);
                    reqFtp.Credentials = new NetworkCredential(FtpUserId, FtpPassword);
                }

                ftpsize.Method = WebRequestMethods.Ftp.GetFileSize;
                FtpWebResponse re = (FtpWebResponse)ftpsize.GetResponse();
                long totalBytes = re.ContentLength;
                re.Close();

                reqFtp.Method = WebRequestMethods.Ftp.DownloadFile;
                response = (FtpWebResponse)reqFtp.GetResponse();
                ftpStream = response.GetResponseStream();

                //更新进度
                if (updateProgress != null)
                {
                    updateProgress((int)totalBytes, 0);//更新进度条
                }
                long totalDownloadedByte = 0;
                int bufferSize = 2048;
                int readCount = 0;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    totalDownloadedByte = readCount + totalDownloadedByte;
                    outputStream.Write(buffer, 0, readCount);
                    //更新进度
                    if (updateProgress != null)
                    {
                        updateProgress((int)totalBytes, (int)totalDownloadedByte);//更新进度条
                    }
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (ftpStream != null)
                {
                    ftpStream.Close();
                }
                if (outputStream != null)
                {
                    outputStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        /// <summary>
        /// 从FTP服务器下载文件，指定本地路径和本地文件名
        /// </summary>
        /// <param name="remoteFileName">远程文件名</param>
        /// <param name="localFileName">保存本地的文件名（包含路径）</param>
        /// <param name="ifCredential">是否启用身份验证（false：表示允许用户匿名下载）</param>
        /// <param name="updateProgress">报告进度的处理(第一个参数：总大小，第二个参数：当前进度)</param>
        /// <returns>是否下载成功</returns>
        public static bool FtpDownload(string remoteFileName, string localFileName, bool ifCredential, Action<int, int> updateProgress = null)
        {
            try
            {
                long size = 0;
                if (File.Exists(localFileName))
                {
                    using (FileStream outputStream = new FileStream(localFileName, FileMode.Open))
                    {
                        size = outputStream.Length;
                    }
                }
                return FtpBrokenDownload(remoteFileName, localFileName, ifCredential, size, updateProgress);
            }
            catch
            {
                throw;
            }
        }
        #endregion 从FTP服务器下载文件，指定本地路径和本地文件名

        #region 上传文件到FTP服务器

        /// <summary>
        /// 上传文件到FTP服务器(断点续传)
        /// </summary>
        /// <param name="localFullPath">本地文件全路径名称：C:\Users\JianKunKing\Desktop\IronPython脚本测试工具</param>
        /// <param name="remoteFilepath">远程文件所在文件夹路径</param>
        /// <param name="updateProgress">报告进度的处理(第一个参数：总大小，第二个参数：当前进度)</param>
        /// <returns></returns>
        public static bool FtpUpload(string localFullPath, string remoteFilepath, Action<int, int> updateProgress = null)
        {
            if (remoteFilepath == null)
            {
                remoteFilepath = "";
            }
            string newFileName = string.Empty;
            bool success = true;
            FileInfo fileInf = new FileInfo(localFullPath);
            long allbye = (long)fileInf.Length;
            if (fileInf.Name.IndexOf("#") == -1)
            {
                newFileName = RemoveSpaces(fileInf.Name);
            }
            else
            {
                newFileName = fileInf.Name.Replace("#", "＃");
                newFileName = RemoveSpaces(newFileName);
            }
            long startfilesize = GetFileSize(newFileName, remoteFilepath);
            if (startfilesize >= allbye)
            {
                return false;
            }
            long startbye = startfilesize;
            //更新进度
            if (updateProgress != null)
            {
                updateProgress((int)allbye, (int)startfilesize);//更新进度条
            }

            string uri;
            if (remoteFilepath.Length == 0)
            {
                uri = "ftp://" + FtpServerIp + "/" + newFileName;
            }
            else
            {
                uri = "ftp://" + FtpServerIp + "/" + remoteFilepath + "/" + newFileName;
            }
            FtpWebRequest reqFtp;
            // 根据uri创建FtpWebRequest对象
            reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            // ftp用户名和密码
            reqFtp.Credentials = new NetworkCredential(FtpUserId, FtpPassword);
            // 默认为true，连接不会被关闭
            // 在一个命令之后被执行
            reqFtp.KeepAlive = false;
            // 指定执行什么命令
            reqFtp.Method = WebRequestMethods.Ftp.AppendFile;
            // 指定数据传输类型
            reqFtp.UseBinary = true;
            // 上传文件时通知服务器文件的大小
            reqFtp.ContentLength = fileInf.Length;
            int buffLength = 2048;// 缓冲大小设置为2kb
            byte[] buff = new byte[buffLength];
            // 打开一个文件流 (System.IO.FileStream) 去读上传的文件
            FileStream fs = fileInf.OpenRead();
            Stream strm = null;
            try
            {
                // 把上传的文件写入流
                strm = reqFtp.GetRequestStream();
                // 每次读文件流的2kb
                fs.Seek(startfilesize, 0);
                int contentLen = fs.Read(buff, 0, buffLength);
                // 流内容没有结束
                while (contentLen != 0)
                {
                    // 把内容从file stream 写入 upload stream
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                    startbye += contentLen;
                    //更新进度
                    if (updateProgress != null)
                    {
                        updateProgress((int)allbye, (int)startbye);//更新进度条
                    }
                }
                // 关闭两个流
                strm.Close();
                fs.Close();
            }
            catch
            {
                success = false;
                throw;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
                if (strm != null)
                {
                    strm.Close();
                }
            }
            return success;
        }

        /// <summary>
        /// 去除空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string RemoveSpaces(string str)
        {
            string res = "";
            CharEnumerator cEnumerator = str.GetEnumerator();
            while (cEnumerator.MoveNext())
            {
                byte[] array = new byte[1];
                array = System.Text.Encoding.ASCII.GetBytes(cEnumerator.Current.ToString());
                int asciicode = (short)(array[0]);
                if (asciicode != 32)
                {
                    res += cEnumerator.Current.ToString();
                }
            }
            return res.Split('.')[res.Split('.').Length - 2] + "." + res.Split('.')[res.Split('.').Length - 1];
        }

        /// <summary>
        /// 获取已上传文件大小
        /// </summary>
        /// <param name="filename">文件名称</param>
        /// <param name="path">服务器文件路径</param>
        /// <returns></returns>
        public static long GetFileSize(string filename, string path)
        {
            long filesize = 0;
            try
            {
                FileInfo fi = new FileInfo(filename);
                string uri;
                if (path.Length == 0)
                {
                    uri = "ftp://" + FtpServerIp + "/" + fi.Name;
                }
                else
                {
                    uri = "ftp://" + FtpServerIp + "/" + path + "/" + fi.Name;
                }
                FtpWebRequest reqFtp = (FtpWebRequest)FtpWebRequest.Create(uri);
                reqFtp.KeepAlive = false;
                reqFtp.UseBinary = true;
                reqFtp.Credentials = new NetworkCredential(FtpUserId, FtpPassword);
                reqFtp.Method = WebRequestMethods.Ftp.GetFileSize;
                FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();
                filesize = response.ContentLength;
                return filesize;
            }
            catch
            {
                return filesize;
            }
        }

        #endregion 上传文件到FTP服务器

        #region 获取文件列表
        public static Dictionary<string, int> GetFtpList(string path)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            if (path == null)
                path = "";
            try
            {
                Uri uri = new Uri("ftp://" + FtpServerIp + "/" + path);
                FtpWebRequest reqFtp = (FtpWebRequest)FtpWebRequest.Create(uri);
                reqFtp.KeepAlive = false;
                reqFtp.UseBinary = true;
                reqFtp.Credentials = new NetworkCredential(FtpUserId, FtpPassword);

                reqFtp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                WebResponse response = reqFtp.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (line != "." && line != "..")
                    {
                        int end = line.LastIndexOf(' ');
                        string filename = line.Substring(end + 1);

                        if (line.Substring(0, 1) == "d")
                        {
                            dic.Add(filename.Trim(), 1);
                        }
                        else
                        {
                            dic.Add(filename.Trim(), 2);
                        }
                    }
                    line = reader.ReadLine();
                }
            }
            catch (Exception e)
            {

            }

            return dic;
        }
        #endregion
    }
}