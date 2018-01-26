using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace OFM.FTP
{
    internal class FtpUpDown
    {
        private string ftpServerIp;

        private string ftpUserId;

        private string ftpPassword;

        private FtpWebRequest reqFtp;

        /// <summary>
        /// 连接ftp
        /// </summary>
        /// <param name="uri">FTP连接地址</param>
        private void Connect(String uri)
        {
            // 根据uri创建FtpWebRequest对象
            reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            // 指定数据传输类型
            reqFtp.UseBinary = true;
            // ftp用户名和密码

            reqFtp.Credentials = new NetworkCredential(ftpUserId, ftpPassword);
        }

        public FtpUpDown(string ftpServerIP, string ftpUserID, string ftpPassword)
        {
            this.ftpServerIp = ftpServerIP;

            this.ftpUserId = ftpUserID;

            this.ftpPassword = ftpPassword;
        }

        //都调用这个

        private string[] GetFileList(string path, string WRMethods)//上面的代码示例了如何从ftp服务器上获得文件列表
        {
            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            try
            {
                Connect(path);

                reqFtp.Method = WRMethods;

                WebResponse response = reqFtp.GetResponse();

                StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);//中文文件名

                string line = reader.ReadLine();

                while (line != null)
                {
                    result.Append(line);

                    result.Append("\n");

                    line = reader.ReadLine();
                }

                // to remove the trailing '\n'

                result.Remove(result.ToString().LastIndexOf('\n'), 1);

                reader.Close();

                response.Close();

                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);

                downloadFiles = null;

                return downloadFiles;
            }
        }

        public string[] GetFileList(string path)//上面的代码示例了如何从ftp服务器上获得文件列表
        {
            return GetFileList("ftp://" + ftpServerIp + "/" + path, WebRequestMethods.Ftp.ListDirectory);
        }

        public string[] GetFileList()//上面的代码示例了如何从ftp服务器上获得文件列表
        {
            return GetFileList("ftp://" + ftpServerIp + "/", WebRequestMethods.Ftp.ListDirectory);
        }

        public void Upload(string filename) //上面的代码实现了从ftp服务器上载文件的功能
        {
            FileInfo fileInf = new FileInfo(filename);

            string uri = "ftp://" + ftpServerIp + "/" + fileInf.Name;

            Connect(uri);//连接

            // 默认为true，连接不会被关闭

            // 在一个命令之后被执行

            reqFtp.KeepAlive = false;

            // 指定执行什么命令

            reqFtp.Method = WebRequestMethods.Ftp.UploadFile;

            // 上传文件时通知服务器文件的大小

            reqFtp.ContentLength = fileInf.Length;
            // 缓冲大小设置为kb
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];

            int contentLen;

            // 打开一个文件流(System.IO.FileStream) 去读上传的文件

            FileStream fs = fileInf.OpenRead();

            try
            {
                // 把上传的文件写入流

                Stream strm = reqFtp.GetRequestStream();

                // 每次读文件流的kb

                contentLen = fs.Read(buff, 0, buffLength);

                // 流内容没有结束

                while (contentLen != 0)
                {
                    // 把内容从file stream 写入upload stream
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }

                // 关闭两个流

                strm.Close();

                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Upload Error");
            }
        }

        public bool Download(string filePath, string fileName, out string errorinfo)////上面的代码实现了从ftp服务器下载文件的功能
        {
            try
            {
                String onlyFileName = Path.GetFileName(fileName);

                string newFileName = filePath + "\\" + onlyFileName;

                if (File.Exists(newFileName))
                {
                    errorinfo = string.Format("本地文件{0}已存在,无法下载", newFileName);
                    return false;
                }
                string url = "ftp://" + ftpServerIp + "/" + fileName;
                Connect(url);//连接
                reqFtp.Credentials = new NetworkCredential(ftpUserId, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);

                FileStream outputStream = new FileStream(newFileName, FileMode.Create);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();

                errorinfo = "";

                return true;
            }
            catch (Exception ex)
            {
                errorinfo = string.Format("因{0},无法下载", ex.Message);

                return false;
            }
        }

        //删除文件
        public void DeleteFileName(string fileName)
        {
            try
            {
                FileInfo fileInf = new FileInfo(fileName);

                string uri = "ftp://" + ftpServerIp + "/" + fileInf.Name;

                Connect(uri);//连接

                // 默认为true，连接不会被关闭

                // 在一个命令之后被执行

                reqFtp.KeepAlive = false;

                // 指定执行什么命令

                reqFtp.Method = WebRequestMethods.Ftp.DeleteFile;

                FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "删除错误");
            }
        }

        //创建目录
        public void MakeDir(string dirName)
        {
            try
            {
                string uri = "ftp://" + ftpServerIp + "/" + dirName;

                Connect(uri);//连接

                reqFtp.Method = WebRequestMethods.Ftp.MakeDirectory;

                FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();

                response.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //删除目录
        public void delDir(string dirName)
        {
            try
            {
                string uri = "ftp://" + ftpServerIp + "/" + dirName;

                Connect(uri);//连接

                reqFtp.Method = WebRequestMethods.Ftp.RemoveDirectory;

                FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();

                response.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //获得文件大小
        public long GetFileSize(string filename)
        {
            long fileSize = 0;

            try
            {
                FileInfo fileInf = new FileInfo(filename);

                string uri = "ftp://" + ftpServerIp + "/" + fileInf.Name;

                Connect(uri);//连接

                reqFtp.Method = WebRequestMethods.Ftp.GetFileSize;

                FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();

                fileSize = response.ContentLength;

                response.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return fileSize;
        }

        //文件改名
        public void Rename(string currentFilename, string newFilename)
        {
            try
            {
                FileInfo fileInf = new FileInfo(currentFilename);

                string uri = "ftp://" + ftpServerIp + "/" + fileInf.Name;

                Connect(uri);//连接

                reqFtp.Method = WebRequestMethods.Ftp.Rename;

                reqFtp.RenameTo = newFilename;

                FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();

                //Stream ftpStream = response.GetResponseStream();

                //ftpStream.Close();

                response.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //获得文件明晰
        public string[] GetFilesDetailList()
        {
            return GetFileList("ftp://" + ftpServerIp + "/", WebRequestMethods.Ftp.ListDirectoryDetails);
        }

        //获得文件明晰
        public string[] GetFilesDetailList(string path)
        {
            return GetFileList("ftp://" + ftpServerIp + "/" + path, WebRequestMethods.Ftp.ListDirectoryDetails);
        }
    }
}
