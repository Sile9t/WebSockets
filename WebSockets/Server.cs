using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WebSockets;

namespace Client
{
    public class Server : Mediator
    {
        private IPEndPoint _self = new IPEndPoint(IPAddress.Any, 0);
        ConcurrentDictionary<string, IPEndPoint> _users { get; } 
            = new ConcurrentDictionary<string, IPEndPoint>();
        public Server() { }
        public async Task Execute(Message msg)
        {
            var command = msg._command;
            switch(command)
            {
                case Command.SendTo:
                    break;
                case Command.SendAll:
                    break;
                case Command.GetUsersList:
                    break;
                case Command.Register:
                    break;
                case Command.Delete:
                    break;
            }
        }
        public async Task<Message> GetUsersList(Message msg)
        {
            StringBuilder sb = new StringBuilder();
            EPInfo ecxept = msg._from;
            foreach (var user in _users)
                if (!user.Value.Equals(ecxept)) sb.Append(user.Key + ", ");
            sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }
        public bool Register((string name, IPEndPoint ip) user)
        {
            bool success = false;
            if (!_users.ContainsKey(user.name))
            {
                success = _users.TryAdd(user.name, user.ip);
            }
            return success;
        }
        public bool Delete((string name, IPEndPoint ip) user) 
        {
            bool success = false;
            if (_users.ContainsKey(user.name))
                success = _users.TryRemove(user.name, out _);
            return success;
        }
        public async Task SendTo(Message msg)
        {
            using (var udpClient = new UdpClient())
            {
                var gotIp = _users.TryGetValue(msg._to.name, out IPEndPoint? ip);
                if (!gotIp)
                {
                    msg._to = (msg._to.name, ip);
                    var jsonMessage = msg.SerializeToJson();
                    var buffer = Encoding.UTF8.GetBytes(jsonMessage);
                    for (int i = 0; i < 3; i++)
                        await udpClient.SendAsync(buffer, buffer.Length, msg._to.ip);
                }
            }
        }
        public async Task SendAll(Message msg)
        {
            foreach (var user in _users)
            {
                if (user.Key == msg._from.name) continue;
                var to = (user.Key, user.Value);
                msg._to = to;
                await SendTo(msg);
            }
        }
        public async Task<Message?> Receive()
        {
            Message? msg = null;
            using (var udpClient = new UdpClient(_self))
            {
                var endPoint = new IPEndPoint(IPAddress.Any, 0);
                udpClient.Connect(endPoint);
                var data = await udpClient.ReceiveAsync();
                var buffer = data.Buffer;
                var jsonMessage = Encoding.UTF8.GetString(buffer);
                msg = Message.DeserializeFromJson(jsonMessage);
            }
            return msg;
        }
    }
}
