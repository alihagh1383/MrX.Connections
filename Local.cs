using FluentFTP;
using FluentFTP.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MrX.Connections
{
    public class Local
    {

        private string _rootPath;
        private string FilePath(string file) => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _rootPath, file);

        public Local(string root)
        {
            Console.WriteLine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, root));
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, root)))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, root));
            }
            _rootPath = root;
        }



        public bool DownloadFile(string fileName, out Stream file)
        {
            file = new MemoryStream();


            try
            {
                file = File.Open(FilePath(fileName), FileMode.OpenOrCreate);
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
                fileStream.Close();
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
            try
            {
                var f = File.Open(FilePath(fileName), (!overwrite) ? FileMode.OpenOrCreate : FileMode.Create);

                var WRITER = new StreamWriter(f);
                WRITER.Write(new StreamReader(file).ReadToEnd());
                WRITER.Flush();
                f.Flush();
                f.Close();
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

            try
            {
                File.Create(FilePath(fileName)).Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Exists(string fileName, out bool exist)
        {

            try
            {

                exist = File.Exists(FilePath(fileName));
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

            try
            {
                var d = Directory.CreateDirectory(FilePath(directory));
                d.Create();
                Console.WriteLine(d.FullName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(string fileName)
        {


            try
            {
                File.Delete(FilePath(fileName));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ListOfFiles(string dir, out string[] files)
        {


            try
            {
                files = Directory.GetFiles(FilePath(dir), "*", SearchOption.AllDirectories);

                return true;
            }
            catch
            {
                files = new string[0];
                return false;
            }
        }
    }
}
