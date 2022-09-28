using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class User
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string UserName { get; set; }
        public IEnumerable<WorkProfile> Profiles { get; set; }
        public List<Message> MessageBox { get; set; }
    }
}
