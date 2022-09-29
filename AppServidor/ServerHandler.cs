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

        private static readonly SettingsManager settingsManager = new SettingsManager();

        private static SocketHelper socketHelper { get; set; }

        public SocketHelper GetSocketHelper()
        {
            return socketHelper = new SocketHelper(this.socketClient);
        }

        public void Start()
        {
            var socketServer = this.StartSocketServer();

            Console.WriteLine("Welcome to LkDin Server");
            Console.WriteLine("Options");
            Console.WriteLine("exit");
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
    }
}