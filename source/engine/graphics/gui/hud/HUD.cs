using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine;

internal partial class Engine
{
    void UploadHUD()
    {
        // Fullscreen quad covering the minimumScreen area
        float x1 = screenHorizontalOffset;
        float x2 = screenHorizontalOffset + minimumScreenSize;
        float y1 = screenVerticalOffset + minimumScreenSize;
        float y2 = screenVerticalOffset;

        // Draw order requirement:
        //0: sword
        //1: vignette
        //2: container
        ShaderHandler.HUDVertexAttribList.AddRange(new float[]
        {
            x1, x2, y1, y2,0f,
            x1, x2, y1, y2,1f,
            x1, x2, y1, y2,2f
        });
    }
}
