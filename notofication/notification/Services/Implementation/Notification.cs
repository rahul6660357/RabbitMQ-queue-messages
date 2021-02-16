using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace notification.Services.Implementation
{
    public class Notification : INotification
    {
        public bool Authenticate(string username, string password)
        {
            if(username=="test1234@gmail.com" && password== "test@1234")
            {
                return true;
            }
            return false;
        }
    }
}
