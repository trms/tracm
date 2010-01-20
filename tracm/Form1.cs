using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using tracm.Properties;
using tracm.Queue;
using FTPLib;

namespace tracm
{
    public partial class MainForm : Form
    {
		private BindingList<WorkItem> m_list = new BindingList<WorkItem>();
		private object m_lockObject = new object();
       
        public MainForm()
        {
            InitializeComponent();

			lock (m_lockObject)
			{
				m_list.AllowEdit = m_list.AllowNew = false;
				m_list.AllowRemove = true;
				m_list.RaiseListChangedEvents = true;
				dataGridView1.DataSource = m_list;
			}

			// set up columns to have the right type
			if (dataGridView1.Columns.Contains("Name"))
				dataGridView1.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

			if (dataGridView1.Columns.Contains("Progress"))
			{
				dataGridView1.Columns.Remove("Progress");

				DataGridViewProgressColumn column = new DataGridViewProgressColumn();
				column.Name = "Progress";
				column.HeaderText = "Progress";
				column.DataPropertyName = "Progress";
				dataGridView1.Columns.Add(column);
			}

			if (dataGridView1.Columns.Contains("Action"))
			{
				dataGridView1.Columns.Remove("Action");

				//DataGridViewButtonColumn column = new DataGridViewButtonColumn();
				//DataGridViewLinkColumn column = new DataGridViewLinkColumn();
				DataGridViewDisableButtonColumn column = new DataGridViewDisableButtonColumn();
				column.Name = "Action";
				column.HeaderText = "Action";
				column.DataPropertyName = "Action";
				dataGridView1.Columns.Add(column);
			}
        }

		void WorkCompletedEvent()
		{
			// clear completed items after each one finishes
			//ClearCompletedItems();
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

			TranscodeIndicator.Text = String.Empty;
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

			// cancel anything still in progress
			lock (m_lockObject)
			{
				for (int i = 0; i < m_list.Count; i++)
				{
					if (m_list[i].Progress.IsDone == false)
						m_list[i].Cancel();
				}
			}

			// wait for tasks to cancel
			while (ItemsInProgress)
			{
				Application.DoEvents();
				System.Threading.Thread.Sleep(200);
			}
        }

		private bool ItemsInProgress
		{
			get
			{
				bool result = false;
				lock (m_lockObject)
				{
					for (int i = 0; i < m_list.Count; i++)
					{
						if (m_list[i].Progress.IsDone == false)
						{
							result = true;
							break;
						}
					}
				}
				return result;
			}
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
				Inentifier.Text = String.Empty;

				string title = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
				Match m = Regex.Match(title, @"^(\d+)-(.*)$");
				if (m.Success)
				{
					Inentifier.Text = m.Groups[1].Value;
					title = m.Groups[2].Value;
				}
                Title.Text = title;
				VideoProcessor vp = new VideoProcessor(openFileDialog1.FileName);
				Length.Text = vp.LengthInSeconds.ToString();
				// check if the file is compatible
				if (vp.VideoFormat != "mpeg2video")
					TranscodeIndicator.Text = "* NOTE: This file will be transcoded before being queued";
				else
					TranscodeIndicator.Text = String.Empty;
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

			TranscodeWorker tw = new TranscodeWorker(FilePath.Text);
			DeleteWorker dw = new DeleteWorker(tw.TempFile);

			tw.WorkCompletedEvent += new WorkItem.WorkCompleted(WorkCompletedEvent);
			dw.WorkCompletedEvent += new WorkItem.WorkCompleted(WorkCompletedEvent);
			tw.NextItem = dw;

			m_list.Add(tw);
			m_list.Add(dw);
			tw.Work();

            //Create the XML file
            if (CreateXML() == false)
            {
                ShowError("Could not create .xml file");
                return;
            }

            //Switch the UI to the queue tab and clear the current form
            tabControl1.SelectedTab = tabQueue;
        }

        private bool CreateXML()
        {
            string path = String.Format("{0}.xml", FilePath.Text);
            try
            {
                //Delete the file is if exists
                if (File.Exists(path))
                    File.Delete(path);

                //Create the XML data
				StringBuilder xml = new StringBuilder();
                
				xml.AppendLine("<?xml version='1.0' encoding='UTF-8'?><PBCoreDescriptionDocument xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xsi:schemaLocation='http://www.pbcore.org/PBCore/PBCoreNamespace.html http://www.pbcore.org/PBCore/PBCoreSchema.xsd' xmlns:fmp='http://www.filemaker.com/fmpxmlresult' xmlns='http://www.pbcore.org/PBCore/PBCoreNamespace.html'>");
				xml.AppendFormat("<pbcoreTitle><title>{0}</title><titleType>Program</titleType></pbcoreTitle>", Title.Text); xml.AppendLine();
				xml.AppendFormat("<pbcoreSubject><subject>{0}</subject></pbcoreSubject>", Subject.Text); xml.AppendLine();
				xml.AppendFormat("<pbcoreDescription><description>{0}</description><descriptionType>Program</descriptionType></pbcoreDescription>", Description.Text); xml.AppendLine();
				xml.AppendFormat("<pbcoreGenre><genre>{0}</genre><genreAuthorityUsed>PBCore v1.1 http://www.pbcore.org</genreAuthorityUsed></pbcoreGenre><pbcoreCreator><creator>{1}</creator><creatorRole>Producer</creatorRole></pbcoreCreator>", Genre.Text, Producer.Text); xml.AppendLine();
				xml.AppendLine("<pbcoreInstantiation>");
				xml.AppendFormat("<formatTimeStart>{0}</formatTimeStart>", Cue.Text); xml.AppendLine();
				xml.AppendFormat("<formatDuration>{0}</formatDuration>", Length.Text); xml.AppendLine();
	            xml.AppendLine("<pbcoreFormatID>");
				xml.AppendFormat("<formatIdentifier>{0}</formatIdentifier>", FilePath.Text); xml.AppendLine();
				xml.AppendFormat("<formatIdentifierSource>{0}</formatIdentifierSource>", Producer.Text); xml.AppendLine();
	            xml.AppendLine("</pbcoreFormatID>");
                xml.AppendLine("</pbcoreInstantiation>");
                xml.AppendLine("</PBCoreDescriptionDocument>");

                //Write the file
                FileInfo t = new FileInfo(path);
                StreamWriter sw = t.CreateText();
                sw.Write(xml.ToString());
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
			ClearCompletedItems();
        }

        private void RefreshQueue_Click(object sender, EventArgs e)
        {
            RefreshDownloadQueue();
        }

        private void RefreshDownloadQueue()
        {
            List<int> dlq = new List<int>();
            try
            {
                dlq = Scs.getDownloadQueue();
            }
            catch(Exception e)
            {
                ShowError(e.Message.ToString());
            }

            //Add items to the que that are't already there
			//foreach (int content_id in dlq)
			//{
			//    if (q.Content_ID_Exists(content_id) == false)
			//    {
			//        try
			//        {
			//            string URL = Scs.getQueuedDownloadUrl(content_id.ToString());
			//        }
			//        catch (Exception e)
			//        {
			//            ShowError(e.Message.ToString());
			//        }
			//    }
			//}

            //Remove any items that are no longer relevent
			//List<QueueItem> removeIndex = new List<QueueItem>();
			//foreach (QueueItem item in q)
			//{
			//    if (item.Content_ID == 0)
			//        continue;

			//    bool exists = false;
			//    foreach (int id in dlq)
			//    {
			//        if (id == item.Content_ID)
			//            exists = true;
			//    }
			//    if (!exists)
			//        removeIndex.Add(item);
			//}
			//foreach (QueueItem item in removeIndex)
			//    q.Remove(item);
        }

		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == 2)
			{
				try
				{
					lock (m_lockObject)
						m_list[e.RowIndex].Cancel();
				}
				catch { }
			}
		}

		private void dataGridView1_SelectionChanged(object sender, EventArgs e)
		{
			// hide any selected rows, will effectively make rows unselectable
			dataGridView1.ClearSelection();
		}

		private delegate void ClearCompletedItemsCallback();

		private void ClearCompletedItems()
		{
			if (this.InvokeRequired)
			{
				ClearCompletedItemsCallback d = new ClearCompletedItemsCallback(ClearCompletedItems);
				this.Invoke(d);
			}
			else
			{
				lock (m_lockObject)
				{
					for (int i = 0; i < m_list.Count; i++)
					{
						if (m_list[i].Progress.IsDone)
						{
							m_list.RemoveAt(i);
							i--;
						}
					}
				}
			}
		}
    }
}