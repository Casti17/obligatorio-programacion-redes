using Communication;
using Domain;
using Domain.DTO;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace GrpcService1.ServerProgram
{
    public class ServerHandler
    {
        private static bool _exit = false;
        private static readonly List<TcpClient> _clients = new List<TcpClient>();
        public LkDin lkdin;
        private FileCommsHandler communication;
        private TcpClient _clientTcp;
        private MainHelper _helper;
        private static readonly SettingsManager settingsManager = new SettingsManager();

        public async Task Start()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "logger",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                this._helper = new MainHelper();

                var tcpListener = this.StartSocketServer(channel);
                Console.WriteLine("Bienvenido al Sistema Server");
                Console.WriteLine("Opciones validas: ");
                Console.WriteLine("exit -> abandonar el programa");
                Console.WriteLine("Ingrese su opcion: ");
                while (!_exit)
                {
                    var userInput = Console.ReadLine();
                    switch (userInput)
                    {
                        case "exit":
                            _exit = true;
                            tcpListener.Stop();
                            foreach (var client in _clients)
                            {
                                client.Close();
                            }
                            break;

                        default:
                            Console.WriteLine("Opcion incorrecta ingresada");
                            break;
                    }
                }
            }
        }

        private TcpListener StartSocketServer(IModel channel)
        {
            var ipEndPoint = new IPEndPoint(
                IPAddress.Parse(settingsManager.ReadSettings(ServerConfig.serverIPconfigkey)),
                int.Parse(settingsManager.ReadSettings(ServerConfig.serverPortconfigkey)));

            var tcpListener = new TcpListener(ipEndPoint);

            tcpListener.Start(100);

            Task.Run(() => this.ListenForConnectionsAsync(tcpListener, channel));
            return tcpListener;
        }

        private async Task ListenForConnectionsAsync(TcpListener listenerTcp, IModel channel)
        {
            while (!_exit)
            {
                try
                {
                    var clientConnected = await listenerTcp.AcceptTcpClientAsync();
                    _clients.Add(clientConnected);
                    Console.WriteLine("Accepted new connection...");
                    var taskClient = new Task(async () => await this.HandleClientAsync(clientConnected, channel));
                    taskClient.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _exit = true;
                }
            }
            Console.WriteLine("Exiting....");
        }

        private async Task HandleClientAsync(TcpClient client, IModel channel)
        {
            this._clientTcp = client;
            this.communication = new FileCommsHandler(this._clientTcp);
            this.lkdin = LkDin.GetInstance();

            await using (var networkStream = this._clientTcp.GetStream())
            {
                while (!_exit)
                {
                    var headerLength = ProtocolConstants.CommandLength + ProtocolConstants.DataLength;
                    var buffer = new byte[headerLength];
                    try
                    {
                        await this.communication.ReceiveDataAsync(headerLength, buffer);
                        var header = new Header();
                        header.SplitHeaderProtocol(buffer);

                        switch (header.ICommand)
                        {
                            case Commands.CreateUser:

                                await this.CreateUser(header);

                                break;

                            case Commands.CreateWorkProfile:
                                await this.CreateWorkProfile(header);

                                break;

                            case Commands.AssociateImageToProfile:
                                await this.AssociateImage(header);

                                break;

                            case Commands.SearchExistingProfiles:
                                await this.SearchExistingProfilesAsync(header);

                                break;

                            case Commands.SearchProfile:
                                await this.SearchProfileAsync(header);
                                break;

                            case Commands.SendMessage:
                                await this.SendMessageAsync(header);
                                break;

                            case Commands.CheckInbox:
                                await this.CheckInbox(header);
                                break;

                            default:
                                _exit = true;
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"El Cliente cierra la conexion -> Message {e.Message}..");
                        return;
                    }
                }
            }

            this._clientTcp.Close();
        }

        private async Task AssociateImage(Header header)
        {
            var message = "";
            var bufferData = new byte[header.IDataLength];
            await this.communication.ReceiveDataAsync(header.IDataLength, bufferData);
            ProfileInfo receivedProfile = new ProfileInfo();
            receivedProfile.Decode(bufferData);
            message = this._helper.AssociateImageToProfile(receivedProfile.Username, receivedProfile.Imagen);
            FileStreamHandler fh = new FileStreamHandler();
            await this.communication.ReceiveFileAsync(fh);
            await this.communication.SendDataAsync(message, Commands.ServerResponse);
        }

        private async Task SearchExistingProfilesAsync(Header header)
        {
            var message = "";
            var bufferInfo = new byte[header.IDataLength];
            await this.communication.ReceiveDataAsync(header.IDataLength, bufferInfo);
            ProfileSearchInfo receivedFilter = new ProfileSearchInfo();
            receivedFilter.Decode(bufferInfo);
            message = this._helper.SearchFilters(receivedFilter.Username);

            await this.communication.SendDataAsync(message, Commands.ServerResponse);
        }

        private async Task CheckInbox(Header header)
        {
            try
            {
                var message = "";
                var bufferInfo = new byte[header.IDataLength];

                await this.communication.ReceiveDataAsync(header.IDataLength, bufferInfo);
                ProfileSearchInfo receivedProfile = new ProfileSearchInfo();
                receivedProfile.Decode(bufferInfo);
                message = this._helper.CheckInbox(receivedProfile.Username);

                await this.communication.SendDataAsync(message, Commands.ServerResponse);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task SendMessageAsync(Header header)
        {
            try
            {
                var message = "";
                var bufferInfo = new byte[header.IDataLength];
                await this.communication.ReceiveDataAsync(header.IDataLength, bufferInfo);
                SendMessageInfo receivedMessage = new SendMessageInfo();
                receivedMessage.Decode(bufferInfo);
                Message newMessage = receivedMessage.ToEntity();
                message = this._helper.SendMessage(receivedMessage.Sender, receivedMessage.Receiver, newMessage);

                await this.communication.SendDataAsync(message, Commands.ServerResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task SearchProfileAsync(Header header)
        {
            try
            {
                var message = "";
                var bufferInfo = new byte[header.IDataLength];
                await this.communication.ReceiveDataAsync(header.IDataLength, bufferInfo);
                ProfileSearchInfo receivedProfile = new ProfileSearchInfo();
                receivedProfile.Decode(bufferInfo);

                message = this._helper.SearchProfileAsync(receivedProfile.Username);
                if (message.Contains("exist"))
                {
                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                }
                else
                {
                    FileStreamHandler fh = new FileStreamHandler();
                    await this.communication.ReceiveFileAsync(fh);
                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task CreateWorkProfile(Header header)
        {
            try
            {
                var message = "";
                var bufferData = new byte[header.IDataLength];
                await this.communication.ReceiveDataAsync(header.IDataLength, bufferData);
                ProfileInfo receivedProfile = new ProfileInfo();
                receivedProfile.Decode(bufferData);
                message = this._helper.CreateWorkProfile(receivedProfile.Username, receivedProfile.Imagen, receivedProfile.Descripcion, receivedProfile.Skills);
                if (message.Contains("existe"))
                {
                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                }
                else
                {
                    FileStreamHandler fh = new FileStreamHandler();
                    await this.communication.ReceiveFileAsync(fh);
                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task CreateUser(Header header)
        {
            try
            {
                var message = "";
                var bufferData = new byte[header.IDataLength];
                await this.communication.ReceiveDataAsync(header.IDataLength, bufferData);

                UserInfo receivedInfo = new UserInfo();
                receivedInfo.Decode(bufferData);

                message = this._helper.CreateUser(receivedInfo.Name, receivedInfo.Surname, receivedInfo.Username);
                await this.communication.SendDataAsync(message, Commands.ServerResponse);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}