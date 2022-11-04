using System.Threading.Tasks;

namespace AppServidor
{
    internal class Servidor
    {
        private static async Task Main(string[] args)
        {
            ServerHandler serverHandler = new ServerHandler();
            await serverHandler.StartAsync();
        }
    }
}