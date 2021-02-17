using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Consumequeue.ViewModels
{
    public class DataViewModel : Hub

    {
        public string username { get; set; }
        public string password { get; set; }
        public bool checkresult { get; set; }
        public string messages { get; set; }
        public HttpStatusCode statusCode { get; set; }
        public string message { get; set; }
    }
}
