﻿using DataAccess;
using Domain;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GrpcService1
{
    public class MainHelper
    {
        private readonly int _maxLength = 20;

        //private readonly Regex _formalRegex = new Regex("[A - Za - z]{3,20}");
        private readonly Regex _regularRegex = new Regex(@"([A - Za - z\w\-\.]{3,20} *)$");

        private Logic.MainHelper MainBusinessLogic { get; set; }

        private static MainHelper instance;
        private static readonly object singletonlock = new object();
        public LkDin lkdin;
        public IModel channel;

        public MainHelper()
        {
            this.lkdin = LkDin.GetInstance();
        }

        public static MainHelper GetInstance()
        {
            lock (singletonlock)
            {
                if (instance == null)
                {
                    instance = new MainHelper();
                }
            }
            return instance;
        }

        public string Message(string message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "logger",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "",
                    routingKey: "logger",
                    basicProperties: null,
                    body: body);
            }
            return message;
        }

        public string DeleteUser(string username_)
        {
            try
            {
                lock (this.lkdin.Users)
                {
                    var users = this.lkdin.Users;
                    User userExists = users.Find(x => x.Username.Equals(username_));
                    if (userExists == null)
                    {
                        throw new Exception("No existe un usuario con ese nombre!");
                    }
                    else
                    {
                        this.lkdin.Users.Remove(userExists);
                        this.Message($"Se elimino el usuario {username_} correctamente [deletion]");
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return "Usuario eliminado con exito!";
        }

        internal string ModifyUser(string username, string newName)
        {
            try
            {
                lock (this.lkdin.Users)
                {
                    var users = this.lkdin.Users;
                    User userExists = users.Find(x => x.Username.Equals(username));
                    if (userExists != null)
                    {
                        userExists.Username = newName;
                        this.Message($"Se modifico el usuario {username} correctamente. Ahora su nombre es {newName} [modify]");
                    }
                    else
                    {
                        throw new Exception("No existe un usuario con ese nombre.");
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return "Usuario modificado correctamente";
        }

        public string CreateUser(string name, string surname, string username)
        {
            try
            {
                lock (this.lkdin.Users)
                {
                    var users = this.lkdin.Users;
                    User userExists = users.Find(x => x.Username.Equals(username));
                    if (userExists == null)
                    {
                        User newUser = new User(name, surname, username);
                        this.lkdin.Users.Add(newUser);
                        this.Message($"Se creo el usuario {username} correctamente [creation]");
                    }
                    else
                    {
                        throw new Exception("Ya existe un usuario con ese nombre!");
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return "Usuario creado correctamente";
        }

        internal string DeleteProfile(string username_)
        {
            try
            {
                lock (this.lkdin.WorkProfiles)
                {
                    var profiles = this.lkdin.WorkProfiles;
                    WorkProfile profileExists = profiles.Find(x => x.UserName.Equals(username_));
                    if (profileExists == null)
                    {
                        throw new Exception("No existe un perfil con ese nombre de usuario!");
                    }
                    else
                    {
                        this.lkdin.WorkProfiles.Remove(profileExists);
                        this.Message($"Se elimino el perfil del usuario {username_} correctamente [deletion]");
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return "Perfil eliminado con exito!";
        }

        public string CreateWorkProfile(string username, string profilepic, string description, string skills)
        {
            string[] skillSplit = skills.Split("-");
            /*if (DataValidator.IsValid(username, this._regularRegex) &&
                DataValidator.IsValid(username, this._regularRegex) &&
                DataValidator.IsValid(username, this._regularRegex))
            {*/
            try
            {
                lock (this.lkdin.WorkProfiles)
                {
                    WorkProfile workProf = this.lkdin.WorkProfiles.Find(u => u.UserName.Equals(username));
                    if (workProf == null)
                    {
                        List<string> skillsList = new List<string>();
                        foreach (string skill in skillSplit)
                        {
                            skillsList.Add(skill);
                        }

                        WorkProfile workProfile = new WorkProfile(username, description, skillsList);
                        workProfile.ProfilePic = profilepic;
                        this.lkdin.WorkProfiles.Add(workProfile);
                    }
                    else
                    {
                        throw new Exception("Ya existe un perfil para ese nombre de usuario!");
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            //}
            return "Se agrego el perfil del usuario con exito!";
        }

        internal string ModifyProfile(string username, string? newPicturePath, string? newDescription, string? newSkills)
        {
            try
            {
                lock (this.lkdin.WorkProfiles)
                {
                    string[] skillSplit = newSkills.Split("-");
                    List<string> skillsList = new List<string>();
                    foreach (string skill in skillSplit)
                    {
                        skillsList.Add(skill);
                    }
                    var profiles = this.lkdin.WorkProfiles;
                    WorkProfile profileExists = profiles.Find(x => x.UserName.Equals(username));
                    if (profileExists != null)
                    {
                        profileExists.ProfilePic = newPicturePath;
                        profileExists.Description = newDescription;
                        profileExists.Skills = skillsList;
                        this.Message($"Se modifico el perfil de {username} correctamente. [modify]");
                    }
                    else
                    {
                        throw new Exception("No existe un perfil con ese nombre de usuario.");
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return "Perfil modificado correctamente";
        }

        internal string DeleteImage(string username_)
        {
            try
            {
                lock (this.lkdin.WorkProfiles)
                {
                    var profiles = this.lkdin.WorkProfiles;
                    WorkProfile profileExists = profiles.Find(x => x.UserName.Equals(username_));
                    if (profileExists == null)
                    {
                        throw new Exception("No existe un perfil con ese nombre de usuario!");
                    }
                    else
                    {
                        //this.lkdin.WorkProfiles.Remove(profileExists);
                        profileExists.ProfilePic = "";
                        this.Message($"Se elimino la imagen del perfil del usuario {username_} correctamente [deletion]");
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return "Imagen eliminada con exito!";
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