using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using tracm.Properties;
using FTPLib;

namespace tracm
{
    public partial class MainForm : Form
    {
        Queue q = new Queue();
       
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ACMServer.Text = Settings.Default.ACMServer;
            ACMUsername.Text = Settings.Default.ACMUsername;
            ACMPassword.Text = Settings.Default.ACMPassword;
            CablecastServer.Text = Settings.Default.CablecastServer;
            CablecastUsername.Text = Settings.Default.CablecastUsername;
            CablecastPassword.Text = Settings.Default.CablecastPassword;
            DownloadPath.Text = Settings.Default.DownloadPath;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.ACMServer = ACMServer.Text;
            Settings.Default.ACMUsername = ACMUsername.Text;
            Settings.Default.ACMPassword = ACMPassword.Text;
            Settings.Default.CablecastServer = CablecastServer.Text;
            Settings.Default.CablecastUsername = CablecastUsername.Text;
            Settings.Default.CablecastPassword = CablecastPassword.Text;
            Settings.Default.DownloadPath = DownloadPath.Text;
            Settings.Default.Save();
        }

        private void FileBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
                DownloadPath.Text = folderBrowserDialog1.SelectedPath;
        }

        private void VideoFileButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
                FilePath.Text = openFileDialog1.FileName;
        }

        private void AddToQueue_Click(object sender, EventArgs e)
        {
            //Make sure the path still exists
            if (File.Exists(FilePath.Text) == false)
            {
                ShowError("The selected file does not exist");
                return;
            }

            //Create the XML file
            if (CreateXML(FilePath.Text + ".xml") == false)
            {
                ShowError("Could not create .xml file");
                return;
            }

            //Add the file to the upload queue
            DisplayQueue new_q = new DisplayQueue();
            new_q.FilePath = FilePath.Text;
            q.Add(new_q);
            DisplayQueue x = q.AddNew();
            x.FilePath = FilePath.Text;
            q.EndNew(0);

            //Switch the UI to the queue tab and clear the current form
            tabControl1.SelectedTab = tabQueue;
        }

        private bool CreateXML(string FilePath)
        {
            try
            {
                //Delete the file is if exists
                if (File.Exists(FilePath))
                    File.Delete(FilePath);

                //Create the XML data
                string xml = "";
                xml += "<xml>";

                //Write the file
                FileInfo t = new FileInfo(FilePath);
                StreamWriter sw = t.CreateText();
                sw.Write(xml);
                sw.Close();

                return true;
            }
            catch { }

            return false;
        }

        private void ShowError(string ErrorText)
        {
            QueueError.Text = ErrorText;
            QueueError.Visible = true;
            timerQueueTimer.Enabled = true;
        }

        private void timerQueueTimer_Tick(object sender, EventArgs e)
        {
            QueueError.Visible = false;
            QueueError.Text = "";
        }

    }
}