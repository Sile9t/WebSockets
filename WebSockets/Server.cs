using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WebSockets;

namespace Client
{
    public class Server : Mediator
    {
        private EPInfo _self = new EPInfo("Server", new IPEndPoint(IPAddress.Any, 0));
        ConcurrentDictionary<string, IPEndPoint> _users { get; } 
            = new ConcurrentDictionary<string, IPEndPoint>();
        public Server() { }
        public async Task Execute(Message msg)
        {
            var command = msg._command;
            Message ans = new Message("", Command.Answer, _self, msg._from._name);
            switch(command)
            {
                case Command.SendTo:
                    await SendTo(msg);
                    break;
                case Command.SendAll:
                    await SendAll(msg);
                    break;
                case Command.GetUsersList:
                    msg._text = GetUsersList(msg);
                    break;
                case Command.Register:
                    msg._text = Register(msg._from);
                    break;
                case Command.Delete:
                    msg._text = Delete(msg._from);
                    break;
            }
            msg._date = DateTime.Now;
            await SendTo(msg);
        }
        private string GetUsersList(Message msg)
        {
            StringBuilder sb = new StringBuilder();
            var ecxept = msg._from;
            foreach (var user in _users)
                if (!user.Value.Equals(ecxept._ip)) sb.Append(user.Key + ", ");
            sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }
        private string Register(EPInfo user)
        {
            bool success = false;
            if (!_users.ContainsKey(user._name))
            {
                success = _users.TryAdd(user._name, user._ip!);
            }
            if (success) return "You are registred.";
            return "User is already exist. Try to change your name.";
        }
        private string Delete(EPInfo user) 
        {
            bool success = false;
            if (_users.ContainsKey(user._name))
                success = _users.TryRemove(user._name, out _);
            if (success) return "You are deleted.";
            return "No user like this.";
        }
        private async Task SendTo(Message msg)
        {
            using (var udpClient = new UdpClient())
            {
                var gotIp = _users.TryGetValue(msg._to, out IPEndPoint ip);
                if (!gotIp)
                {
                    var jsonMessage = msg.SerializeToJson();
                    var buffer = Encoding.UTF8.GetBytes(jsonMessage);
                    for (int i = 0; i < 3; i++)
                        await udpClient.SendAsync(buffer, buffer.Length, ip);
                }
            }
        }
        private async Task SendAll(Message msg)
        {
            foreach (var user in _users)
            {
                if (user.Key == msg._from._name) continue;
                msg._to = user.Key;
                await SendTo(msg);
            }
        }
        public async Task<Message> Receive()
        {
            Message msg;
            using (var udpClient = new UdpClient(_self._ip!))
            {
                var endPoint = new IPEndPoint(IPAddress.Any, 0);
                udpClient.Connect(endPoint);
                var data = await udpClient.ReceiveAsync();
                var buffer = data.Buffer;
                var jsonMessage = Encoding.UTF8.GetString(buffer);
                msg = Message.DeserializeFromJson(jsonMessage)!;
            }
            return msg;
        }
    }
}
