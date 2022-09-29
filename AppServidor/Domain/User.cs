using System.Collections.Generic;
using Domain;

namespace AppServidor.Domain
{
    public class User
    {
        public User(string name, string surname, string username)
        {
            this.Name = name;
            this.Surname = surname;
            this.Username = username;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public IEnumerable<WorkProfile> Profiles { get; set; }
        public List<Message> MessageBox { get; set; }
    }
}