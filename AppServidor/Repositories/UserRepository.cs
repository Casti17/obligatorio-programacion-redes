using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using AppServidor.Domain;
using Domain;

namespace Repositories
{
    public class UserRepository : IQueryable
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
            return _userRepository.Where(u => u.Username.Equals(userName)) as User;
        }

        public bool Exists(string userName)
        {
            return _userRepository.Any(x => x.Username.Equals(userName));
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public Type ElementType { get; }
        public Expression Expression { get; }
        public IQueryProvider Provider { get; }
    }
}
