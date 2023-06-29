using System.Numerics;
using Ancona.Multiplayer;
using Raylib_cs;
using static Raylib_cs.CameraProjection;

namespace Ancona;

public class Game
{
    public GameClient client;
    
    float timeSinceLastUpdate = 0;
    float updateInterval = 1.0f / 20.0f;

    public Model map;
    //public BoundingBox[] testbox = new BoundingBox[15];
    
    public Game()
    {
        map = Raylib.LoadModel("Assets/map8.obj");
        /*
        for (int i = 0; i < 15; i++)
        {
            testbox[i] = Raylib.GetMeshBoundingBox(map.meshes[i]);
        }
        
        */
        //client.player = new Player();
    }

    public void Play()
    {
        bool look = !Raylib.IsKeyDown(KeyboardKey.KEY_TAB);
        client.player.Play(look);

        if (look)
        {
            if (!Raylib.IsCursorHidden())
            {
                Raylib.HideCursor();
            }
            Raylib.SetMousePosition(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2);
        }
        else
        {
            if (Raylib.IsCursorHidden())
            {
                Raylib.ShowCursor();
            }
        }
    }
    public void Work()
    {
        client.player.Work(updateInterval);
        
        timeSinceLastUpdate += Raylib.GetFrameTime();
        
        if (timeSinceLastUpdate >= updateInterval)
        {
            client.Send(client.Id + " loc"+ " " + client.player.location.X + " " + client.player.location.Y + " " + client.player.location.Z);
                
            timeSinceLastUpdate = 0;
        }

        Raylib.UpdateCamera(ref client.player.camera);
    }
    public void Draw()
    {
        Raylib.BeginMode3D(client.player.camera);
        
        Raylib.DrawModel(map, Vector3.Zero, 1f, Color.WHITE);
        
        /*
        foreach (var box in testbox)
        {
            Raylib.DrawBoundingBox(box, Color.BLACK);
        }
        */
        int size = 32;

        //Raylib.DrawGrid(size, size / 10f);

        //Raylib.DrawLine3D(Vector3.Zero, new Vector3(size, 0f,0f), Color.RED);
        //Raylib.DrawLine3D(Vector3.Zero, new Vector3(0f, size,0f), Color.GREEN);
        //Raylib.DrawLine3D(Vector3.Zero, new Vector3(0f, 0f,size), Color.BLUE);
                
        //Raylib.DrawSphere(camera.target, 15f, Color.RED);
        
        foreach (var player in client.players.Values)
        {
            player.Work(updateInterval);
            player.DrawMultiplayer();
            //Raylib.DrawText("Amount: " +player.amount, 10, 120, 20, Color.DARKGRAY);
        }
        
        client.player.DrawLocal();

        Raylib.EndMode3D();
        
        /*
        foreach (var player in client.players.Values)
        {
            //player.Draw(timeSinceLastUpdate, updateInterval);
            Raylib.DrawText("Amount: " +player.amount, 10, 120, 20, Color.DARKGRAY);
        }
        */
        //Raylib.DrawText("Rotation: "+ client.player.rotation, 10, 40, 20, Color.DARKGRAY);
        Raylib.DrawText("Posistion: "+client.player.camera.position, 10, 80, 20, Color.DARKGRAY);
        //Raylib.DrawText("Amount: "+client.player.amount, 10, 120, 20, Color.DARKGRAY);
        //Raylib.DrawText(client.player.model.materialCount.ToString(), 10, 400, 20, Color.DARKGRAY);
        //Raylib.DrawText("Mouse delta: "+Raylib.GetMouseDelta(), 10, 120, 20, Color.DARKGRAY);
        //Raylib.DrawText("Target: " + client.player.camera.target, 10, 160, 20, Color.DARKGRAY);
        //Raylib.DrawText("Quaternion: "+ client.player.quaternionLook, 10, 240, 20, Color.DARKGRAY);
        //Raylib.DrawText("Offset: "+ client.player.offsetLook, 10, 320, 20, Color.DARKGRAY);
    }
}