using Raylib_cs;
using System;
using System.Numerics;

public class UIElement {
    public void DrawBar(Vector2 position, Vector2 size, float progress) {
        Raylib.DrawRectangleV(position, size, Color.BLACK);
        Raylib.DrawRectangleV(position + new Vector2(4, 4), new Vector2((size.X - 8) * progress, size.Y - 8), Color.WHITE);
    }
}