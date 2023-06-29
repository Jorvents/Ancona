using System.Numerics;
using NetCoreServer;
using Packing;
using Raylib_cs;

namespace Ancona.Multiplayer;

public class Join
{
    //public GameClient client;
    public Button joinbutton;

    public TextBox textbox;

    public bool joined;

    public Join()
    {
        Vector2 size = new Vector2(350, 200);

        int spacing = 50;
        int height = 150;
        
        joinbutton =
            new Button(new Rectangle(Raylib.GetScreenWidth() / 2 - size.X / 2, Raylib.GetScreenHeight() / 2 - size.Y / 2, 
                    size.X, size.Y),
                "Join", 150
            );
        textbox = new TextBox(new Rectangle(spacing, Raylib.GetScreenHeight() - spacing - height, Raylib.GetScreenWidth() - spacing * 2, height), 15f, 60);
    }
    public GameClient ConnectToPlace() //BECAUSE WINDOWS DEFENDER DOESNT LIKE THE FUNCTION NAME "ConnectToServer()"
    {
        string address = "127.0.0.1";
        
        /*
        if (textbox.output != "")
        {
            address = textbox.output;
        }
        */
        int port = 8493;

        Console.WriteLine($"Game address: {address}");
        Console.WriteLine($"Game port: {port}");

        Console.WriteLine();
        
        GameClient client = new GameClient(address, port);
        
        Console.Write("Thing connecting...");
        client.Connect();
        Console.WriteLine("Done!");
        
        Console.WriteLine("Sending alive "); 
        client.Send(client.Id + " hi");

        Console.WriteLine("Press Enter to stop the thing or '!' to reconnect the client...");

        Console.WriteLine(client.Socket.Connected);
        Console.WriteLine(client.Id);
        joined = true;

        return client;
    }
    
    public void Play()
    {
        if (Raylib.IsCursorHidden())
        {
            Raylib.ShowCursor();
        }
        
        joinbutton.Play();
        textbox.Play();
    }
    public void Work() 
    {
        /*
        if(joinbutton.isPressed)
        {
            ConnectToPlace();
        }
        */
        textbox.Work();
    }
    public void Draw() 
    {
        joinbutton.Draw(Color.WHITE, 10f);
        textbox.Draw(Color.WHITE, Color.PINK);
    }
}