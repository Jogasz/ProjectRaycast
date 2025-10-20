using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

public class GraphicWindow
{
    int GraphicWindowWidth = 1000;
    int GraphicWindowHeight = 600;
    public float[] RayDistances = new float[Settings.Graphics.RayCount];
    int RayCount = Settings.Graphics.RayCount;

    public void Run()
    {
        GameWindow Screen = new GameWindow(GraphicWindowWidth, GraphicWindowHeight, GraphicsMode.Default, "Graphic Screen");
        WindowManager.SetupPixelCoordinates(Screen);

        Screen.RenderFrame += (sender, e) =>
        {
            // Ég világoskék
            GL.ClearColor(0.5f, 0.8f, 1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            float wallWidth = GraphicWindowWidth / (float)RayCount;

            for (int i = 0; i < RayCount; i++)
            {
                float dist = RayDistances[i];

                // Erősebben csökkenő magasság a távolsággal
                float height = 8000f / (dist + 0.001f);

                float x = i * wallWidth;
                float yTop = (GraphicWindowHeight / 2f) - (height / 2f);
                float yBottom = yTop + height;

                // Fal zöld
                GL.Color3(0f, 1f, 0f);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(x, yTop);
                GL.Vertex2(x + wallWidth, yTop);
                GL.Vertex2(x + wallWidth, yBottom);
                GL.Vertex2(x, yBottom);
                GL.End();
            }

            Screen.SwapBuffers();
        };

        Screen.Run();
    }

    public void UpdateFrame(float[] rayDistances)
    {
        RayDistances = (float[])rayDistances.Clone();
    }
}
