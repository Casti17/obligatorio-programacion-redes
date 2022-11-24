using System.Threading.Tasks;

namespace AppCliente
{
    public class Client
    {
        private static async Task Main(string[] args)
        {
            var clientHandler = new ClientHandler();
            await clientHandler.StartAsync();
        }
    }
}