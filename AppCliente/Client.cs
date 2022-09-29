using Communication;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace AppCliente
{
    public class Client
    {
        private static readonly SettingsManager settingsMngr = new SettingsManager();
        public static ClientHandler clientHandler = new ClientHandler();

        private static void Main(string[] args)
        {
            bool connected = true;
            Console.WriteLine("Iniciando Aplicacion Cliente....!!!");

            var socketCliente = new Socket(
            AddressFamily.InterNetwork,
                SocketType.Stream,
                    ProtocolType.Tcp);
            string ipServer = settingsMngr.ReadSettings(ClientConfig.serverIPconfigkey);
            string ipClient = settingsMngr.ReadSettings(ClientConfig.clientIPconfigkey);
            int serverPort = int.Parse(settingsMngr.ReadSettings(ClientConfig.serverPortconfigkey));

            var localEndPoint = new IPEndPoint(IPAddress.Parse(ipClient), 0);
            socketCliente.Bind(localEndPoint);
            var serverEndpoint = new IPEndPoint(IPAddress.Parse(ipServer), serverPort);
            socketCliente.Connect(serverEndpoint);
            Console.WriteLine("Cliente Conectado al Servidor...!!!");

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
            Console.WriteLine("Ingrese su opcion: ");
            while (connected)
            {
                String mensaje = Console.ReadLine();
                switch (mensaje)
                {
                    case "1":
                        CaseCreateUser(socketCliente);
                        break;

                    case "2":
                        CaseCreateWorkProfile();
                        break;

                    case "3":
                        CaseUpdateImageWorkProfile();
                        break;

                    case "4":
                        CaseSearchProfileWithFilter();
                        break;

                    case "5":
                        CaseSearchProfile();
                        break;

                    case "6":
                        CaseSendMessage();
                        break;

                    case "7":
                        CaseCheckInbox();
                        break;

                    default:
                        Console.WriteLine("Opcion Invalida");
                        break;
                }
                if (mensaje.Equals("Exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    //parar = true;
                }
            }

            Console.WriteLine("Cierro el Cliente");
            // Cerrar la conexion.
            socketCliente.Shutdown(SocketShutdown.Both);
            socketCliente.Close();
        }

        public static void CaseCreateUser(Socket socketCliente)
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
            clientHandler.CreateUser(user);
        }

        public static void CaseCreateWorkProfile()
        {
            Console.WriteLine("Please provide us with a description");
            var description = Console.ReadLine();
            Console.WriteLine("Please list your skills/abilities:");
            var skill = Console.ReadLine();
            while (!skill.Equals("No"))
            {
                skill += skill;
                Console.WriteLine("If you do not have more abilities, type No");
            }
        }

        private static void CaseUpdateImageWorkProfile()
        {
            throw new NotImplementedException();
        }

        private static void CaseSearchProfile()
        {
            Console.WriteLine("Please insert the username you want to search");
            var filter = Console.ReadLine();
        }

        private static void CaseSearchProfileWithFilter()
        {
            Console.WriteLine("Which parameter would you like to search by?");
            Console.WriteLine("Abilities - Word");
            var filter = Console.ReadLine();
            switch (filter)
            {
                case "Abilities":
                    Console.WriteLine("Please insert the ability you would like to search");
                    var ability = Console.ReadLine();
                    //buscar por ability.
                    break;

                case "Word":
                    Console.WriteLine("Please insert the word you would like to search");
                    var word = Console.ReadLine();
                    break;
            }
        }

        private static void CaseSendMessage()
        {
            Console.WriteLine("");
        }

        private static void CaseCheckInbox()
        {
            throw new NotImplementedException();
        }
    }
}