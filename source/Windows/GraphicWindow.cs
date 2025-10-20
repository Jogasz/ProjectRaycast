using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

public class GraphicWindow
{
    public static int ScreenWidth = 1000;
    public static int ScreenHeight = 800;


    public void Run()
    {
        GameWindow Screen = new GameWindow(ScreenWidth, ScreenHeight, GraphicsMode.Default, "Graphic Screen");
        WindowManager.SetupPixelCoordinates(Screen);

        Screen.RenderFrame += (sender, e) =>
        {
            //Window color anf clear
            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Engine.Update();
            
            //Updated variables
            float WallWidth = Engine.WallWidth;
            float[] WallHeights = Engine.WallHeights;

            //Drawing walls
            GL.Color3(1f, 0f, 1f);
            GL.Begin(PrimitiveType.Quads);
            for (int i = 0; i < WallHeights.Length; i++)
            {
                GL.Vertex2(i * WallWidth, (ScreenHeight / 2) - (WallHeights[i] / 2));
                GL.Vertex2((i + 1) * WallWidth, (ScreenHeight / 2) - (WallHeights[i] / 2));
                GL.Vertex2((i + 1) * WallWidth, (ScreenHeight / 2) + WallHeights[i]);
                GL.Vertex2(i * WallWidth, (ScreenHeight / 2) + WallHeights[i]);
            }
            GL.End();

            Screen.SwapBuffers();
        };

        Screen.Run();
    }
}
