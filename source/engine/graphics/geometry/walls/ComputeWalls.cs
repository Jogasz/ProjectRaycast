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
        //int texWidth = Textures.datas[textureIndex][0];
        //int texHeight = Textures.datas[textureIndex][1];

        float quadX1 = nthRay * wallWidth + screenHorizontalOffset;
        float quadX2 = (nthRay + 1) * wallWidth + screenHorizontalOffset;

        float quadY1 = (ClientSize.Y / 2f) + (wallHeight / 2f);
        float quadY2 = (ClientSize.Y / 2f) - (wallHeight / 2f);

        //Shading and lighting with distance
        float shadeCalc = rayLength * distanceShade;

        //Optimizing with shade (if the shade is strong enough to make everything black, just paint the line black)
        int shadeLimit = 255;

        float r = (70f - shadeCalc) / 255f;
        float g = (120f - shadeCalc) / 255f;
        float b = (210f - shadeCalc) / 255f;

        Engine.defVertexAttributesList.AddRange(new float[]
        {
            quadX1 + debugBorder,
            quadX2 - debugBorder,
            quadY1 + debugBorder,
            quadY2 + debugBorder,
            r,
            g,
            b
        });

        //if (shadeCalc >= shadeLimit)
        //{
        //    float quadY2 = (ClientSize.Y / 2) + (wallHeight / 2) + pitch;

        //    Engine.defVertexAttributesList.AddRange(new float[]
        //    {
        //        quadX1,
        //        quadX2,
        //        quadY1,
        //        quadY2,
        //        1f,
        //        0f,
        //        0f
        //    });
        //}
        //else
        //{
        //    float quadY2 = (ClientSize.Y / 2) - (wallHeight / 2) + (wallHeight / texHeight) + pitch;

        //    //Drawing pixels in lines from up to down (walls)
        //    for (int k = 0; k < texHeight; k++)
        //    {
        //        //Ensuring that the graphical image stays within the interpolated screen size
        //        if (quadY2 < screenVerticalOffset || quadY1 > (screenVerticalOffset + minimumScreenHeight))
        //        {
        //            quadY1 = quadY2;
        //            quadY2 += (wallHeight / texHeight);

        //            continue;
        //        }
        //        else
        //        {
        //            //If Y1 has reached the top of the screen, it may be equal to it
        //            quadY1 =
        //            (quadY1 < screenVerticalOffset && quadY2 > screenVerticalOffset) ?
        //            screenVerticalOffset :
        //            quadY1;

        //            //If Y2 has reached the bottom of the screen, it may be equal to it
        //            quadY2 =
        //            (quadY1 < (screenVerticalOffset + minimumScreenHeight) && quadY2 > (screenVerticalOffset + minimumScreenHeight)) ?
        //            (screenVerticalOffset + minimumScreenHeight) :
        //            quadY2;

        //            //Calcuating RGB value's index in texture array
        //            int RGBCalc = (wallSide == 1 || wallSide == 3) ?
        //                //Mirroring wrong textures
        //                (int)(Math.Floor(Math.Clamp((tileSize - rayTilePosition) / tileSize, 0, 0.9999) * texWidth) * 3) + (k * ((texHeight) * 3)) :
        //                (int)(Math.Floor(Math.Clamp(rayTilePosition / tileSize, 0, 0.9999) * texWidth) * 3) + (k * ((texHeight) * 3));

        //            float r = (Textures.datas[textureIndex][2 + RGBCalc] - shadeCalc) / 255f;
        //            float g = (Textures.datas[textureIndex][2 + RGBCalc + 1] - shadeCalc) / 255f;
        //            float b = (Textures.datas[textureIndex][2 + RGBCalc + 2] - shadeCalc) / 255f;

        //            Engine.defVertexAttributesList.AddRange(new float[]
        //            {
        //                quadX1 + debugBorder,
        //                quadX2 - debugBorder,
        //                quadY1 + debugBorder,
        //                quadY2 - debugBorder,
        //                r,
        //                g,
        //                b
        //            });

        //            quadY1 = quadY2;
        //            quadY2 += (wallHeight / texHeight);
        //        }
        //    }
        //}
    }
}
