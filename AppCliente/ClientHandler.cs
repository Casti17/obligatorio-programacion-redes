using AppCliente.Interfaces;
using Communication;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace AppCliente
{
    public class ClientHandler
    {
        private SettingsManager settingsManager { get; set; }
        private IMainHelper mainHelper { get; set; }
        //private UserLogic userLogic { get; set; }

        //public Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // private readonly SettingsManager settingsMngr = new SettingsManager();

        public ClientHandler()
        {
            this.settingsManager = new SettingsManager();
            this.mainHelper = new MainHelper();
        }

        public void Start()
        {
            var socket = this.StartSocket();
            var connected = true;
            Console.WriteLine("Client started");
            while (connected)
            {
                this.ShowMenu();
                var option = Console.ReadLine();
                connected = this.ExecuteSelectedOption(option, socket, connected);
            }
        }

        private Socket StartSocket()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string ipServer = this.settingsManager.ReadSettings(ClientConfig.serverIPconfigkey);
            string ipClient = this.settingsManager.ReadSettings(ClientConfig.clientIPconfigkey);
            int serverPort = int.Parse(this.settingsManager.ReadSettings(ClientConfig.serverPortconfigkey));

            var localEndPoint = new IPEndPoint(IPAddress.Parse(ipClient), 0);
            socket.Bind(localEndPoint);
            var serverEndpoint = new IPEndPoint(IPAddress.Parse(ipServer), serverPort);
            socket.Connect(serverEndpoint);
            return socket;
        }

        private bool ExecuteSelectedOption(string option, Socket socket, bool connected)
        {
            switch (option)
            {
                case "1":
                    this.CaseCreateUser(socket);//CaseCreateUser(socket);
                    break;

                case "2":
                    //CaseCreateWorkProfile();
                    break;

                case "3":
                    //CaseUpdateImageWorkProfile();
                    break;

                case "4":
                    //CaseSearchProfileWithFilter();
                    break;

                case "5":
                    //CaseSearchProfile();
                    break;

                case "6":
                    //CaseSendMessage();
                    break;

                case "7":
                    //CaseCheckInbox();
                    break;

                default:
                    Console.WriteLine("Opcion Invalida");
                    break;
            }
            return connected;
        }

        private void ShowMenu()
        {
            Console.WriteLine("\n\n\n-------------------------------------------------");
            Console.WriteLine("\nWelcome to the LkDin");
            Console.WriteLine("Please, select one of the valid options: ");
            Console.WriteLine("1- Create user");
            Console.WriteLine("2- Create work profile");
            Console.WriteLine("3- Update picture");
            Console.WriteLine("4- Consultar perfiles existentes");
            Console.WriteLine("5- Consultar perfil especifico");
            Console.WriteLine("6- Enviar mensaje");
            Console.WriteLine("7- Check inbox");
            Console.WriteLine("8- Disconnect");
            Console.WriteLine("Insert your command: ");
        }

        public void CaseCreateUser(Socket socketCliente)
        {
            Console.WriteLine("Please input the following information. NO SPACES ALLOWED");
            Console.WriteLine("Username?");
            var userName = Console.ReadLine();
            bool correctFormat = Regex.IsMatch(userName, @"^[a-zA-Z]+$");
            while (!correctFormat)
            {
                Console.WriteLine("Incorrect format, please try again.");
                Console.WriteLine("Name?");
                userName = Console.ReadLine();
                correctFormat = Regex.IsMatch(userName, @"^[a-zA-Z]+$");
            }
            Console.WriteLine("Name?");
            var name = Console.ReadLine();
            correctFormat = Regex.IsMatch(name, @"^[a-zA-Z]+$");
            while (!correctFormat)
            {
                Console.WriteLine("Incorrect format, please try again");
                Console.WriteLine("Name?");
                name = Console.ReadLine();
                correctFormat = Regex.IsMatch(name, @"^[a-zA-Z]+$");
            }

            Console.WriteLine("Last name?");
            var lastName = Console.ReadLine();
            correctFormat = Regex.IsMatch(lastName, @"^[a-zA-Z]+$");
            while (!correctFormat)
            {
                Console.WriteLine("Incorrect format, please try again");
                Console.WriteLine("Last name?");
                lastName = Console.ReadLine();
                correctFormat = Regex.IsMatch(name, @"^[a-zA-Z]+$");
            }
            string user = $"{name}#{lastName}#{userName}";
            this.mainHelper.CreateUser(user, socketCliente);
        }

        /*
        public void CreateUser(string user, Socket socketCliente)
        {
            var header = new Header(Commands.CreateUser, user.Length);
            Request protocol = new Request() { Header = header, Body = user };
            SocketHelper socketHelper = new SocketHelper(socketCliente);
            try
            {
                socketHelper.SendRequest(protocol);
                //Request = HandleS
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }*/
    }
}