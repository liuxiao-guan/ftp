using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Windows;
namespace ftp
{
    public partial class MainForm : Form
    {
        private ClientConnect clientConnect;

        string ClientPath;
        string SerPath;
        public MainForm()
        {
            InitializeComponent();

            DateTime dateTime = DateTime.Now;
            ClientPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            SerPath = "/";
             DirectoryInfo root = new DirectoryInfo(ClientPath);
            DirectoryInfo[] directory = root.GetDirectories();
            FileInfo[] files = root.GetFiles();
            label14.Text = ClientPath;
            //label14.Text = TimeZone.CurrentTimeZone.ToLocalTime(dateTime).ToString();
            ShowCliFile(ClientPath);


        }
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (txb_host.Text == "" || txb_username.Text == "" || txb_password.Text == "")
            {
                MessageBox.Show("请输入地址、用户名、密码");
                return;
            }

            IPEndPoint IPEPoint = new IPEndPoint(IPAddress.Parse(txb_host.Text), 21);

            clientConnect = new ClientConnect(IPEPoint, txb_username.Text, txb_password.Text);

            try
            {
                clientConnect.Connect();//尝试连接服务器
            }
            catch (FTPException exception)
            {
                info.Text = exception.ToString();
            }

            if (clientConnect.Connected) { 
                info.Text = "连接成功";
                //string SerPath = "/";
                ShowSerFile(SerPath, SerPath);


            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            clientConnect.Disconnect();
            info.Text = "关闭连接";
        }
        /// <summary>
        /// 上传按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label12_Click(object sender, EventArgs e)
        {
            string filename = NavigationList.SelectedItems[0].Text.Trim();
            //string path = Server.MapPath("/test.txt");
            string PrimPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string path = ClientPath + '\\' + filename;
            if(Directory.Exists(path))
            {
                MessageBox.Show("请选择文件而不是文件夹！", "提醒", MessageBoxButtons.OK);
                return;
            }
                
            //filename = path.Replace(PrimPath, string.Empty);
            FileStream fs = new FileStream(path, FileMode.Open);
            //获取文件大小
            long size = fs.Length;
            byte[] array = new byte[size];

            //将文件读到byte数组中
            fs.Read(array, 0, array.Length);
            clientConnect.Upload(ClientPath+ "\\"+filename,SerPath+ "\\" + filename, array, 0);
            ShowSerFile(SerPath, SerPath);
            fs.Close();

            
        }
        /// <summary>
        /// 下载按钮 下载到客户端所在的当前目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label11_Click(object sender, EventArgs e)
        {
            string filename = ServerList.SelectedItems[0].Text.Trim();
            //下载到客户端的位置对应的文件，名字没有变
            string localPath = ClientPath + "\\" + filename;
           
            clientConnect.Download(localPath, filename, 0);
            ShowCliFile(ClientPath);

        }
        /// <summary>
        /// 客户端返回上一层目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label12_Click_1(object sender, EventArgs e)
        {
            ClientPath = ClientPath + "\\..";
            ShowCliFile(ClientPath);
        }
        /// <summary>
        /// 服务器端返回上一层目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label11_Click_1(object sender, EventArgs e)
        {
            SerPath = SerPath + "\\..";
            ShowSerFile(SerPath,SerPath);
        }
        /// <summary>
        /// 删除客户端文件夹或文件 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label4_Click(object sender, EventArgs e)
        {
            //string remotePath = "//"+ NavigationList.SelectedItems[0].Text.Trim();
            //clientConnect.DeleteDirectory(remotePath);
            string filename = NavigationList.SelectedItems[0].Text.Trim();

            if (filename == null)
                return;

            int lastpath = filename.LastIndexOf(".");
            string localPath = ClientPath + "\\" + filename;

            if (lastpath == -1)//文件夹
            {
                DirectoryInfo MyDir = new DirectoryInfo(localPath);
                MyDir.Delete(true);
                MessageBox.Show("已删除文件夹");
            }
            else//文件
            {
                FileInfo fi = new FileInfo(localPath);
                fi.Delete();
                MessageBox.Show("已删除文件");
            }
            ShowCliFile(ClientPath);
        }

        private void NavigationList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ClientPath = ClientPath +"\\"+ NavigationList.SelectedItems[0].Text.Trim();
            ShowCliFile(ClientPath);

        }
        /// <summary>
        /// 显示客户端文件夹
        /// </summary>
        /// <param name="ClientPath"></param>
        public void ShowCliFile(string ClientPath)
        {
            if (!Directory.Exists(ClientPath))
            {
                return;
            }
            NavigationList.Items.Clear();
            string[] MyAllFile = Directory.GetFileSystemEntries(ClientPath);

            foreach (string file in MyAllFile)

            {
                string DateTime = File.GetLastWriteTime(file.ToString()).ToString();

                if (Directory.Exists(file))
                {
                    //Directory.GetFileSystemEntries(file)
                    string[] ShowFolders = { file.Substring(file.LastIndexOf("\\") + 1), DateTime, "<DIR>" };
                    ListViewItem MyItem1 = new ListViewItem(ShowFolders);
                    NavigationList.Items.Add(MyItem1);

                }
                else
                {

                    string FileSize = (file.Length / 1024.0).ToString("f2") + "KB";
                    string[] ShowFolders = { file.Substring(file.LastIndexOf("\\") + 1), DateTime, FileSize };
                    ListViewItem MyItem1 = new ListViewItem(ShowFolders);
                    NavigationList.Items.Add(MyItem1);

                }


            }
        }
        /// <summary>
        /// 列出服务器的文件列表
        /// </summary>
        /// <param name="SerPath"></param>
        /// <param name="filename"></param>
        public void ShowSerFile(string SerPath,string filename)
        {
            ServerList.Items.Clear();
            if (String.IsNullOrEmpty(SerPath)) SerPath = "/";
            List<List<string>> ret = new List<List<string>>();
            ret = clientConnect.List(filename,SerPath);
            //fileinfo[4] == "0" 时这是个文件夹而不是文件
            foreach (List<string> fileinfo in ret)
            {
                if (fileinfo[4] == "0")
                {
                    fileinfo[4] = "<DIR>";
                }
                else
                {
                    if (fileinfo[4] != null)
                    {
                        int a = int.Parse(fileinfo[4]);
                        fileinfo[4] = (a / 1024.0).ToString("f2") + "KB";
                    }

                }
                string[] ShowFolders = { fileinfo[8], (fileinfo[7] + ' ' + fileinfo[5] + ' ' + fileinfo[6]).ToString(), fileinfo[4] };
                ListViewItem MyItem1 = new ListViewItem(ShowFolders);
                MyItem1.Tag = 0;
                if (fileinfo[4] == "<DIR>")
                {
                    MyItem1.Tag = 1;
                }
                ServerList.Items.Add(MyItem1);
            }
        }
        /// <summary>
        /// 进入服务器端文件夹的子目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string filename = ServerList.SelectedItems[0].Text.Trim();

            SerPath = (SerPath + "\\" + filename);
            SerPath = SerPath.Replace("/",string.Empty);
            ShowSerFile(SerPath,filename);
        }
        /// <summary>
        /// 客户端重命名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label5_Click(object sender, EventArgs e)
        {
           if (NavigationList.SelectedItems.Count == 0){
                MessageBox.Show("请选择要重命名的文件夹", "提醒", MessageBoxButtons.OK);
                return;
            }
            string filename = this.NavigationList.SelectedItems[0].Text.Trim();
            int lastpath = filename.LastIndexOf("\\");
            int lastdot = filename.LastIndexOf(".");
            int length = lastdot - lastpath - 1;

            ReFile refile = new ReFile(filename, false);
            refile.ShowDialog();
            string localPath = ClientPath + "\\" + filename;

            bool isExist = false;
            string[] MyAllFile = Directory.GetFileSystemEntries(ClientPath);
            if (!refile.isESC)
            {
                if (lastdot == -1)
                {
                    string fullnewname = ClientPath + "\\" + refile.newFileName;
                    foreach (string file in MyAllFile)
                    {
                        if (file == fullnewname)
                        {
                            isExist = true;
                            break;
                        }
                    }
                    if (!isExist)
                    {
                        Directory.Move(localPath, fullnewname);
                        MessageBox.Show("重命名文件夹成功");
                        ShowCliFile(ClientPath);
                    }
                    else
                    {
                        MessageBox.Show("文件夹已存在");
                    }
                }
                else
                {
                    string ext = filename.Substring(lastdot);
                    string fullnewname = ClientPath + "\\" + refile.newFileName + ext;
                    foreach (string file in MyAllFile)
                    {
                        if (file == fullnewname)
                        {
                            isExist = true;
                            break;
                        }
                    }
                    if (!isExist)
                    {
                        System.IO.File.Move(localPath, fullnewname);
                        MessageBox.Show("重命名文件成功");
                        ShowCliFile(ClientPath);
                    }
                    else
                    {
                        MessageBox.Show("文件已存在");
                    }
                }
            }
        }
        /// <summary>
        /// 客户端建立新文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label6_Click(object sender, EventArgs e)
        {
            ReFile addfile = new ReFile("", true);
            addfile.ShowDialog();
            string localPath = ClientPath + "\\" + addfile.newFileName;

            bool isExist = false;
            string[] MyAllFile = Directory.GetFileSystemEntries(ClientPath);
            string fullnewname = ClientPath + "\\" + addfile.newFileName;
            if (!addfile.isESC)
            {
                foreach (string file in MyAllFile)
                {
                    if (file == localPath)
                    {
                        isExist = true;
                        break;
                    }
                }
            }
            if (!isExist && !addfile.isESC)
            {
                    Directory.CreateDirectory(localPath);
                    MessageBox.Show("新建文件夹成功");
                    ShowCliFile(ClientPath);
             }
            else if (!isExist && !addfile.isESC)
            {
                    MessageBox.Show("文件夹已存在");
             }
            
        }
        /// <summary>
        /// 服务器端删除空文档以及文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label8_Click(object sender, EventArgs e)
        {
            int count = ServerList.SelectedItems.Count;
            for (int i = 0;i  < count; i++)
            {
                string filename = ServerList.SelectedItems[i].Text.Trim();
                if (int.Parse(ServerList.SelectedItems[i].Tag.ToString()) == 1)
                {
                    clientConnect.DeleteDirectory(SerPath + "\\" + filename);
                }
                else
                {
                    clientConnect.Delete(SerPath + "\\" + filename);
                }
            }
            
            ShowSerFile(SerPath,SerPath);
        }

        /// <summary>
        /// 服务器端建立新文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label10_Click(object sender, EventArgs e)
        {
            ReFile addfile = new ReFile("", true);
            addfile.ShowDialog();
            string remotePath = SerPath + "\\" + addfile.newFileName;
            bool isExist = false;
            string[] MyAllFile = Directory.GetFileSystemEntries(SerPath);
            if (!addfile.isESC)
            {
                foreach (string file in MyAllFile)
                {
                    if (file == remotePath)
                    {
                        isExist = true;
                        break;
                    }
                }
            }
            if (!isExist && !addfile.isESC)
            {
                clientConnect.CreateDirectory(remotePath);
                MessageBox.Show("新建文件夹成功");
                ShowSerFile(SerPath, SerPath);    
                
            }
            if (!isExist && !addfile.isESC)
            {
                MessageBox.Show("文件夹已存在");
            }
            
        }
        /// <summary>
        /// 服务器端重命名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label9_Click(object sender, EventArgs e)
        {
            if (ServerList.SelectedItems.Count == 0)
            {
                MessageBox.Show("请选择要重命名的文件夹", "提醒", MessageBoxButtons.OK);
                return;
            }
            string filename = this.ServerList.SelectedItems[0].Text.Trim();
            ReFile refile = new ReFile(filename, false);
            refile.ShowDialog();
            string oldPath = SerPath + "\\" + filename;
            string newPath = SerPath + "\\" + refile.newFileName;
            
            int lastpath = filename.LastIndexOf("\\");
            int lastdot = filename.LastIndexOf(".");
            int length = lastdot - lastpath - 1; 

            bool isExist = false;
            string[] MyAllFile = Directory.GetFileSystemEntries(SerPath);
            if (!refile.isESC)
            {
                if (lastdot == -1)
                {
                    string fullnewname = SerPath + "\\" + refile.newFileName;
                    foreach (string file in MyAllFile)
                    {
                        if (file == fullnewname)
                        {
                            isExist = true;
                            break;
                        }
                    }
                    if (!isExist)
                    {
                        clientConnect.ReNameDirectory(oldPath, newPath);
                        MessageBox.Show("重命名文件夹成功");
                        ShowSerFile(SerPath, SerPath);
                    }
                    else
                    {
                        MessageBox.Show("文件夹已存在");
                    }
                }
                else
                {
                    string ext = filename.Substring(lastdot);
                    string fullnewname = ClientPath + "\\" + refile.newFileName + ext;
                    foreach (string file in MyAllFile)
                    {
                        if (file == fullnewname)
                        {
                            isExist = true;
                            break;
                        }
                    }
                    if (!isExist)
                    {
                        clientConnect.ReNameDirectory(oldPath, newPath);
                        MessageBox.Show("重命名文件成功");
                        ShowSerFile(SerPath, SerPath);
                    }
                    else
                    {
                        MessageBox.Show("文件已存在");
                    }
                }

            }
        }

    }
}
