using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Domain;
using Repositories;

namespace BusinessLogic
{
    public class WorkProfileLogic
    {
        private WorkProfileRepository _workProfileRepository;
        public WorkProfileLogic(WorkProfileRepository workProfileRepo)
        {
            _workProfileRepository = workProfileRepo;
        }
        public void CreateWorkProfile(string username, string profilepic, string description, string skills)
        {
            string[] skillSplit = skills.Split("-");
            //validar datos


        }
    }
}
