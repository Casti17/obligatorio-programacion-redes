﻿using Communication;
using Domain;
using Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using RabbitMQ.Client;

namespace AppServidor
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

        public async Task StartAsync()
        {
            Console.WriteLine("Server is starting...");
            var ipEndPoint = new IPEndPoint(
                IPAddress.Parse(settingsManager.ReadSettings(ServerConfig.serverIPconfigkey)),
                int.Parse(settingsManager.ReadSettings(ServerConfig.serverPortconfigkey)));
            var tcpListener = new TcpListener(ipEndPoint);
            tcpListener.Start(100);
            MainHelper helper = new MainHelper();
            this._helper = helper;
            var factory = new ConnectionFactory(){HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "weather",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                Task.Run(() => this.ListenForConnectionsAsync(tcpListener, channel));
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

                                await this.CreateUser(header, channel);

                                break;

                            case Commands.CreateWorkProfile:
                                await this.CreateWorkProfile(header, channel);

                                break;

                            case Commands.AssociateImageToProfile:

                                break;

                            case Commands.SearchExistingProfiles:
                                await this.SearchExistingProfilesAsync(header, channel);

                                break;

                            case Commands.SearchProfile:
                                await this.SearchProfileAsync(header, channel);
                                break;

                            case Commands.SendMessage:
                                await this.SendMessageAsync(header, channel);
                                break;

                            case Commands.CheckInbox:
                                await this.CheckInbox(header, channel);
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
        }

        private async Task CheckInbox(Header header, IModel channel)
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
                    message = user.Username + $"{receivedProfile.Username} no tiene mensajes en su buzon";
                    Message(channel, message + " [inbox]");
                    Console.WriteLine(message);
                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                }
                else
                {
                    var message1 = $"{receivedProfile.Username} accedio a su buzon";
                    Message(channel, message1 + " [inbox]");
                    Console.WriteLine(message);
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

        private async Task SearchExistingProfilesAsync(Header header, IModel channel)
        {
            string profiles = lkdin.GetProfilesString();
            var message1 = $"Se consultaron todos los perfiles de trabajo";
            Message(channel, message1 + " [search]");
            Console.WriteLine(message1);
            await this.communication.SendDataAsync(profiles, Commands.ServerResponse);
        }

        private async Task SendMessageAsync(Header header, IModel channel)
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
                    Message(channel, message + " [message]");
                    Console.WriteLine(message);
                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                }
                else
                {
                    message = "No existe dicho usuario receptor.";
                    Message(channel, message + " [message]");
                    Console.WriteLine(message);
                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private static string Message(IModel channel, string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "",
                routingKey: "weather",
                basicProperties: null,
                body: body);
            return message;
        }

        private async Task SearchProfileAsync(Header header, IModel channel)
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
                    message = $"Se encontro el perfil de {receivedProfile.Username}, descargando imagen...";
                    FileStreamHandler fh = new FileStreamHandler();
                    Message(channel, message + " [search]");
                    Console.WriteLine(message);
                    message += $"\nUserName: {found.UserName}\n" +
                               $"Description: {found.Description}\n" +
                               $"Skills: {found.Skills.ToString()}";
                              
                    //agregar mostrar los datos del perfil
                    
                    

                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                    await this.communication.SendFileAsync(found.ProfilePic, fh);
                }
                else
                {
                    message = $"El perfil de {receivedProfile.Username} no existe.";
                    Message(channel, message + " [search]");
                    Console.WriteLine(message);
                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                }
            }
            catch (Exception e)
            {
                var message = $"Ocurrio un error en la operacion.";
                Message(channel, message + " [search]");
                Console.WriteLine(message);
                await this.communication.SendDataAsync(message, Commands.ServerResponse);
            }
        }

        private async Task CreateWorkProfile(Header header, IModel channel)
        {
            try
            {
                var message = "";
                var bufferData = new byte[header.IDataLength];
                await this.communication.ReceiveDataAsync(header.IDataLength, bufferData);
                WorkProfile newProfile = _helper.CreateWorkProfile(bufferData);
                if(lkdin.WorkProfiles.Any(x=>x.UserName.Equals(newProfile.UserName)))
                {
                    message = $"Ya existe un perfil de trabajo para el usuario {newProfile.UserName}";
                    Message(channel, message + " [creation]");
                    Console.WriteLine(message);
                    await this.communication.SendDataAsync(message, Commands.ServerResponse);

                }
                else
                {
                    this.lkdin.WorkProfiles.Add(newProfile);
                    FileStreamHandler fh = new FileStreamHandler();
                    await this.communication.ReceiveFileAsync(fh);
                    WorkProfile x = this.lkdin.WorkProfiles.Find(n => n.UserName.Equals(newProfile.UserName));
                    message = $"El perfil de trabajo de {x.UserName} se agrego correctamente.";
                    Message(channel, message + " [creation]");
                    Console.WriteLine(message);
                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                }
                //ProfileInfo receivedProfile = new ProfileInfo();
                //receivedProfile.Decode(bufferData);
               // WorkProfile newProfile = receivedProfile.ToEntity();

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task CreateUser(Header header, IModel channel)
        {
            try
            {
                var bufferData = new byte[header.IDataLength];
                await this.communication.ReceiveDataAsync(header.IDataLength, bufferData);
                UserInfo receivedInfo = new UserInfo();
                receivedInfo.Decode(bufferData);
                User newUser = receivedInfo.ToEntity();
                if (this.lkdin.Users.Any(n => n.Username.Equals(newUser.Username)))
                {
                    throw new Exception();
                }
                else
                {
                    this.lkdin.Users.Add(newUser);
                    User x = this.lkdin.Users.Find(n => n.Username.Equals(newUser.Username));
                    var message = $"El usuario {x.Username} se agrego correctamente.";
                    Message(channel, message + " [creation]");
                    Console.WriteLine(message);
                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}