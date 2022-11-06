using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public class FileCommsHandler
    {
        private readonly ConversionHandler _conversionHandler;
        private readonly FileHandler _fileHandler;
        private readonly FileStreamHandler _fileStreamHandler;
        private readonly SocketHelper _socketHelper;
        private readonly TcpClient tcpClient;

        public FileCommsHandler(TcpClient tcp)
        {
            this.tcpClient = tcp;
            this._conversionHandler = new ConversionHandler();
            this._fileHandler = new FileHandler();
            this._fileStreamHandler = new FileStreamHandler();
            this._socketHelper = new SocketHelper();
        }

        public async Task SendDataAsync(string sentData, int command)
        {
            {
                var header = new Header(command, sentData.Length);
                var data = header.GetRequest();
                var sentBytes = 0;
                while (sentBytes < data.Length)
                {
                    int size = data.Length - sentBytes;
                    await this.tcpClient.GetStream().WriteAsync(data, sentBytes, size);
                    sentBytes += size;
                }
                sentBytes = 0;
                var bytesMessage = Encoding.UTF8.GetBytes(sentData);
                while (sentBytes < bytesMessage.Length)
                {
                    int size = bytesMessage.Length - sentBytes;
                    await this.tcpClient.GetStream().WriteAsync(bytesMessage, sentBytes, size);
                    sentBytes += size;
                }
            }
        }

        public async Task<string> RecieveMessageAsync()
        {
            var headerLength = ProtocolConstants.CommandLength + ProtocolConstants.DataLength;
            var buffer = new byte[headerLength];
            await this.ReceiveDataAsync(headerLength, buffer);
            Header header = new Header();
            header.SplitHeaderProtocol(buffer);
            var bufferDataUser = new byte[header.IDataLength];
            await this.ReceiveDataAsync(header.IDataLength, bufferDataUser);
            string recMessage = Encoding.UTF8.GetString(bufferDataUser);
            Console.WriteLine(recMessage);
            return recMessage;
        }

        public async Task<byte[]> ReceiveDataAsync(int length, byte[] buffer)
        {
            var streamReciever = this.tcpClient.GetStream();
            int offset = 0;
            while (offset < length)
            {
                int read = await streamReciever.ReadAsync(buffer, offset, length - offset);
                if (read == 0)
                {
                    throw new SocketException();
                }

                offset += read;
            }

            return buffer;
        }

        public async Task<byte[]> ReceiveDataFileAsync(int length)
        {
            var stream = this.tcpClient.GetStream();
            int offset = 0;
            byte[] response = new byte[length];
            while (offset < length)
            {
                int received = await stream.ReadAsync(response, offset, length - offset);
                if (received == 0)
                {
                    throw new SocketException();
                }

                offset += received;
            }
            return response;
        }

        public async Task SendDataFileAsync(byte[] data)
        {
            var streamToSend = this.tcpClient.GetStream();
            int offset = 0;
            int size = data.Length;
            while (offset < data.Length)
            {
                int sent = size;
                await streamToSend.WriteAsync(data, offset, size - offset);
                if (sent == 0)
                {
                    throw new SocketException();
                }
                offset += sent;
            }
        }

        public async Task SendFileAsync(string path, FileStreamHandler fileStreamHandler)
        {
            var fileInfo = new FileInfo(path);
            string fileName = fileInfo.Name;
            byte[] fileNameData = Encoding.UTF8.GetBytes(fileName);
            int fileNameLength = fileNameData.Length;
            byte[] fileNameLengthData = BitConverter.GetBytes(fileNameLength);
            await this.SendDataFileAsync(fileNameLengthData);
            await this.SendDataFileAsync(fileNameData);
            long fileSize = fileInfo.Length;
            byte[] fileSizeDataLength = BitConverter.GetBytes(fileSize);
            await this.SendDataFileAsync(fileSizeDataLength);
            await this.FileSenderAsync(fileSize, path, fileStreamHandler);
        }

        public async Task ReceiveFileAsync(FileStreamHandler fileStreamHandler)
        {
            byte[] fileNameLengthData = await this.ReceiveDataFileAsync(Protocol.FixedDataSize);
            int fileNameLength = BitConverter.ToInt32(fileNameLengthData);
            byte[] fileNameData = await this.ReceiveDataFileAsync(fileNameLength);
            string fileName = Encoding.UTF8.GetString(fileNameData);
            byte[] fileSizeDataLength = await this.ReceiveDataFileAsync(Protocol.FixedFileSize);
            long fileSize = BitConverter.ToInt64(fileSizeDataLength);
            await this.FileReceiverAsync(fileSize, fileName, fileStreamHandler);
        }

        private async Task FileSenderAsync(long fileSize, string path, FileStreamHandler fileStreamHandler)
        {
            long fileParts = Protocol.CalculateFileParts(fileSize);
            long offset = 0;
            long currentPart = 1;
            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart != fileParts)
                {
                    data = await fileStreamHandler.ReadDataAsync(path, offset, Protocol.MaxPacketSize);
                    offset += Protocol.MaxPacketSize;
                }
                else
                {
                    int lastPartSize = (int)(fileSize - offset);
                    data = await fileStreamHandler.ReadDataAsync(path, offset, lastPartSize);
                    offset += lastPartSize;
                }

                await this.SendDataFileAsync(data);
                currentPart++;
            }
        }

        private async Task FileReceiverAsync(long fileSize, string fileName, FileStreamHandler fileStreamHandler)
        {
            long fileParts = Protocol.CalculateFileParts(fileSize);
            long offset = 0;
            long currentPart = 1;
            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart != fileParts)
                {
                    data = await this.ReceiveDataFileAsync(Protocol.MaxPacketSize);
                    offset += Protocol.MaxPacketSize;
                }
                else
                {
                    int lastPartSize = (int)(fileSize - offset);
                    data = await this.ReceiveDataFileAsync(lastPartSize);
                    offset += lastPartSize;
                }
                await fileStreamHandler.WriteData(fileName, data);
                currentPart++;
            }
        }
    }
}