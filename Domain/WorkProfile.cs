using System.Collections.Generic;

namespace Domain
{
    public class WorkProfile
    {
        public WorkProfile(string username, string description, string skills, string imagen)
        {
            this.UserName = username;
            this.Description = description;
            this.Skills = skills;
            this.ProfilePic = imagen;
        }

        public WorkProfile()
        {
        }

        public string ProfilePic { get; set; }
        public string Description { get; set; }
        public string Skills { get; set; }
        public string UserName { get; set; }
    }
}