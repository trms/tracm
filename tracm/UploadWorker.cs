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
	public class UploadWorker : WorkItem
	{
		private string m_id;
		private string m_path;
		private string m_title;
		private string m_subject;
		private string m_description;
		private string m_genre;
		private string m_producer;
		private int m_cue;
		private int m_length;

		public UploadWorker(string id, string path, string title, string subject, string description, string genre, string producer, int cue, int length)
		{
			m_id = id;
			m_path = path;
			m_title = title;
			m_subject = subject;
			m_description = description;
			m_genre = genre;
			m_producer = producer;
			m_cue = cue;
			m_length = length;
		}

		protected override void WorkMethod()
		{
			string server = Settings.Default.ACMServer;
			int idx = server.IndexOf(':');
			if (idx != -1)
				server = server.Substring(0, idx);

			// upload file via FTP
			FTPLib.FTP ftp = new FTPLib.FTP(server, Settings.Default.ACMUsername, Settings.Default.ACMPassword);
			ftp.Connect();
			ftp.PassiveMode = Properties.Settings.Default.PassiveFTP;

			string xmlPath = CreateXML();
			UploadFile(ftp, xmlPath);
			UploadFile(ftp, m_path);
			File.Delete(xmlPath);

			ftp.Disconnect();

			/* <?xml version="1.0" encoding="UTF-8"?>
<response status="fail">
  <error_code>INTERNAL_ERROR</error_code>
  <error_message>Internal server error parsing PBCore XML /media/psg/vol1/users/tightrope/TheKeyandthePortal-download.mpg.xml.</error_message>
</response>
*/
			Scs.addContent(Path.GetFileName(m_path));
		}

		private void UploadFile(FTPLib.FTP ftp, string path)
		{
			ftp.OpenUpload(path, Path.GetFileName(path));
			long fileSize = new FileInfo(path).Length;
			long position = 0;
			long bytes = 0;
			while ((bytes = ftp.DoUpload()) != 0)
			{
				if (IsRunning == false)
					break;
				position += bytes;
				try
				{
					ProgressValue = Convert.ToInt32(100.0 * (Convert.ToDouble(position) / Convert.ToDouble(fileSize)));
				}
				catch { }
			}
		}

		public override string ToString()
		{
			return String.Format("Uploading {0}", Path.GetFileName(m_path));
		}

		private string CreateXML()
		{
			string path = String.Format("{0}.xml", m_path);
			try
			{
				//Delete the file is if exists
				if (File.Exists(path))
					File.Delete(path);

				//Create the XML data
				StringBuilder xml = new StringBuilder();

				/*
<?xml version="1.0" encoding="UTF-8"?>
<response status="ok">
<?xml version='1.0' encoding='UTF-8'?>
<PBCoreDescriptionDocument xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xsi:schemaLocation='http://www.pbcore.org/PBCore/PBCoreNamespace.html http://www.pbcore.org/PBCore/PBCoreSchema.xsd' xmlns:fmp='http://www.filemaker.com/fmpxmlresult' xmlns='http://www.pbcore.org/PBCore/PBCoreNamespace.html'>
<pbcoreIdentifier><identifier>4</identifier><identifierSource>test</identifierSource></pbcoreIdentifier>
				 * <pbcoreTitle><title>test</title><titleType>Program</titleType></pbcoreTitle>
				 * <pbcoreTitle><title>test</title><titleType>Episode</titleType></pbcoreTitle>
				 * <pbcoreSubject><subject>test</subject></pbcoreSubject>
				 * <pbcoreDescription><description>test</description><descriptionType>Program</descriptionType></pbcoreDescription>
				 * <pbcoreGenre><genre>Action</genre><genreAuthorityUsed>PBCore v1.1 http://www.pbcore.org</genreAuthorityUsed></pbcoreGenre>
				 * <pbcoreAudienceRating><audienceRating>E</audienceRating></pbcoreAudienceRating>
				 * <pbcoreCreator><creator>test</creator><creatorRole>Producer</creatorRole></pbcoreCreator>
				 * <pbcoreRightsSummary><rightsSummary>http://creativecommons.org/licenses/by-nc/3.0/</rightsSummary></pbcoreRightsSummary>
				 * <pbcoreInstantiation>
					 * <formatPhysical>Hard Drive</formatPhysical>
					 * <formatDigital>video/MP2P</formatDigital>
					 * <formatLocation> USA </formatLocation>
					 * <formatGenerations>Moving image/Master</formatGenerations>
					 * <formatStandard>NTSC video (INTERLACED)</formatStandard>
					 * <formatFileSize>25739264</formatFileSize>
					 * <formatTimeStart>00:00:00</formatTimeStart>
					 * <formatDuration>00:00:36</formatDuration>
					 * <formatDataRate>Video 6000000 bits/sec;Audio 224000 bits/sec</formatDataRate>
					 * <formatFrameSize>720x480</formatFrameSize>
					 * <formatAspectRatio>4:3</formatAspectRatio>
					 * <formatFrameRate>29.97 fps</formatFrameRate>
					 * <pbcoreFormatID>
						 * <formatIdentifier>Stop_hate_teen_30-nolead.mpg</formatIdentifier>
						 * <formatIdentifierSource>Admin, PSG</formatIdentifierSource>
					 * </pbcoreFormatID>
				 * </pbcoreInstantiation>
				 * <pbcoreExtension><extension>indemnification:test[ACM]</extension><extensionAuthorityUsed>Alliance For Community Media</extensionAuthorityUsed></pbcoreExtension>
				 * <pbcoreExtension><extension>tags:test[ACM]</extension><extensionAuthorityUsed>Alliance For Community Media</extensionAuthorityUsed></pbcoreExtension>
				 * <pbcoreExtension><extension>producer_address:test[ACM]</extension><extensionAuthorityUsed>Alliance For Community Media</extensionAuthorityUsed></pbcoreExtension>
				 * <pbcoreExtension><extension>producer_email:test@test.com[ACM]</extension><extensionAuthorityUsed>Alliance For Community Media</extensionAuthorityUsed></pbcoreExtension>
				 * </PBCoreDescriptionDocument></response>
				*/

				xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
				xml.AppendLine("<PBCoreDescriptionDocument xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.pbcore.org/PBCore/PBCoreNamespace.html http://www.pbcore.org/PBCore/PBCoreSchema.xsd\" xmlns:fmp=\"http://www.filemaker.com/fmpxmlresult\" xmlns=\"http://www.pbcore.org/PBCore/PBCoreNamespace.html\">");
				xml.AppendFormat("<pbcoreIdentifier><identifier>{0}</identifier><identifierSource>{1}</identifierSource></pbcoreIdentifier>", m_id, m_producer); xml.AppendLine();
				xml.AppendFormat("<pbcoreTitle><title>{0}</title><titleType>Program</titleType></pbcoreTitle>", m_title); xml.AppendLine();
				xml.AppendFormat("<pbcoreSubject><subject>{0}</subject></pbcoreSubject>", m_subject); xml.AppendLine();
				xml.AppendFormat("<pbcoreDescription><description>{0}</description><descriptionType>Program</descriptionType></pbcoreDescription>", m_description); xml.AppendLine();
				xml.AppendFormat("<pbcoreGenre><genre>{0}</genre><genreAuthorityUsed>PBCore v1.1 http://www.pbcore.org</genreAuthorityUsed></pbcoreGenre><pbcoreCreator><creator>{1}</creator><creatorRole>Producer</creatorRole></pbcoreCreator>", m_genre, m_producer); xml.AppendLine();
				xml.AppendLine("<pbcoreInstantiation>");
				xml.AppendLine("<formatPhysical>Hard Drive</formatPhysical>");
				xml.AppendLine("<formatDigital>video/MP2P</formatDigital>");
				xml.AppendLine("<formatLocation>USA</formatLocation>");
				xml.AppendLine("<formatGenerations>Moving image/Master</formatGenerations>");
				xml.AppendLine("<formatStandard>MPEG video</formatStandard>");
				xml.AppendFormat("<formatFileSize>{0}</formatFileSize>", new FileInfo(m_path).Length); xml.AppendLine();
				xml.AppendFormat("<formatTimeStart>{0}</formatTimeStart>", SecondsToLength(m_cue)); xml.AppendLine();
				xml.AppendFormat("<formatDuration>{0}</formatDuration>", SecondsToLength(m_length)); xml.AppendLine();
				xml.AppendLine("<formatFrameSize>720x480</formatFrameSize>");
				xml.AppendLine("<formatAspectRatio>4:3</formatAspectRatio>");
				xml.AppendLine("<formatFrameRate>29.97</formatFrameRate>");
				xml.AppendLine("<pbcoreFormatID>");
				xml.AppendFormat("<formatIdentifier>{0}</formatIdentifier>", Path.GetFileName(m_path)); xml.AppendLine();
				xml.AppendFormat("<formatIdentifierSource>{0}</formatIdentifierSource>", m_producer); xml.AppendLine();
				xml.AppendLine("</pbcoreFormatID>");
				xml.AppendLine("</pbcoreInstantiation>");
				xml.AppendLine("</PBCoreDescriptionDocument>");

				//Write the file
				FileInfo t = new FileInfo(path);
				StreamWriter sw = t.CreateText();
				sw.Write(xml.ToString());
				sw.Close();
			}
			catch { }

			return path;
		}

		private string SecondsToLength(int input)
		{
			int inputSecs = input;
			int seconds = 0;
			int minutes = 0;
			int hours = 0;
			string secondsStr = "";

			if (inputSecs > 0)
			{
				seconds = inputSecs % 60;
				inputSecs = inputSecs - seconds;
				if (inputSecs > 0)
				{
					inputSecs = inputSecs / 60;
					minutes = inputSecs % 60;
					inputSecs = inputSecs - minutes;
					if (inputSecs > 0)
					{
						inputSecs = inputSecs / 60;
						hours = inputSecs;
					}
				}
				if (hours < 10)
					secondsStr = "0" + hours.ToString() + ":";
				else
					secondsStr = hours.ToString() + ":";

				if (minutes < 10)
					secondsStr += "0" + minutes.ToString() + ":";
				else
					secondsStr += minutes.ToString() + ":";

				if (seconds < 10)
					secondsStr += "0" + seconds.ToString();
				else
					secondsStr += seconds.ToString();
			}
			else
				secondsStr = "00:00:00";

			return secondsStr;
		}
	}
}
