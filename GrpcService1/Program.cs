using AppServidor;
using Communication;
using GrpcService1.ServerProgram;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace GrpcService1
{
    public class Program
    {
        private static readonly SettingsManager settingsManager = new SettingsManager();

        public static void Main(string[] args)
        {
            var serverIpAddress = settingsManager.ReadSettings(ServerProgram.ServerConfig.serverIPconfigkey);
            var serverPort = settingsManager.ReadSettings(ServerProgram.ServerConfig.serverPortconfigkey);
            Console.WriteLine($"Server is starting in address {serverIpAddress} and port {serverPort}");

            Server server = new Server();
            server.StartAsync();
            //////////////////////////////////////////////////
            CreateHostBuilder(args).Build().Run();  // Bloqueante

            Task.Run(() => (new ServerHandler()).StartAsync()).ConfigureAwait(false);
            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
.ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder.UseStartup<Startup>();
});
        }
    }
}