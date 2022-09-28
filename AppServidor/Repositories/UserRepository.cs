using System;
using System.Collections;
using System.Collections.Generic;
using Domain;

namespace Repositories
{
    public class UserRepository
    {
        private static IList<User> _userRepository;
        public static void AddUser(User user)
        {
            _userRepository.Add(user);
        }

        public static User GetUser(string userName)
        {
            User returnUser = null;
            foreach (var user in _userRepository)
            {
                if (user.UserName == userName)
                {
                    returnUser = user;
                }
            }
            return returnUser;
        }
    }
}
