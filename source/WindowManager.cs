using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

public class WindowManager {
    //Setting up pixel coordinates for 2D rendering, Handling window resizing
    public static void SetupPixelCoordinates(GameWindow Screen)
    {
        Screen.Resize += (sender, e) =>
        {
            GL.Viewport(0, 0, Screen.Width, Screen.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Screen.Width, Screen.Height, 0, -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GraphicWindow.ScreenWidth = Screen.Width;
            GraphicWindow.ScreenHeight = Screen.Height;
        };
    }
}
