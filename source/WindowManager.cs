using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

public class WindowManager {
    //Setting up pixel coordinates for 2D rendering
    public static void SetupPixelCoordinates(GameWindow Screen)
    {
        GL.Viewport(0, 0, Screen.Width, Screen.Height);
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Ortho(0, Screen.Width, Screen.Height, 0, -1, 1);
        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();
    }

    //Handling window resizing
    public static void Resize(GameWindow Screen)
    {
        Screen.Resize += (sender, e) =>
        {
            SetupPixelCoordinates(Screen);
        };
    }
}
