using System.Net;

namespace AnconaServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // UDP multicast port
            int port = 8493;
            if (args.Length > 0)
                port = int.Parse(args[0]);

            Console.WriteLine($"Game server port: {port}");

            Console.WriteLine();

            // Create a new UDP echo server
            var server = new GameServer(IPAddress.Any, port);

            // Start the server
            Console.Write("Server starting...");
            server.Start();
            Console.WriteLine("Done!");

            Console.WriteLine("Press Enter to stop the server or '!' to restart the server...");

            // Perform text input
            for (;;)
            {
                string line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;

                // Restart the server
                if (line == "!")
                {
                    Console.Write("Server restarting...");
                    server.Restart();
                    Console.WriteLine("Done!");
                }
            }

            // Stop the server
            Console.Write("Server stopping...");
            server.Stop();
            Console.WriteLine("Done!");
        }
    }
}