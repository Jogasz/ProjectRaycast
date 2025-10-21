using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

public class GraphicWindow
{//Befolyásolja a RayCount-ot!!!
    public static int ScreenWidth = 1000;
    public static int ScreenHeight = 800;


    public void Run()
    {
        GameWindow Screen = new GameWindow(ScreenWidth, ScreenHeight, GraphicsMode.Default, "Graphic Screen");

        Screen.VSync = VSyncMode.Off;

        WindowManager.SetupPixelCoordinates(Screen);

        Screen.RenderFrame += (sender, e) =>
        {
            //Window color anf clear
            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            float[,] RayDatas = Engine.RayDatas;
            float WallWidth = ScreenWidth / Settings.Graphics.RayCount;

            //Drawing walls
            GL.Color3(1f, 0f, 1f);
            GL.Begin(PrimitiveType.Quads);
            for (int i = 0; i < Settings.Graphics.RayCount; i++)
            {
                GL.Vertex2(i * WallWidth, (ScreenHeight / 2) - (RayDatas[i, 3] / 2));
                GL.Vertex2((i + 1) * WallWidth, (ScreenHeight / 2) - (RayDatas[i, 3] / 2));
                GL.Vertex2((i + 1) * WallWidth, (ScreenHeight / 2) + RayDatas[i, 3]);
                GL.Vertex2(i * WallWidth, (ScreenHeight / 2) + RayDatas[i, 3]);
            }
            GL.End();

            Screen.SwapBuffers();
        };

        Screen.Run();
    }
}
