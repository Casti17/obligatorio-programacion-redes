using System;
using System.Net.Sockets;

namespace Communication
{
    public class FileCommsHandler
    {
        private readonly ConversionHandler _conversionHandler;
        private readonly FileHandler _fileHandler;
        private readonly FileStreamHandler _fileStreamHandler;
        private readonly SocketHelper _socketHelper;

        public FileCommsHandler(Socket socket)
        {
            this._conversionHandler = new ConversionHandler();
            this._fileHandler = new FileHandler();
            this._fileStreamHandler = new FileStreamHandler();
            this._socketHelper = new SocketHelper(socket);
        }

        public void SendFile(string path)
        {
            if (this._fileHandler.FileExists(path))
            {
                var fileName = this._fileHandler.GetFileName(path);
                // ---> Enviar el largo del nombre del archivo
                this._socketHelper.Send(this._conversionHandler.ConvertIntToBytes(fileName.Length));
                // ---> Enviar el nombre del archivo
                this._socketHelper.Send(this._conversionHandler.ConvertStringToBytes(fileName));

                // ---> Obtener el tamaño del archivo
                long fileSize = this._fileHandler.GetFileSize(path);
                // ---> Enviar el tamaño del archivo
                var convertedFileSize = this._conversionHandler.ConvertLongToBytes(fileSize);
                this._socketHelper.Send(convertedFileSize);
                // ---> Enviar el archivo (pero con file stream)
                this.SendFileWithStream(fileSize, path);
            }
            else
            {
                throw new Exception("File does not exist");
            }
        }

        public void ReceiveFile()
        {
            // ---> Recibir el largo del nombre del archivo
            int fileNameSize = this._conversionHandler.ConvertBytesToInt(
                this._socketHelper.Receive(Protocol.FixedDataSize));
            // ---> Recibir el nombre del archivo
            string fileName = this._conversionHandler.ConvertBytesToString(this._socketHelper.Receive(fileNameSize));
            // ---> Recibir el largo del archivo
            long fileSize = this._conversionHandler.ConvertBytesToLong(
                this._socketHelper.Receive(Protocol.FixedFileSize));
            // ---> Recibir el archivo
            this.ReceiveFileWithStreams(fileSize, fileName);
        }

        private void SendFileWithStream(long fileSize, string path)
        {
            long fileParts = Protocol.CalculateFileParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == fileParts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = this._fileStreamHandler.Read(path, offset, lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = this._fileStreamHandler.Read(path, offset, Protocol.MaxPacketSize);
                    offset += Protocol.MaxPacketSize;
                }

                this._socketHelper.Send(data);
                currentPart++;
            }
        }

        private void ReceiveFileWithStreams(long fileSize, string fileName)
        {
            long fileParts = Protocol.CalculateFileParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == fileParts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = this._socketHelper.Receive(lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = this._socketHelper.Receive(Protocol.MaxPacketSize);
                    offset += Protocol.MaxPacketSize;
                }
                this._fileStreamHandler.Write(fileName, data);
                currentPart++;
            }
        }
    }
}