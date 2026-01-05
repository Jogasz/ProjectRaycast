using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine;

internal partial class RayCasting
{
    public static void ComputeWalls(
        Vector2i ClientSize,
        float distanceShade,
        int minimumScreenHeight,
        int minimumScreenWidth,
        int screenHorizontalOffset,
        int screenVerticalOffset,
        int tileSize,
        int nthRay,
        float rayLength,
        float rayTilePosition,
        float wallHeight,
        float wallWidth,
        int wallSide,
        int textureIndex,
        float pitch,
        float debugBorder
    )
    {
        float quadX1 = nthRay * wallWidth + screenHorizontalOffset;
        float quadX2 = (nthRay + 1) * wallWidth + screenHorizontalOffset;

        float quadY1 = (ClientSize.Y / 2f) + (wallHeight / 2f) - pitch;
        float quadY2 = (ClientSize.Y / 2f) - (wallHeight / 2f) - pitch;

        //if wallside = 1, 3 -> flip

        Shader.wallVertexAttribList.AddRange(new float[]
        {
            quadX1 + debugBorder,
            quadX2 - debugBorder,
            quadY1 - debugBorder,
            quadY2 + debugBorder,
            wallHeight, //Quantize
            rayLength, //Shading
            rayTilePosition, //Horizontal texture stepping
            textureIndex, //Selecting correct texture
            wallSide //Flipping wrong sided textures
        });
    }
}
