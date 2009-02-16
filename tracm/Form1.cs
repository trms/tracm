using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using tracm.Properties;

namespace tracm
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void FileBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
                DownloadPath.Text = folderBrowserDialog1.SelectedPath;
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

    }
}