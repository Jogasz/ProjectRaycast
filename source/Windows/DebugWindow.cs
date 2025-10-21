using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Threading;

public class DebugWindow
{
    //Declaring Engine Window size
    public static int ScreenWidth = Level.mapWalls.GetLength(1) * Settings.Gameplay.TileSize;
    public static int ScreenHeight = Level.mapWalls.GetLength(0) * Settings.Gameplay.TileSize;

    int TileSize = Settings.Gameplay.TileSize;
    int[,] MapWalls = Level.mapWalls;
    int RayCount = Settings.Graphics.RayCount;

    public void Run()
    {
        using (GameWindow Screen = new GameWindow(ScreenWidth, ScreenHeight, GraphicsMode.Default, "Debug Window"))
        {
            //For test-only
            Screen.VSync = VSyncMode.Off;

            WindowManager.SetupPixelCoordinates(Screen);

            Screen.RenderFrame += (sender, e) =>
            {
                GL.ClearColor(0.6f, 0.6f, 0.6f, 1f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                //Getting calculations from engine
                Engine.EngineUpdate();

                //Updated variables
                Vector2 PlayerPosition = Engine.playerPosition;
                int PlayerWidth = Engine.PlayerWidth;
                int PlayerHeight = Engine.PlayerHeight;
                float PlayerAngle = Engine.PlayerAngle;
                float PlayerDeltaOffsetX = Engine.PlayerDeltaOffsetX;
                float PlayerDeltaOffsetY = Engine.PlayerDeltaOffsetY;
                float[,] RayDatas = Engine.RayDatas;

                //Drawing map
                for (int x = 0; x < MapWalls.GetLength(1); x++)
                {
                    for (int y = 0; y < MapWalls.GetLength(0); y++)
                    {
                        if (MapWalls[y, x] == 0) { GL.Color3(1f, 1f, 1f); }
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
                GL.Vertex2(PlayerPosition.X - PlayerWidth / 2f, PlayerPosition.Y - PlayerHeight / 2f);
                GL.Vertex2(PlayerPosition.X + PlayerWidth / 2f, PlayerPosition.Y - PlayerHeight / 2f);
                GL.Vertex2(PlayerPosition.X + PlayerWidth / 2f, PlayerPosition.Y + PlayerHeight / 2f);
                GL.Vertex2(PlayerPosition.X - PlayerWidth / 2f, PlayerPosition.Y + PlayerHeight / 2f);
                GL.End();

                //Drawing rays
                GL.Color3(1f, 0f, 0f);
                GL.LineWidth(1f);
                GL.Begin(PrimitiveType.Lines);
                for (int i = 0; i < RayCount; i++)
                {
                    GL.Vertex2(PlayerPosition.X, PlayerPosition.Y);
                    GL.Vertex2(RayDatas[i, 1], RayDatas[i, 2]);
                }

                GL.Color3(0f, 1f, 0f);
                GL.LineWidth(2f);
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(PlayerPosition.X, PlayerPosition.Y);
                GL.Vertex2(PlayerPosition.X + PlayerDeltaOffsetX * 100, PlayerPosition.Y + PlayerDeltaOffsetY * 100);
                GL.End();

                Screen.SwapBuffers();
            };

            Screen.Run();
        }
    }
}
