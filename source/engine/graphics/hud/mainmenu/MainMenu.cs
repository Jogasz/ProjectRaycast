using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine;

internal partial class DrawGui
{
    public static void MainMenu(
        float minimumScreenSize,
        float screenVerticalOffset,
        float screenHorizontalOffset)
    {
        float quadX1 = screenHorizontalOffset;
        float quadX2 = screenHorizontalOffset + minimumScreenSize;
        float quadY1 = screenVerticalOffset;
        float quadY2 = screenVerticalOffset + minimumScreenSize;

        ShaderHandler.mainMenuVertexAttribList.AddRange(new float[]
        {
            quadX1,
            quadX2,
            quadY1,
            quadY2,
            0.0f, //r
            1.0f, //g
            0.0f  //b
        });
    }
}