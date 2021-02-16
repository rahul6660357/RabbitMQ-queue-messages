using Microsoft.AspNetCore.Http;
using System;

namespace QueueModel
{
    public class EmailNotificationModel
    {
        public string AppID { get; set; }
        public string UerId { get; set; }

        public string Emailfrom { get; set; }

        public string Emailto { get; set; }

        public string CC { get; set; }

        public string BCC { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public bool HasAttachment { get; set; }

        public string AttachedContent { get; set; }


        public ApplicationLogs GetlogObject(ApplicationLogs log, HttpContext _context)
        {
            log.AppID = "Notification";
            log.ActivityDate = DateTime.Now;
            log.Browserdetails = _context.Request.Headers["User-Agent"];
            log.UerId = "";
            return log;
        }

    }


}
