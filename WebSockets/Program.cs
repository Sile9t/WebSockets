using System.Net.Sockets;
using System.Net;
using System.Text;

namespace WebSockets
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string? input;
            Message msg = new Message("", "Alex", "Server");
            do
            {
                Console.WriteLine("Enter message or 'exit'");
                input = Console.ReadLine();
                msg._text = input;
                await Sender.Send(msg, 12345);
                Message? ans = await Sender.Receive(12346);
                input = ans?._text?.ToLower();
            } while (!input!.Equals("server off"));
            Console.ReadLine();
        }
        //example
        public void Task()
        {
            var udpClient = new UdpClient();
            var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
            string? input;
            do
            {
                Console.WriteLine("Enter message");
                input = Console.ReadLine();
                Message msg = new Message(input!, "Alex", "Server");
                var json = msg.SerializeToJson();
                var buffer = Encoding.UTF8.GetBytes(json);
                udpClient.Send(buffer, buffer.Length, endPoint);
                Console.WriteLine("Message sent");
                udpClient.Connect(endPoint);
                buffer = udpClient.Receive(ref endPoint);
                json = Encoding.UTF8.GetString(buffer);
                msg = Message.DeserializeFromJson(json)!;
                Console.WriteLine(msg);
            } while (!string.IsNullOrEmpty(input));
            Console.WriteLine("CLient off.");
        }
    }
}
