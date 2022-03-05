using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ftp
{
    class Authorization//授权 用户登录后的身份/角色识别
    {
        private readonly Socket socket;

        FTPCode ftpcode;

        public string Username { get; }//用户名

        public string Password { get; }//密码

        public Authorization(string username, string password, Socket socket)//构造函数
        {
            this.Username = username;
            this.Password = password;

            this.socket = socket;
            this.ftpcode = new FTPCode(socket);
        }


        public void Login(out bool isWindows)//登录
        {
            int status;
            string line;

            ftpcode.WriteIn("USER " + Username);
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
            if (status != 331) throw new FTPException(status, line);//报错  331 Please specify the password. 用户名正确，需要口令

            ftpcode.WriteIn("PASS " + Password);
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
            if (status != 230) throw new FTPException(status, line);//ftp应答码230： 用户登录

            ftpcode.WriteIn("SYST");//SYST命令用来获取服务器的操作系统
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
            if (status != 215) throw new FTPException(status, line);//215： 名字系统类型
            isWindows = CheckSystemSupport(line);//检测os

            ftpcode.WriteIn("OPTS UTF8 ON");
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
            // if (status / 100 != 2) throw new FTPClientException(status, line);//200： 命令成功

            ftpcode.WriteIn("PWD");
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
            if (status != 257) throw new FTPException(status, line);//227： 进入被动模式

            ftpcode.WriteIn("TYPE I");
            line = System.Text.Encoding.UTF8.GetString(ftpcode.Readln(out status));
            if (status / 100 != 2) throw new FTPException(status, line);//200： 命令成功
        }

        public bool CheckSystemSupport(string line)//确认系统是否支持
        {
            string[] t = line.Split(' ');//遇到空格分割
            if (t.Length >= 2)
            {
                string s = t[1].ToLower();

                if (s.Contains("windows"))//题目指定客户端
                {
                    return true;
                }

            }

            return false;
            //throw new Exception("Server only supported windows.\r\n" );
        }
    }
}
