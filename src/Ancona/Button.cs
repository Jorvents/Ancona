using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace Packing
{
    public class Button
    {
        public Rectangle box;
        public bool isPressed = false;
        public bool isHovered = false;
        
        int size;
        string text;
        float rotation;
        
        public Button(Rectangle box, string text, int size)
        {
            this.box = box;
            this.text = text;
            this.size = size;
        }
        public void Play()
        {
            isPressed = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), box) && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT);
            isHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), box);
        }
        public void Draw(Color colour, float thick)
        {
            Raylib.DrawRectangleRoundedLines(box, 0.07f,2,thick, colour);
            
            Raylib.DrawText(text, (int)(box.x + box.width / 2 - Raylib.MeasureText(text, size) / 2), (int)(box.y + box.height / 2 - Raylib.MeasureText(text, size) / 4), size, colour);
        }
    }
}