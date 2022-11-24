using Communication;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminServer.Controllers
{
    [ApiController]
    [Route("WorkProfiles")]
    public class WorkProfileController : ControllerBase
    {
        private Greeter.GreeterClient client;
        private readonly string grpcURL;
        private static readonly SettingsManager settingsManager = new SettingsManager();

        public WorkProfileController()
        {
            AppContext.SetSwitch(
                  "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            this.grpcURL = settingsManager.ReadSettings(ServerConfig.GrpcURL);
        }

        [HttpPost("profile")]
        public async Task<IActionResult> CreateProfile([FromBody] CreateProfileRequest profile)
        {
            using var channel = GrpcChannel.ForAddress(this.grpcURL);
            this.client = new Greeter.GreeterClient(channel);
            var reply = await this.client.CreateProfileAsync(profile);
            return this.Ok(reply.Message);
        }

        [HttpPut("update")]
        public async Task<IActionResult> ModifyProfile([FromBody] ModifyProfileRequest profile)
        {
            using var channel = GrpcChannel.ForAddress(this.grpcURL);
            this.client = new Greeter.GreeterClient(channel);
            var reply = await this.client.ModifyProfileAsync(profile);
            return this.Ok(reply.Message);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProfile([FromQuery] Username userName)
        {
            using var channel = GrpcChannel.ForAddress(this.grpcURL);
            this.client = new Greeter.GreeterClient(channel);
            var reply = await this.client.DeleteProfileAsync(userName);
            return this.Ok(reply.Message);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProfilePicture([FromQuery] Username userName)
        {
            using var channel = GrpcChannel.ForAddress(this.grpcURL);
            this.client = new Greeter.GreeterClient(channel);
            var reply = await this.client.DeleteImageAsync(userName);
            return this.Ok(reply.Message);
        }
    }
}