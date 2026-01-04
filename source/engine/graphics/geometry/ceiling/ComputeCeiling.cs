using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine;

internal partial class RayCasting
{
    public static void ComputeCeiling(
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
        //Height of the player
        float stepX = wallWidth;
        float quadX1 = screenHorizontalOffset + (i * stepX);
        float quadX2 = screenHorizontalOffset + ((i + 1) * stepX);

        float quadY1 = screenVerticalOffset + minimumScreenHeight;
        float quadY2 = screenVerticalOffset + (minimumScreenHeight / 2f) + (wallHeight / 2f) - pitch;

        float r = 0f;
        float g = 0f;
        float b = 1f;

        //No ceiling can be rendered if the wall's top is on the top of the screen
        if (quadY1 > quadY2)
        {
            Engine.ceilingVertexAttributesList.AddRange(new float[]
            {
                quadX1 + debugBorder,
                quadX2 - debugBorder,
                quadY1 + debugBorder,
                quadY2 - debugBorder,
                rayAngle
            });
        }
    }
}