using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using AppServidor.BusinessLogic;
using Domain;
using Repositories;

namespace BusinessLogic
{
    public class WorkProfileLogic
    {
        private WorkProfileRepository _workProfileRepository;
        private Regex _formalRegex = new Regex("[A - Za - z]{3,20}");
        private Regex _regularRegex = new Regex(@"([A - Za - z\w\-\.]{3,20} *)$");
        public WorkProfileLogic(WorkProfileRepository workProfileRepo)
        {
            _workProfileRepository = workProfileRepo;
        }
        public void CreateWorkProfile(string username, string profilepic, string description, string skills)
        {
            string[] skillSplit = skills.Split("-");
            if (DataValidator.IsValid(username, _regularRegex) &&
                DataValidator.IsValid(username, _regularRegex) &&
                DataValidator.IsValid(username, _regularRegex))
            {
                try
                {
                    WorkProfile workProf = _workProfileRepository.GetProfile(username);
                    if (workProf == null)
                    {
                        List<string> skillsList = new List<string>();
                        foreach (string skill in skillSplit)
                        {
                            skillsList.Add(skill);
                        }

                        WorkProfile workProfile = new WorkProfile(username, description, skillsList);
                        _workProfileRepository.AddProfile(workProfile);
                    }
                }
                catch
                {
                    throw new Exception("Ya existe un perfil para ese nombre de usuario");
                }
            }


        }
    }
}
