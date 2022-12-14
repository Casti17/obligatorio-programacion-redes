using System;
using System.IO;

namespace Communication
{
    public class FileHandler
    {
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string GetFileName(string path)
        {
            if (this.FileExists(path))
            {
                return new FileInfo(path).Name;
            }

            throw new Exception("File does not exist");
        }

        public long GetFileSize(string path)
        {
            if (this.FileExists(path))
            {
                return new FileInfo(path).Length;
            }

            throw new Exception("File does not exist");
        }

        public void DeleteFile(string path)
        {
            if (this.FileExists(path))
            {
                File.Delete(this.GetFileName(path));
            }
        }
    }
}