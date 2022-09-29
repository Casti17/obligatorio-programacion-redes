using DataAccess;
using Domain;
using Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Logic
{
    public class MainHelper : IMainHelper
    {
        private readonly int _maxLength = 20;
        private readonly Regex _formalRegex = new Regex("[A - Za - z]{3,20}");
        private readonly Regex _regularRegex = new Regex(@"([A - Za - z\w\-\.]{3,20} *)$");

        public MainHelper()
        {
            Repository.GetRepository();
        }

        public void CreateUser(string userAndGameName)
        {
            throw new System.NotImplementedException();
        }

        public void CreateUser(string name, string surname, string username)
        {
            if (DataValidator.IsValid(name, this._formalRegex) && DataValidator.IsValid(surname, this._formalRegex) && DataValidator.IsValid(username, this._regularRegex))
            {
                try
                {
                    var users = (List<User>)Repository.GetUsers();
                    User userExists = Repository.RepositoryInstance.GetUser(username);
                    if (userExists == null)
                    {
                        User newUser = new User(name, surname, username);
                        Repository.Users.Add(newUser);
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
            User user = Repository.RepositoryInstance.GetUser(username);
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
            User user = Repository.RepositoryInstance.GetUser(username);
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

        public void SendMessage(string sender, string receptor, string message)
        {
        }
    }
}