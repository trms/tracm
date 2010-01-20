using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using tracm.Properties;
using tracm.Queue;

namespace tracm
{
	public class DeleteWorker : WorkItem
	{
		private string m_path;

		public DeleteWorker(string path)
		{
			m_path = path;
			Progress.ShowProgressBar = false;
		}

		protected override void WorkMethod()
		{
			if (IsRunning && File.Exists(m_path))
				File.Delete(m_path);
		}

		public override string ToString()
		{
			return String.Format("Cleaning up {0}", Path.GetFileName(m_path));
		}
	}
}
