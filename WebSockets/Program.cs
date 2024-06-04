using ClientSide;

namespace WebSockets
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Client client = new Client();
            Command cmd = new Command();
            Console.WriteLine("Clients is running");
            while (cmd != Command.Answer)
            {
                cmd = await SelectCommand();
                await client.Execute(cmd);
            }
            Console.WriteLine("Client stopped!");
            Console.ReadLine();
        }
        public static void PrintCommands()
        {
            Console.WriteLine("Enter command number:\n1 - Send message to someone\n" +
                "2 - Send message to all\n3 - Wait for message\n4 - Get users list\n" +
                "5 - Register\n6 - Delete your account\n7 - Exit the app");
        }
        public static async Task<Command> SelectCommand()
        {
            int mcd = await ReadCommandNumber();
            Command res = new Command();
            switch (mcd)
            {
                case 0:
                    res = Command.SendTo;
                    break;
                case 1:
                    res = Command.SendAll;
                    break;
                case 2:
                    res = Command.Receive;
                    break;
                case 3:
                    res = Command.GetUsersList;
                    break;
                case 4:
                    res = Command.Register;
                    break;
                case 5:
                    res = Command.Delete;
                    break;
                default:
                    res = Command.Answer;
                    break;
            }
            return res;
        }
        public static async Task<int> ReadCommandNumber()
        {
            bool isValidInput = false;
            int cmd = 7;
            while (!isValidInput)
            {
                PrintCommands();
                string? input = Console.ReadLine();
                while (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Input can't be empty");
                }
                isValidInput = Int32.TryParse(input, out int command);
                if (isValidInput && ((command > 7) || (command < 1)))
                {
                    isValidInput = false; 
                    Console.WriteLine("Wrong command number");
                }
                cmd = command - 1;
            }
            return cmd;
        }
    }
}
