///此窗口的的代码功能已被转移到MainForm中
///

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

namespace ftp
{
    public partial class Form1 : Form
    {
        private ClientConnect clientConnect;

        public Form1()
        {
            InitializeComponent();

        }

        private void btn_conn_Click(object sender, EventArgs e)
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
                //info.Text = exception.ToString();
                info.Text = exception.Message;
            }
            
            if (clientConnect.Connected) { info.Text = "连接成功"; }
        }

        private void btn__Click(object sender, EventArgs e)
        {
            clientConnect.Disconnect();
            info.Text = "关闭连接";
        }

        public void Connect()
        {
            try
            {

            }
            catch(FTPException exception)
            {
                info.Text = exception.ToString();
            }
        }
    }
}
