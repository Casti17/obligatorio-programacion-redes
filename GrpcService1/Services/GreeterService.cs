using Domain;
using Grpc.Core;
using GrpcService1.ServerProgram;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GrpcService1
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly ServerHandler serverHandler;

        public GreeterService(ILogger<GreeterService> logger)
        {
            this._logger = logger;
        }

        /*public override Task<StandardResponse> CreateUser(StandardRequest request, ServerCallContext context)
        {
            try
            {
                var result = this.serverHandler.CreateUser(request.Message);
                return Task.FromResult(new StandardResponse()
                {
                    Message = result
                }
                );
            }
            catch (UserAlreadyExistsException e)
            {
                return Task.FromResult(new StandardResponse()
                { Message = e.Message }
                );
            }
            catch (Exception e)
            {
                return Task.FromResult(new StandardResponse()
                { Message = e.Message }
                );
            }
        }*/
    }
}