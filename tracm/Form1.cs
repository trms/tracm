using System;
using System.Collections;
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

            dataGridView1.DataSource = q;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.Columns["Title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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
            {
                FilePath.Text = openFileDialog1.FileName;
                Title.Text = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
            }
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
            if (CreateXML() == false)
            {
                ShowError("Could not create .xml file");
                return;
            }

            //Add the file to the upload queue
            QueueItem new_q = new QueueItem();
            new_q.Title = Title.Text;
            new_q.FilePath = FilePath.Text;
            q.Add(new_q);

            //Switch the UI to the queue tab and clear the current form
            tabControl1.SelectedTab = tabQueue;
        }

        private bool CreateXML()
        {
            string path = FilePath.Text + ".xml";
            try
            {
                //Delete the file is if exists
                if (File.Exists(path))
                    File.Delete(path);

                //Create the XML data
                string xml = "";
                
                xml += @"<?xml version='1.0' encoding='UTF-8'?><PBCoreDescriptionDocument xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xsi:schemaLocation='http://www.pbcore.org/PBCore/PBCoreNamespace.html http://www.pbcore.org/PBCore/PBCoreSchema.xsd' xmlns:fmp='http://www.filemaker.com/fmpxmlresult' xmlns='http://www.pbcore.org/PBCore/PBCoreNamespace.html'>";
                xml += @"<pbcoreTitle><title>" + Title.Text + "</title><titleType>Program</titleType></pbcoreTitle>";
                xml += @"<pbcoreSubject><subject>" + Subject.Text + "</subject></pbcoreSubject>";
                xml += @"<pbcoreDescription><description>" + Description.Text + "</description><descriptionType>Program</descriptionType></pbcoreDescription>";
                xml += @"<pbcoreGenre><genre>" + Genre.Text + "</genre><genreAuthorityUsed>PBCore v1.1 http://www.pbcore.org</genreAuthorityUsed></pbcoreGenre><pbcoreCreator><creator>" + Producer.Text + "</creator><creatorRole>Producer</creatorRole></pbcoreCreator>";
                xml += @"<pbcoreInstantiation>" + 
	                "<formatTimeStart>" + Cue.Text + "</formatTimeStart>" +
	                "<formatDuration>" + Length.Text + "</formatDuration>" +
	                "<pbcoreFormatID>" +
		                "<formatIdentifier>" + FilePath.Text + "</formatIdentifier>" +
		                "<formatIdentifierSource>" + Producer.Text + "</formatIdentifierSource>" +
	                "</pbcoreFormatID>" +
                    "</pbcoreInstantiation>";
                xml += @"</PBCoreDescriptionDocument>";

                //Write the file
                FileInfo t = new FileInfo(path);
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
            QueueErrorLabel.Text = ErrorText;
            QueueErrorLabel.Visible = true;
            timerQueueTimer.Enabled = true;
        }

        private void timerQueueTimer_Tick(object sender, EventArgs e)
        {
            QueueError.Visible = false;
            QueueError.Text = "";
            QueueErrorLabel.Visible = false;
            QueueErrorLabel.Text = "";
        }

        private void DeleteQueueItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows[0] != null)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                if(  (q[index] as QueueItem).Content_ID > 0)
                {
                    try { new Scs(Settings.Default.ACMServer, Settings.Default.ACMUsername).removeQueuedDownload((q[index] as QueueItem).Content_ID.ToString(), false); }
                    catch(Exception ex) { ShowError(ex.Message.ToString()); }
                }
                dataGridView1.Rows.Remove(dataGridView1.SelectedRows[0]);
            }
        }

        private void RefreshQueue_Click(object sender, EventArgs e)
        {
            RefreshDownloadQueue();
        }

        private void RefreshDownloadQueue()
        {
            List<int> dlq = new List<int>();
            Scs server = new Scs(Settings.Default.ACMServer, Settings.Default.ACMUsername);
            try
            {
                dlq = server.getDownloadQueue();
            }
            catch(Exception e)
            {
                ShowError(e.Message.ToString());
            }

            //Add items to the que that are't already there
            foreach (int content_id in dlq)
            {
                if (q.Content_ID_Exists(content_id) == false)
                {
                    try
                    {
                        string URL = server.getQueuedDownloadUrl(content_id.ToString());

                        QueueItem new_q = new QueueItem();
                        new_q.Title = "ACM Content ID: " + content_id;
                        new_q.FilePath = URL;
                        new_q.Content_ID = content_id;
                        q.Add(new_q);
                    }
                    catch (Exception e)
                    {
                        ShowError(e.Message.ToString());
                    }
                }
            }

            //Remove any items that are no longer relevent
            List<QueueItem> removeIndex = new List<QueueItem>();
            foreach (QueueItem item in q)
            {
                if (item.Content_ID == 0)
                    continue;

                bool exists = false;
                foreach (int id in dlq)
                {
                    if (id == item.Content_ID)
                        exists = true;
                }
                if (!exists)
                    removeIndex.Add(item);
            }
            foreach (QueueItem item in removeIndex)
                q.Remove(item);
        }

       
    }
}