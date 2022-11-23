using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AdminServer.Controllers
{
    [Route("api/v1/users")]
    public class UserController : Controller
    {
        /*private static string ServerUrl => "https://localhost:5001";

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO gameData)
        {
            using var channel = GrpcChannel.ForAddress(ServerUrl);
            var client = new Greeter.GreeterClient(channel);
            //var message = MessageHelper.FormatGameMessage(gameData);
            //var messageWithId = MessageHelper.AddUserId(message, "");
            var reply = await client.CreateUserAsync(
                new CreateUserRequest()
                {
                    Message = messageWithId
                }
            );
            return Ok(reply.Message);
        }*/
    }
}