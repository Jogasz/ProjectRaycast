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
                Vector2 PlayerPosition = engine.playerPosition;
                int PlayerWidth = engine.PlayerWidth;
                int PlayerHeight = engine.PlayerHeight;
                float PlayerAngle = engine.PlayerAngle;
                float PlayerDeltaOffsetX = engine.PlayerDeltaOffsetX;
                float PlayerDeltaOffsetY = engine.PlayerDeltaOffsetY;
                float RayAngle = engine.RayAngle;
                float RadBetweenRays = engine.RadBetweenRays;
                float RayX = 0f;
                float RayY = 0f;
                float OffsetX = 0f;
                float OffsetY = 0f;
                float VerticalPythagoras = 0f;
                float HorizontalPythagoras = 0f;
                int DepthOfField = 0;
                int CheckingMapCol = 0;
                int CheckingMapRow = 0;

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
                GL.Vertex2(PlayerPosition.X - PlayerWidth / 2f, PlayerPosition.Y - PlayerHeight / 2f);
                GL.Vertex2(PlayerPosition.X + PlayerWidth / 2f, PlayerPosition.Y - PlayerHeight / 2f);
                GL.Vertex2(PlayerPosition.X + PlayerWidth / 2f, PlayerPosition.Y + PlayerHeight / 2f);
                GL.Vertex2(PlayerPosition.X - PlayerWidth / 2f, PlayerPosition.Y + PlayerHeight / 2f);
                GL.End();

                GL.Color3(1f, 0f, 0f);
                GL.LineWidth(1f);
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(PlayerPosition.X, PlayerPosition.Y);
                GL.Vertex2(PlayerPosition.X + PlayerDeltaOffsetX * 500, PlayerPosition.Y + PlayerDeltaOffsetY * 500);
                GL.End();

                //Drawing FOV rays
                /*for (int i = 0; i < RayCount; i++)
                {
                    GL.Color3(1f, 0f, 0f);
                    GL.LineWidth(1f);
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex2(Position.X, Position.Y);
                    GL.Vertex2(Position.X + (float)Math.Cos(PlayerAngle + RayAngle) * 300, Position.Y + (float)Math.Sin(PlayerAngle + RayAngle) * 300);
                    GL.End();
                    RayAngle += RadBetweenRays;
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
                        RayX = PlayerPosition.X + (TileSize - (PlayerPosition.X % TileSize));
                        RayY = PlayerPosition.Y - ((TileSize - (PlayerPosition.X % TileSize)) * (float)Math.Tan(MathX.Quadrant4 - PlayerAngle));
                        OffsetX = TileSize;
                        OffsetY = OffsetX * (float)Math.Tan(MathX.Quadrant4 - PlayerAngle);
                    }

                    //Player is looking left
                    else if (((MathX.Quadrant4 - PlayerAngle) > MathX.Quadrant1) && ((MathX.Quadrant4 - PlayerAngle) < MathX.Quadrant3))
                    {
                        RayX = PlayerPosition.X - (PlayerPosition.X % TileSize);
                        RayY = PlayerPosition.Y - ((PlayerPosition.X % TileSize) * (float)Math.Tan(PlayerAngle));
                        OffsetX = -TileSize;
                        OffsetY = -(OffsetX * (float)Math.Tan(PlayerAngle));
                    }

                    CheckingMapCol = Convert.ToInt32((RayX / TileSize));
                    CheckingMapRow = Convert.ToInt32(Math.Floor(RayY / TileSize));

                    while (DepthOfField < 8 && CheckingMapRow < grid.GetLength(0) && CheckingMapRow >= 0)
                    {
                        if (grid[CheckingMapRow, CheckingMapCol] == 1)
                        {
                            Console.WriteLine("Ray is checking: (" + CheckingMapRow + ", " + CheckingMapCol + ")");
                            Console.WriteLine("RayX: " + RayX);
                            Console.WriteLine("RayY: " + RayY);
                            Console.WriteLine("OffsetX: " + OffsetX);
                            Console.WriteLine("OffsetY: " + RayX);

                            GL.Color3(0f, 1f, 0f);
                            GL.LineWidth(4f);
                            GL.Begin(PrimitiveType.Lines);
                            GL.Vertex2(PlayerPosition.X, PlayerPosition.Y);
                            GL.Vertex2(RayX, RayY);
                            GL.End();
                            //VerticalPythagoras = Math.Sqrt(Math.Pow(RayX - PlayerPosition.X, 2) + Math.Pow());
                            DepthOfField = 8;
                        }

                        else
                        {
                            RayX += OffsetX;
                            RayY -= OffsetY;
                            DepthOfField++;
                            CheckingMapCol = Convert.ToInt32((RayX / TileSize));
                            CheckingMapRow = Convert.ToInt32(Math.Floor(RayY / TileSize));
                        }
                    }
                }

                Screen.SwapBuffers();
            };

            Screen.Run();
        }
    }
}
