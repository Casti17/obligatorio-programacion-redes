using System.Collections.Generic;
using System.Text;

namespace Domain.DTO
{
    public class UserInfo
    {
        public string Username { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string MessageBox { get; set; }

        public string Profiles { get; set; }

        public User ToEntity()
        {
            return new User()
            {
                Username = Username,
                Name = Name,
                Surname = Surname,
                MessageBox = new List<Message>(),
                Profiles = new List<WorkProfile>()
            };
        }

        public string Code()
        {
            return $"{this.Name}#{this.Surname}#{this.Username}#{this.MessageBox}#{this.Profiles}";
        }

        public void Decode(byte[] data)
        {
            string[] attributes = Encoding.UTF8.GetString(data).Split("#");

            this.Name = attributes[0];
            this.Surname = attributes[1];
            this.Username = attributes[2];
            this.MessageBox = attributes[3];
            this.Profiles = attributes[4];
        }
    }
}