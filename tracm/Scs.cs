using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

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

    public class Scs
    {
        private string m_ServerAddress = "";
        private string m_Username = "";

        public Scs(string ServerAddress, string Username)
        {
            m_ServerAddress = ServerAddress;
            m_Username = Username;
        }

        public void addContent(string FileName)
        {
            string status = "fail";
            string data = "";

            try
            {
                data = getData("<?xml version ='1.0' encoding 'UTF-8'?><request><type>addContent</type><version_number>0.3</version_number><username>" + m_Username + "</username><filename>" + FileName + "</filename></request>");
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

        public List<int> getDownloadQueue()
        {
            //"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<response status=\"ok\">\n  <download_queue>\n    <content_id>10</content_id>\n    <content_id>11</content_id>\n  </download_queue>\n</response>\n"

            string status = "fail";
            string data = "";
            List<int> ids = new List<int>();

            try
            {
                data = getData("<?xml version ='1.0' encoding 'UTF-8'?><request><type>getDownloadQueue</type><version_number>0.3</version_number><username>" + m_Username + "</username></request>");
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

        public string getContentMetadata(string content_id)
        {
            string status = "fail";
            string data = "";

            try
            {
                data = getData("<?xml version ='1.0' encoding 'UTF-8'?><request><type>getContentMetadata</type><version_number>0.3</version_number><username>" + m_Username + "</username><content_id>" + content_id + "</content_id></request>");
            }
            catch { }
            
            if (status.ToLower() == "ok")
                return data;
            else
                throw new Exception("Error getting the download queue");
        }

        public string getQueuedDownloadUrl(string content_id)
        {
            string status = "fail";
            string url = "";
            string data = "";

            try
            {
                data = getData("<?xml version ='1.0' encoding 'UTF-8'?><request><type>getQueuedDownloadUrl</type><version_number>0.3</version_number><username>" + m_Username + "</username><content_id>" + content_id + "</content_id></request>");

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

        public void removeQueuedDownload(string content_id, bool Downloaded)
        {
            string status = "fail";
            string data = "";

            try
            {
                data = getData("<?xml version ='1.0' encoding 'UTF-8'?><request><type>removeQueuedDownload</type><version_number>0.3</version_number><username>" + m_Username + "</username><content_id>" + content_id + "</content_id><downloaded>" + Downloaded.ToString().ToLower() + "</downloaded></request>");
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

        private string getData(string postData)
        {
            string responseFromServer = "";
            StreamReader reader = null;
            Stream dataStream = null;
            WebResponse response = null;
            
            try
            {
                WebRequest request = WebRequest.Create("http://" + m_ServerAddress + "/acm_rest");
                ((HttpWebRequest)request).ServicePoint.Expect100Continue = false;
                request.Method = "POST";
                ((HttpWebRequest)request).UserAgent = "tracm client";
                ((HttpWebRequest)request).Accept = "text/xml";
                request.ContentType = "application/xml";
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = byteArray.Length;
                dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                
                response = request.GetResponse();
                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
            }
            catch(Exception e) 
            {
                Console.Write(e.Message.ToString());
            }
            finally
            {
                if(reader != null)
                    reader.Close();
                if (dataStream != null)
                    dataStream.Close();
                if (response != null)
                    response.Close();
            }
            return responseFromServer;
        }
    }
}
