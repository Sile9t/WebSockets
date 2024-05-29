using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class BaseComponent
    {
        private Mediator _mediator;
        public BaseComponent(Mediator mediator)
        {
            _mediator = mediator;
        }
        public void SendTo()
        {
            _mediator.Execute(this, Command.SendTo);
        }
        public void SendAll()
        {
            _mediator.Execute(this, Command.SendAll);
        }
        public void Receive()
        {
            _mediator.Execute(this, Command.Receive);
        }
    }
}
