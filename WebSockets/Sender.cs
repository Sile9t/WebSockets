using System.Net.Sockets;
using System.Net;
using System.Text;

namespace WebSockets
{
    public partial class Sender
    {
        public static async Task Send(Message msg, int port, string address = "127.0.0.1")
        {
            var timet = TimeSpan.FromSeconds(10);
            Console.WriteLine("Sending message");
            var udpClient = new UdpClient();
            var endPoint = new IPEndPoint(IPAddress.Parse(address), port);
            var jsonMessage = msg.SerializeToJson();
            var buffer = Encoding.UTF8.GetBytes(jsonMessage);
            for (int i = 0; i < 3; i++)
                await udpClient.SendAsync(buffer, buffer.Length, endPoint);
            Console.WriteLine("Message sent");
            udpClient.Dispose();
        }
        public static async Task<Message?> Receive(int port, string address = "127.0.0.1")
        {
            var udpClient = new UdpClient(port);
            var endPoint = new IPEndPoint(IPAddress.Parse(address), 0);
            Console.WriteLine("Waiting for connection");
            udpClient.Connect(endPoint);
            Console.WriteLine("Connected");
            Console.WriteLine("Receiving message");
            var data = await udpClient.ReceiveAsync();
            var buffer = data.Buffer;
            var jsonMessage = Encoding.UTF8.GetString(buffer);
            var message = Message.DeserializeFromJson(jsonMessage);
            Console.WriteLine("Message received.");
            Console.WriteLine(message);
            udpClient.Dispose();
            return message;
        }
        public static async Task<Message?> Receive(CancellationToken token, int port,
            string address = "127.0.0.1")
        {
            var udpClient = new UdpClient(port);
            try
            {
                var endPoint = new IPEndPoint(IPAddress.Parse(address), 0);
                Console.WriteLine("Waiting for connection");
                udpClient.Connect(endPoint);
                Console.WriteLine("Connected");
                Console.WriteLine("Receiving message");
                var data = await udpClient.ReceiveAsync(token);
                var buffer = data.Buffer;
                var jsonMessage = Encoding.UTF8.GetString(buffer);
                var message = Message.DeserializeFromJson(jsonMessage);
                Console.WriteLine("Message received.");
                Console.WriteLine(message);
                udpClient.Dispose();
                return message;
            }
            catch (OperationCanceledException) 
            {
                Console.WriteLine("Operaion cancelled!");
                return null;
            }
            finally 
            {
                udpClient.Dispose();
            }
        }
    }
}
