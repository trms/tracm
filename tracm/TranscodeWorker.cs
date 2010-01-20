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
			m_vp.Transcode(VideoProcessor.OutputVideoFormat.mpeg2, 4000, VideoProcessor.OutputAudioFormat.mp2, TempFile);

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
