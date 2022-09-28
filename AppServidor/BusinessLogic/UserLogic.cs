using System;
using Domain;
using Repositories;

namespace BusinessLogic
{
    public static class UserLogic
    {
        public static void CreateUser(string username, string name, string surname)
        {
            //validar datos

            User newUser = new User
            {
                UserName = username,
                Name = name,
                Surname = surname
            };
            UserRepository.AddUser(newUser);
        }
    }
}
