using System.Collections.Immutable;
using System.Net;
using System.Text;
using FluentFTP;

namespace MrX.Connections
{
    public class Ftp
    {
        private FtpClient _client;

        private string _rootPath;
        private string FilePath(string file) => (_rootPath + "/" + file);

        public Ftp(string hostName, string? userName, string? password, string root)
        {
            var credentials = new NetworkCredential(userName, password);
            _client = new FtpClient();
            _client.Host = hostName;
            _client.Credentials = credentials;
            _client.Connect();
            if (Exists(root, out bool ex) && !ex)
                CreateDirectory(root);
            _rootPath = root;
        }

        public bool Opened
        {
            get
            {
                if (_client.IsConnected) return _client.IsConnected;
                else return Open();
            }
        }

        public bool Open()
        {
            _client.Connect();
            return _client.IsConnected;
        }

        public bool DownloadFile(string fileName, out Stream file)
        {
            file = new MemoryStream();
            if (!Opened)
            {
                return false;
            }

            try
            {
                _client.DownloadStream(file, FilePath(fileName));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DownloadFile(string fileName, out string file)
        {
            if (!DownloadFile((fileName), out Stream fileStream))
            {
                file = "";
                return false;
            }

            try
            {
                fileStream.Position = 0;
                var streamReader = new StreamReader(fileStream);
                file = streamReader.ReadToEnd();
                return true;
            }
            catch
            {
                file = "";
                return false;
            }
        }

        public bool UpLoadFile(string fileName, Stream file, bool overwrite = true)
        {
            if (!Opened) return false;
            try
            {
                _client.UploadStream(file, FilePath(fileName),
                    (overwrite) ? FtpRemoteExists.Overwrite : FtpRemoteExists.Resume);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpLoadFile(string fileName, string fileText, bool overwrite = true)
        {
            var file = new MemoryStream(Encoding.UTF8.GetBytes(fileText));
            return UpLoadFile(fileName, file, overwrite);
        }

        public bool CreateFile(string fileName, bool overwrite = true)
        {
            if (!Opened) return false;
            try
            {
                _client.UploadStream(new MemoryStream(), FilePath(fileName),
                    (overwrite) ? FtpRemoteExists.Overwrite : FtpRemoteExists.Resume);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Exists(string fileName, out bool exist)
        {
            if (!Opened)
            {
                {
                    exist = false;
                    return false;
                }
            }

            try
            {
                exist = _client.FileExists(FilePath(fileName));
                return true;
            }
            catch
            {
                exist = false;
                return false;
            }
        }

        public bool CreateDirectory(string directory)
        {
            if (!Opened) return false;
            try
            {
                _client.CreateDirectory(FilePath(directory));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(string fileName)
        {
            if (!Opened)
            {
                {
                    return false;
                }
            }

            try
            {
                _client.DeleteFile(FilePath(fileName));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ListOfFiles(string dir, out FtpListItem[] files)
        {
            if (!Opened)
            {
                files = new FtpListItem[0];
                return false;
            }

            try
            {
                files = _client.GetListing(FilePath(dir));
                
                return true;
            }
            catch
            {
                files = new FtpListItem[0];
                return false;
            }
        }
    }
}