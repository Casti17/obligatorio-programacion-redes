namespace AppServidor
{
    internal class Servidor
    {
        public static void Main(string[] args)
        {
            ServerHandler serverHandler = new ServerHandler();
            serverHandler.Start();
        }
    }
}