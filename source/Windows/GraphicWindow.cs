using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

public class GraphicWindow
{//Befolyásolja a RayCount-ot!!!
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
                if (Engine.RayDatas[i, 5] == 0)
                {
                    continue;
                }
                else if (Engine.RayDatas[i, 4] == 1)
                {
                    if (Engine.RayDatas[i, 5] == 1)
                    {
                        GL.Color3(0f, (1f - VerticalShade), 0f);
                    }
                    else if (Engine.RayDatas[i, 5] == 2)
                    {
                        GL.Color3(0f, 0f, (1f - VerticalShade));
                    }
                }
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
                }
                GL.Vertex2(i * WallWidth, (ScreenHeight / 2) - (Engine.RayDatas[i, 3] / 2));
                GL.Vertex2((i + 1) * WallWidth, (ScreenHeight / 2) - (Engine.RayDatas[i, 3] / 2));
                GL.Vertex2((i + 1) * WallWidth, (ScreenHeight / 2) + Engine.RayDatas[i, 3]);
                GL.Vertex2(i * WallWidth, (ScreenHeight / 2) + Engine.RayDatas[i, 3]);
            }
            GL.End();

            Screen.SwapBuffers();
        };

        Screen.Run();
    }
}
