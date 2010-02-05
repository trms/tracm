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
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
				string id = String.Empty;
				string title = Path.GetFileName(m_path);
				string episodeTitle = String.Empty;
				string producer = String.Empty;
				string genre = String.Empty;
				string rating = String.Empty;
				int cueTime = 0;
				int length = 0;
				StringBuilder summary = new StringBuilder();
	
				Match m = Regex.Match(m_metaData, "<pbcoreIdentifier><identifier>([^<]*)</identifier>");
				if (m.Success)
					id = m.Groups[1].Value;
				m = Regex.Match(m_metaData, "<pbcoreTitle><title>([^<]*)</title><titleType>Program</titleType></pbcoreTitle>");
				if (m.Success)
					title = m.Groups[1].Value;
				m = Regex.Match(m_metaData, "<pbcoreTitle><title>([^<]*)</title><titleType>Episode</titleType></pbcoreTitle>");
				if (m.Success)
					episodeTitle = m.Groups[1].Value;
				m = Regex.Match(m_metaData, "<pbcoreCreator><creator>([^<]*)</creator><creatorRole>Producer</creatorRole></pbcoreCreator>");
				if (m.Success)
					producer = m.Groups[1].Value;
				m = Regex.Match(m_metaData, "<pbcoreGenre><genre>([^<]*)</genre>");
				if (m.Success)
					genre = m.Groups[1].Value;
				m = Regex.Match(m_metaData, "<pbcoreAudienceRating><audienceRating>([^<]*)</audienceRating></pbcoreAudienceRating>");
				if (m.Success)
					rating = m.Groups[1].Value;
				m = Regex.Match(m_metaData, "<formatTimeStart>([\\d:]+)</formatTimeStart>");
				if (m.Success)
					cueTime = LengthToSeconds(m.Groups[1].Value);
				m = Regex.Match(m_metaData, "<formatDuration>([\\d:]+)</formatDuration>");
				if (m.Success)
					length = LengthToSeconds(m.Groups[1].Value);

				if (episodeTitle != String.Empty)
				{
					summary.AppendFormat("Episode: {0}", episodeTitle);
					summary.AppendLine();
				}
				if (producer != String.Empty)
				{
					summary.AppendFormat("Producer: {0}", producer);
					summary.AppendLine();
				}
				if (genre != String.Empty)
				{
					summary.AppendFormat("Genre: {0}", genre);
					summary.AppendLine();
				}
				if (rating != String.Empty)
				{
					summary.AppendFormat("Rating: {0}", rating);
					summary.AppendLine();
				}

				// create show record in cablecast
				Cablecast.CablecastWS webService = CablecastFactory.Create();
				tracm.Cablecast.NewReel reel = new tracm.Cablecast.NewReel();
				reel.LengthSeconds = length;
				reel.CueSeconds = cueTime;
				reel.FormatID = Properties.Settings.Default.CablecastFormat;
				reel.Chapter = 0;
				reel.Title = 0;
				tracm.Cablecast.NewReel[] reels = { reel };
				tracm.Cablecast.CustomField[] customFields = new tracm.Cablecast.CustomField[0];
				int showID = webService.CreateNewShowRecord(id, Properties.Settings.Default.CablecastLocation, title, title, -1, false, 0, reels, 0, DateTime.Now, summary.ToString(), customFields, Properties.Settings.Default.CablecastUsername, Properties.Settings.Default.CablecastPassword);
				if (showID > 0)
				{
					// rename file to include show ID
					File.Move(m_path, Path.Combine(Path.GetDirectoryName(m_path), String.Format("{0}-{1}", showID, Path.GetFileName(m_path))));
				}

			}
		}

		private int LengthToSeconds(string lengthStr)
		{
			string newLengthStr = "";
			string[] lengthArray;
			int seconds = 0;


			//First, remove any and all spaces from the length string,
			//change it to lowercase, and replace all "." with ":"
			lengthStr = lengthStr.Replace(" ", "");
			lengthStr = lengthStr.Replace(".", ":");
			lengthStr = lengthStr.ToLower();

			//Iterate through the string, replacing all non numeric characters
			//with a '0', with the exception of ':'.
			foreach (char c in lengthStr)
			{
				if (Char.IsDigit(c) || c == ':')
					newLengthStr += c;
				else
					newLengthStr += "0";
			}

			//Split newTimeStr on the ":", stick elements in timeArray.
			lengthArray = newLengthStr.Split(':');

			//Create correctly formatted length string to be converted into seconds.
			//The for loop adds any missing elements.
			//For example, if the user enters "12:00", it adds the hours element, resulting in "00:12:00".
			/*
			newLengthStr = "";
			int range = lengthArray.GetUpperBound(0);
			for (int i = 0; i <= 2; i ++)
			{
				if (i <= range)
					newLengthStr += lengthArray[i] + ":";
				else
					newLengthStr = "00:" + newLengthStr;
			}
			newLengthStr = newLengthStr.Substring(0, newLengthStr.Length - 1);  //remove trailing ":"
			*/

			//Split properly formatted length string into an array.
			//lengthArray = newLengthStr.Split(':');

			//Check to see if any of the elements are empty
			string Length1 = "";
			string Length2 = "";
			string Length3 = "";
			string CompileLength = "";

			//Check the split
			if (lengthArray.Length > 0)
				Length1 = lengthArray[0];

			if (lengthArray.Length > 1)
				Length2 = lengthArray[1];

			if (lengthArray.Length > 2)
				Length3 = lengthArray[2];

			//Compile the length
			if (Length1.Length != 0)
				CompileLength += Length1;

			if (Length2.Length != 0)
			{
				if (Length1.Length != 0)
					CompileLength += ":";
				CompileLength += Length2;
			}

			if (Length3.Length != 0)
			{
				if ((Length1.Length != 0) || (Length2.Length != 0))
					CompileLength += ":";
				CompileLength += Length3;
			}

			lengthArray = CompileLength.Split(':');

			//Calculate the seconds using fields in the lengthArray.
			try
			{
				//No input
				if (lengthArray.Length == 0)
				{
					seconds = 0;
				}
				//1 input, asume minutes
				else if (lengthArray.Length == 1)
				{
					if (lengthArray[0].Length != 0)
						seconds = Convert.ToInt32(lengthArray[0]) * 60;
					else
						seconds = 0;
				}
				//2 inputs, assume minutes, seconds
				else if (lengthArray.Length == 2)
				{
					seconds = seconds + Convert.ToInt32(lengthArray[0]) * 60;		//minutes
					seconds = seconds + Convert.ToInt32(lengthArray[1]);			//seconds

				}
				//3 inputs, assume hours, minutes, seconds
				else if (lengthArray.Length == 3)
				{
					seconds = Convert.ToInt32(lengthArray[0]) * 60 * 60;			//hours
					seconds = seconds + Convert.ToInt32(lengthArray[1]) * 60;		//minutes
					seconds = seconds + Convert.ToInt32(lengthArray[2]);			//seconds
				}
			}
			catch (Exception exc)
			{
				throw exc;
			}

			//done!
			return seconds;


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
