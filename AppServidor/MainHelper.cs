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
    }
}