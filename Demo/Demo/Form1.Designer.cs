namespace Demo
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.userControlcs2 = new Demo.UserControlcs();
            this.userControlcs1 = new Demo.UserControlcs();
            this.SuspendLayout();
            // 
            // userControlcs2
            // 
            this.userControlcs2.Location = new System.Drawing.Point(41, 12);
            this.userControlcs2.Name = "userControlcs2";
            this.userControlcs2.Size = new System.Drawing.Size(476, 38);
            this.userControlcs2.TabIndex = 1;
            this.userControlcs2.TextValue = "";
            // 
            // userControlcs1
            // 
            this.userControlcs1.Location = new System.Drawing.Point(77, 29);
            this.userControlcs1.Name = "userControlcs1";
            this.userControlcs1.Size = new System.Drawing.Size(239, 21);
            this.userControlcs1.TabIndex = 0;
            this.userControlcs1.TextValue = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 289);
            this.Controls.Add(this.userControlcs2);
            this.Controls.Add(this.userControlcs1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private UserControlcs userControlcs1;
        private UserControlcs userControlcs2;









    }
}

