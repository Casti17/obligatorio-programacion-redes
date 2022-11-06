using DataAccess;
using Domain;
using Logic;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AppServidor
{
    public class MainHelper
    {
        private readonly int _maxLength = 20;
        private readonly Regex _formalRegex = new Regex("[A - Za - z]{3,20}");
        private readonly Regex _regularRegex = new Regex(@"([A - Za - z\w\-\.]{3,20} *)$");
        private Logic.MainHelper MainBusinessLogic { get; set; }

        public MainHelper()
        {
            this.MainBusinessLogic = new Logic.MainHelper();
        }

        /*private void DoAuthResponse(TextHeader textHeader, NetworkStream networkStream, int responseType)
        {
            var connectionHelper = ServerHandler.GetConnectionHelper();
            var username = connectionHelper.ReceiveTextData(textHeader, networkStream).Result;
            try
            {
                var userId = responseType == CommandConstants.Login
                                    ? LogInUser(username, networkStream)
                                    : RegisterUser(username, networkStream);

                SendIdToClient(userId, networkStream);

                Console.WriteLine($"Logged in as: {username}");
            }
            catch (UserAlreadyExistsException)
            {
                Console.WriteLine($"Sign-up Failed for requested user {username}");
                connectionHelper.SendFailedResponse(networkStream);
            }
            catch (UserNotFoundException)
            {
                Console.WriteLine($"Login Failed for requested user {username}");
                connectionHelper.SendFailedResponse(networkStream);
            }
        }*/

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

        public void CreateWorkProfile(string username, string profilepic, string description, string skills)
        {
            string[] skillSplit = skills.Split("-");
            if (DataValidator.IsValid(username, this._regularRegex) &&
                DataValidator.IsValid(username, this._regularRegex) &&
                DataValidator.IsValid(username, this._regularRegex))
            {
                try
                {
                    WorkProfile workProf = Repository.RepositoryInstance.GetProfile(username);
                    if (workProf == null)
                    {
                        List<string> skillsList = new List<string>();
                        foreach (string skill in skillSplit)
                        {
                            skillsList.Add(skill);
                        }

                        WorkProfile workProfile = new WorkProfile(username, description, skillsList);
                        Repository.WorkProfiles.Add(workProfile);
                    }
                }
                catch
                {
                    throw new Exception("Ya existe un perfil para ese nombre de usuario");
                }
            }
        }

        public void AssociateImageToProfile(string username, string path)
        {
            try
            {
                WorkProfile workProf = Repository.RepositoryInstance.GetProfile(username);
                if (workProf == null)
                {
                    throw new Exception("Profile does not exist");
                }
                workProf.ProfilePic = path;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public List<WorkProfile> SearchFilters(string[] dataArray)
        {
            return (List<WorkProfile>)Repository.RepositoryInstance.GetProfilesByKeyWord(dataArray);
        }

        public WorkProfile Search(string data)
        {
            return Repository.RepositoryInstance.GetProfile(data);
        }

        public void SendMessage(string sender, string receptor, string message)
        {
            Repository.RepositoryInstance.SendMessage(sender, receptor, message);
        }

        public List<Message> CheckInbox(string username)
        {
            List<Message> unreads = (List<Message>)Repository.RepositoryInstance.GetUnreadMessages(username);
            return unreads;
        }
    }
}