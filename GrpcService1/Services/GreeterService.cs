using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GrpcService1
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly MainHelper mainHelper;

        public GreeterService(ILogger<GreeterService> logger)
        {
            this._logger = logger;
            this.mainHelper = MainHelper.GetInstance();
        }

        public override Task<MessageReply> CreateUser(UserDTO request, ServerCallContext context)
        {
            try
            {
                var result = this.mainHelper.CreateUser(request.FirstName, request.LastName, request.Username);
                return Task.FromResult(new MessageReply()
                {
                    Message = result
                }
                );
            }
            catch (Exception e)
            {
                return Task.FromResult(new MessageReply()
                { Message = e.Message }
                );
            }
        }

        public override Task<MessageReply> DeleteUser(Username request, ServerCallContext context)
        {
            try
            {
                bool couldDelete = this.mainHelper.DeleteUser(request.Username_);
                string message = couldDelete ? "Usuario eliminado correctamente" : "No se pudo eliminar usuario";
                return Task.FromResult(new MessageReply { Message = message });
            }
            catch (Exception e)
            {
                return Task.FromResult(new MessageReply()
                { Message = e.Message }
                );
            }
        }

        public override Task<MessageReply> ModifyUser(ModifyUserRequest request, ServerCallContext context)
        {
            try
            {
                string message = this.mainHelper.ModifyUser(request.Username, request.NewName);
                return Task.FromResult(new MessageReply { Message = message });
            }
            catch (Exception e)
            {
                return Task.FromResult(new MessageReply()
                { Message = e.Message }
                );
            }
        }

        public override Task<MessageReply> CreateProfile(CreateProfileRequest request, ServerCallContext context)
        {
            try
            {
                bool couldUpdate = this.mainHelper.CreateWorkProfile(request.Username, request.PicturePath, request.Description, request.Skills);
                string message = couldUpdate ? "Perfil creado correctamente" : "No se pudo crear perfil";
                return Task.FromResult(new MessageReply { Message = message });
            }
            catch (Exception e)
            {
                return Task.FromResult(new MessageReply()
                { Message = e.Message }
                );
            }
        }

        public override Task<MessageReply> DeleteProfile(Username request, ServerCallContext context)
        {
            try
            {
                bool couldDelete = this.mainHelper.DeleteProfile(request.Username_);
                string message = couldDelete ? "Usuario eliminado correctamente" : "No se pudo eliminar usuario";
                return Task.FromResult(new MessageReply { Message = message });
            }
            catch (Exception e)
            {
                return Task.FromResult(new MessageReply()
                { Message = e.Message }
                );
            }
        }

        public override Task<MessageReply> ModifyProfile(ModifyProfileRequest request, ServerCallContext context)
        {
            try
            {
                bool couldUpdate = this.mainHelper.ModifyProfile(request.Username, request.NewPicturePath, request.NewDescription, request.NewSkills);
                string message = couldUpdate ? "Usuario actualizado correctamente" : "No se pudo actualizar usuario";
                return Task.FromResult(new MessageReply { Message = message });
            }
            catch (Exception e)
            {
                return Task.FromResult(new MessageReply()
                { Message = e.Message }
                );
            }
        }
    }
}