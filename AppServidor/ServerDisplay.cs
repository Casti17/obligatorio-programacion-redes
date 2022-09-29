using BusinessLogic;
using Communication;
using Repositories;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AppServidor
{
    public class ServerDisplay
    {
        private static readonly SettingsManager settingsMng = new SettingsManager();
        private static readonly Socket socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly UserLogic userLogic;
        private readonly WorkProfileLogic workProfileLogic;
        private UserRepository userRepository;
        private WorkProfileRepository workProfileRepository;

        public ServerDisplay()
        {
        }

        public void StartServer()
        {
            try
            {
                Console.WriteLine("Iniciando Aplicacion Servidor....!!!");
                this.userRepository = new UserRepository();
                this.workProfileRepository = new WorkProfileRepository();
                string ipServidor = settingsMng.ReadSettings(ServerConfig.ServerIPConfigKey);
                int puerto = int.Parse(settingsMng.ReadSettings(ServerConfig.ServerPortConfigKey));
                UserLogic userLogic = new UserLogic(this.userRepository);
                WorkProfileLogic workProfileLogic = new WorkProfileLogic(this.workProfileRepository);
                var localEndpoint = new IPEndPoint(IPAddress.Parse(ipServidor), puerto);
                Console.WriteLine(localEndpoint);
                // puertos 0 a 65535   pero del 1 al 1024 estan reservados

                socketServer.Bind(localEndpoint); // vinculo el socket al EndPoint
                socketServer.Listen(2); // Pongo al Servidor en modo escucha
                new Thread(() => this.HandleClient(socketServer)).Start();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Connection error, {e.Message}");
            }
        }

        public void HandleClient(Socket socketServer)
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Primer checkpoint");
                    Socket socketClient = socketServer.Accept();
                    Console.WriteLine("2");
                    new Thread(() => new ServerHandler(socketClient, this.userLogic, this.workProfileLogic, this.userRepository, this.workProfileRepository));
                }
            }
            catch (Exception)
            {
            }
        }
    }
}