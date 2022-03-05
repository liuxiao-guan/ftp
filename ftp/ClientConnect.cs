using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace ftp
{
    class ClientConnect
    {
        private readonly Socket socket;//包含一个本地及远程的终结点

        private readonly FTPCode ftpcode;//计算应答码与对应信息

        private readonly Authorization authorization; //授权 用户登录后的身份/角色识别

        private readonly IPEndPoint serverIPE;//程序连主机服务的所需的主机和端口信息

        private bool isWindows;

        public bool Connected { get; private set; } = false;//默认未连接
        public bool Working { get; private set; } = true;


        public ClientConnect(IPEndPoint server, string username, string password)//构造函数(服务器，用户名，密码)
        {
            this.serverIPE = server;

            socket = new Socket(server.AddressFamily, SocketType.Stream, ProtocolType.Tcp);//Socket类自带构造函数

            ftpcode = new FTPCode(socket);

            this.authorization = new Authorization(username, password, socket);
        }

        /// <summary>
        /// 尝试连接服务器
        /// </summary>
        public void Connect()
        {
            socket.Connect(serverIPE);//Socket自带Connect,只能应用于客户端

            int status;
            String line;
            do
            {
                line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));//把int应答码转为string
                if (line.Substring(0, 3) != "220")//220： 新用户服务准备好了
                    throw new FTPException(status, line);
            } while (line.Substring(0, 4) == "220-");

            authorization.Login(out this.isWindows);//授权

            this.Connected = true;//连接成功
        }

        /// <summary>
        /// 选择模式并创建数据连接
        /// </summary>
        private void InitDataConnection(out Socket dataSocket, out FTPCode dataHelper)//两个输出参数
        {
            string line;
            int status;

            // 被动模式PASV对应227;
            ftpcode.WriteIn("PASV");
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
            //if (status / 100 == 5) // 不支持 EPSV, 使用 PASV被动模式
            ///{

                if (status == 227)
                {
                    string dataConnection = line;
                    // 创建数据连接
                    dataSocket = Address.AddressParserAndConnect(0, dataConnection);
                    dataHelper = new FTPCode(dataSocket);
                }
                else
                {
                    throw new FTPException(status, line);
                }
           
        }
        public void ReNameDirectory(string oldPath,string newPath)
        {
            string line;
            int status;

            // 建立目录
            string[] folders = oldPath.Split('/');
            bool flag = false;
            // 切换工作区
            for (int i = 0; i < folders.Length - 1; i++)
            {
                if (folders[i].Length == 0) continue;

                // CWD 切换工作区 => 250
                ftpcode.WriteIn("CWD " + folders[i]);
                line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));

                // 路径不存在
                if (status != 250) throw new FTPException(status, line);
            }
            //  指定重命名的路径=> 250
            ftpcode.WriteIn("RNFR " + oldPath);
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
            //if (status != 250) throw new FTPException(status, line);
            ftpcode.WriteIn("RNTO " + newPath);
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
            if (status != 250) throw new FTPException(status, line);

        }

        public void CreateDirectory(string remotePath)
        {
            string line;
            int status;

            // 建立目录
            string[] folders = remotePath.Split('/');
            bool flag = false;

            for (int i = 0; i < folders.Length; i++)
            {
                if (folders[i].Length == 0) continue;

                if (flag)
                {
                    // MKD 创建目录 => 257
                    ftpcode.WriteIn("MKD " + folders[i]);
                    line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
                    if (status != 257) throw new FTPException(status, line);
                }

                // CWD 切换工作区 => 250
                ftpcode.WriteIn("CWD " + folders[i]);
                line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));

                // 路径不存在
                if (status != 250)
                {
                    flag = true;
                    i--;
                }
            }

            // CWD 切换工作区至根目录
            ftpcode.WriteIn("CWD /");
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
        }
        /// <summary>
        /// 删除文件夹 目前仅可以删除空文件夹
        /// </summary>
        /// <param name="remotePath"></param>
        public bool DeleteDirectory(string remotePath)
        {
            int status;
            string line;
            bool Delete = true;
            string[] folders = remotePath.Split('/');

            // 切换工作区
            for (int i = 0; i < folders.Length - 1; i++)
            {
                if (folders[i].Length == 0) continue;

                // CWD 切换工作区 => 250
                ftpcode.WriteIn("CWD " + folders[i]);
                line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));

                // 路径不存在
                if (status != 250) throw new FTPException(status, line);
            }

            // 删除目录 => 250
            ftpcode.WriteIn("RMD " + folders[folders.Length - 1]);
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
            if (status != 250)
            {
                //throw new FTPException(status, line);
                FTPException ex = new FTPException(status, line);
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Delete = false;
                return Delete;
            }


            // CWD 切换工作区至根目录
            ftpcode.WriteIn("CWD /");
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
            MessageBox.Show("删除成功", "提醒", MessageBoxButtons.OK);
            return Delete;
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        public void Delete(string path)
        {
            int status;
            string line;

            ftpcode.WriteIn("DELE " + path);
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
            if (status != 250) throw new FTPException(status, line);
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="remotePath"></param>
        /// <param name="fileContentsBytes"></param>
        /// <param name="offset"></param>
        
        public void Upload(string localPath, string remotePath, byte[] fileContentsBytes, long offset = 0)
        {
            string line;
            int status;
            Socket dataSocket;
            FTPCode dataHelper;

            InitDataConnection(out dataSocket, out dataHelper);

            // 处理路径
            string[] folders = remotePath.Split('/');
            string filename = folders[folders.Length - 1];
            bool flag = false;

            for (int i = 0; i < folders.Length - 1; i++)
            {
                if (folders[i].Length == 0) continue;

                if (flag)
                {
                    // MKD 创建目录 => 257
                    ftpcode.WriteIn("MKD " + folders[i]);
                    line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
                    if (status != 257) throw new FTPException(status, line);
                }

                // CWD 切换工作区 => 250
                ftpcode.WriteIn("CWD " + folders[i]);
                line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));

                // 路径不存在
                if (status != 250)
                {
                    flag = true;
                    i--;
                }
            }

            // REST 续传 => 350
            ftpcode.WriteIn("REST " + offset);
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
            if (status / 100 == 5) offset = 0;
            if (status != 350 && status / 100 != 5) throw new FTPException(status, line);

            // STOR 路径 => 150
            ftpcode.WriteIn("STOR " + filename);
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));

            // 分段读取文件字节
            long buffsize = 1048576;
            byte[] buff = new byte[buffsize];
            long start = offset;
           
            try
            {
                // 上传
                int datasize = 1024;
                byte[] data = new byte[datasize];
                for (int i = 0; i < fileContentsBytes.Length; i++)
                {
                    int j = i;
                    if (fileContentsBytes.Length - i < datasize)
                    {
                        byte[] tmp = new byte[fileContentsBytes.Length - i];
                        while (j < fileContentsBytes.Length)
                        {
                            tmp[j - i] = fileContentsBytes[j];
                            j++;
                        }

                        dataSocket.Send(tmp);
                        break;
                    }

                    while (j < i + datasize)
                    {
                        data[j - i] = fileContentsBytes[j];
                        j++;
                    }

                    dataSocket.Send(data);
                    i = j - 1;
                }

                // 关闭socket连接
                dataSocket.Close();

                // 226 or 250
                line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
                if ((status != 226) && (status != 250)) throw new FTPException(status, line);
                else
                {
                    MessageBox.Show("上传成功", "提醒", MessageBoxButtons.OK);
                }
                //Thread.Sleep(200);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw ex;
                
            }
           
            
        }
        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="remotePath"></param>
        /// <param name="offset"></param>
        public void Download(string localPath, string remotePath, long offset = 0)
        {
            int status;
            string line;
            Socket dataSocket;
            FTPCode dataHelper;

            InitDataConnection(out dataSocket, out dataHelper);

            // 尝试下载文件
            // SIZE 路径 => 213 文件大小;
            ftpcode.WriteIn("SIZE " + remotePath);
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
            if (status != 213)
            {
                //throw new FTPException(status, line);
                FTPException exception = new FTPException(status, line);
                //MessageBox.Show(exception.Message, "提醒", MessageBoxButtons.OK);

                if (exception.Message == "550 File not found")
                {
                    MessageBox.Show("请选择文件而不是文件夹！", "提醒", MessageBoxButtons.OK);
                    return;
                }
            }

            int fileSize = Int32.Parse(line.Split(' ')[1]);

            // REST 续传 => 350
            ftpcode.WriteIn("REST " + offset);
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
            if (status / 100 == 5) offset = 0;
            if (status != 350 && status / 100 != 5) throw new FTPException(status, line);

            // RETR 路径 => 150;
            ftpcode.WriteIn("RETR " + remotePath);
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
            if (status != 150 && status != 125) throw new FTPException(status, line);

            // 获取本地目录
            string localDirectory = Directory.GetParent(localPath).FullName;
            // 判断目录是否存在
            if (!Directory.Exists(localDirectory))
            {
                // 创建目录
                Directory.CreateDirectory(localDirectory);
            }
            

            // 接收数据
            long start = offset;
            try
            {
                FileStream fs;
                if (offset == 0) fs = new FileStream(localPath, FileMode.Create, FileAccess.Write);
                else fs = new FileStream(localPath, FileMode.Append, FileAccess.Write);

                int buffsize = 1048576;
                byte[] buff = new byte[buffsize];

                while (start < fileSize && this.Working)
                {
                    //if (ProcessUpdate != null) ProcessUpdate(start, fileSize);

                    // 接收数据
                    int length = dataSocket.Receive(buff);
                    fs.Write(buff, 0, length);
                    start += length;
                }

                fs.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            dataSocket.Close();

            // 226 => 结束数据连接
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
            if ((status != 226) && (status != 250) && (status != 550)) throw new FTPException(status, line);
            if (status == 550) throw new FTPException(status, line);
            else
            {
                MessageBox.Show("下载成功", "提醒", MessageBoxButtons.OK);
            }
           

        }


        /// <summary>
        /// 列出服务器端文件列表
        /// </summary>
        /// <param name="path"></param>
        /// <param name="SerPath"></param>
        /// <returns></returns>
        public List<List<string>> List(string path,string SerPath)
        { 
            int status;
            string line;
            Socket dataSocket;
            FTPCode dataHelper;

            InitDataConnection(out dataSocket, out dataHelper);

            // 执行指令 此时要相对路径
            ftpcode.WriteIn("LIST " + SerPath);
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
            if (status != 150 && status != 125) throw new FTPException(status, line);
            string[] MyAllFile = Directory.GetFileSystemEntries(SerPath);
            // 列出信息
            List<List<string>> ret = new List<List<string>>();
            List<ListViewItem> MyItem1 = new List<ListViewItem>();
            do
            {
                line = System.Text.Encoding.UTF8.GetString(dataHelper.Readln());
                List<string> fileinfo = new List<string>(GetSerTime(line));
                
                
                ret.Add(fileinfo);
              
                
            } while (line.Length != 0);

            // 226 => 结束数据连接
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));

            if ((status != 226) && (status != 250)) throw new FTPException(status, line);
            dataSocket.Close();

            return ret;
        }

        

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            this.socket.Disconnect(false);//Socket自带函数
        }

        public string[] GetSerTime(string line)
        {
             DateTime now = DateTime.Now;
            int nameIndex = 8;
            int columnCount = 9;
            string[] s = new string[columnCount];
            DateTime time;
            int i = 0;
            int flag = 1;
            int p = 0;
            for (p = 0; p < line.Length; p++)
                if (line[p] != ' ')
                    break;

            for (; p < line.Length; p++)
            {
                // 文件名部分无视空格判断
                if (i == nameIndex)
                {
                    s[i] += line[p];
                    continue;
                }

                if (flag == 0)
                {
                    if (line[p] == ' ') continue;

                    flag = 1;
                    i++;
                    s[i] = "" + line[p];
                    continue;
                }

                if (flag == 1)
                {
                    if (line[p] != ' ')
                    {
                        s[i] += line[p];
                        continue;
                    }

                    flag = 0;
                    continue;
                }
            }

            return s;
        }

    }
}
