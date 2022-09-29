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
                    this.CaseCreateUser(socket);
                    break;

                case "2":
                    this.CaseCreateWorkProfile(socket);
                    break;

                case "3":
                    this.CaseUpdateImageWorkProfile(socket);
                    break;

                case "4":
                    this.CaseSearchProfileWithFilter(socket);
                    break;

                case "5":
                    this.CaseSearchProfile();
                    break;

                case "6":
                    this.CaseSendMessage();
                    break;

                case "7":
                    this.CaseCheckInbox();
                    break;

                default:
                    Console.WriteLine("Opcion Invalida");
                    break;
            }
            return connected;
        }

        private void CaseCheckInbox()
        {
            throw new NotImplementedException();
        }

        private void CaseSendMessage()
        {
            throw new NotImplementedException();
        }

        private void CaseSearchProfile()
        {
            throw new NotImplementedException();
        }

        private void CaseSearchProfileWithFilter(Socket socket)
        {
            Console.WriteLine("Please type the keywords you would like to search by, separating with -");
            var filters = Console.ReadLine();
            bool correctFormat;
            correctFormat = Regex.IsMatch(filters, "^[a - zA - Z0 - 9_.-] *$");
            while (!correctFormat)
            {
                Console.WriteLine("Incorrect format, please try again");
                Console.WriteLine("Please type the keywords you would like to search by, separating with -");
                filters = Console.ReadLine();
                correctFormat = Regex.IsMatch(filters, @"^[a-zA-Z]+$");
            }
        }

        private void CaseUpdateImageWorkProfile(Socket socket)
        {
            Console.WriteLine("Which work profile would you like to update?");
            var username = Console.ReadLine();
            bool correctFormat = Regex.IsMatch(username, @"^[a-zA-Z]+$");
            while (!correctFormat)
            {
                Console.WriteLine("Incorrect format, please try again.");
                Console.WriteLine("Which work profile would you like to update?");
                username = Console.ReadLine();
                correctFormat = Regex.IsMatch(username, @"^[a-zA-Z]+$");
            }
            Console.WriteLine("Please insert the image path:");
            var path = Console.ReadLine();
            correctFormat = Regex.IsMatch(path, @"^[a-zA-Z0-9\_]+$");
            while (!correctFormat)
            {
                Console.WriteLine("Incorrect format, please try again.");
                Console.WriteLine("Image path?");
                path = Console.ReadLine();
                correctFormat = Regex.IsMatch(path, @"^[a-zA-Z]+$");
            }
            string update = username + path;
            this.mainHelper.UpdateProfilePicture(update, socket);
        }

        private void CaseCreateWorkProfile(Socket socket)
        {
            Console.WriteLine("Please input the following information. NO SPACES ALLOWED");
            Console.WriteLine("Username?");
            var userName = Console.ReadLine();
            bool correctFormat = Regex.IsMatch(userName, @"^[a-zA-Z]+$");
            while (!correctFormat)
            {
                Console.WriteLine("Incorrect format, please try again.");
                Console.WriteLine("Username?");
                userName = Console.ReadLine();
                correctFormat = Regex.IsMatch(userName, @"^[a-zA-Z]+$");
            }
            Console.WriteLine("Image path?");
            var path = Console.ReadLine();
            correctFormat = Regex.IsMatch(path, @"^[a - zA - Z0 - 9_.-]+$");
            while (!correctFormat)
            {
                Console.WriteLine("Incorrect format, please try again.");
                Console.WriteLine("Image path?");
                path = Console.ReadLine();
                correctFormat = Regex.IsMatch(path, @"^[a-zA-Z]+$");
            }
            Console.WriteLine("Skills? (please type them in with a - between them.");
            var skills = Console.ReadLine();
            correctFormat = Regex.IsMatch(skills, "^[a - zA - Z0 - 9_.-]+$");
            while (!correctFormat)
            {
                Console.WriteLine("Incorrect format, please try again");
                Console.WriteLine("Skills? (please type them in with a - between them.");
                skills = Console.ReadLine();
                correctFormat = Regex.IsMatch(skills, @"^[a-zA-Z]+$");
            }

            Console.WriteLine("Description");
            var description = Console.ReadLine();
            correctFormat = Regex.IsMatch(description, @"^[A-Za-z ]+$");
            while (!correctFormat)
            {
                Console.WriteLine("Incorrect format, please try again");
                Console.WriteLine("Description");
                description = Console.ReadLine();
                correctFormat = Regex.IsMatch(description, @"^[a-zA-Z]+$");
            }
            string workprofile = $"{userName}#{path}#{skills}#{description}";
            this.mainHelper.CreateWorkProfile(workprofile, socket);
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
            Console.WriteLine("User created successfully");
        }
    }
}