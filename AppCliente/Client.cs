namespace AppCliente
{
    public class Client
    {
        private static void Main(string[] args)
        {
            var clientHandler = new ClientHandler();
            clientHandler.Start();
        }
    }
}