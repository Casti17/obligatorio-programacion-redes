using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Communication
{
    public class SocketHelper
    {
        private readonly Socket _socket;
        public bool Disconnect { get; set; }

        public List<Socket> Clients { get; set; }

        public SocketHelper()
        {
            this.Clients = new List<Socket>();
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
                var buffer = new byte[headerLength];
                try
                {
                    //ReceiveData(clientSocket, headerLength, buffer);
                    var header = new Header(buffer);
                    switch (header.ICommand)
                    {
                        /*case Commands.CreateUser:
                            this.userLogic*/
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
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