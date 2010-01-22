using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using tracm.Properties;
using tracm.Queue;

namespace tracm
{
	public class DownloadWorker : WorkItem
	{
		private string m_url;
		private string m_name;
		private string m_path;
		private int m_contentID;
		private long m_fileSize = 0;

		public DownloadWorker(int contentID, string url, string name, long size)
		{
			m_contentID = contentID;
			m_url = url;
			m_name = name;
			m_path = Path.Combine(Properties.Settings.Default.DownloadPath, m_name);
			m_fileSize = size;
			if (size == 0)
				Progress.ShowProgressBar = false;
		}

		protected override void WorkMethod()
		{
			using (WebClient wc = new WebClient())
			{
				using (Stream data = wc.OpenRead(new Uri(m_url)))
				{
					//write to temp file
					long position = 0;
					using (FileStream fs = new FileStream(m_path, FileMode.Create))
					{
						byte[] buffer = new byte[4096];
						int len = 0;
						while ((len = data.Read(buffer, 0, 4096)) != 0)
						{
							position += len;
							try
							{
								ProgressValue = Convert.ToInt32(100.0 * (Convert.ToDouble(position) / Convert.ToDouble(m_fileSize)));
							}
							catch { }
							fs.Write(buffer, 0, len);
							if(IsRunning == false)
							{
								fs.Close();
								data.Close();
								File.Delete(m_path);
								break;
							}
						}
						fs.Close();
					}
					data.Close();
				}
			}

			try
			{
				Scs.removeQueuedDownload(m_contentID.ToString(), true);
			}
			catch { }
		}

		public override string ToString()
		{
			return String.Format("Downloading {0}", m_name);
		}

		public int ContentID
		{
			get { return m_contentID; }
		}
	}
}
