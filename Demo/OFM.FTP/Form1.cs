using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace OFM.FTP
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FileUpDownload.FtpServerIp = "192.168.11.84:2121";
            FileUpDownload.FtpUserId = "ofm_dev";
            FileUpDownload.FtpPassword = "ofm_dev";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<string, int> getFtpList = FileUpDownload.GetFtpList("");
            
        }
        private void ShowResult(int a, int b)
        {
            WaitFormService.WaitFormService.SetWaitFormCaption_bp(a, b);
        }

        private void 下载ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename = "";// listBox1.SelectedItem.ToString();
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "所有文件(*.*)|*.*";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                Action<int, int> t = new Action<int, int>(ShowResult);
                WaitFormService.WaitFormService.ShowWaitForm();
                FileUpDownload.FtpDownload(filename, fileDialog.FileName, true, t);
                WaitFormService.WaitFormService.CloseWaitForm();
            }
        }

        private void 上传ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.*)|*.*";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                Action<int, int> t = new Action<int, int>(ShowResult);
                WaitFormService.WaitFormService.ShowWaitForm();
                FileUpDownload.FtpUpload(fileDialog.FileName, "", t);
                WaitFormService.WaitFormService.CloseWaitForm();
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            string filename = "";// listBox1.SelectedItem.ToString();
            Dictionary<string, int> GetFtpList = FileUpDownload.GetFtpList("filename");
        }
    }
}