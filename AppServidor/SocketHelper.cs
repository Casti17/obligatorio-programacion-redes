using Communication;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AppServidor
{
    public class SocketHelper
    {
        private readonly Socket _socket;
        public bool Disconnect { get; set; }
        private MainHelper MainHelper { get; set; }
        public List<Socket> Clients { get; set; }

        public SocketHelper()
        {
            this.Clients = new List<Socket>();
            this.MainHelper = new MainHelper();
        }

        public SocketHelper(Socket socket)
        {
            this._socket = socket;
        }

        public void ListenClients(Socket socketServer)
        {
            while (!this.Disconnect)
            {
                try
                {
                    var clientConnected = socketServer.Accept();
                    this.Clients.Add(clientConnected);
                    Console.WriteLine("Accepted new connection!");
                    var client = new Thread(() => this.ClientHandle(clientConnected));
                    client.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error when connecting, ", e.Message);
                    this.Disconnect = true;
                }
            }
            Console.WriteLine("Disconnecting. . .");
        }

        private void ClientHandle(Socket clientSocket)
        {
            bool endConnection = false;
            while (!this.Disconnect && !endConnection)
            {
                var headerLength = ProtocolConstants.CommandLength + ProtocolConstants.DataLength;
                try
                {
                    byte[] buffer = this.Receive(ProtocolConstants.CommandLength + ProtocolConstants.DataLength);
                    var header = new Header(buffer);
                    byte[] bufferData = this.Receive(header.IDataLength);
                    var data = Encoding.UTF8.GetString(bufferData);
                    string[] dataArray;
                    //this.ReceiveData(clientSocket, headerLength, buffer);
                    //var header = new Header(buffer);
                    //byte[] buffer = this.socketHelper.Receive(ProtocolConstants.CommandLength + ProtocolConstants.DataLength);

                    switch (header.ICommand)
                    {
                        case Commands.CreateUser:
                            dataArray = data.Split('#');
                            this.MainHelper.CreateUser(dataArray[0], dataArray[1], dataArray[2]);
                            Console.WriteLine("Hola");
                            break;

                        case Commands.CreateWorkProfile:
                            dataArray = data.Split('#');
                            this.MainHelper.CreateWorkProfile(dataArray[0], dataArray[1], dataArray[2], dataArray[3]);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("error", e.Message);
                }
            }
        }

        public void SendRequest(Request protocol)
        {
            this.Send(protocol.Header.GetRequest()); //agarro command y datalength y envio
            this.Send(Encoding.UTF8.GetBytes(protocol.Body)); //agarro la data y envio
        }

        public void Send(byte[] data)
        {
            int offset = 0;
            while (offset < data.Length)
            {
                var sent = this._socket.Send(
                    data,
                    offset,
                    data.Length - offset,
                    SocketFlags.None);
                if (sent == 0)
                {
                    throw new Exception("Connection lost");
                }

                offset += sent;
            }
        }

        public void ReceiveData(Socket clientSocket, int Length, byte[] buffer)
        {
            var offset = 0;
            while (offset < Length)
            {
                try
                {
                    var localRecv = clientSocket.Receive(buffer, offset, Length - offset, SocketFlags.None);
                    if (localRecv == 0) // Si recieve retorna 0 -> la conexion se cerro desde el endpoint remoto
                    {
                        if (!this.Disconnect)
                        {
                            clientSocket.Shutdown(SocketShutdown.Both);
                            clientSocket.Close();
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
                    return;
                }
            }
        }

        public byte[] Receive(int length)
        {
            int offset = 0;
            var data = new byte[length];
            while (offset < length)
            {
                var received = this._socket.Receive(
                    data,
                    offset,
                    length - offset,
                    SocketFlags.None);
                if (received == 0)
                {
                    throw new Exception("Connection lost");
                }

                offset += received;
            }

            return data;
        }
    }
}