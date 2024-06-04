using System.Net.Sockets;
using System.Net;
using System.Text;
using WebSockets;

namespace Client
{
    internal class Client
    {
        private (string name, IPEndPoint ip) _self = ("",new IPEndPoint(IPAddress.Any, 0));
        private (string name, IPEndPoint ip) _server = ("server",new IPEndPoint(IPAddress.Any, 0));
        public Client((string name,IPEndPoint ip) server)
        {
            _server = server;
        }
        public async Task Execute(Command command)
        {
            Message? ans = null;
            switch (command)
            {
                case Command.SendTo:
                    ans = await SendTo();
                    break;
                case Command.SendAll:
                    ans = await SendAll();
                    break;
                case Command.GetUsersList:
                    await GetUsersList();
                    break;
                case Command.Register:
                    ans = await Register();
                    break;
                case Command.Delete:
                    ans = await Delete();
                    break;
            }
            Console.WriteLine(ans);
        }
        public async Task<Message?> Register()
        {
            Console.WriteLine("Enter your name");
            _self.name = Console.ReadLine()!.ToLower();
            while (String.IsNullOrEmpty(_self.name) || String.Equals(_self, ""))
            {
                Console.WriteLine("Incorrect name! Enter again.");
                _self.name = Console.ReadLine()!;
            }
            var msg = new Message(_self.name, Command.Register, _self, _server);
            return await SendTo(msg);
        }
        public async Task<Message?> Delete()
        {
            var msg = new Message("Delete", Command.Delete, _self, _server);
            return await SendTo(msg);
        }
        public async Task<Message?> SendTo()
        {
            Console.WriteLine("Enter message text");
            var text = Console.ReadLine()!;
            Console.WriteLine("Enter recipient name");
            var name = Console.ReadLine()!;
            (string, IPEndPoint?) recipient = 
                (name.ToLower() == _server.name)? _server : (name, null);
            var msg = new Message(text, Command.SendTo, _self, recipient);
            return await SendTo(msg);
        }
        public async Task<Message?> SendAll()
        {
            Console.WriteLine("Enter message text");
            var text = Console.ReadLine()!;
            var msg = new Message(text, Command.SendTo, _self, _server);
            return await SendTo(msg);
        }
        public async Task<Message?> SendTo(Message msg)
        {
            using (var udpClient = new UdpClient(_self.ip))
            {
                var jsonMessage = msg.SerializeToJson();
                var buffer = Encoding.UTF8.GetBytes(jsonMessage);
                for (int i = 0; i < 3; i++)
                    await udpClient.SendAsync(buffer, buffer.Length, _server.ip);
            }
            return await Receive();
        }
        public async Task<Message?> Receive()
        {
            Message? msg = null;
            using (var udpClient = new UdpClient(_self.ip))
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
        public async Task<Message?> GetUsersList()
        {
            var msg = new Message("", Command.GetUsersList, _self, _server);
            return await SendTo(msg);
        }
    }
}
