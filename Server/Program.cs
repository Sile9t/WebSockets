using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WebSockets
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            string? txt;
            Message? ans = new Message("OK", "Server", "Alex");
            do
            {
                Message? msg = await Sender.Receive(token, 12345);
                txt = msg?._text;
                if (txt!.Equals("exit"))
                {
                    ans._text = "Server off";
                    tokenSource.Cancel();
                }
                await Sender.Send(ans,12346);
            } while (!tokenSource.IsCancellationRequested);
            Console.WriteLine("Server off");
        }
        //example
        public void Task()
        {
            var udpClient = new UdpClient(12345);
            var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
            while (true)
            {
                Console.WriteLine("Waiting for connection");
                udpClient.Connect(endPoint);
                Console.WriteLine("Connected");
                byte[] buffer = udpClient.Receive(ref endPoint);
                var jsonMessage = Encoding.UTF8.GetString(buffer);
                var message = Message.DeserializeFromJson(jsonMessage);
                Console.WriteLine("Message received.");
                Console.WriteLine(message);
                message = new Message("OK", "Server", "Alex");
                jsonMessage = message.SerializeToJson();
                buffer = Encoding.UTF8.GetBytes(jsonMessage);
                udpClient.Send(buffer, buffer.Length, endPoint);
                Console.WriteLine("Message sent");
            }
        }
    }
}
