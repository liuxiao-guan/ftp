
namespace ftp
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txb_host = new System.Windows.Forms.TextBox();
            this.txb_username = new System.Windows.Forms.TextBox();
            this.txb_password = new System.Windows.Forms.TextBox();
            this.info = new System.Windows.Forms.Label();
            this.btn_conn = new System.Windows.Forms.Button();
            this.btn_ = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(58, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "ftp地址";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(61, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 18);
            this.label2.TabIndex = 1;
            this.label2.Text = "用户名";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(64, 206);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 18);
            this.label3.TabIndex = 2;
            this.label3.Text = "密码";
            // 
            // txb_host
            // 
            this.txb_host.Location = new System.Drawing.Point(222, 61);
            this.txb_host.Name = "txb_host";
            this.txb_host.Size = new System.Drawing.Size(385, 28);
            this.txb_host.TabIndex = 3;
            // 
            // txb_username
            // 
            this.txb_username.Location = new System.Drawing.Point(222, 129);
            this.txb_username.Name = "txb_username";
            this.txb_username.Size = new System.Drawing.Size(385, 28);
            this.txb_username.TabIndex = 4;
            // 
            // txb_password
            // 
            this.txb_password.Location = new System.Drawing.Point(222, 195);
            this.txb_password.Name = "txb_password";
            this.txb_password.Size = new System.Drawing.Size(385, 28);
            this.txb_password.TabIndex = 5;
            // 
            // info
            // 
            this.info.Location = new System.Drawing.Point(58, 275);
            this.info.Name = "info";
            this.info.Size = new System.Drawing.Size(546, 51);
            this.info.TabIndex = 6;
            // 
            // btn_conn
            // 
            this.btn_conn.Location = new System.Drawing.Point(156, 393);
            this.btn_conn.Name = "btn_conn";
            this.btn_conn.Size = new System.Drawing.Size(80, 31);
            this.btn_conn.TabIndex = 7;
            this.btn_conn.Text = "连接";
            this.btn_conn.UseVisualStyleBackColor = true;
            this.btn_conn.Click += new System.EventHandler(this.btn_conn_Click);
            // 
            // btn_
            // 
            this.btn_.Location = new System.Drawing.Point(388, 393);
            this.btn_.Name = "btn_";
            this.btn_.Size = new System.Drawing.Size(80, 31);
            this.btn_.TabIndex = 8;
            this.btn_.Text = "断开";
            this.btn_.UseVisualStyleBackColor = true;
            this.btn_.Click += new System.EventHandler(this.btn__Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(702, 493);
            this.Controls.Add(this.btn_);
            this.Controls.Add(this.btn_conn);
            this.Controls.Add(this.info);
            this.Controls.Add(this.txb_password);
            this.Controls.Add(this.txb_username);
            this.Controls.Add(this.txb_host);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txb_host;
        private System.Windows.Forms.TextBox txb_username;
        private System.Windows.Forms.TextBox txb_password;
        private System.Windows.Forms.Label info;
        private System.Windows.Forms.Button btn_conn;
        private System.Windows.Forms.Button btn_;
    }
}

