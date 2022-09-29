using Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Repositories
{
    public class UserRepository : IQueryable
    {
        private readonly IList<User> _userRepository;

        public UserRepository()
        {
            this._userRepository = new List<User>();
        }

        public void AddUser(User user)
        {
            this._userRepository.Add(user);
        }

        public User GetUser(string userName)
        {
            return this._userRepository.Where(u => u.Username.Equals(userName)) as User;
        }

        public bool Exists(string userName)
        {
            return this._userRepository.Any(x => x.Username.Equals(userName));
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