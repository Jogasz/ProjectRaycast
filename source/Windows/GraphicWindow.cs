
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

public class GraphicWindow
{
    public static int TileSize = Settings.Gameplay.TileSize;
    public static int ScreenWidth = 1000;
    public static int ScreenHeight = 800;
    public static float VerticalShade = Settings.Graphics.VerticalShade;
    public static float HorizontalShade = Settings.Graphics.HorizontalShade;

    public void Run()
    {
        GameWindow Screen = new GameWindow(ScreenWidth, ScreenHeight, GraphicsMode.Default, "Graphic Screen");

        //For test-only
        //Screen.VSync = VSyncMode.Off;

        WindowManager.SetupPixelCoordinates(Screen);

        Screen.RenderFrame += (sender, e) =>
        {
            //Window color anf clear
            GL.ClearColor(0.4f, 0.9f, 1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            float WallWidth = (float)ScreenWidth / (float)Settings.Graphics.RayCount;

            //Drawing walls
            GL.Begin(PrimitiveType.Quads);
            for (int i = 0; i < Settings.Graphics.RayCount; i++)
            {
                //No wall
                if (Engine.RayDatas[i, 5] == 0)
                {
                    continue;
                }
                //Vertical wall
                else if (Engine.RayDatas[i, 4] == 1)
                {
                    if (Engine.RayDatas[i, 5] == 1)
                    {
                        GL.Color3(0f, (1f - VerticalShade), 0f);
                        GL.Vertex2(i * WallWidth, (ScreenHeight / 2) - (Engine.RayDatas[i, 6] / 2));
                        GL.Vertex2((i + 1) * WallWidth, (ScreenHeight / 2) - (Engine.RayDatas[i, 6] / 2));
                        GL.Vertex2((i + 1) * WallWidth, (ScreenHeight / 2) + (Engine.RayDatas[i, 6] / 2));
                        GL.Vertex2(i * WallWidth, (ScreenHeight / 2) + (Engine.RayDatas[i, 6] / 2));
                    }
                    else if (Engine.RayDatas[i, 5] == 2)
                    {
                        for (int k = 0; k < Engine.TestTexture2.GetLength(0); k++) {
                            if (Engine.TestTexture2[k, (int)Math.Floor(Engine.RayDatas[i, 3] / (TileSize / (float)Engine.TestTexture2.GetLength(1)))] == 1)
                            {
                                GL.Color3(1f, 1f, 1f);
                                GL.Vertex2(i * WallWidth, (ScreenHeight / 2) - (Engine.RayDatas[i, 6] / 2) + (k * (Engine.RayDatas[i, 6] / Engine.TestTexture2.GetLength(0))));
                                GL.Vertex2((i + 1) * WallWidth, (ScreenHeight / 2) - (Engine.RayDatas[i, 6] / 2) + (k * (Engine.RayDatas[i, 6] / Engine.TestTexture2.GetLength(0))));
                                GL.Vertex2((i + 1) * WallWidth, (ScreenHeight / 2) - (Engine.RayDatas[i, 6] / 2) + ((k + 1) * (Engine.RayDatas[i, 6] / Engine.TestTexture2.GetLength(0))));
                                GL.Vertex2(i * WallWidth, (ScreenHeight / 2) - (Engine.RayDatas[i, 6] / 2) + ((k + 1) * (Engine.RayDatas[i, 6] / Engine.TestTexture2.GetLength(0))));
                            }
                            else
                            {
                                GL.Color3(0f, 0f, 0f);
                                GL.Vertex2(i * WallWidth, (ScreenHeight / 2) - (Engine.RayDatas[i, 6] / 2) + (k * (Engine.RayDatas[i, 6] / Engine.TestTexture2.GetLength(0))));
                                GL.Vertex2((i + 1) * WallWidth, (ScreenHeight / 2) - (Engine.RayDatas[i, 6] / 2) + (k * (Engine.RayDatas[i, 6] / Engine.TestTexture2.GetLength(0))));
                                GL.Vertex2((i + 1) * WallWidth, (ScreenHeight / 2) - (Engine.RayDatas[i, 6] / 2) + ((k + 1) * (Engine.RayDatas[i, 6] / Engine.TestTexture2.GetLength(0))));
                                GL.Vertex2(i * WallWidth, (ScreenHeight / 2) - (Engine.RayDatas[i, 6] / 2) + ((k + 1) * (Engine.RayDatas[i, 6] / Engine.TestTexture2.GetLength(0))));
                            }
                        }
                    }
                }
                //Horizontal wall
                else if (Engine.RayDatas[i, 4] == 2)
                {
                    if (Engine.RayDatas[i, 5] == 1)
                    {
                        GL.Color3(0f, (1f - HorizontalShade), 0f);
                    }
                    else if (Engine.RayDatas[i, 5] == 2)
                    {
                        GL.Color3(0f, 0f, (1f - HorizontalShade));
                    }
                    GL.Vertex2(i * WallWidth, (ScreenHeight / 2) - (Engine.RayDatas[i, 6] / 2));
                    GL.Vertex2((i + 1) * WallWidth, (ScreenHeight / 2) - (Engine.RayDatas[i, 6] / 2));
                    GL.Vertex2((i + 1) * WallWidth, (ScreenHeight / 2) + (Engine.RayDatas[i, 6] / 2));
                    GL.Vertex2(i * WallWidth, (ScreenHeight / 2) + (Engine.RayDatas[i, 6] / 2));
                }
            }
            GL.End();

            Screen.SwapBuffers();
        };

        Screen.Run();
    }
}
