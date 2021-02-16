using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consumequeue.Services
{
    public interface INotification
    {
        bool Authenticate(string username, string password);
    }
}
