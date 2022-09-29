using Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccess
{
    public class Repository
    {
        public static List<User> Users { get; set; }
        public static List<WorkProfile> WorkProfiles { get; set; }
        public static Repository RepositoryInstance { get; set; }

        public Repository()
        {
            Users = new List<User>();
            WorkProfiles = new List<WorkProfile>();
        }

        public static IList<User> GetUsers()
        {
            return (Users ??= new List<User>());
        }

        public static IList<WorkProfile> GetWorkProfiles()
        {
            return (WorkProfiles ??= new List<WorkProfile>());
        }

        public static Repository GetRepository()
        {
            return RepositoryInstance ??= new Repository();
        }

        public void AddUser(User user)
        {
            Users.Add(user);
        }

        public User GetUser(string userName)
        {
            return Users.Where(u => u.Username.Equals(userName)) as User;
        }

        public bool Exists(string userName)
        {
            return Users.Any(x => x.Username.Equals(userName));
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void AddProfile(WorkProfile workProfile)
        {
            WorkProfiles.Add(workProfile);
        }

        public WorkProfile GetProfile(string userName)
        {
            return WorkProfiles.FirstOrDefault(x => x.UserName == userName);
        }

        public IList<WorkProfile> GetProfilesBySkills(string skill)
        {
            return WorkProfiles.Where(x => x.Skills.Contains(skill)).ToList();
        }

        public IList<WorkProfile> GetProfilesByKeyWord(string[] keyWords)
        {
            IList<WorkProfile> workProfiles = new List<WorkProfile>();
            foreach (var word in keyWords)
            {
                workProfiles.Concat(WorkProfiles.Where(x => x.Description.Contains(word)).ToList());
            }
            return workProfiles;
        }

        public IList<Message> GetUnreadMessages(string username)
        {
            User user = RepositoryInstance.GetUser(username);
            IList<Message> messages = new List<Message>();
            foreach (var message in user.MessageBox)
            {
                if (!message.Read)
                {
                    messages.Add(message);
                }
            }
            return messages;
        }

        public IList<string> GetMessageHistory(string username)
        {
            User user = Repository.RepositoryInstance.GetUser(username);
            string messageHead;
            IList<string> messages = new List<string>();
            foreach (var message in user.MessageBox)
            {
                if (message.Receptor.Equals(username))
                {
                    messageHead = $"{username} recieved a message from {message.Sender}";
                }
                else
                {
                    messageHead = $"{username} sent a message to {message.Receptor}";
                }

                messages.Add(messageHead);
            }
            return messages;
        }

        public void SendMessage(string sender, string receptor, string message)
        {
        }

        public Type ElementType { get; }
        public Expression Expression { get; }
        public IQueryProvider Provider { get; }
    }
}