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
using System.IO;
using System.Text;
using tracm.Properties;
using tracm.Queue;

namespace tracm
{
	public class TranscodeWorker : WorkItem
	{
		private string m_path;
		private VideoProcessor m_vp = null;

		public TranscodeWorker(string path)
		{
			m_path = path;
		}

		protected override void WorkMethod()
		{
			m_vp = new VideoProcessor(m_path);
			m_vp.Progress += new VideoProcessor.ProgressCallback(vp_Progress);
			m_vp.Transcode(TempFile);

			if (IsRunning == false && File.Exists(TempFile))
				File.Delete(TempFile);
		}

		public string TempFile
		{
			get { return String.Format("{0}.mpg", Path.Combine(Settings.Default.DownloadPath, Path.GetFileNameWithoutExtension(m_path))); }
		}

		void vp_Progress(int current, int total)
		{
			double partial = Convert.ToDouble(current) / Convert.ToDouble(total);
			int percent = Convert.ToInt32(100.0 * partial);

			ProgressValue = percent;

			if (IsRunning == false)
				m_vp.Cancel();
		}

		public override string ToString()
		{
			return String.Format("Transcoding {0}", Path.GetFileName(m_path));
		}
	}
}
