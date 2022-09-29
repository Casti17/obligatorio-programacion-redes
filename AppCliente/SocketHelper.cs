using Communication;
using System;
using System.Net.Sockets;
using System.Text;

namespace AppCliente
{
    public class SocketHelper
    {
        public static class ConnectionHelper
        {
            public static void SendData(string message, Socket socket, int command)
            {
                var header = new Header(command, message.Length);
                var data = header.GetRequest();
                var sentBytes = 0;
                while (sentBytes < data.Length)
                {
                    sentBytes += socket.Send(data, sentBytes, data.Length - sentBytes, SocketFlags.None);
                }

                sentBytes = 0;
                var bytesMessage = Encoding.UTF8.GetBytes(message);
                while (sentBytes < bytesMessage.Length)
                {
                    sentBytes += socket.Send(bytesMessage, sentBytes, bytesMessage.Length - sentBytes,
                        SocketFlags.None);
                }
            }

            public static void ReceiveData(Socket socket, int length, byte[] buffer)
            {
                var iRecv = 0;
                while (iRecv < length)
                {
                    try
                    {
                        var localRecv = socket.Receive(buffer, iRecv, length - iRecv, SocketFlags.None);
                        if (localRecv == 0) // Si recieve retorna 0 -> la conexion se cerro desde el endpoint remoto
                        {
                            socket.Shutdown(SocketShutdown.Both);
                            socket.Close();
                        }

                        iRecv += localRecv;
                    }
                    catch (SocketException se)
                    {
                        Console.WriteLine(se.Message);
                        return;
                    }
                }
            }

            public static Header ReceiveHeader(Socket socket)
            {
                var headerLength = ProtocolConstants.CommandLength + ProtocolConstants.DataLength;
                var buffer = new byte[headerLength];
                ReceiveData(socket, headerLength, buffer);
                var header = new Header();
                header.SplitHeaderProtocol(buffer);
                return header;
            }

            public static string ReceiveHeaderTextData(Socket socket, Header header)
            {
                var bufferData = new byte[header.IDataLength];
                ReceiveData(socket, header.IDataLength, bufferData);
                return Encoding.UTF8.GetString(bufferData);
            }

            public static void SendTextRequest(Socket socket, string message, int command)
            {
                var requestStatus = Status.Pending;
                do
                {
                    SendData(message, socket, command);
                    var header = ReceiveHeader(socket);
                    var statusResponseMessage = new byte[header.IDataLength];
                    switch (header.ICommand)
                    {
                        case Commands.Ok:
                            requestStatus = Status.Ok;
                            break;

                        case Commands.Failed:
                            var errorMessage = ReceiveHeaderTextData(socket, header);
                            throw new Exception("Problem with header");
                        default:
                            requestStatus = Status.Failed;
                            break;
                    }

                    ReceiveData(socket, header.IDataLength, statusResponseMessage);
                } while (requestStatus != Status.Ok);
            }
        }
    }
}