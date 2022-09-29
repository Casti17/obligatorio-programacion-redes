using BusinessLogic;
using Communication;
using Domain;
using Repositories;
using System;
using System.Net.Sockets;
using System.Text;

namespace AppServidor
{
    public class ServerHandler
    {
        private readonly Socket socketClient;
        private readonly UserLogic userLogic;
        private readonly WorkProfileLogic workProfileLogic;
        private readonly UserRepository userRepository;
        private readonly WorkProfileRepository workProfileRepository;
        private readonly SocketHelper socketHelper;

        public ServerHandler(Socket socketClient, UserLogic userLogic, WorkProfileLogic workProfileLogic, UserRepository userRepository, WorkProfileRepository workProfileRepository)
        {
            this.socketClient = socketClient;
            this.socketHelper = new SocketHelper(this.socketClient);
            this.userLogic = userLogic;
            this.workProfileLogic = workProfileLogic;
            this.userRepository = userRepository;
            this.workProfileRepository = workProfileRepository;
            bool isClientConnected = true;
            Console.WriteLine("segundo");
            while (isClientConnected)
            {
                try
                {
                    byte[] buffer = this.socketHelper.Receive(ProtocolConstants.CommandLength + ProtocolConstants.DataLength);
                    var header = new Header(buffer);
                    byte[] bufferData = this.socketHelper.Receive(header.IDataLength);
                    var data = Encoding.UTF8.GetString(bufferData);
                    string[] dataArray;
                    switch (header.ICommand)
                    {
                        case 1:
                            dataArray = data.Split('#');
                            userLogic.CreateUser(dataArray[0], dataArray[1], dataArray[2]);
                            var user = this.userRepository.GetUser(dataArray[2]);
                            Console.WriteLine("hola");
                            break;

                        case 2:
                            dataArray = data.Split('#');
                            workProfileLogic.CreateWorkProfile(dataArray[0], dataArray[1], dataArray[2], dataArray[3]);
                            break;

                        case 3:
                            //workProfileLogic.UpdatePicture()
                            break;

                        case 4:
                            workProfileRepository.GetProfilesBySkills(data);
                            break;

                        case 5:
                            workProfileRepository.GetProfile(data);
                            break;

                        case 6:
                            dataArray = data.Split('#');
                            Message message = new Message(dataArray[0], dataArray[1], dataArray[2]);
                            break;
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}