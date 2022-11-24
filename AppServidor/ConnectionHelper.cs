using Communication;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AppServidor
{
    public class ConnectionHelper
    {
        public bool Disconnect { get; set; }
        private MainHelper MainHelper { get; set; }
        public List<TcpClient> Clients { get; set; }

        public ConnectionHelper()
        {
            this.Clients = new List<TcpClient>();
            this.MainHelper = new MainHelper();

            this.Disconnect = false;
        }

        public async Task ListenClients(TcpListener tcpListener)
        {
            while (!this.Disconnect)
            {
                try
                {
                    var tcpClientSocket = await tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
                    this.Clients.Add(tcpClientSocket);
                    Console.WriteLine("Accepted new connection...");
                    Task.Run(async () => await this.ClientHandleAsync(tcpClientSocket).ConfigureAwait(false));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error when connecting, ", e.Message);
                    this.Disconnect = true;
                }
            }
            Console.WriteLine("Disconnecting. . .");
        }

        private async Task ClientHandleAsync(TcpClient clientSocket)
        {
            bool endConnection = false;
            while (!this.Disconnect && !endConnection)
            {
            }
        }

        public async void SendData(string message, NetworkStream networkStream, int command)
        {
            var header = new Header(command, message.Length);
            var data = header.GetRequest();
            await networkStream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);

            var bytesMessage = Encoding.UTF8.GetBytes(message);
            await networkStream.WriteAsync(bytesMessage, 0, bytesMessage.Length).ConfigureAwait(false);
        }

        public async Task ReceiveData(NetworkStream networkStream, int Length, byte[] buffer)
        {
            var offset = 0;
            while (offset < Length)
            {
                try
                {
                    var localRecv = await networkStream.ReadAsync(buffer, offset, Length - offset);
                    if (localRecv == 0) // Si recieve retorna 0 -> la conexion se cerro desde el endpoint remoto
                    {
                        if (!this.Disconnect)
                        {
                            networkStream.Close();
                        }
                        else
                        {
                            throw new Exception("Server is closing");
                        }
                    }

                    offset += localRecv;
                }
                catch (SocketException se)
                {
                    Console.WriteLine(se.Message);
                    throw new Exception("error");
                }
            }
        }

        public async Task<string> ReceiveTextData(Header header, NetworkStream networkStream)
        {
            var bufferData = new byte[header.IDataLength];
            await this.ReceiveData(networkStream, header.IDataLength, bufferData);
            return Encoding.UTF8.GetString(bufferData);
        }
    }
}