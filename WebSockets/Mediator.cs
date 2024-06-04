using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSockets;

namespace Client
{
    public interface Mediator
    {
        public Task Execute(Message msg);
    }
}
