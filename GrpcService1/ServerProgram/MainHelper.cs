using Communication;
using DataAccess;
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

        private readonly Regex _regularRegex = new Regex(@"([A - Za - z\w\-\.]{3,20} *)$");

        private Logic.MainHelper MainBusinessLogic { get; set; }

        private static MainHelper instance;
        private static readonly object singletonlock = new object();
        public LkDin lkdin;
        public IModel channel;
        private FileHandler fh = new FileHandler();

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
            var message = "";
            try
            {
                lock (this.lkdin.Users)
                {
                    var users = this.lkdin.Users;
                    User userExists = users.Find(x => x.Username.Equals(username_));
                    if (userExists == null)
                    {
                        message = "No existe un usuario con ese nombre!";
                    }
                    else
                    {
                        this.lkdin.Users.Remove(userExists);
                        message = $"Se elimino el usuario {username_} correctamente [deletion]";
                        this.Message($"Se elimino el usuario {username_} correctamente [deletion]");
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return message;
        }

        internal string ModifyUser(string username, string newName)
        {
            var message1 = "";
            try
            {
                lock (this.lkdin.Users)
                {
                    var users = this.lkdin.Users;
                    User userExists = users.Find(x => x.Username.Equals(username));
                    if (userExists != null)
                    {
                        userExists.Username = newName;
                        message1 = $"Se modifico el usuario {username} correctamente [modify]";
                        this.Message($"Se modifico el usuario {username} correctamente. Ahora su nombre es {newName} [modify]");
                    }
                    else
                    {
                        message1 = "No existe un usuario con ese nombre.";
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return message1;
        }

        public string CreateUser(string name, string surname, string username)
        {
            var message1 = "";
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
                        message1 = $"Se creo el usuario {username} correctamente [creation]";
                        this.Message($"Se creo el usuario {username} correctamente [creation]");
                    }
                    else
                    {
                        message1 = "Ya existe un usuario con ese nombre!";
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return message1;
        }

        internal string DeleteProfile(string username_)
        {
            var message = "";
            try
            {
                lock (this.lkdin.WorkProfiles)
                {
                    var profiles = this.lkdin.WorkProfiles;
                    WorkProfile profileExists = profiles.Find(x => x.UserName.Equals(username_));
                    if (profileExists == null)
                    {
                        message = "No existe un perfil con ese nombre de usuario!";
                    }
                    else
                    {
                        this.lkdin.WorkProfiles.Remove(profileExists);
                        message = $"Se elimino el perfil del usuario {username_} correctamente [deletion]";
                        this.Message($"Se elimino el perfil del usuario {username_} correctamente [deletion]");
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return message;
        }

        public string CreateWorkProfile(string username, string profilepic, string description, string skills)
        {
            var message = "";
            string[] skillSplit = skills.Split("-");
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
                        message = $"Se agrego el perfil de {username} correctamente. [creation]";
                        this.Message($"Se agrego el perfil de {username} correctamente. [creation]");
                    }
                    else
                    {
                        message = "Ya existe un perfil para ese nombre de usuario!";
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return message;
        }

        internal string ModifyProfile(string username, string? newPicturePath, string? newDescription, string? newSkills)
        {
            var message = "";
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
                        message = $"Se modifico el perfil de {username} correctamente. [modify]";
                        this.Message($"Se modifico el perfil de {username} correctamente. [modify]");
                    }
                    else
                    {
                        message = "No existe un perfil con ese nombre de usuario.";
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return message;
        }

        internal string DeleteImage(string username_)
        {
            var message = "";
            try
            {
                lock (this.lkdin.WorkProfiles)
                {
                    var profiles = this.lkdin.WorkProfiles;
                    WorkProfile profileExists = profiles.Find(x => x.UserName.Equals(username_));
                    if (profileExists == null)
                    {
                        message = "No existe un perfil con ese nombre de usuario!";
                    }
                    else
                    {
                        this.fh.DeleteFile(profileExists.ProfilePic);
                        profileExists.ProfilePic = "";
                        message = $"Se elimino la imagen del perfil del usuario {username_} correctamente [deletion]";
                        this.Message($"Se elimino la imagen del perfil del usuario {username_} correctamente [deletion]");
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return message;
        }

        internal string SearchProfileAsync(string username)
        {
            var message = "";
            try
            {
                WorkProfile found = this.lkdin.WorkProfiles.Find(prof => prof.UserName.Equals(username));
                if (found == null)
                {
                    message = "Profile does not exist";
                }
                else
                {
                    message = $"Se encontro el perfil de {username}, descargando imagen...";

                    this.Message(message + " [search]");

                    message += $"\nUserName: {found.UserName}\n" +
                               $"Description: {found.Description}\n" +
                               $"Skills: {found.Skills.ToString()}";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return message;
        }

        public string AssociateImageToProfile(string username, string path)
        {
            var message = "";
            try
            {
                WorkProfile workProf = this.lkdin.WorkProfiles.Find(u => u.UserName.Equals(username));
                if (workProf == null)
                {
                    message = "Profile does not exist";
                }
                else
                {
                    workProf.ProfilePic = path;
                    message = $"Se actualizo la imagen de perfil {username} correctamente!";
                    this.Message(message + "[modify]");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return message;
        }

        public string SearchFilters(string keyword)
        {
            var message = "";
            var profiles = this.lkdin.WorkProfiles;
            bool flag = false;
            if (profiles.Count > 0)
            {
                foreach (var profile in profiles)
                {
                    if (profile.UserName.Contains(keyword) || profile.Skills.Contains(keyword) || profile.Description.Contains(keyword))
                    {
                        message += $"\nUserName: {profile.UserName}\n" +
                           $"Description: {profile.Description}\n";
                    }
                }
                message += "\n Busqueda finalizada";
            }
            else
            {
                message = "No hay perfiles en el sistema.";
            }

            return message;
        }

        public string SendMessage(string sender, string receptor, Message newMessage)
        {
            var message = "";
            try
            {
                User receiver = this.lkdin.Users.Find(u => u.Username.Equals(receptor));
                if (receiver != null)
                {
                    receiver.MessageBox.Add(newMessage);
                    message = $"{sender} envio un mensaje a {receptor}. [message]";
                    this.Message($"{sender} envio un mensaje a {receptor}. [message]");
                }
                else
                {
                    message = "No existe dicho receptor.";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return message;
        }

        public string CheckInbox(string username)
        {
            var message = "";
            try
            {
                User user = this.lkdin.Users.Find(u => u.Username.Equals(username));
                if (user != null)
                {
                    List<Message> messagesToSend = user.MessageBox;
                    if (messagesToSend.Count == 0)
                    {
                        message = $"El usuario {username} no tiene mensajes en su inbox.";
                    }
                    else
                    {
                        message = this.lkdin.SendStringListOfMessages(messagesToSend, user);
                        this.Message(message + "[message]");
                    }
                }
                else
                {
                    message = "No existe ese usuario.";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return message;
        }
    }
}