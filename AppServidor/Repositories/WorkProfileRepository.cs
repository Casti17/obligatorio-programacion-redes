using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Domain;

namespace Repositories
{
    public class WorkProfileRepository : IQueryable
    {
        private IList<WorkProfile> _workProfiles;

        public WorkProfileRepository()
        {
            _workProfiles = new List<WorkProfile>();
        }

        public void AddProfile(WorkProfile workProfile)
        {
            _workProfiles.Add(workProfile);
        }

        public WorkProfile GetProfile(string userName)
        {
            return _workProfiles.FirstOrDefault(x => x.UserName == userName);
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
            return _workProfiles.Where(x => x.Skills.Contains(skill)).ToList();
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
            return _workProfiles.Where(x => x.Description.Contains(keyWord)).ToList();
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

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public Type ElementType { get; }
        public Expression Expression { get; }
        public IQueryProvider Provider { get; }
    }
}
