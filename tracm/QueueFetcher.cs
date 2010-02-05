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
