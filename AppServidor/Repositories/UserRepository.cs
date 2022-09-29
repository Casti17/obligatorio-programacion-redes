using System;
using System.Collections;
using System.Collections.Generic;
using AppServidor.Domain;
using Domain;

namespace Repositories
{
    public class UserRepository
    {
        private IList<User> _userRepository;
        public UserRepository()
        {
            _userRepository = new List<User>();
        }
       
        public void AddUser(User user)
        {
            _userRepository.Add(user);
        }

        public User GetUser(string userName)
        {
            User returnUser = null;
            foreach (var user in _userRepository)
            {
                if (user.Username == userName)
                {
                    returnUser = user;
                }
            }
            return returnUser;
        }
    }
}
