using GrpcService1.ServerProgram;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace GrpcService1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //ServerHandler server = new ServerHandler();
            //await server.StartAsync();
            Task.Run(() => (new ServerHandler()).Start());
            //////////////////////////////////////////////////
            CreateHostBuilder(args).Build().Run();  // Bloqueante
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