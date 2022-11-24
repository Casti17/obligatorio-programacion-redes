using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

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
            logs = new List<string>();
            padlock = new object();
        }

        public void AddLog(string log)
        {
            lock (padlock)
            {
                logs.Add(log);
            }
        }

        public string[] GetLogs()
        {
            lock(padlock)
            {
                return logs.ToArray();
            }
        }

        public IEnumerable<string> GetFilteredLogs(string filter)
        {
           return logs.ToArray().Where(x => x.Contains(filter));
        }
    }
}
