using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ftp
{
    class Address
    {
        //IPE地址解析
        public static IPEndPoint AddressParser(string s)
        {
            var str = GetMiddleText(s, "(", ")");//取文本中间
            var ipe = str.Split(',');
            if (ipe.Length != 6)
                throw new NotImplementedException();
            var ip = IPAddress.Parse(string.Format("{0}.{1}.{2}.{3}", ipe[0], ipe[1], ipe[2], ipe[3]));
            var port = Convert.ToInt32(ipe[4]) * 256 + Convert.ToInt32(ipe[5]);
            return new IPEndPoint(ip, port);
        }


        private static string GetMiddleText(string t, string k, string j)
        {
            try
            {
                var kn = t.IndexOf(k, StringComparison.Ordinal) + k.Length;
                var jn = t.IndexOf(j, kn, StringComparison.Ordinal);
                return t.Substring(kn, jn - kn);
            }
            catch
            {
                return "";
            }
        }

        public static Socket AddressParserAndConnect(int mode, string line, IPAddress ip = null)
        {
            if (mode == 0)
            {
                IPEndPoint IPE = AddressParser(line);
                Socket dataSocket = new Socket(IPE.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                dataSocket.Connect(IPE);
                return dataSocket;
            }
            else if (mode == 1)
            {
                int port = Convert.ToInt32(GetMiddleText(line, "|||", "|"));
                IPEndPoint IPE = new IPEndPoint(ip, port);
                Socket dataSocket = new Socket(IPE.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                dataSocket.Connect(IPE);
                return dataSocket;
            }
            else
            {
                throw new Exception("Unknown mode.");
            }
        }

    }
}
