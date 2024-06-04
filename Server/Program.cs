using ClientSide;

namespace WebSockets
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Server server = new Server();
            while (true) 
            {
                Console.WriteLine("Sever is waiting for requests.");
                Message msg = await server.Receive();
                await server.Execute(msg);
            }
            Console.WriteLine("Server off");
        }
    }
}
