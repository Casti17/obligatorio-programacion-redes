using System;
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
    }
}
