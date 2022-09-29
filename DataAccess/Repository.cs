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
            //WorkProfile profile = null;
            //foreach (var workProfile in _workProfiles)
            //{
            //    if (workProfile.UserName == userName)
            //    {
            //        profile = workProfile;
            //    }
            //}
            //return profile;
        }

        public IList<WorkProfile> GetProfilesBySkills(string skill)
        {
            return WorkProfiles.Where(x => x.Skills.Contains(skill)).ToList();
            //IList<WorkProfile> workProfiles = new List<WorkProfile>();
            //foreach (var profile in _workProfiles)
            //{
            //    if (profile.Skills.Contains(skill))
            //    {
            //        workProfiles.Add(profile);
            //    }
            //}

            //return workProfiles;
        }

        public IList<WorkProfile> GetProfilesByKeyWord(string keyWord)
        {
            return WorkProfiles.Where(x => x.Description.Contains(keyWord)).ToList();
            //IList<WorkProfile> workProfiles = new List<WorkProfile>();
            //foreach (var profile in _workProfiles)
            //{
            //    if (profile.Description.Contains(keyWord))
            //    {
            //        workProfiles.Add(profile);
            //    }
            //}
            //return workProfiles;
        }

        public Type ElementType { get; }
        public Expression Expression { get; }
        public IQueryProvider Provider { get; }
    }
}