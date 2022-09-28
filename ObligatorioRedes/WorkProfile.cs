using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Domain
{
    public class WorkProfile
    {
        public WorkProfile(string profilePic, string description, List<string> skills)
        {
            ProfilePic = profilePic;
            Description = description;
            Skills = skills;
        }
        public string ProfilePic { get; set; }
        public string Description { get; set; }
        public List<string> Skills { get; set; }
    }
}
