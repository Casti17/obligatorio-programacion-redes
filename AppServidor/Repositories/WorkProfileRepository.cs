﻿using System;
using System.Collections.Generic;
using System.Text;
using Domain;

namespace Repositories
{
    public class WorkProfileRepository
    {
        private static IList<WorkProfile> _workProfiles;

        public static void AddProfile(WorkProfile workProfile)
        {
            _workProfiles.Add(workProfile);
        }

        public static WorkProfile GetProfile(string userName)
        {
            WorkProfile profile = null;
            foreach (var workProfile in _workProfiles)
            {
                if (workProfile.UserName == userName)
                {
                    profile = workProfile;
                }
            }
            return profile;
        }

        public static IList<WorkProfile> GetProfilesBySkills(string skill)
        {
            IList<WorkProfile> workProfiles = new List<WorkProfile>();
            foreach (var profile in _workProfiles)
            {
                if (profile.Skills.Contains(skill))
                {
                    workProfiles.Add(profile);
                }
            }

            return workProfiles;
        }
        public static IList<WorkProfile> GetProfilesByKeyWord(string keyWord)
        {
            IList<WorkProfile> workProfiles = new List<WorkProfile>();
            foreach (var profile in _workProfiles)
            {
                if (profile.Description.Contains(keyWord))
                {
                    workProfiles.Add(profile);
                }
            }
            return workProfiles;
        }
    }
}
