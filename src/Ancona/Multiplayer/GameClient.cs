using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using Raylib_cs;
using UdpClient = NetCoreServer.UdpClient;

namespace Ancona.Multiplayer;

public class GameClient : UdpClient
{
    public Player player;
    public Dictionary<Guid, Player> players = new Dictionary<Guid, Player>();

    public Model model;
    public Texture2D texture;
    private BoundingBox[] boxes;
    
    public GameClient(string address, int port) : base(address, port)
    {
        model = Raylib.LoadModel("Assets/bean8.obj");
        //texture = Raylib.LoadTexture("Assets/ace.png");
        //Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.MATERIAL_MAP_NORMAL, ref texture);
        this.boxes = boxes; 
        player = new Player(model);
    }
    
    public void AddPlayer(Guid guid)
    {
        Player player = new Player(model);
        players.Add(guid, player);
    }
    
    public void DisconnectAndStop()
    {
        _stop = true;
        Disconnect();
        while (IsConnected)
            Thread.Yield();
    }

    protected override void OnConnected()
    {
        Console.WriteLine($"Game connected a new session with Id {Id}");

        // Start receive datagrams
        
        //Console.WriteLine("Sending alive "); 
        //Send(Id + " hi");
        
        ReceiveAsync();
    }

    protected override void OnDisconnected()
    {
        Console.WriteLine($"Game disconnected a session with Id {Id}");

        //Send(Id + " bye");

        // Wait for a while...
        Thread.Sleep(1000);

        // Try to connect again
        if (!_stop)
            Connect();
    }

    protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
    {
        string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
        string[] words = message.Split(' ');
        Guid guid;
        bool guidgood = Guid.TryParse(words[0], out guid);
        
        Console.WriteLine("Incoming: " + message);
        
        float x, y, z;

        if (!players.ContainsKey(guid) && Id != guid && guidgood)
        {
            AddPlayer(guid);
        }
        
        if (words.Length == 5)
        {
            if (float.TryParse(words[2], out x) && float.TryParse(words[3], out y) && float.TryParse(words[4], out z))
            {
                Player player = players[guid];
                Vector3 loc = new Vector3(x, y, z);
                
                player.OLDlocation = player.location;
                player.location = loc;
                //player.timeSinceLastUpdate = timeSinceLastUpdate;
                player.Tick();
            }
        }
        // Continue receive datagrams
        ReceiveAsync();
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Game caught an error with code {error}");
    }

    private bool _stop;
}