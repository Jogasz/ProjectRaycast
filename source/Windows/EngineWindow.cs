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
                GL.LineWidth(2f);
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(PlayerPosition.X, PlayerPosition.Y);
                GL.Vertex2(PlayerPosition.X + PlayerDeltaOffsetX * 300, PlayerPosition.Y + PlayerDeltaOffsetY * 300);
                GL.End();


                for (int i = 0; i < 1; i++) {
                    //Vertical wall check
                    DepthOfField = 0;
                    if (PlayerAngle > MathX.Quadrant3 || PlayerAngle < MathX.Quadrant1)
                    {
                        //Player is looking right
                        RayX = (float)Math.Floor(PlayerPosition.X / TileSize) * TileSize + TileSize;
                        RayY = PlayerPosition.Y - ((TileSize - (PlayerPosition.X % TileSize)) * (float)Math.Tan(MathX.Quadrant4 - PlayerAngle));
                        OffsetX = TileSize;
                        OffsetY = -(OffsetX * (float)Math.Tan(MathX.Quadrant4 - PlayerAngle));
                    }
                    else if (PlayerAngle < MathX.Quadrant3 && PlayerAngle > MathX.Quadrant1)
                    {
                        //Player is looking left
                        RayX = (float)Math.Floor(PlayerPosition.X / TileSize) * TileSize - 0.0001f;
                        RayY = PlayerPosition.Y - ((PlayerPosition.X % TileSize) * (float)Math.Tan(PlayerAngle));
                        OffsetX = -TileSize;
                        OffsetY = OffsetX * (float)Math.Tan(PlayerAngle);
                    }

                    CheckingMapCol = (int)Math.Floor(RayX / TileSize);
                    CheckingMapRow = (int)Math.Floor(RayY / TileSize);

                    while (DepthOfField < 8 && CheckingMapRow >= 0 && CheckingMapRow < grid.GetLength(0) && CheckingMapCol >= 0 && CheckingMapCol < grid.GetLength(1))
                    {
                        if (grid[CheckingMapRow, CheckingMapCol] == 1)
                        {
                            DepthOfField = 8;
                        }
                        else
                        {
                            RayX += OffsetX;
                            RayY += OffsetY;
                            DepthOfField++;
                            CheckingMapCol = (int)(RayX / TileSize);
                            CheckingMapRow = (int)(RayY / TileSize);
                        }
                    }

                    GL.Color3(0f, 1f, 0f);
                    GL.LineWidth(15f);
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex2(PlayerPosition.X, PlayerPosition.Y);
                    GL.Vertex2(RayX, RayY);
                    GL.End();

                    //Horizontal wall check
                    DepthOfField = 0;
                    if (PlayerAngle > MathX.Quadrant2)
                    {
                        //Player is looking up
                        RayY = (float)Math.Floor(PlayerPosition.Y / TileSize) * TileSize - 0.0001f;
                        RayX = PlayerPosition.X + (PlayerPosition.Y % TileSize) / (float)Math.Tan(MathX.Quadrant4 - PlayerAngle);
                        OffsetY = -TileSize;
                        OffsetX = TileSize / (float)Math.Tan(MathX.Quadrant4 - PlayerAngle);
                    }

                    if (PlayerAngle < MathX.Quadrant2)
                    {
                        //Player is looking down
                        RayY = (float)Math.Floor(PlayerPosition.Y / TileSize) * TileSize + TileSize;
                        RayX = PlayerPosition.X - (TileSize - (PlayerPosition.Y % TileSize)) / (float)Math.Tan(MathX.Quadrant4 - PlayerAngle);
                        OffsetY = TileSize;
                        OffsetX = -OffsetY / (float)Math.Tan(MathX.Quadrant4 - PlayerAngle);
                    }

                    CheckingMapCol = (int)Math.Floor(RayX / TileSize);
                    CheckingMapRow = (int)Math.Floor(RayY / TileSize);

                    while (DepthOfField < 8 && CheckingMapRow >= 0 && CheckingMapRow < grid.GetLength(0) && CheckingMapCol >= 0 && CheckingMapCol < grid.GetLength(1))
                    {
                        if (grid[CheckingMapRow, CheckingMapCol] == 1)
                        {
                            DepthOfField = 8;
                        }
                        else
                        {
                            RayY += OffsetY;
                            RayX += OffsetX;
                            DepthOfField++;
                            CheckingMapCol = (int)(RayX / TileSize);
                            CheckingMapRow = (int)(RayY / TileSize);
                        }
                    }

                    GL.Color3(0f, 0f, 1f);
                    GL.LineWidth(5f);
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex2(PlayerPosition.X, PlayerPosition.Y);
                    GL.Vertex2(RayX, RayY);
                    GL.End();
                }
                //map[row, col] = [y, x] = row -> y irány (fentről lefelé), col -> x irány (balről jobbra)

                Screen.SwapBuffers();
            };

            Screen.Run();
        }
    }
}
