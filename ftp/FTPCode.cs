using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ftp
{
    public class FTPCode
    {
        private Socket socket;

        public FTPCode(Socket s)
        {
            this.socket = s;
        }

        public byte[] Readln(out int status)
        {
            byte[] buffer = new byte[65536];//定义字节数组

            int index = 0;
            byte[] read = new byte[1];//定义数组只有2位
            do
            {
                if (socket.Receive(read) == 0) break;

                if (read[0] != '\r' && read[0] != '\n')//接收的数据非特殊符号
                {
                    buffer[index] = read[0];//依次复制到buffer
                    index++;
                }
            } while (read[0] != '\n');//遇到换行符号结束

            byte[] ret = new byte[index];

            status = 0;
            bool flag = true;
            for (int i = 0; i < index; i++)
            {
                ret[i] = buffer[i];//从buffer复制到ret

                if (ret[i] < '0' || ret[i] > '9')
                {
                    flag = false;
                }
                else if (flag)
                {
                    status = status * 10 + ret[i] - 48;//ret[i]的值<0或>9时，修改status(输出参数)
                }
            }

            return ret;
        }

        public byte[] Readln()
        {
            int status;
            return Readln(out status);
        }

        public void WriteIn(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            socket.Send(bytes);
            socket.Send(Encoding.ASCII.GetBytes("\r\n"));
        }
    }
}
