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
		private bool m_cablecastCanCreateShows = false;
       
        public MainForm()
        {
            InitializeComponent();

			VerifyCablecast();

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

		private delegate void AddDownloadCallback(DownloadWorker download);

		private void AddDownload(DownloadWorker download)
		{
			if (this.InvokeRequired)
			{
				AddDownloadCallback d = new AddDownloadCallback(AddDownload);
				this.Invoke(d, new object[] { download });
			}
			else
				m_list.Add(download);
		}

		void WorkCompletedEvent()
		{
			DownloadWorker previous = null;
			lock (m_lockObject)
			{
				for (int i = 0; i < m_list.Count; i++)
				{
					if (m_list[i].Progress.IsDone && m_list[i] is QueueFetcher)
					{
						List<DownloadWorker> files = ((QueueFetcher)m_list[i]).Files;
						foreach (DownloadWorker dw in files)
						{
							if (ContentExists(dw.ContentID) == false)
							{
								AddDownload(dw);
								if (previous != null) // queue item
									previous.NextItem = dw;
								else // otherwise start it immediately
									dw.Work();
								previous = dw;
							}
						}
						files.Clear();
					}
				}
			}
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

		private bool ContentExists(int contentID)
		{
			bool result = false;
			lock (m_lockObject)
			{
				for (int i = 0; i < m_list.Count; i++)
				{
					if (m_list[i] is DownloadWorker && ((DownloadWorker)m_list[i]).ContentID == contentID)
					{
						result = true;
						break;
					}
				}
			}
			return result;
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
			{
				DownloadPath.Text = folderBrowserDialog1.SelectedPath;
				Settings.Default.DownloadPath = DownloadPath.Text;
				Settings.Default.Save();
			}
        }

        private void VideoFileButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                FilePath.Text = openFileDialog1.FileName;
				Inentifier.Text = String.Empty;
				Title.Text = String.Empty;
				Subject.Text = String.Empty;
				Genre.Text = String.Empty;
				Producer.Text = String.Empty;
				Description.Text = String.Empty;
				Cue.Text = "0";

				string title = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
				Match m = Regex.Match(title, @"^(\d+)-(.*)$");
				if (m.Success)
				{
					Inentifier.Text = m.Groups[1].Value;
					title = m.Groups[2].Value;
					Title.Text = title;
					Inentifier_Validating(this, new CancelEventArgs(false));
				}
				else
					Title.Text = title;
				VideoProcessor vp = new VideoProcessor(openFileDialog1.FileName);
				Length.Text = vp.LengthInSeconds.ToString();
				// check if the file is compatible
				if (vp.VideoFormat != "mpeg2video" || vp.AudioFormat != "mp2")
					TranscodeIndicator.Text = "* NOTE: This file will be transcoded before being queued";
				else
					TranscodeIndicator.Text = String.Empty;
            }
        }

        private void AddToQueue_Click(object sender, EventArgs e)
        {
			if (String.IsNullOrEmpty(TranscodeIndicator.Text))
			{
				// straight upload
				UploadWorker upload = new UploadWorker(FilePath.Text, Title.Text, Subject.Text, Description.Text, Genre.Text, Producer.Text, Convert.ToInt32(Cue.Text), Convert.ToInt32(Length.Text));
				upload.WorkCompletedEvent += new WorkItem.WorkCompleted(WorkCompletedEvent);
				m_list.Add(upload);
				upload.Work();
			}
			else
			{
				// otherwise we have to transcode, then upload
				TranscodeWorker tw = new TranscodeWorker(FilePath.Text);
				UploadWorker upload = new UploadWorker(tw.TempFile, Title.Text, Subject.Text, Description.Text, Genre.Text, Producer.Text, Convert.ToInt32(Cue.Text), Convert.ToInt32(Length.Text));
				DeleteWorker dw = new DeleteWorker(tw.TempFile);

				tw.WorkCompletedEvent += new WorkItem.WorkCompleted(WorkCompletedEvent);
				upload.WorkCompletedEvent += new WorkItem.WorkCompleted(WorkCompletedEvent);
				dw.WorkCompletedEvent += new WorkItem.WorkCompleted(WorkCompletedEvent);
				tw.NextItem = upload;
				upload.NextItem = dw;

				m_list.Add(tw);
				m_list.Add(upload);
				m_list.Add(dw);
				tw.Work();
			}

            //Switch the UI to the queue tab and clear the current form
            tabControl1.SelectedTab = tabQueue;
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
			QueueFetcher qf = new QueueFetcher();

			qf.WorkCompletedEvent += new WorkItem.WorkCompleted(WorkCompletedEvent);
			m_list.Add(qf);
			qf.Work();
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

		// save settings immediately after changes are made
		private void ACMServer_Validating(object sender, CancelEventArgs e)
		{
			Settings.Default.ACMServer = ACMServer.Text;
			Settings.Default.Save();
		}

		private void ACMUsername_Validating(object sender, CancelEventArgs e)
		{
			Settings.Default.ACMUsername = ACMUsername.Text;
			Settings.Default.Save();
		}

		private void ACMPassword_Validating(object sender, CancelEventArgs e)
		{
			Settings.Default.ACMPassword = ACMPassword.Text;
			Settings.Default.Save();
		}

		private void CablecastServer_Validating(object sender, CancelEventArgs e)
		{
			Settings.Default.CablecastServer = CablecastServer.Text;
			Settings.Default.Save();
			VerifyCablecast();
		}

		private void CablecastUsername_Validating(object sender, CancelEventArgs e)
		{
			Settings.Default.CablecastUsername = CablecastUsername.Text;
			Settings.Default.Save();
		}

		private void CablecastPassword_Validating(object sender, CancelEventArgs e)
		{
			Settings.Default.CablecastPassword = CablecastPassword.Text;
			Settings.Default.Save();
		}

		private void DownloadPath_Validating(object sender, CancelEventArgs e)
		{
			Settings.Default.DownloadPath = DownloadPath.Text;
			Settings.Default.Save();
		}

		private void Inentifier_Validating(object sender, CancelEventArgs e)
		{
			int showID = 0;
			if(Int32.TryParse(Inentifier.Text, out showID) && (String.IsNullOrEmpty(Settings.Default.CablecastServer) == false))
			{
				Cablecast.CablecastWS webService = new Cablecast.CablecastWS();
				webService.Url = String.Format("http://{0}/CablecastWS/CablecastWS.asmx", Properties.Settings.Default.CablecastServer.Trim());
				webService.GetShowInformationCompleted += new tracm.Cablecast.GetShowInformationCompletedEventHandler(webService_GetShowInformationCompleted);
				webService.GetShowInformationAsync(showID, webService);
			}
		}

		private void webService_GetShowInformationCompleted(object sender, tracm.Cablecast.GetShowInformationCompletedEventArgs e)
		{
			Cablecast.CablecastWS webService = (Cablecast.CablecastWS)e.UserState;
			Cablecast.ShowInfo showInfo = (Cablecast.ShowInfo)e.Result;
			Title.Text = showInfo.Title;
			if (String.IsNullOrEmpty(Producer.Text))
				Producer.Text = showInfo.Producer;
			if (String.IsNullOrEmpty(Genre.Text))
				Genre.Text = showInfo.Category;

			// requires 4.9 -->
			if (m_cablecastCanCreateShows)
			{
				Cablecast.ReelInfo[] reels = webService.GetShowReels(showInfo.ShowID);
				if (reels.Length > 0)
					Cue.Text = reels[0].Cue.ToString();
			}
		}

		private void VerifyCablecast()
		{
			// check web service version for cablecast 4.9
			if (String.IsNullOrEmpty(Settings.Default.CablecastServer) == false)
			{
				Cablecast.CablecastWS webService = new Cablecast.CablecastWS();
				webService.Url = String.Format("http://{0}/CablecastWS/CablecastWS.asmx", Properties.Settings.Default.CablecastServer.Trim());
				webService.WSVersionCompleted += new tracm.Cablecast.WSVersionCompletedEventHandler(webService_WSVersionCompleted);
				webService.WSVersionAsync();
			}
		}

		private void webService_WSVersionCompleted(object sender, tracm.Cablecast.WSVersionCompletedEventArgs e)
		{
			int version = 0;
			Int32.TryParse(e.Result.Replace(".", ""), out version);
			if (version >= 300)
				m_cablecastCanCreateShows = true;
		}
    }
}