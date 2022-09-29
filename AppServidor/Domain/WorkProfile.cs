using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Domain
{
    public class WorkProfile
    {
        public WorkProfile(string username, string description, List<string> skills)
        {
            UserName = username;
            Description = description;
            Skills = skills;
        }
        public string ProfilePic { get; set; }
        public string Description { get; set; }
        public List<string> Skills { get; set; }
        public string UserName { get; set; }
    }
}
