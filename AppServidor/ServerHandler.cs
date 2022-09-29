using Communication;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AppServidor
{
    public class ServerHandler
    {
        private readonly Socket socketClient;

        //private readonly UserLogic userLogic;
        //private readonly WorkProfileLogic workProfileLogic;
        //private readonly UserRepository userRepository;
        //private readonly WorkProfileRepository workProfileRepository;
        //private readonly SocketHelper socketHelper;
        private static readonly SettingsManager settingsManager = new SettingsManager();

        private static SocketHelper socketHelper { get; set; }

        public SocketHelper GetSocketHelper()
        {
            return socketHelper = new SocketHelper(this.socketClient);
        }

        public void Start()
        {
            var socketServer = this.StartSocketServer();

            Console.WriteLine("Welcome to LkDin server");

            Console.WriteLine("Options");
            Console.WriteLine("Welcome to LkDin server");

            Console.WriteLine("Welcome to LkDin server");
            Console.WriteLine("Insert option");
            while (!this.GetSocketHelper().Disconnect)
            {
                var userInput = Console.ReadLine();
                switch (userInput)
                {
                    case "exit":
                        this.ExitServer(socketServer);
                        break;

                    default:
                        Console.WriteLine("Incorrect option");
                        break;
                }
            }
        }

        private Socket StartSocketServer()
        {
            var socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //var configServerIpAddress = settingsManager.ReadSettings("ServerIpAddress");
            //var parsedIpAddress = IPAddress.Parse(configServerIpAddress);
            //var configServerPort = settingsManager.ReadSettings("ServerPort");
            //var serverPort = Int32.Parse(configServerPort);
            string ipServer = settingsManager.ReadSettings(ServerConfig.serverIPconfigkey);
            int ipPort = int.Parse(settingsManager.ReadSettings(ServerConfig.serverPortconfigkey));

            var localEndpoint = new IPEndPoint(IPAddress.Parse(ipServer), ipPort);
            socketServer.Bind(localEndpoint);
            socketServer.Listen(100);

            //Lanzar un thread para manejar las conexiones
            var threadServer = new Thread(() => this.GetSocketHelper().ListenClients(socketServer));
            threadServer.Start();
            return socketServer;
        }

        private void ExitServer(Socket socketServer)
        {
            this.GetSocketHelper().Disconnect = true;
            socketServer.Close(0);
            foreach (var client in this.GetSocketHelper().Clients)
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }

            var fakeSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var configServerIpAddress = settingsManager.ReadSettings("ServerIpAddress");
            fakeSocket.Connect(configServerIpAddress, 20000);
        }

        /*public ServerHandler(Socket socketClient, UserLogic userLogic, WorkProfileLogic workProfileLogic, UserRepository userRepository, WorkProfileRepository workProfileRepository)
        {
            this.socketClient = socketClient;
            //this.socketHelper = new SocketHelper(this.socketClient);
            this.userLogic = userLogic;
            this.workProfileLogic = workProfileLogic;
            this.userRepository = userRepository;
            this.workProfileRepository = workProfileRepository;
            bool isClientConnected = true;
            Console.WriteLine("segundo");
            while (isClientConnected)
            {
                try
                {
                    byte[] buffer = this.socketHelper.Receive(ProtocolConstants.CommandLength + ProtocolConstants.DataLength);
                    var header = new Header(buffer);
                    byte[] bufferData = this.socketHelper.Receive(header.IDataLength);
                    var data = Encoding.UTF8.GetString(bufferData);
                    string[] dataArray;
                    switch (header.ICommand)
                    {
                        case 1:
                            dataArray = data.Split('#');
                            userLogic.CreateUser(dataArray[0], dataArray[1], dataArray[2]);
                            var user = this.userRepository.GetUser(dataArray[2]);
                            Console.WriteLine("hola");
                            break;

                        case 2:
                            dataArray = data.Split('#');
                            workProfileLogic.CreateWorkProfile(dataArray[0], dataArray[1], dataArray[2], dataArray[3]);
                            break;

                        case 3:
                            //workProfileLogic.UpdatePicture()
                            break;

                        case 4:
                            workProfileRepository.GetProfilesBySkills(data);
                            break;

                        case 5:
                            workProfileRepository.GetProfile(data);
                            break;

                        case 6:
                            dataArray = data.Split('#');
                            Message message = new Message(dataArray[0], dataArray[1], dataArray[2]);
                            break;
                    }
                }
                catch (Exception)
                {
                }
            }
        }*/
    }
}