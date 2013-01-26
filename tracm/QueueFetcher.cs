/*
//-------------------------------------------------------------------------------//
This file is part of tracm, a client for the ACM Shared Content Server.
Copyright (C) 2010 Tightrope Media Systems
http://www.trms.com
http://labs.trms.com

tracm is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License.

tracm is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
//-------------------------------------------------------------------------------//
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using tracm.Queue;

namespace tracm
{
	public class QueueFetcher : WorkItem
	{
		private List<DownloadWorker> m_files = new List<DownloadWorker>();

		public QueueFetcher()
		{
			Progress.ShowProgressBar = false;
		}

		protected override void WorkMethod()
		{
			List<int> dlq = new List<int>();
			dlq = Scs.getDownloadQueue();
			foreach (int content_id in dlq)
			{
				string data = Scs.getContentMetadata(content_id.ToString());
				string filename = String.Format("Content{0}.mpg", content_id);
				long size = 0;
				Match m = Regex.Match(data, "<formatIdentifier>([^<]*)");
				if (m.Success)
					filename = m.Groups[1].Value;
				m = Regex.Match(data, @"<formatFileSize>(\d+)");
				if (m.Success)
					size = Convert.ToInt64(m.Groups[1].Value);
				string URL = Scs.getQueuedDownloadUrl(content_id.ToString());
				URL = System.Web.HttpUtility.UrlDecode(URL);
				URL = System.Web.HttpUtility.HtmlDecode(URL);
				if (IsRunning == false)
					break;
				m_files.Add(new DownloadWorker(content_id, URL, filename, size, data));
			}
		}

		public override string ToString()
		{
			return "Updating SCS Download Queue";
		}

		public List<DownloadWorker> Files
		{
			get { return m_files; }
		}
	}
}
