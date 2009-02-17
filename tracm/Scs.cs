using System;
using System.Collections.Generic;
using System.Text;
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

        public string addContent(string FileName)
        {
            return getData("<?xml version ='1.0' encoding 'UTF-8'?><request><type>addContent</type><version_number>0.3</version_number><username>" + m_Username + "</username><filename>" + FileName + "</filename></request>");
        }

        public string getDownloadQueue()
        {
            return getData("<?xml version ='1.0' encoding 'UTF-8'?><request><type>getDownloadQueue</type><version_number>0.3</version_number><username>" + m_Username + "</username></request>");
        }

        public string getContentMetadata(string content_id)
        {
            return getData("<?xml version ='1.0' encoding 'UTF-8'?><request><type>getDownloadQueue</type><version_number>0.3</version_number><username>" + m_Username + "</username><content_id>" + content_id + "</content_id></request>");
        }

        public string getQueuedDownloadUrl(string content_id)
        {
            return getData("<?xml version ='1.0' encoding 'UTF-8'?><request><type>getDownloadQueue</type><version_number>0.3</version_number><username>" + m_Username + "</username><content_id>" + content_id + "</content_id></request>");
        }

        public string removeQueuedDownload(string content_id)
        {
            return getData("<?xml version ='1.0' encoding 'UTF-8'?><request><type>getDownloadQueue</type><version_number>0.3</version_number><username>" + m_Username + "</username><content_id>" + content_id + "</content_id></request>");
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
                string error = e.Message.ToString();
            }
            finally
            {
                reader.Close();
                dataStream.Close();
                response.Close();
            }
            return responseFromServer;
        }
    }
}
