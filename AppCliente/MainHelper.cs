using AppCliente.Interfaces;
using Communication;
using System;
using System.Net.Sockets;

namespace AppCliente
{
    public class MainHelper : IMainHelper
    {
        private SocketHelper socketHelper;

        public void CreateUser(string userData, Socket socket)
        {
            this.socketHelper = new SocketHelper(socket);
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

        public void CreateWorkProfile(string workprofile, Socket socketCliente)
        {
            try
            {
                Request req = new Request();
                req.Body = workprofile;
                req.Header = new Header(Commands.CreateWorkProfile, ProtocolConstants.DataLength);
                this.socketHelper.SendRequest(req);
            }
            catch (Exception)
            {
                Console.WriteLine("Cant create work profile");
            }
        }

        public void UpdateProfilePicture(string update, Socket socket)
        {
            try
            {
                Request req = new Request();
                req.Body = update;
                req.Header = new Header(Commands.AssociateImageToProfile, ProtocolConstants.DataLength);
                this.socketHelper.SendRequest(req);
            }
            catch (Exception)
            {
                Console.WriteLine("Cant update image");
            }
        }
    }
}