using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class EPInfo
    {
        string _name { get; set; }
        IPEndPoint _ip {  get; set; }
        public EPInfo(string name, IPEndPoint ip)
        {
             _name = name;
            _ip = ip;
        }
    }
}
