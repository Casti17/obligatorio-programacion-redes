﻿using Communication;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AdminServer.Controllers
{
    [ApiController]
    [Route("User")]
    public class UserController : Controller
    {
        private Greeter.GreeterClient client;
        private readonly string grpcURL;
        private static readonly SettingsManager settingsManager = new SettingsManager();

        public UserController()
        {
            AppContext.SetSwitch(
                  "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            this.grpcURL = settingsManager.ReadSettings(ServerConfig.GrpcURL);
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO user)
        {
            using var channel = GrpcChannel.ForAddress(this.grpcURL);
            this.client = new Greeter.GreeterClient(channel);
            var reply = await this.client.CreateUserAsync(user);
            return this.Ok(reply.Message);
        }

        [HttpPut("update")]
        public async Task<IActionResult> ModifyUser([FromBody] ModifyUserRequest user)
        {
            using var channel = GrpcChannel.ForAddress(this.grpcURL);
            this.client = new Greeter.GreeterClient(channel);
            var reply = await this.client.ModifyUserAsync(user);
            return this.Ok(reply.Message);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(Username userName)
        {
            using var channel = GrpcChannel.ForAddress(this.grpcURL);
            this.client = new Greeter.GreeterClient(channel);
            var reply = await this.client.DeleteUserAsync(userName);
            return this.Ok(reply.Message);
        }
    }
}