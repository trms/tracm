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
		private string m_metaData;
		private int m_contentID;
		private long m_fileSize = 0;

		public DownloadWorker(int contentID, string url, string name, long size, string data)
		{
			m_contentID = contentID;
			m_url = url;
			m_name = name;
			m_path = Path.Combine(Properties.Settings.Default.DownloadPath, m_name);
			m_fileSize = size;
			m_metaData = data;
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

			/*
<?xml version="1.0" encoding="UTF-8"?>
<response status="ok">
<?xml version='1.0' encoding='UTF-8'?><PBCoreDescriptionDocument xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xsi:schemaLocation='http://www.pbcore.org/PBCore/PBCoreNamespace.html http://www.pbcore.org/PBCore/PBCoreSchema.xsd' xmlns:fmp='http://www.filemaker.com/fmpxmlresult' xmlns='http://www.pbcore.org/PBCore/PBCoreNamespace.html'><pbcoreIdentifier><identifier>249</identifier><identifierSource>UChannel</identifierSource></pbcoreIdentifier><pbcoreTitle><title>Bob Schieffer</title><titleType>Program</titleType></pbcoreTitle><pbcoreTitle><title>Bob Schieffer</title><titleType>Episode</titleType></pbcoreTitle><pbcoreSubject><subject>Public Affairs</subject></pbcoreSubject><pbcoreDescription><description>Bob Schieffer</description><descriptionType>Program</descriptionType></pbcoreDescription><pbcoreGenre><genre>Community</genre><genreAuthorityUsed>PBCore v1.1 http://www.pbcore.org</genreAuthorityUsed></pbcoreGenre><pbcoreGenre><genre>Educational</genre><genreAuthorityUsed>PBCore v1.1 http://www.pbcore.org</genreAuthorityUsed></pbcoreGenre><pbcoreGenre><genre>News</genre><genreAuthorityUsed>PBCore v1.1 http://www.pbcore.org</genreAuthorityUsed></pbcoreGenre><pbcoreGenre><genre>Politics</genre><genreAuthorityUsed>PBCore v1.1 http://www.pbcore.org</genreAuthorityUsed></pbcoreGenre><pbcoreAudienceRating><audienceRating>TV-PG</audienceRating></pbcoreAudienceRating><pbcoreCreator><creator>UChannel</creator><creatorRole>Producer</creatorRole></pbcoreCreator><pbcoreRightsSummary><rightsSummary>http://creativecommons.org/licenses/by-nc-nd/3.0/</rightsSummary></pbcoreRightsSummary><pbcoreInstantiation><formatPhysical>Hard Drive</formatPhysical><formatDigital>video/MP2P</formatDigital><formatLocation>





USA
</formatLocation><formatGenerations>Moving image/Master</formatGenerations><formatStandard>NTSC video (INTERLACED)</formatStandard><formatFileSize>1600026624</formatFileSize><formatTimeStart>00:00:00</formatTimeStart><formatDuration>00:21:10</formatDuration><formatDataRate>Video 7987200 bits/sec;Audio 224000 bits/sec</formatDataRate><formatFrameSize>720x480</formatFrameSize><formatAspectRatio>4:3</formatAspectRatio><formatFrameRate>29.97 fps</formatFrameRate><pbcoreFormatID><formatIdentifier>20070709BobSchieffer.mpg</formatIdentifier><formatIdentifierSource>University Channel</formatIdentifierSource></pbcoreFormatID></pbcoreInstantiation><pbcoreExtension><extension>indemnification:Intended for public and community rebroadcast[ACM]</extension><extensionAuthorityUsed>Alliance For Community Media</extensionAuthorityUsed></pbcoreExtension><pbcoreExtension><extension>tags:UChannel,Public Affairs[ACM]</extension><extensionAuthorityUsed>Alliance For Community Media</extensionAuthorityUsed></pbcoreExtension><pbcoreExtension><extension>producer_address:UChannel
Rm 217, Robertson Hall
Princeton University
Princeton, NJ 08544
[ACM]</extension><extensionAuthorityUsed>Alliance For Community Media</extensionAuthorityUsed></pbcoreExtension><pbcoreExtension><extension>producer_email:uc@princeton.edu[ACM]</extension><extensionAuthorityUsed>Alliance For Community Media</extensionAuthorityUsed></pbcoreExtension></PBCoreDescriptionDocument></response>
*/
			if (CablecastFactory.CanUseCablecast && CablecastFactory.CanCreateShows && Settings.Default.UseCablecast)
			{
				// create show record in cablecast
				Cablecast.CablecastWS webService = CablecastFactory.Create();
				tracm.Cablecast.NewReel reel = new tracm.Cablecast.NewReel();
				reel.LengthSeconds = 0;
				reel.FormatID = 0;
				tracm.Cablecast.NewReel[] reels = { reel };
				tracm.Cablecast.CustomField[] customFields = new tracm.Cablecast.CustomField[0];
				int showID = webService.CreateNewShowRecord(String.Empty, Properties.Settings.Default.CablecastLocation, "test", "test", 0, false, 0, reels, 0, DateTime.Now, String.Empty, customFields, Properties.Settings.Default.CablecastUsername, Properties.Settings.Default.CablecastPassword);
				// rename file to include show ID
				//File.Move(m_path, Path.Combine(Path.GetDirectoryName(m_path), String.Format("{0}-{1}", showID, Path.GetFileName(m_path))));

			}
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
