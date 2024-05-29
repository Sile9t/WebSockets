using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public interface Mediator
    {
        public Task Execute(BaseComponent sender, Command command);
    }
}
