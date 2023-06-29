using System.Net;
using System.Net.Sockets;
using System.Text;
using NetCoreServer;

namespace AnconaServer;

public class GameServer : UdpServer
{
    private List<ServerClient> clients = new List<ServerClient>();
    public GameServer(IPAddress address, int port) : base(address, port) {}

    protected override void OnStarted()
    {
        ReceiveAsync();
    }

    protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
    {
        string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
        string[] words = message.Split(' ');
        Guid guid;
        Guid.TryParse(words[0], out guid);

        Console.WriteLine(endpoint + " Incoming: " + message);

        if (words[1] == "hi")
        {
            Console.WriteLine("Welcome to the server " + endpoint);

            ServerClient adding = new ServerClient(endpoint, guid);
            
            clients.Add(adding);

            //SendAsync(endpoint, Encoding.UTF8.GetBytes("You are registered " + endpoint), 0, size);
        }

        if (words[1] == "bye")
        {
            Console.WriteLine("Saying bye");
            List<ServerClient> clientsToRemove = new List<ServerClient>();
            foreach (var client in clients)
            {
                //SendAsync(endpoint, Encoding.UTF8.GetBytes("You left " + endpoint), 0, size);
                clientsToRemove.Add(client);
                Console.WriteLine("Client left " + endpoint);
            }
            foreach (var client in clientsToRemove)
            {
                clients.Remove(client);
            }
        }
        Console.WriteLine("I have " + clients.Count + " clients");
        if (words[1] == "loc")
        {
            foreach (var client in clients)
            {
                Console.WriteLine(client.endpoint + " " + endpoint);
                if (client.endpoint.ToString() != endpoint.ToString())
                {
                    Console.WriteLine("False");
                    byte[] locations = Encoding.UTF8.GetBytes(message);
                    SendAsync(client.endpoint, locations, 0, locations.Length);
                }
                else
                {
                    Console.WriteLine("True");
                }
                //Console.WriteLine("Sent: " +client.endpoint);
                //Console.WriteLine("I wish I could send: " + Encoding.UTF8.GetString(Encoding.UTF8.GetBytes( client.id.ToString())));
            }
        }
        byte[] mayday = Encoding.UTF8.GetBytes("done");
        SendAsync(endpoint, mayday, 0, mayday.Length);
    }

    protected override void OnSent(EndPoint endpoint, long sent)
    {
        // Continue receive datagrams
        ReceiveAsync();
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Game server caught an error with code {error}");
    }
}