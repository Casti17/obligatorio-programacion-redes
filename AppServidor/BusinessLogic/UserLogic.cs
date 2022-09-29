using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using AppServidor.BusinessLogic;
using AppServidor.Domain;
using Domain;
using Repositories;

namespace BusinessLogic
{
    public class UserLogic
    {
        private static UserRepository _userRepository;
        private int _maxLength = 20;
        private Regex _formalRegex = new Regex("[A - Za - z]{3,20}");
        private Regex _regularRegex = new Regex(@"([A - Za - z\w\-\.]{3,20} *)$");
        public UserLogic(UserRepository userRepo)
        {
            _userRepository = userRepo;
        }
        public void CreateUser(string name, string surname, string username)
        {
            if (DataValidator.IsValid(name,_formalRegex) && DataValidator.IsValid(surname,_formalRegex) && DataValidator.IsValid(username,_regularRegex))
            {
                try
                {
                    User userExists = _userRepository.GetUser(username);
                    if (userExists == null)
                    {
                        User newUser = new User(name, surname, username);
                        _userRepository.AddUser(newUser);
                    }
                }
                catch
                {
                    throw new Exception("El usuario ya existe");
                }
            }
            else
            {
                throw new ArgumentException("Los datos Ingresados son erroneos");
            }
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

        public void SendMessage(string sender,string receptor, string message)
        {
            
        }
    }
}
