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
            Message ans = new Message("", Command.SendTo, msg._to!, msg._from);
            switch(command)
            {
                case Command.SendTo:
                    await SendTo(msg);
                    ans._text = "Message sent";
                    ans._date = DateTime.Now;
                    break;
                case Command.SendAll:
                    await SendAll(msg);
                    ans._text = "Message sent";
                    ans._date = DateTime.Now;
                    break;
                case Command.GetUsersList:
                    ans._text = GetUsersList(msg._from);
                    ans._date = DateTime.Now;
                    break;
                case Command.Register:
                    var isRegistred = Register(msg._from);
                    ans._text = isRegistred? "Registred" : "This name is already exist";
                    ans._date = DateTime.Now;
                    break;
                case Command.Delete:
                    var isDeleted = Delete(msg._from);
                    ans._text = isDeleted? "U have been deleted" : "No user with name like this";
                    ans._date = DateTime.Now;
                    break;
            }
            await SendTo(ans);
        }
        public string GetUsersList((string name, IPEndPoint ip) ecxept)
        {
            StringBuilder sb = new StringBuilder();
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
