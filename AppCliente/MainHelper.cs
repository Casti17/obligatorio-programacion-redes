using AppCliente.Interfaces;
using Communication;
using System;
using System.Net.Sockets;

namespace AppCliente
{
    public class MainHelper : IMainHelper
    {
        private readonly SocketHelper socketHelper;

        private string ReadWhileEmpty(string message)
        {
            string input = "";
            while (input.Equals(""))
            {
                Console.WriteLine(message);
                input = Console.ReadLine();
            }

            return input;
        }

        public void CreateUser(string userData, Socket socket)
        {
            try
            {
                Request req = new Request();
                req.Body = userData;
                req.Header = new Header(Commands.CreateUser, ProtocolConstants.DataLength);
                this.socketHelper.SendRequest(req);
            }
            catch (Exception)
            {
                Console.WriteLine("Cant create user");
            }
        }

        private string UserCreation()
        {
            var username = this.ReadWhileEmpty("Username?");
            var name = this.ReadWhileEmpty("Name?");
            var lastName = this.ReadWhileEmpty("Last name?");

            return $"{username}#{name}#{lastName}";
        }
    }
}