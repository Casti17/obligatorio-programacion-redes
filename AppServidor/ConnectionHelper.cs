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
                    //client.Start();
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
                /*var headerLength = ProtocolConstants.CommandLength + ProtocolConstants.DataLength;
                try
                {
                    byte[] buffer = this.Receive(ProtocolConstants.CommandLength + ProtocolConstants.DataLength);
                    var header = new Header(buffer);
                    byte[] bufferData = this.Receive(header.IDataLength);
                    var data = Encoding.UTF8.GetString(bufferData);
                    string[] dataArray;
                    switch (header.ICommand)
                    {
                        case Commands.CreateUser:
                            dataArray = data.Split('#');
                            this.MainHelper.CreateUser(dataArray[0], dataArray[1], dataArray[2]);
                            break;

                        case Commands.CreateWorkProfile:
                            dataArray = data.Split('#');
                            this.MainHelper.CreateWorkProfile(dataArray[0], dataArray[1], dataArray[2], dataArray[3]);
                            break;

                        case Commands.AssociateImageToProfile:
                            dataArray = data.Split('#');
                            this.MainHelper.AssociateImageToProfile(dataArray[0], dataArray[1]);
                            break;

                        case Commands.SearchExistingProfiles:
                            dataArray = data.Split('#');
                            List<WorkProfile> allFoundProfiles = this.MainHelper.SearchFilters(dataArray);
                            foreach (WorkProfile wrkProfile in allFoundProfiles)
                            {
                                Console.WriteLine(wrkProfile.UserName);
                            }
                            break;

                        case Commands.SearchProfile:
                            WorkProfile wp = this.MainHelper.Search(data);
                            if (wp == null)
                            {
                                Console.WriteLine("No profile was found");
                            }
                            else
                            {
                                Console.WriteLine(wp.ProfilePic);
                            }

                            break;

                        case Commands.SendMessage:
                            dataArray = data.Split('#');
                            this.MainHelper.SendMessage(dataArray[0], dataArray[1], dataArray[2]);
                            break;

                        case Commands.CheckInbox:
                            List<Message> messages = this.MainHelper.CheckInbox(data);
                            foreach (Message m in messages)
                            {
                                Console.WriteLine($"{m.Sender} sent the following message: {m.MessageBody}");
                            }
                            break;

                        default:
                            endConnection = true;
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("error", e.Message);
                }
            }*/
            }
        }

        /*public void SendRequest(Request protocol)
        {
            this.Send(protocol.Header.GetRequest()); //agarro command y datalength y envio
            this.Send(Encoding.UTF8.GetBytes(protocol.Body)); //agarro la data y envio
        }*/

        public async void SendData(string message, NetworkStream networkStream, int command)
        {
            var header = new Header(command, message.Length);
            var data = header.GetRequest();
            await networkStream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);

            var bytesMessage = Encoding.UTF8.GetBytes(message);
            await networkStream.WriteAsync(bytesMessage, 0, bytesMessage.Length).ConfigureAwait(false);
        }

        /*public void Send(byte[] data)
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
            }*/

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

        /*public byte[] Receive(int length)
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
        }*/
    }
}