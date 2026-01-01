using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace Engine;

internal class Utils
{
    public static Matrix4 SetViewportAndProjection(int width, int height)
    {
        GL.Viewport(0, 0, width, height);

        return Matrix4.CreateOrthographicOffCenter(0f, width, 0f, height, -1f, 1f);
    }

    public static float NormalizeAngle(float angle)
    {
        if (angle > MathX.Quadrant4)
        {
            angle -= MathX.Quadrant4;
        }

        else if (angle < 0)
        {
            angle += MathX.Quadrant4;
        }

        return angle;
    }
}
