using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class User
    {
        public User(string name, string surname, string username)
        {
            Name = name;
            Surname = surname;
            Username = username;
        }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public IEnumerable<WorkProfile> Profiles { get; set; }
        public List<Message> MessageBox { get; set; }
    }
}
