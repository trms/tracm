using System;
using System.Net;
using System.IO;
using System.ComponentModel;

namespace tracm
{
    public class ProgressEventArgs : EventArgs
    {
        public long TotalBytesDownloaded { get; private set; }
        public int BytesReceived { get; private set; }
        public long TotalBytes { get; private set; }

        public ProgressEventArgs(long totalBytesDownloaded, int bytesRecieved, long totalBytes)
        {
            this.TotalBytesDownloaded = totalBytesDownloaded;
            this.BytesReceived = bytesRecieved;
            this.TotalBytes = totalBytes;
        }
    }

    internal class RestartableDownload
    {
        private Uri _uri;
        private string _destFile;
        private FileStream _writeStream;
        private Stream _readStream;
        private long _length = 0;
        private long _downloaded =0;

        public delegate void ProgressUpdateHandler(object sender, ProgressEventArgs e);
        public event ProgressUpdateHandler OnProgressUpdate;

        private void ProgressUpdate(int bytesReceived)
        {
            if (OnProgressUpdate == null)
                return;
            var args = new ProgressEventArgs(_downloaded, bytesReceived, _length);
            OnProgressUpdate(this, args);
        }

        public long Length
        { get { return _length; } }

        public long BytesDownloaded
        { get { return _downloaded; } }

        public bool DownloadComplete
        { get { return (_length > 0 && _downloaded == _length); } }

        internal RestartableDownload(string uri, string destinationFile)
        {
            _uri = new Uri(uri);
            _destFile = destinationFile;
        }

        internal void StartDownload()
        {
            if (_uri.Scheme.Equals("http"))
            {
                try
                {
                    long start = OpenWriteStream();
                    _length = GetContentLength();
                    if (start < _length)
                    {
                        OpenReadStream(start, _length);
                        Copy();
                    }
                }
                catch (System.Exception ex)
                {
                    LogHelper.Logger.Error(String.Format("Error starting download for file {0}.", _uri.AbsolutePath));
                    throw new Exception("Error starting download");
                }
                finally
                {
                    if (_writeStream != null)
                        _writeStream.Close();
                    if (_readStream != null)
                        _readStream.Close();
                }
            }
        }

        private long OpenWriteStream()
        {
            _writeStream = new FileStream(_destFile, FileMode.Append, FileAccess.Write);
            return _writeStream.Length;
        }

        private long GetContentLength()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            long length = response.ContentLength;
            response.Close();
            return length;
        }

        private void OpenReadStream(long start, long length)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_uri);
            request.AddRange(start, length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.ContentLength == length)
            {
                _writeStream.Seek(0, SeekOrigin.Begin);
            }
            _readStream = response.GetResponseStream();
        }

        private void Copy()
        {
            byte[] buffer = new byte[1024];
            int count = _readStream.Read(buffer, 0, buffer.Length);
            while (count > 0)
            {

                _downloaded += count;

                _writeStream.Write(buffer, 0, count);
                _writeStream.Flush();
                count = _readStream.Read(buffer, 0, buffer.Length);

                ProgressUpdate(count);
            }
        }
    }
}