using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using tracm.Properties;

namespace tracm
{
    public enum ScsErrors
    {
        REQUEST_INVALID,
        VERSION_INVALID,
        FILE_NOT_FOUND,
        FILE_INVALID,
        FILE_EXISTS,
        XML_NOT_FOUND,
        XML_INVALID,
        USER_NOT_FOUND,
        INTERNAL_ERROR
    }

    public static class Scs
    {
        public static void addContent(string FileName)
        {
            string status = "fail";
            string data = String.Empty;

            try
            {
				data = getData(String.Format("<?xml version ='1.0' encoding 'UTF-8'?><request><type>addContent</type><version_number>0.3</version_number><username>{0}</username><filename>{1}</filename></request>", Settings.Default.ACMUsername, FileName));
                Match m = Regex.Match(data, "response status=\"([^\"]*)");
                if (m.Success)
                    status = m.Groups[1].Value;
            }
            catch { }

            if (status.ToLower() == "ok")
                return;
            else
                throw new Exception("Error adding content");
        }

		public static List<int> getDownloadQueue()
        {
            //"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<response status=\"ok\">\n  <download_queue>\n    <content_id>10</content_id>\n    <content_id>11</content_id>\n  </download_queue>\n</response>\n"

            string status = "fail";
			string data = String.Empty;
            List<int> ids = new List<int>();

            try
            {
				data = getData(String.Format("<?xml version ='1.0' encoding 'UTF-8'?><request><type>getDownloadQueue</type><version_number>0.3</version_number><username>{0}</username></request>", Settings.Default.ACMUsername));
                Match m = Regex.Match(data, "response status=\"([^\"]*)");
                if (m.Success)
                    status = m.Groups[1].Value;
                MatchCollection contentIDs = Regex.Matches(data, "<content_id>([^<]*)");
                foreach (Match contentID in contentIDs)
                    ids.Add(Convert.ToInt32(contentID.Groups[1].Value));
            }
            catch { }

            if (status.ToLower() == "ok")
                return ids;
            else
                throw new Exception("Error getting the download queue");
        }

		public static string getContentMetadata(string content_id)
        {
            string status = "fail";
			string data = String.Empty;

            try
            {
				data = getData(String.Format("<?xml version ='1.0' encoding 'UTF-8'?><request><type>getContentMetadata</type><version_number>0.3</version_number><username>{0}</username><content_id>{1}</content_id></request>", Settings.Default.ACMUsername, content_id));
            }
            catch { }
            
            if (status.ToLower() == "ok")
                return data;
            else
                throw new Exception("Error getting the download queue");
        }

		public static string getQueuedDownloadUrl(string content_id)
        {
            string status = "fail";
			string url = String.Empty;
			string data = String.Empty;

            try
            {
				data = getData(String.Format("<?xml version ='1.0' encoding 'UTF-8'?><request><type>getQueuedDownloadUrl</type><version_number>0.3</version_number><username>{1}</username><content_id>{1}</content_id></request>", Settings.Default.ACMUsername, content_id));

                Match m = Regex.Match(data, "response status=\"([^\"]*)");
                if (m.Success)
                    status = m.Groups[1].Value;

                m = Regex.Match(data, "<url>([^<]*)");
                if (m.Success)
                    url = m.Groups[1].Value;
            }
            catch { }

            if (status.ToLower() == "ok")
                return url;
            else
                throw new Exception("Error getting the download url");
        }

		public static void removeQueuedDownload(string content_id, bool Downloaded)
        {
            string status = "fail";
			string data = String.Empty;

            try
            {
				data = getData(String.Format("<?xml version ='1.0' encoding 'UTF-8'?><request><type>removeQueuedDownload</type><version_number>0.3</version_number><username>{0}</username><content_id>{1}</content_id><downloaded>{2}</downloaded></request>", Settings.Default.ACMUsername, content_id, Downloaded.ToString().ToLower()));
                Match m = Regex.Match(data, "response status=\"([^\"]*)");
                if (m.Success)
                    status = m.Groups[1].Value;
            }
            catch { }

            if (status.ToLower() == "ok")
                return;
            else
                throw new Exception("Error removing the file from the queue");
        }

		private static string getData(string postData)
        {
			string responseFromServer = String.Empty;

			try
			{
				WebRequest request = WebRequest.Create(String.Format("http://{0}/acm_rest", Settings.Default.ACMServer));
				((HttpWebRequest)request).ServicePoint.Expect100Continue = false;
				request.Method = "POST";
				((HttpWebRequest)request).UserAgent = "tracm/1.0";
				((HttpWebRequest)request).Accept = "text/xml";
				request.ContentType = "application/xml";

				byte[] byteArray = Encoding.UTF8.GetBytes(postData);
				request.ContentLength = byteArray.Length;
				using (Stream writeStream = request.GetRequestStream())
				{
					writeStream.Write(byteArray, 0, byteArray.Length);
					writeStream.Close();
				}

				using (WebResponse response = request.GetResponse())
				using (Stream readStream = response.GetResponseStream())
				using (StreamReader reader = new StreamReader(readStream))
					responseFromServer = reader.ReadToEnd();
			}
			catch (Exception e)
			{
				Console.Write(e.Message.ToString());
			}
            return responseFromServer;
        }
    }
}
