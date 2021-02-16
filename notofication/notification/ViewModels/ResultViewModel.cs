using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace notification.ViewModels
{
    public class ResultViewModel
    {
        public HttpStatusCode statusCode { get; set; }
        public string message { get; set; }

    }
}
