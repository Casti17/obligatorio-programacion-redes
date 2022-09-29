using System.Net.Sockets;

namespace AppCliente.Interfaces
{
    public interface IMainHelper
    {
        public void CreateUser(string userData, Socket socket);

        void CreateWorkProfile(string workprofile, Socket socketCliente);

        void UpdateProfilePicture(string update, Socket socket);
    }
}