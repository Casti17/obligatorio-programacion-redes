namespace BusinessLogic
{
    /*public class WorkProfileLogic
    {
        private WorkProfileRepository _workProfileRepository;
        private readonly Regex _formalRegex = new Regex("[A - Za - z]{3,20}");
        private readonly Regex _regularRegex = new Regex(@"([A - Za - z\w\-\.]{3,20} *)$");

        public WorkProfileLogic(WorkProfileRepository workProfileRepo)
        {
            this._workProfileRepository = workProfileRepo;
        }

        public void CreateWorkProfile(string username, string profilepic, string description, string skills)
        {
            string[] skillSplit = skills.Split("-");
            if (DataValidator.IsValid(username, this._regularRegex) &&
                DataValidator.IsValid(username, this._regularRegex) &&
                DataValidator.IsValid(username, this._regularRegex))
            {
                try
                {
                    WorkProfile workProf = this._workProfileRepository.GetProfile(username);
                    if (workProf == null)
                    {
                        List<string> skillsList = new List<string>();
                        foreach (string skill in skillSplit)
                        {
                            skillsList.Add(skill);
                        }

                        WorkProfile workProfile = new WorkProfile(username, description, skillsList);
                        this._workProfileRepository.AddProfile(workProfile);
                    }
                }
                catch
                {
                    throw new Exception("Ya existe un perfil para ese nombre de usuario");
                }
            }
        }
    }*/
}