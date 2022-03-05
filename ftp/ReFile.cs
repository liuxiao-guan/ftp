using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ftp
{
    public partial class ReFile : Form
    {
        public string newFileName;
        public bool isESC = false;
        public ReFile(string oldName, bool isAdd)
        {
            InitializeComponent();
            newFileName = oldName;
            if (isAdd)
            {
                label1.Text = "新建文件夹名称";

            }
            else
            {
                label1.Text = "重命名";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("名称不能为空");
            }
            else
            {
                newFileName = textBox1.Text;
                this.Close();
            }

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            isESC = true;
            this.Close();
        }
    }
}
