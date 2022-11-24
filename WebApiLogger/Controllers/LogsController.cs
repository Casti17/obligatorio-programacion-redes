using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiLogger.Data;

namespace WebApiLogger.Controllers
{
    [ApiController]
    [Route("/logs")]
    public class LogsController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> GetLogs()
        {
            return DataAccess.GetInstance().GetLogs();
        }

        [HttpGet]
        [Route("/creations")]
        public IEnumerable<string> GetCreationLogs(string filter)
        {
            return DataAccess.GetInstance().GetFilteredLogs("creation");
        }
        [HttpGet]
        [Route("/messages")]
        public IEnumerable<string> GetMessageLogs(string filter)
        {
            return DataAccess.GetInstance().GetFilteredLogs("message");
        }
        [HttpGet]
        [Route("/searchs")]
        public IEnumerable<string> GetSearchLogs(string filter)
        {
            return DataAccess.GetInstance().GetFilteredLogs("search");
        }
        [HttpGet]
        [Route("/inboxes")]
        public IEnumerable<string> GetInboxLogs(string filter)
        {
            return DataAccess.GetInstance().GetFilteredLogs("inbox");
        }
    }
}
