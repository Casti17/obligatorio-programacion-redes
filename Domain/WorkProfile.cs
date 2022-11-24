using System.Collections.Generic;

namespace Domain
{
    public class WorkProfile
    {
        public WorkProfile(string username, string description, List<string> skills)
        {
            this.UserName = username;
            this.Description = description;
            this.Skills = skills;
        }

        public WorkProfile()
        {
        }

        public string ProfilePic { get; set; }
        public string Description { get; set; }
        public List<string> Skills { get; set; }
        public string UserName { get; set; }
    }
}