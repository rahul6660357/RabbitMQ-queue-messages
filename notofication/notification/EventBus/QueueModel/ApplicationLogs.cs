using Microsoft.AspNetCore.Http;
using System;

namespace QueueModel
{
    public class ApplicationLogs
    {
        public string AppID { get; set; }
        public DateTime ActivityDate { get; set; }
        public string UerId { get; set; }

        public string Browserdetails { get; set; }

        public string ActivityType { get; set; }

        public string ActivitySubType { get; set; }

        public int Severity { get; set; }

        public int Severitylevel { get; set; }

        public string LogMessage { get; set; }


        public ApplicationLogs ()
        {
            this.AppID = "Notification";
            this.ActivityDate = DateTime.Now;
            this.Browserdetails = "Not Applicable";
            this.UerId = "";
            
        }

    }


}
