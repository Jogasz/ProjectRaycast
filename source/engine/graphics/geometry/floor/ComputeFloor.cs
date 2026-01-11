using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine;

internal partial class RayCasting
{
    public static void ComputeFloor(
        float distanceShade,
        int minimumScreenHeight,
        int minimumScreenWidth,
        int screenHorizontalOffset,
        int screenVerticalOffset,
        int i,
        float rayAngle,
        float wallHeight,
        float wallWidth,
        float pitch,
        float debugBorder
    )
    {
        float stepX = wallWidth;
        float quadX1 = screenHorizontalOffset + (i * stepX);
        float quadX2 = screenHorizontalOffset + ((i + 1) * stepX);

        float quadY1 = Math.Clamp(screenVerticalOffset + (minimumScreenHeight / 2f) - (wallHeight / 2f) - pitch, screenVerticalOffset, screenVerticalOffset + minimumScreenHeight);
        float quadY2 = screenVerticalOffset;

        //No ceiling can be rendered if the wall's top is on the top of the screen
        if (quadY1 > quadY2)
        {
            Shader.floorVertexAttribList.AddRange(new float[]
            {
                quadX1 + debugBorder,
                quadX2 - debugBorder,
                quadY1 + debugBorder,
                quadY2 - debugBorder,
                rayAngle,
                wallHeight
            });
        }
    }
}