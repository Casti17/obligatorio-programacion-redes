using System;
using System.Collections.Generic;
using AppServidor.Domain;
using Domain;
using Repositories;

namespace BusinessLogic
{
    public class UserLogic
    {
        private static UserRepository _userRepository;
        public UserLogic(UserRepository userRepo)
        {
            _userRepository = userRepo;
        }
        public void CreateUser(string name, string surname, string username)
        {
            //validar datos

            User newUser = new User(name, surname, username);
            _userRepository.AddUser(newUser);
        }

        public IList<Message> GetUnreadMessages(string username)
        {
            User user = _userRepository.GetUser(username);
            IList<Message> messages = new List<Message>();
            foreach (var message in user.MessageBox)
            {
                if (!message.Read)
                {
                    messages.Add(message);
                }
            }
            return messages;
        }

        public IList<string> GetMessageHistory(string username)
        {
            User user = _userRepository.GetUser(username);
            string messageHead;
            IList<string> messages = new List<string>();
            foreach (var message in user.MessageBox)
            {
                if (message.Receptor.Equals(username))
                {
                    messageHead = $"{username} recieved a message from {message.Sender}";
                }
                else
                {
                    messageHead = $"{username} sent a message to {message.Receptor}";
                }

                messages.Add(messageHead);
            }
            return messages;
        }
    }
}
