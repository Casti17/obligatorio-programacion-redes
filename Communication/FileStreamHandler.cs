using System;
using System.IO;
using System.Threading.Tasks;

namespace Communication
{
    public class FileStreamHandler
    {
        private readonly FileHandler _fileHandler;

        public FileStreamHandler()
        {
            this._fileHandler = new FileHandler();
        }

        public async Task<byte[]> ReadDataAsync(string path, long offset, int length)
        {
            if (this._fileHandler.FileExists(path))
            {
                var data = new byte[length];

                await using var fs = new FileStream(path, FileMode.Open) { Position = offset };
                var bytesRead = 0;
                while (bytesRead < length)
                {
                    var read = await fs.ReadAsync(data, bytesRead, length - bytesRead);
                    if (read == 0)
                    {
                        throw new Exception("Error reading file");
                    }

                    bytesRead += read;
                }

                return data;
            }

            throw new Exception("File does not exist");
        }

        public async Task WriteData(string fileName, byte[] data)
        {
            if (File.Exists(fileName))
            {
                await using FileStream fileStream = new FileStream(fileName, FileMode.Append);
                await fileStream.WriteAsync(data, 0, data.Length);
            }
            else
            {
                await using FileStream fileStream = new FileStream(fileName, FileMode.Create);
                await fileStream.WriteAsync(data, 0, data.Length);
            }
        }
    }
}