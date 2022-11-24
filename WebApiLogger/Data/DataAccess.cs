using System.Collections.Generic;
using System.Linq;

namespace WebApiLogger.Data
{
    public class DataAccess
    {
        private object padlock;
        private static DataAccess instance;
        private List<string> logs;

        private static object singletonPadlock = new object();

        public static DataAccess GetInstance()
        {
            lock (singletonPadlock)
            { // bloqueante
                if (instance == null)
                {
                    instance = new DataAccess();
                }
            }
            return instance;
        }

        private DataAccess()
        {
            this.logs = new List<string>();
            this.padlock = new object();
        }

        public void AddLog(string log)
        {
            lock (this.padlock)
            {
                this.logs.Add(log);
            }
        }

        public string[] GetLogs()
        {
            lock (this.padlock)
            {
                return this.logs.ToArray();
            }
        }

        public IEnumerable<string> GetFilteredLogs(string filter)
        {
            return this.logs.ToArray().Where(x => x.Contains(filter));
        }
    }
}