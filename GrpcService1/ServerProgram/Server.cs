using Communication;
using Domain;
using Domain.DTO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace GrpcService1.ServerProgram
{
    public class Server
    {
        private static bool _exit = false;
        private static readonly List<TcpClient> _clients = new List<TcpClient>();
        public LkDin lkdin;
        private FileCommsHandler communication;
        private TcpClient _clientTcp;
        private static readonly SettingsManager settingsManager = new SettingsManager();

        public async Task StartAsync()
        {
            Console.WriteLine("Server is starting...");
            var ipEndPoint = new IPEndPoint(
                IPAddress.Parse(settingsManager.ReadSettings(ServerConfig.serverIPconfigkey)),
                int.Parse(settingsManager.ReadSettings(ServerConfig.serverPortconfigkey)));
            var tcpListener = new TcpListener(ipEndPoint);
            tcpListener.Start(100);
            Task.Run(() => this.ListenForConnectionsAsync(tcpListener));
            Console.WriteLine("Server started listening connections on {0}-{1}", settingsManager.ReadSettings(ServerConfig.serverIPconfigkey), settingsManager.ReadSettings(ServerConfig.serverPortconfigkey));

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

        private async Task ListenForConnectionsAsync(TcpListener listenerTcp)
        {
            while (!_exit)
            {
                try
                {
                    var clientConnected = await listenerTcp.AcceptTcpClientAsync();
                    _clients.Add(clientConnected);
                    Console.WriteLine("Accepted new connection...");
                    var taskClient = new Task(async () => await this.HandleClientAsync(clientConnected));
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

        private async Task HandleClientAsync(TcpClient client)
        {
            this._clientTcp = client;
            this.communication = new FileCommsHandler(this._clientTcp);
            this.lkdin = LkDin.GetInstance();

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

        private async Task CheckInbox(Header header)
        {
            var message = "";
            var bufferInfo = new byte[header.IDataLength];

            await this.communication.ReceiveDataAsync(header.IDataLength, bufferInfo);
            ProfileSearchInfo receivedProfile = new ProfileSearchInfo();
            receivedProfile.Decode(bufferInfo);
            User user = this.lkdin.Users.Find(u => u.Username.Equals(receivedProfile.Username));
            if (user != null)
            {
                List<Message> messagesToSend = user.MessageBox;
                if (messagesToSend.Count == 0)
                {
                    message = user.Username + " No tiene mensajes para mostrar";
                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                }
                else
                {
                    message = this.lkdin.SendStringListOfMessages(messagesToSend, user);
                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                }
            }
            else
            {
                message = "No existe ese usuario.";
                await this.communication.SendDataAsync(message, Commands.ServerResponse);
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
                User receiver = this.lkdin.Users.Find(u => u.Username.Equals(receivedMessage.Receiver));
                if (receiver != null)
                {
                    receiver.MessageBox.Add(newMessage);
                    message = $"El mensaje fue enviado al usuario {receiver.Username} correctamente.";
                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                }
                else
                {
                    message = "No existe dicho usuario receptor.";
                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                }
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
                WorkProfile found = this.lkdin.WorkProfiles.Find(prof => prof.UserName.Equals(receivedProfile.Username));
                Console.WriteLine(receivedProfile.Username);
                if (found != null)
                {
                    message = "Se encontro el perfil, descargando imagen...";
                    FileStreamHandler fh = new FileStreamHandler();

                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                    await this.communication.SendFileAsync(found.ProfilePic, fh);
                }
                else
                {
                    message = "Ese perfil no existe.";
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
                WorkProfile newProfile = receivedProfile.ToEntity();
                this.lkdin.WorkProfiles.Add(newProfile);
                FileStreamHandler fh = new FileStreamHandler();
                await this.communication.ReceiveFileAsync(fh);
                WorkProfile x = this.lkdin.WorkProfiles.Find(n => n.UserName.Equals(newProfile.UserName));
                message = $"El perfil de  {x.UserName} se agrego correctamente.";
                await this.communication.SendDataAsync(message, Commands.ServerResponse);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task CreateUser(Header header)
        {
            try
            {
                var bufferData = new byte[header.IDataLength];
                await this.communication.ReceiveDataAsync(header.IDataLength, bufferData);
                UserInfo receivedInfo = new UserInfo();
                receivedInfo.Decode(bufferData);
                User newUser = receivedInfo.ToEntity();

                this.lkdin.Users.Add(newUser);
                User x = this.lkdin.Users.Find(n => n.Username.Equals(newUser.Username));
                var message = $"El usuario {x.Username} se agrego correctamente.";
                await this.communication.SendDataAsync(message, Commands.ServerResponse);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}