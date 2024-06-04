using Client;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WebSockets
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Server server = new Server();
            while (true) 
            {
                Message msg = await server.Receive();
                await server.Execute(msg);
            }
            Console.WriteLine("Server off");
        }
    }
}
