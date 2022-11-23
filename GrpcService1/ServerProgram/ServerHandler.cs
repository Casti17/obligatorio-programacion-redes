using Communication;
using Domain;
using Domain.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrpcService1.ServerProgram
{
    public class ServerHandler
    {
        private FileCommsHandler communication;
        public LkDin lkdin;

        private async Task CheckInbox(Header header)
        {
            var message = "";
            var bufferInfo = new byte[header.IDataLength];

            await this.communication.ReceiveDataAsync(header.IDataLength, bufferInfo);
            ProfileSearchInfo receivedProfile = new ProfileSearchInfo();
            receivedProfile.Decode(bufferInfo);
            User user = this.lkdin.Users.Find(u => u.Username.Equals(receivedProfile.Username));
            if (user != null)
            {
                List<Message> messagesToSend = user.MessageBox;
                if (messagesToSend.Count == 0)
                {
                    message = user.Username + " No tiene mensajes para mostrar";
                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                }
                else
                {
                    message = this.lkdin.SendStringListOfMessages(messagesToSend, user);
                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                }
            }
            else
            {
                message = "No existe ese usuario.";
                await this.communication.SendDataAsync(message, Commands.ServerResponse);
            }
        }

        private async Task SendMessageAsync(Header header)
        {
            try
            {
                var message = "";
                var bufferInfo = new byte[header.IDataLength];
                await this.communication.ReceiveDataAsync(header.IDataLength, bufferInfo);
                SendMessageInfo receivedMessage = new SendMessageInfo();
                receivedMessage.Decode(bufferInfo);
                Message newMessage = receivedMessage.ToEntity();
                User receiver = this.lkdin.Users.Find(u => u.Username.Equals(receivedMessage.Receiver));
                if (receiver != null)
                {
                    receiver.MessageBox.Add(newMessage);
                    message = $"El mensaje fue enviado al usuario {receiver.Username} correctamente.";
                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                }
                else
                {
                    message = "No existe dicho usuario receptor.";
                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task SearchProfileAsync(Header header)
        {
            try
            {
                var message = "";
                var bufferInfo = new byte[header.IDataLength];
                await this.communication.ReceiveDataAsync(header.IDataLength, bufferInfo);
                ProfileSearchInfo receivedProfile = new ProfileSearchInfo();
                receivedProfile.Decode(bufferInfo);
                WorkProfile found = this.lkdin.WorkProfiles.Find(prof => prof.UserName.Equals(receivedProfile.Username));
                Console.WriteLine(receivedProfile.Username);
                if (found != null)
                {
                    message = "Se encontro el perfil, descargando imagen...";
                    FileStreamHandler fh = new FileStreamHandler();

                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                    await this.communication.SendFileAsync(found.ProfilePic, fh);
                }
                else
                {
                    message = "Ese perfil no existe.";
                    await this.communication.SendDataAsync(message, Commands.ServerResponse);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task CreateWorkProfile(Header header)
        {
            try
            {
                var message = "";
                var bufferData = new byte[header.IDataLength];
                await this.communication.ReceiveDataAsync(header.IDataLength, bufferData);
                ProfileInfo receivedProfile = new ProfileInfo();
                receivedProfile.Decode(bufferData);
                WorkProfile newProfile = receivedProfile.ToEntity();
                this.lkdin.WorkProfiles.Add(newProfile);
                FileStreamHandler fh = new FileStreamHandler();
                await this.communication.ReceiveFileAsync(fh);
                WorkProfile x = this.lkdin.WorkProfiles.Find(n => n.UserName.Equals(newProfile.UserName));
                message = $"El perfil de  {x.UserName} se agrego correctamente.";
                await this.communication.SendDataAsync(message, Commands.ServerResponse);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task CreateUser(Header header)
        {
            try
            {
                var bufferData = new byte[header.IDataLength];
                await this.communication.ReceiveDataAsync(header.IDataLength, bufferData);
                UserInfo receivedInfo = new UserInfo();
                receivedInfo.Decode(bufferData);
                //this.businessLogic.CreateUser(receivedInfo);
                User newUser = receivedInfo.ToEntity();

                this.lkdin.Users.Add(newUser);
                User x = this.lkdin.Users.Find(n => n.Username.Equals(newUser.Username));
                var message = $"El usuario {x.Username} se agrego correctamente.";
                await this.communication.SendDataAsync(message, Commands.ServerResponse);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}