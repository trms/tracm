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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace tracm
{
	public class VideoProcessor
	{
		private string m_path;
		public delegate void ProgressCallback(int current, int total);
		public event ProgressCallback Progress;
		// format details
		private string m_originalVideoFormat = String.Empty;
		private int m_originalWidth = 0;
		private int m_originalHeight = 0;
		private string m_originalAudioFormat = String.Empty;
		private int m_originalAudioRate = 0;
		private string m_originalAudioChannels = String.Empty;
		private double m_originalFrameRate = 29.97;
		private string m_videoFormat = String.Empty;
		private int m_width = 0;
		private int m_height = 0;
		private string m_audioFormat = String.Empty;
		private int m_audioRate = 0;
		private int m_audioBitRate = 0;
		private string m_audioChannels = String.Empty;
		private double m_frameRate = 29.97;
		private int m_length = 0;
		private int m_frameCount = 0;
		private int m_bitRate = 0;
		private bool useAVS = false;
		private string m_avsPath = null;
		private volatile bool m_run = true;

		public enum OutputVideoFormat
		{
			copy,
			mpeg2,
			mpeg4,
			h264,
			flv,
			wm7,
			wm9
		};

		public enum OutputAudioFormat
		{
			copy,
			mp2,
			mp3
		};

		public VideoProcessor(string path)
		{
			m_path = path;
			//try
			//{
				ScanFileFormatDirect();
			//}
			//catch(Exception ex)
			//{
			//    string ext = new FileInfo(m_path).Extension.ToLower();
			//    if (ext == ".avi" || ext == ".asf" || ext == ".wmv" || ext == ".dv" || ext == ".mov")
			//        ScanFileFormatAVS();
			//    else
			//        throw ex;
			//}
		}

		private string AVSPath
		{
			get
			{
				if(m_avsPath != null)
					return(m_avsPath);
				FileInfo fi = new FileInfo(m_path);
				string path = Path.Combine(fi.DirectoryName, "temp.avs");
				using (StreamWriter sw = new StreamWriter(path))
				{
					sw.WriteLine(String.Format("DirectShowSource(\"{0}\")", m_path));
					sw.Close();
				}
				return (path);
			}
		}

		/*
2.18 How can I read DirectShow files?
If you have built FFmpeg with ./configure --enable-avisynth (only possible on MinGW/Cygwin platforms), then you may use any file that DirectShow can read as input. (Be aware that this feature has been recently added, so you will need to help yourself in case of problems.) 

Just create an "input.avs" text file with this single line ... 

  DirectShowSource("C:\path to your file\yourfile.asf")

... and then feed that text file to FFmpeg: 

  ffmpeg -i input.avs

For ANY other help on Avisynth, please visit http://www.avisynth.org/. 
		*/
		private void ScanFileFormatDirect()
		{
			string details = RunFFMPEG(String.Format("-i \"{0}\"", m_path));
			ScanFileFormat(details);
		}

		private void ScanFileFormat(string details)
		{
			/*
Input #0, mpeg, from 'file.mpg':
Duration: 00:01:01.0, start: 1.186233, bitrate: 11788 kb/s
Stream #0.0[0x1e0]: Video: mpeg2video, yuv420p, 1280x720 [PAR 1:1 DAR 16:9],
12877 kb/s, 59.94 tb(r)
Stream #0.1[0x80]: Audio: liba52, 48000 Hz, stereo, 384 kb/s
*/
			if (details.ToLower().Contains("could not find codec parameters") || details.ToLower().Contains("unknown format"))
				throw new ArgumentException("Not a recognized file format.");
			Match m;
			m = Regex.Match(details, @"Video: ([^,]*), [^,]*, (\d+)x(\d+)");
			if(!m.Success) // catch weird case that happens sometimes
				m = Regex.Match(details, @"Video: ([^,]*), (\d+)x(\d+)");
			if (m.Success)
			{
				m_videoFormat = m.Groups[1].Value;
				m_width = Convert.ToInt32(m.Groups[2].Value);
				m_height = Convert.ToInt32(m.Groups[3].Value);
			}
			m = Regex.Match(details, @"(\d+\.\d+)\s+tb\(r\)");
			if (m.Success)
			{
				m_frameRate = Convert.ToDouble(m.Groups[1].Value);
			}
			m = Regex.Match(details, @"Duration:\s+(\d+):(\d+):(\d+)\.(\d+)");
			if (m.Success)
			{
				// assume ntsc framerate (29.97)
				int hours = Convert.ToInt32(m.Groups[1].Value);
				int minutes = Convert.ToInt32(m.Groups[2].Value);
				int seconds = Convert.ToInt32(m.Groups[3].Value);
				int frames = Convert.ToInt32(m.Groups[4].Value);

				double total = hours * 3600.0;
				total += minutes * 60.0;
				total += seconds;
				m_length = Convert.ToInt32(total);
				total *= m_frameRate;
				total += frames;

				// pad seconds by 1 if there are extra frames
				if (frames > 0)
					m_length++;

				m_frameCount = Convert.ToInt32(total);
			}
			m = Regex.Match(details, @"bitrate:\s+(\d+)");
			if (m.Success)
			{
				m_bitRate = 1000 * Convert.ToInt32(m.Groups[1].Value);
			}
			m = Regex.Match(details, @"Audio: ([^,]*), ([^\s]*)\s\w*, ([^,\r\n]*), ([^\s]*)");
			if (m.Success)
			{
				m_audioFormat = m.Groups[1].Value;
				m_audioRate = Convert.ToInt32(m.Groups[2].Value);
				m_audioChannels = m.Groups[3].Value;
				m_audioBitRate = 1000 * Convert.ToInt32(m.Groups[4].Value);
			}

			m_originalVideoFormat = m_videoFormat;
			m_originalWidth = m_width;
			m_originalHeight = m_height;
			m_originalAudioFormat = m_audioFormat;
			m_originalAudioRate = m_audioRate;
			m_originalAudioChannels = m_audioChannels;
			m_originalFrameRate = m_frameRate;
		}

		private string ApplicationDirectory
		{
			get
			{
				FileInfo fi = new FileInfo(Application.ExecutablePath);
				return (fi.DirectoryName);
			}
		}

		public void Transcode(string outputPath)
		{
			string path = m_path;
            
            // Video Options
            // Following from archive.org for the Community Media Collection. Should enforce even more compaitable files
            // -vcodec mpeg2video -r 29.97 -f dvd -copyts -g 15 -b:v 5000000 -maxrate 6000000 -minrate 4000000 -bufsize 635008 -packetsize 2048 -muxrate 10080000
            string vcodec = " -vcodec mpeg2video -r 29.97 -f dvd -copyts -g 15 -b 5000000 -maxrate 6000000 -minrate 4000000 -bufsize 635008 -packetsize 2048 -muxrate 10080000 -s 720x480 -aspect 4:3";
			
            int outputChannels = 2;

            //Audio Options
            // Following from archive.org for the Community Media Collection. Should enforce even more compaitable files
            // -acodec mp2 -ar 48000 -b:a 160k
            string acodec = String.Format(" -acodec mp2 -ab 192k -ar 48000 -ac {0}", outputChannels);
            

			string result = RunFFMPEG(String.Format("-y -i \"{0}\" {1} {2} \"{3}\"", path, vcodec, acodec, outputPath));
			
            if (result.Contains("Unknown format is not supported as input format"))
				throw new Exception("Can't process this video format");
		}

		private string RunFFMPEG(string parameters)
		{
			Process p = new Process();
			p.StartInfo.FileName = Path.Combine(ApplicationDirectory, "ffmpeg.exe");
			if (File.Exists(p.StartInfo.FileName) == false)
				throw new IOException(String.Format("Please install ffmpeg.exe into {0}.  It can be downloaded from http://ffdshow.faireal.net/mirror/ffmpeg/", ApplicationDirectory));
			p.StartInfo.Arguments = parameters;
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			p.Start();
			string line;
			string result = String.Empty;
			int totalFrames = 0;
			while ((line = p.StandardError.ReadLine()) != null)
			{
				Match m;
				Application.DoEvents();
				result += line + Environment.NewLine;

				if (totalFrames == 0)
				{
					m = Regex.Match(line, @"Duration:\s+(\d+):(\d+):(\d+)\.(\d+)");
					if (m.Success)
					{
						// assume ntsc framerate (29.97)
						int hours = Convert.ToInt32(m.Groups[1].Value);
						int minutes = Convert.ToInt32(m.Groups[2].Value);
						int seconds = Convert.ToInt32(m.Groups[3].Value);
						int frames = Convert.ToInt32(m.Groups[4].Value);

						double total = hours * 3600.0;
						total += minutes * 60.0;
						total += seconds;
						total *= 29.97;
						total += frames;

						totalFrames = Convert.ToInt32(total);
					}
				}

				m = Regex.Match(line, @"frame=\s+(\d+)");
				if (m.Success)
				{
					int currentFrame = Convert.ToInt32(m.Groups[1].Value);
					if (Progress != null)
						Progress(currentFrame, totalFrames);

				}

				if (m_run == false)
				{
					p.Kill();
					break;
				}
			}
			if (Progress != null)
				Progress(totalFrames, totalFrames);
			m_run = true;
			p.WaitForExit();
			p.Dispose();
			return (result);
		}

		public void Cancel()
		{
			m_run = false;
		}

		public string VideoFormat
		{
			get { return m_videoFormat; }
			set { m_videoFormat = value; }
		}

		public int Width
		{
			get { return m_width; }
			set { m_width = value; }
		}

		public int Height
		{
			get { return m_height; }
			set { m_height = value; }
		}

		public string AudioFormat
		{
			get { return m_audioFormat; }
			set { m_audioFormat = value; }
		}

		public int AudioRate
		{
			get { return m_audioRate; }
			set { m_audioRate = value; }
		}

		public string AudioChannels
		{
			get { return m_audioChannels; }
			set { m_audioChannels = value; }
		}

		public double FrameRate
		{
			get { return m_frameRate; }
			set { m_frameRate = value; }
		}

		public int FrameCount
		{
			get { return m_frameCount; }
		}

		public int BitRate
		{
			get { return m_bitRate; }
		}

		public int AudioBitRate
		{
			get { return m_audioBitRate; }
		}

		public int LengthInSeconds
		{
			get { return m_length; }
		}
	}
}
