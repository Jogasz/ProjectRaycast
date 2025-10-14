using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

public class EngineWindow
{
    //Declaring Engine Window size
    int DebugWindowWidth = Map.grid.GetLength(1) * Settings.Gameplay.TileSize;
    int DebugWindowHeight = Map.grid.GetLength(0) * Settings.Gameplay.TileSize;

    int FOV = Settings.Graphics.FOV;
    int RayCount = Settings.Graphics.RayCount;
    int TileSize = Settings.Gameplay.TileSize;
    int[,] grid = Map.grid;
    public void Run()
    {
        using (GameWindow Screen = new GameWindow(DebugWindowWidth, DebugWindowHeight, GraphicsMode.Default, "Engine Screen"))
        {
            Engine engine = new Engine();

            WindowManager.SetupPixelCoordinates(Screen);

            Screen.RenderFrame += (sender, e) =>
            {
                GL.ClearColor(0.6f, 0.6f, 0.6f, 1f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                //Getting calculations from engine
                engine.Update();

                //Updated variables
                Vector2 Position = engine.playerPosition;
                int PlayerWidth = engine.PlayerWidth;
                int PlayerHeight = engine.PlayerHeight;
                float PlayerAngle = engine.PlayerAngle;
                float PlayerDeltaOffsetX = engine.PlayerDeltaOffsetX;
                float PlayerDeltaOffsetY = engine.PlayerDeltaOffsetY;
                float FOVUnit = engine.FOVUnit;
                float RadBetweenRays = engine.RadBetweenRays;
                float VerticalOffsetX = 0f;
                float VerticalOffsetY = 0f;
                float VerticalPythagoras = 0f;
                float HorizontalOffsetX = 0f;
                float HorizontalOffsetY = 0f;
                float HorizontalPythagoras = 0f;

                //Drawing map
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    for (int y = 0; y < grid.GetLength(0); y++)
                    {
                        if (grid[y, x] == 0) { GL.Color3(1f, 1f, 1f); }
                        else { GL.Color3(0f, 0f, 0f); }
                        GL.Begin(PrimitiveType.Quads);
                        //Left top
                        GL.Vertex2(TileSize * x + 1, TileSize * y + 1);
                        //Right top
                        GL.Vertex2(TileSize * x + TileSize - 1, TileSize * y + 1);
                        //Right bottom
                        GL.Vertex2(TileSize * x + TileSize - 1, TileSize * y + TileSize - 1);
                        //Left bottom
                        GL.Vertex2(TileSize * x + 1, TileSize * y + TileSize - 1);
                        GL.End();
                    }
                }

                //Drawing player
                GL.Color3(0f, 0f, 1f);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(Position.X - PlayerWidth / 2f, Position.Y - PlayerHeight / 2f);
                GL.Vertex2(Position.X + PlayerWidth / 2f, Position.Y - PlayerHeight / 2f);
                GL.Vertex2(Position.X + PlayerWidth / 2f, Position.Y + PlayerHeight / 2f);
                GL.Vertex2(Position.X - PlayerWidth / 2f, Position.Y + PlayerHeight / 2f);
                GL.End();

                GL.Color3(0f, 0f, 1f);
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(Position.X, Position.Y);
                GL.Vertex2(Position.X + PlayerDeltaOffsetX * 200, Position.Y + PlayerDeltaOffsetY * 200);
                GL.End();

                //Drawing FOV rays
                /*for (int i = 0; i < RayCount; i++)
                {
                    GL.Color3(1f, 0f, 0f);
                    GL.LineWidth(1f);
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex2(Position.X, Position.Y);
                    GL.Vertex2(Position.X + (float)Math.Cos(PlayerAngle + FOVUnit) * 300, Position.Y + (float)Math.Sin(PlayerAngle + FOVUnit) * 300);
                    GL.End();
                    FOVUnit += RadBetweenRays;
                }

                if (((MathX.Quadrant4 - PlayerAngle) < MathX.Quadrant1) || ((MathX.Quadrant4 - PlayerAngle) > MathX.Quadrant3))
                {
                    Console.WriteLine("Player jobbra néz! (" + (MathX.Quadrant4 - PlayerAngle) + ")");
                }
                else if (((MathX.Quadrant4 - PlayerAngle) > MathX.Quadrant1) && ((MathX.Quadrant4 - PlayerAngle) < MathX.Quadrant3))
                {
                    Console.WriteLine("Player balra néz! (" + (MathX.Quadrant4 - PlayerAngle) + ")");
                }*/

                for (int i = 0; i < 1; i++) {
                    //Vertical wall check
                    //Player is looking right
                    if (((MathX.Quadrant4 - PlayerAngle) < MathX.Quadrant1) || ((MathX.Quadrant4 - PlayerAngle) > MathX.Quadrant3))
                    {
                        Console.WriteLine("Player jobbra néz! (" + (MathX.Quadrant4 - PlayerAngle) + ")");
                    }

                    //Player is looking left
                    else if (((MathX.Quadrant4 - PlayerAngle) > MathX.Quadrant1) && ((MathX.Quadrant4 - PlayerAngle) < MathX.Quadrant3))
                    {
                        Console.WriteLine("Player balra néz! (" + (MathX.Quadrant4 - PlayerAngle) + ")");
                    }

                    GL.Color3(1f, 0f, 0f);
                    GL.LineWidth(1f);
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex2(Position.X, Position.Y);
                    GL.Vertex2(Position.X + (float)Math.Cos(PlayerAngle) * 300, Position.Y + (float)Math.Sin(PlayerAngle) * 300);
                    GL.End();
                }

                Screen.SwapBuffers();
            };

            Screen.Run();
        }
    }
}
