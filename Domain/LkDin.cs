using System.Collections.Generic;
using System.Threading;

namespace Domain
{
    public class LkDin
    {
        public List<WorkProfile> WorkProfiles { get; set; }
        public List<User> Users { get; set; }
        private static LkDin instance;
        private static readonly SemaphoreSlim _instanceSemaphore = new SemaphoreSlim(1);

        public LkDin()
        {
            this.WorkProfiles = new List<WorkProfile>();
            this.Users = new List<User>();
        }

        public static LkDin GetInstance()
        {
            _instanceSemaphore.WaitAsync();
            if (instance == null)
            {
                instance = new LkDin();
            }
            _instanceSemaphore.Release();

            return instance;
        }

        public string SendStringListOfMessages(List<Message> messagesToSend, User user)
        {
            string returns = " \n" + "MENSAJES DE " + user.Username.ToUpper() + " \n";
            for (int i = 0; i < messagesToSend.Count; i++)
            {
                returns = returns + "Mensaje #" + i + " :\n" + "Enviado por : " + messagesToSend[i].Sender + " \n" +
                          "Contenido: " + messagesToSend[i].MessageBody + "\n\n";
            }
            return returns;
        }

        public string GetProfilesString()
        {
            string ret = "";
            foreach (WorkProfile profile in WorkProfiles)
            {
                ret += $"\nUserName: {profile.UserName}\n" +
                       $"Description: {profile.Description}\n";
            }
            return ret;
        }
    }
}