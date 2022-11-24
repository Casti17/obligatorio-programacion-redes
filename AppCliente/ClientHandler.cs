using AppCliente.Interfaces;
using Communication;
using Domain.DTO;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AppCliente
{
    public class ClientHandler
    {
        private SettingsManager settingsManager { get; set; }
        private IMainHelper mainHelper { get; set; }
        private FileCommsHandler communication;
        private TcpClient tcpClient;
        private bool connected = true;

        public ClientHandler()
        {
            this.settingsManager = new SettingsManager();
            this.mainHelper = new MainHelper();
        }

        public async Task StartAsync()
        {
            Console.WriteLine("Client starting...");
            this.tcpClient = this.ConfigureTCP().Result;
            this.communication = new FileCommsHandler(this.tcpClient);

            await using (var networkStream = this.tcpClient.GetStream())
            {
                var word = string.Empty;
                while (this.connected)
                {
                    this.ShowMenu();
                    var option = Console.ReadLine();
                    switch (option)
                    {
                        case "1":
                            await this.CaseCreateUserAsync();
                            break;

                        case "2":
                            await this.CaseCreateWorkProfileAsync();
                            break;

                        case "3":
                            await this.CaseUpdateImageWorkProfile();
                            break;

                        case "4":
                            await this.CaseSearchProfileWithFilterAsync();
                            break;

                        case "5":
                            await this.CaseSearchProfileAsync();
                            break;

                        case "6":
                            await this.CaseSendMessageAsync();
                            break;

                        case "7":
                            await this.CaseCheckInboxAsync();
                            break;

                        case "8":
                            this.connected = false;
                            break;

                        default:
                            Console.WriteLine("Opcion Invalida");
                            break;
                    }
                }
            }
        }

        private async Task<TcpClient> ConfigureTCP()
        {
            var clientIpEndPoint = new IPEndPoint(
                IPAddress.Parse(this.settingsManager.ReadSettings(ClientConfig.clientIPconfigkey)),
                int.Parse(this.settingsManager.ReadSettings(ClientConfig.ClientPortConfigKey)));
            var tcpClient = new TcpClient(clientIpEndPoint);
            Console.WriteLine("Trying to connect to server");

            await tcpClient.ConnectAsync(
                IPAddress.Parse(this.settingsManager.ReadSettings(ClientConfig.serverIPconfigkey)),
                int.Parse(this.settingsManager.ReadSettings(ClientConfig.serverPortconfigkey))).ConfigureAwait(false);
            return tcpClient;
        }

        private async Task CaseCheckInboxAsync()
        {
            string username;
            Console.WriteLine("Ingrese el usuario del que quiera ver su inbox:");
            username = Console.ReadLine();
            ProfileSearchInfo userToCheck = new ProfileSearchInfo()
            {
                Username = username,
            };
            await this.communication.SendDataAsync(userToCheck.Code(), Commands.CheckInbox);
            Console.WriteLine("Procesando...");
            await this.communication.RecieveMessageAsync();
        }

        private async Task CaseSendMessageAsync()
        {
            Console.WriteLine("Que usuario esta enviando el mensaje?");
            string sender = Console.ReadLine();
            Console.WriteLine("A quien se lo envia?");
            var receiver = Console.ReadLine();
            Console.WriteLine("Escriba el mensaje:");
            var message = Console.ReadLine();
            SendMessageInfo messageSent = new SendMessageInfo()
            {
                Sender = sender,
                Receiver = receiver,
                Message = message,
            };
            await this.communication.SendDataAsync(messageSent.Code(), Commands.SendMessage);
            Console.WriteLine("Procesando ...");
            await this.communication.RecieveMessageAsync();
        }

        private async Task CaseSearchProfileAsync()
        {
            string username;
            Console.WriteLine("Escriba el nombre de usuario del perfil que este buscando.");
            username = Console.ReadLine();
            ProfileSearchInfo newSearch = new ProfileSearchInfo()
            {
                Username = username,
            };
            await this.communication.SendDataAsync(newSearch.Code(), Commands.SearchProfile);
            Console.WriteLine("Procesando...");
            var x = await this.communication.RecieveMessageAsync();
            if (x.Contains("imagen"))
            {
                FileStreamHandler fh = new FileStreamHandler();
                await this.communication.ReceiveFileAsync(fh);
            }
        }

        private async Task CaseSearchProfileWithFilterAsync()
        {
            string username;
            Console.WriteLine("Escriba una palabra para filtrar.");
            username = Console.ReadLine();
            ProfileSearchInfo newSearch = new ProfileSearchInfo()
            {
                Username = username,
            };
            await this.communication.SendDataAsync(newSearch.Code(), Commands.SearchExistingProfiles);
            Console.WriteLine("Procesando...");
            var x = await this.communication.RecieveMessageAsync();
        }

        private async Task CaseUpdateImageWorkProfile()
        {
            string username, path;
            Console.WriteLine("Ingrese el Nombre de Usuario");
            username = Console.ReadLine();
            Console.WriteLine("Ingrese nuevo path de la imagen");
            path = Console.ReadLine();
            ProfileInfo newProfile = new ProfileInfo()
            {
                Username = username,
                Imagen = path,
            };
            try
            {
                await this.communication.SendDataAsync(newProfile.Code(), Commands.AssociateImageToProfile);
                FileStreamHandler fh = new FileStreamHandler();
                await this.communication.SendFileAsync(path, fh);
                Console.WriteLine("Procesando...");
                await this.communication.RecieveMessageAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Algo salio mal, verifique que haya puesto campos validos (no se permiten numeros)" + e);
            }
        }

        private async Task CaseCreateWorkProfileAsync()
        {
            string username, path, skills, descripcion;
            Console.WriteLine("A continuacion se pediran los datos requeridos para crear usuario.");
            Console.WriteLine("Ingrese el Nombre de Usuario");
            username = Console.ReadLine();
            Console.WriteLine("Ingrese path de la imagen");
            path = Console.ReadLine();
            Console.WriteLine("Ingrese sus habilidades (si tiene mas de 1, utilice guiones)");
            skills = Console.ReadLine();
            Console.WriteLine("Ingrese descripcion");
            descripcion = Console.ReadLine();
            ProfileInfo newProfile = new ProfileInfo()
            {
                Username = username,
                Imagen = path,
                Skills = skills,
                Descripcion = descripcion,
            };
            try
            {
                await this.communication.SendDataAsync(newProfile.Code(), Commands.CreateWorkProfile);
                FileStreamHandler fh = new FileStreamHandler();
                await this.communication.SendFileAsync(path, fh);
                Console.WriteLine("Procesando...");
                await this.communication.RecieveMessageAsync();
            }
            catch (Exception)
            {
                Console.WriteLine("Algo salio mal, verifique que haya puesto campos validos (no se permiten numeros)");
            }
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

        public async Task CaseCreateUserAsync()
        {
            string username, name, surname;
            Console.WriteLine("A continuacion se pediran los datos requeridos para crear usuario.");
            Console.WriteLine("Ingrese el Nombre de Usuario");
            username = Console.ReadLine();
            Console.WriteLine("Ingrese su nombre");
            name = Console.ReadLine();
            Console.WriteLine("Ingrese su apellido");
            surname = Console.ReadLine();
            UserInfo newUser = new UserInfo()
            {
                Username = username,
                Name = name,
                Surname = surname,
            };
            try
            {
                await this.communication.SendDataAsync(newUser.Code(), Commands.CreateUser);
                Console.WriteLine("Procesando...");
                await this.communication.RecieveMessageAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Algo salio mal, verifique que haya puesto campos validos (no se permiten numeros)");
            }
        }
    }
}