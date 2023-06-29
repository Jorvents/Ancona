using System.Numerics;
using Ancona.Multiplayer;
using Raylib_cs;
using static Raylib_cs.CameraProjection;

namespace Ancona
{
    public class Program
    {
        static void Main(string[] args)
        {
            Raylib.InitWindow(1280, 720, "Ancona");
            Raylib.SetTargetFPS(163);
            
            Controller controller = new Controller();
            
            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();

                Raylib.ClearBackground(Color.BLACK);
                
                controller.JustRun();
                
                Raylib.EndDrawing();
            }

            if (controller.join.joined)
            {
                controller.game.client.Send(controller.game.client.Id + " bye");
                Raylib.UnloadTexture(controller.game.client.texture);
                Raylib.UnloadModel(controller.game.client.model);
            }
            Raylib.UnloadModel(controller.game.map);
            Raylib.CloseWindow();
        }
    }
}