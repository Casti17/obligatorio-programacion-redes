using Communication;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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

        public async Task StartAsync()
        {
            /*var socketServer = this.StartSocketServer();

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
            }*/
            Console.WriteLine("Server is starting...");
            var ipEndPoint = new IPEndPoint(
                IPAddress.Parse(settingsManager.ReadSettings(ServerConfig.serverIPconfigkey)),
                int.Parse(settingsManager.ReadSettings(ServerConfig.serverPortconfigkey)));
            var tcpListener = new TcpListener(ipEndPoint);

            tcpListener.Start(100);
            Console.WriteLine("Server started listening connections on {0}-{1}", settingsManager.ReadSettings(ServerConfig.serverIPconfigkey), settingsManager.ReadSettings(ServerConfig.serverPortconfigkey));

            Console.WriteLine("Server will start displaying messages from the clients");

            while (true)
            {
                var tcpClientSocket = await tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
                var task = Task.Run(async () => await HandleClient(tcpClientSocket).ConfigureAwait(false)); // Pedir un "hilo" del CLR prestado
            }
        }

        private static async Task HandleClient(TcpClient tcpClientSocket)
        {
            var isClientConnected = true;
            try
            {
                using (var networkStream = tcpClientSocket.GetStream())
                {
                    while (isClientConnected)
                    {
                        var dataLength = new byte[Protocol.FixedDataSize];
                        int totalReceived = 0;
                        while (totalReceived < Protocol.FixedDataSize)
                        {
                            var received = await networkStream.ReadAsync(dataLength, totalReceived, Protocol.FixedDataSize - totalReceived).ConfigureAwait(false);
                            if (received == 0)
                            {
                                throw new SocketException();
                            }
                            totalReceived += received;
                        }
                        var length = BitConverter.ToInt32(dataLength, 0);
                        var data = new byte[length];
                        totalReceived = 0;
                        while (totalReceived < length)
                        {
                            int received = await networkStream.ReadAsync(data, totalReceived, length - totalReceived).ConfigureAwait(false);
                            if (received == 0)
                            {
                                throw new SocketException();
                            }
                            totalReceived += received;
                        }
                        var word = Encoding.UTF8.GetString(data);
                        if (word.Equals("exit"))
                        {
                            isClientConnected = false;
                            Console.WriteLine("Client is leaving");
                        }
                        else
                        {
                            Console.WriteLine("Client says: " + word);
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"The client connection was interrupted - Exception {e.Message}");
            }
        }
    }

    /*
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
    }*/
}