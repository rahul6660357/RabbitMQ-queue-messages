using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace notification.ViewModels
{
    public class DataViewModel : ResultViewModel

    {
        public string username { get; set; }
        public string password { get; set; }
        public bool checkresult { get; set; }
        public string messages { get; set; }

    }
}
