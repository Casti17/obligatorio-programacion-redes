using Communication;
using System;
using System.Net.Sockets;

namespace AppCliente
{
    public class ClientHandler
    {
        public Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public ClientHandler()
        {
        }

        public void CreateUser(string user)
        {
            var header = new Header(Commands.CreateUser, user.Length);
            Request protocol = new Request() { Header = header, Body = user };
            SocketHelper socketHelper = new SocketHelper(this.clientSocket);
            try
            {
                socketHelper.SendRequest(protocol);
                //Request = HandleS
            }
            catch (Exception)
            {
                Console.WriteLine("No se pudo crear el usuario correctamente.");
            }
        }
    }
}