using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static OpenTK.Graphics.OpenGL.GL;

namespace Engine;

internal partial class Engine
{
    void RayCast()
    {
        //CeilingCast();

        float rayAngle = NormalizeAngle(playerAngle + FOVStart);

        for (int i = 0; i < rayCount; i++)
        {
            //Vertical wall check variables
            float offsetX = 0f,
                  offsetY = 0f,
                  verticalRayHitX = 0f,
                  verticalRayHitY = 0f,
                  verticalHypotenuse = 0f;

            int verticalRayCheckingCol,
                verticalRayCheckingRow;

            bool verticalWallFound = false;

            //Pre-defining variables for vertical wall checking
            //If player is looking right
            if (rayAngle > MathX.Quadrant3 || rayAngle < MathX.Quadrant1)
            {
                verticalRayHitX =
                    MathX.Clamp
                    (
                        (float)Math.Floor(playerPosition.X / tileSize) * tileSize + tileSize,
                        0,
                        (float)(mapWalls.GetLength(1) * tileSize - 0.0001f)
                    );
                verticalRayHitY =
                    MathX.Clamp
                    (
                        playerPosition.Y - ((tileSize - (playerPosition.X % tileSize)) * (float)Math.Tan(MathX.Quadrant4 - rayAngle)),
                        0,
                        (float)(mapWalls.GetLength(0) * tileSize - 0.0001f)
                    );
                offsetX = tileSize;
                offsetY = -(offsetX * (float)Math.Tan(MathX.Quadrant4 - rayAngle));
            }
            //If player is looking left
            else if (rayAngle < MathX.Quadrant3 && rayAngle > MathX.Quadrant1)
            {
                verticalRayHitX =
                    MathX.Clamp
                    (
                        (float)Math.Floor(playerPosition.X / tileSize) * tileSize - 0.0001f,
                        0,
                        (float)(mapWalls.GetLength(1) * tileSize - 0.0001f)
                    );
                verticalRayHitY =
                    MathX.Clamp
                    (
                        playerPosition.Y - ((playerPosition.X % tileSize) * (float)Math.Tan(rayAngle)),
                        0,
                        (float)(mapWalls.GetLength(0) * tileSize - 0.0001f)
                    );
                offsetX = -tileSize;
                offsetY = offsetX * (float)Math.Tan(rayAngle);
            }

            //Grid checker
            verticalRayCheckingCol = (int)Math.Floor(verticalRayHitX / tileSize);
            verticalRayCheckingRow = (int)Math.Floor(verticalRayHitY / tileSize);

            //Setting iterator
            int renderDistanceIterator = 0;

            //Cycle to find a wall
            while
            (
                renderDistanceIterator < renderDistance &&
                !verticalWallFound &&
                verticalRayHitX != (float)(mapWalls.GetLength(1) * tileSize - 0.0001f) &&
                verticalRayHitX != 0 &&
                verticalRayHitY != (float)(mapWalls.GetLength(0) * tileSize - 0.0001f) &&
                verticalRayHitY != 0
            )
            {
                //If wall is found
                if (mapWalls[verticalRayCheckingRow, verticalRayCheckingCol] > 0)
                {
                    renderDistanceIterator = renderDistance;
                    verticalWallFound = true;
                }
                //If wall isn't found
                else
                {
                    verticalRayHitX =
                        MathX.Clamp
                        (
                            verticalRayHitX + offsetX,
                            0,
                            (float)(mapWalls.GetLength(1) * tileSize - 0.0001f)
                        );
                    verticalRayHitY =
                        MathX.Clamp
                        (
                            verticalRayHitY + offsetY,
                            0,
                            (float)(mapWalls.GetLength(0) * tileSize - 0.0001f)
                        );
                    renderDistanceIterator++;
                    verticalRayCheckingCol = (int)(verticalRayHitX / tileSize);
                    verticalRayCheckingRow = (int)(verticalRayHitY / tileSize);
                }
            }

            //Vertical hypotenuse for line's length
            verticalHypotenuse = (float)Math.Sqrt(Math.Pow(playerPosition.X - verticalRayHitX, 2) + Math.Pow(playerPosition.Y - verticalRayHitY, 2));

            //Horizontal wall check variables
            float horizontalRayHitX = 0f,
                  horizontalRayHitY = 0f,
                  horizontalHypotenuse = 0f;

            int horizontalRayCheckingCol = 0,
                horizontalRayCheckingRow = 0;

            bool horizontalWallFound = false;

            //Pre-defining variables for horizontal wall checking
            //If player is looking up
            if (rayAngle > MathX.Quadrant2)
            {
                horizontalRayHitY =
                    MathX.Clamp
                    (
                        (float)Math.Floor(playerPosition.Y / tileSize) * tileSize - 0.0001f,
                        0,
                        (float)(mapWalls.GetLength(0) * tileSize - 0.0001f)
                    );
                horizontalRayHitX =
                    MathX.Clamp
                    (
                        playerPosition.X + (playerPosition.Y % tileSize) / (float)Math.Tan(MathX.Quadrant4 - rayAngle),
                        0,
                        (float)(mapWalls.GetLength(1) * tileSize - 0.0001f)
                    );
                offsetY = -tileSize;
                offsetX = tileSize / (float)Math.Tan(MathX.Quadrant4 - rayAngle);
            }
            //If player is looking down
            if (rayAngle < MathX.Quadrant2)
            {
                horizontalRayHitY =
                    MathX.Clamp
                    (
                        (float)Math.Floor(playerPosition.Y / tileSize) * tileSize + tileSize,
                        0,
                        (float)(mapWalls.GetLength(0) * tileSize - 0.0001f)
                    );
                horizontalRayHitX =
                    MathX.Clamp
                    (
                        playerPosition.X - (tileSize - (playerPosition.Y % tileSize)) / (float)Math.Tan(MathX.Quadrant4 - rayAngle),
                        0,
                        (float)(mapWalls.GetLength(1) * tileSize - 0.0001f)
                    );
                offsetY = tileSize;
                offsetX = -offsetY / (float)Math.Tan(MathX.Quadrant4 - rayAngle);
            }

            //Grid checker
            horizontalRayCheckingCol = (int)Math.Floor(horizontalRayHitX / tileSize);
            horizontalRayCheckingRow = (int)Math.Floor(horizontalRayHitY / tileSize);

            //Resetting iterator variable
            renderDistanceIterator = 0;

            //Cycle to find a wall
            while
            (
                renderDistanceIterator < renderDistance &&
                !horizontalWallFound &&
                horizontalRayHitY != (float)(mapWalls.GetLength(0) * tileSize - 0.0001f) &&
                horizontalRayHitY != 0 &&
                horizontalRayHitX != (float)(mapWalls.GetLength(1) * tileSize - 0.0001f) &&
                horizontalRayHitX != 0
            )
            {
                //If wall is found
                if (mapWalls[horizontalRayCheckingRow, horizontalRayCheckingCol] > 0)
                {
                    renderDistanceIterator = renderDistance;
                    horizontalWallFound = true;
                }
                else
                {
                    //If wall isn't found
                    horizontalRayHitY =
                        MathX.Clamp
                        (
                            horizontalRayHitY + offsetY,
                            0,
                            (float)(mapWalls.GetLength(0) * tileSize - 0.0001f)
                        );
                    horizontalRayHitX =
                        MathX.Clamp
                        (
                            horizontalRayHitX + offsetX,
                            0,
                            (float)(mapWalls.GetLength(1) * tileSize - 0.0001f)
                        );
                    renderDistanceIterator++;
                    horizontalRayCheckingCol = (int)(horizontalRayHitX / tileSize);
                    horizontalRayCheckingRow = (int)(horizontalRayHitY / tileSize);
                }
            }

            //Horizontal hypotenuse for line's length
            horizontalHypotenuse = (float)Math.Sqrt(Math.Pow(playerPosition.X - horizontalRayHitX, 2) + Math.Pow(playerPosition.Y - horizontalRayHitY, 2));

            //==========
            //Line datas
            //==========
            float rayLength = Math.Min(verticalHypotenuse, horizontalHypotenuse);
            float rayTilePosition = 0;
            int wallSide = 0;
            int wallType = 0;
            //If wall is vertical
            if (rayLength == verticalHypotenuse && verticalWallFound)
            {
                //Ray's end's position relative to the tile
                rayTilePosition = verticalRayHitY % tileSize;

                //If wall side is left
                if (rayAngle > MathX.Quadrant1 && rayAngle < MathX.Quadrant3)
                {
                    wallSide = 1;
                }
                //If wall side is right
                else if (rayAngle > MathX.Quadrant3 || rayAngle < MathX.Quadrant1)
                {
                    wallSide = 2;
                }

                //What type of wall did the ray hit (if out of bounds, 0)
                wallType = mapWalls[verticalRayCheckingRow, verticalRayCheckingCol];
            }
            //If wall is horizontal
            else if (rayLength == horizontalHypotenuse && horizontalWallFound)
            {
                //Ray's end's position relative to the tile
                rayTilePosition = horizontalRayHitX % tileSize;

                //If wall side is top
                if (rayAngle > 0 && rayAngle < Math.PI)
                {
                    wallSide = 3;
                }
                //If wall side is bottom
                else if (rayAngle > Math.PI && rayAngle < MathX.Quadrant4)
                {
                    wallSide = 4;
                }

                //What type of wall did the ray hit (if out of bounds, 0)
                wallType = mapWalls[horizontalRayCheckingRow, horizontalRayCheckingCol];
            }
            //No wall hit
            else
            {
                wallType = 0;
            }

            float wallHeight = (float)((tileSize * minimumScreenHeight) /
                (rayLength * (float)Math.Cos(playerAngle -
                (playerAngle + FOVStart + i * radBetweenRays))));

            CeilingCast(i, wallHeight, rayAngle);

            FloorCast(i, wallHeight, rayAngle);

            if (wallType != 0)
            {
                ComputeWalls(i, wallType, wallSide, wallHeight, rayLength, rayTilePosition);
            }
            
            //Incrementing rayAngle for next ray
            rayAngle = NormalizeAngle(rayAngle + radBetweenRays);
        }
    }

    void CeilingCast(int i, float wallHeight, float rayAngle)
    {
        int textureIndex;

        //Height of the player
        float cameraZ = ClientSize.Y / 2;

        float stepValue = wallWidth;

        float quadX1 = screenHorizontalOffset + (i * wallWidth);
        float quadX2 = screenHorizontalOffset + ((i + 1) * wallWidth);

        float quadY1 = screenVerticalOffset;
        float quadY2 = screenVerticalOffset + stepValue;

        float wallTop = ((screenVerticalOffset + (minimumScreenHeight / 2) - (wallHeight / 2)) + pitch);

        while (quadY1 < wallTop)
        {
            //If Y2 has reached wallHeight, Y2 may be equal to wallHeight
            if (quadY2 > wallTop)
            {
                quadY2 = wallTop;
            }

            //Y of the current quad on the screen
            float rowY = (ClientSize.Y / 2) - (quadY1 + ((quadY2 - quadY1) / 2)) + pitch;

            //Distance of the pixel from the player
            float ceilingPixelDistance = ((cameraZ / rowY) * tileSize) / (float)Math.Cos(playerAngle - rayAngle);

            //World X position of the pixel
            float ceilingPixelX = playerPosition.X + ((float)Math.Cos(rayAngle) * ceilingPixelDistance);

            //World Y position of the pixel
            float ceilingPixelY = playerPosition.Y + ((float)Math.Sin(rayAngle) * ceilingPixelDistance);

            //Texture
            //If out of bounds
            if
            (
                ceilingPixelX >= mapCeiling.GetLength(1) * tileSize ||
                ceilingPixelX < 0f ||
                ceilingPixelY >= mapCeiling.GetLength(0) * tileSize ||
                ceilingPixelY < 0f
            )
            {
                quadY1 = quadY2;
                quadY2 += stepValue;
                continue;
            }

            textureIndex = (int)mapCeiling[(int)Math.Floor(ceilingPixelY / tileSize), (int)Math.Floor(ceilingPixelX / tileSize)];

            //If index is zero
            if (textureIndex == 0)
            {
                quadY1 = quadY2;
                quadY2 += stepValue;
                continue;
            }

            //Calculating RGB variables
            int RGBCalc = ((int)Math.Floor(Textures.datas[textureIndex][1] / (tileSize / (ceilingPixelX % tileSize))) * Textures.datas[textureIndex][1] * 3) +
                          ((int)Math.Floor(Textures.datas[textureIndex][0] / (tileSize / (ceilingPixelY % tileSize))) * 3);

            //Calculating shading and lighting with distance
            float shadeCalc = ceilingPixelDistance * distanceShade;

            float r = (Textures.datas[textureIndex][2 + RGBCalc] - shadeCalc) / 255f;
            float g = (Textures.datas[textureIndex][2 + RGBCalc + 1] - shadeCalc) / 255f;
            float b = (Textures.datas[textureIndex][2 + RGBCalc + 2] - shadeCalc) / 255f;

            vertexAttributesList.AddRange(new float[]
            {
                        quadX1 + debugBorder,
                        quadX2 - debugBorder,
                        quadY1 + debugBorder,
                        quadY2 - debugBorder,
                        r,
                        g,
                        b
            });

            //Incrementing Y1 and Y2
            quadY1 = quadY2;
            quadY2 += stepValue;
        }
    }

    void FloorCast(int i, float wallHeight, float rayAngle)
    {
        int textureIndex;

        //Height of the player
        float cameraZ = ClientSize.Y / 2;

        float stepValue = wallWidth;

        float quadX1 = screenHorizontalOffset + (i * wallWidth);
        float quadX2 = screenHorizontalOffset + ((i + 1) * wallWidth);

        float quadY1 = screenVerticalOffset + minimumScreenHeight - stepValue;
        float quadY2 = screenVerticalOffset + minimumScreenHeight;

        float wallBottom = (screenVerticalOffset + (minimumScreenHeight / 2) + (wallHeight / 2)) + pitch;

        while (quadY2 > wallBottom)
        {
            //If Y2 has reached wallHeight, Y2 may be equal to wallHeight
            if (quadY1 < wallBottom)
            {
                quadY1 = wallBottom;
            }

            //Y of the current quad on the screen
            float rowY = quadY1 + ((quadY2 - quadY1) / 2) - (ClientSize.Y / 2) - pitch;

            //Distance of the pixel from the player
            float floorPixelDistance = ((cameraZ / rowY) * tileSize) / (float)Math.Cos(playerAngle - rayAngle);

            //World X position of the pixel
            float floorPixelX = playerPosition.X + ((float)Math.Cos(rayAngle) * floorPixelDistance);

            //World Y position of the pixel
            float floorPixelY = playerPosition.Y + ((float)Math.Sin(rayAngle) * floorPixelDistance);

            //Texture
            //If out of bounds
            if
            (
                floorPixelX >= mapFloor.GetLength(1) * tileSize ||
                floorPixelX < 0f ||
                floorPixelY >= mapFloor.GetLength(0) * tileSize ||
                floorPixelY < 0f
            )
            {
                quadY2 = quadY1;
                quadY1 -= stepValue;
                continue;
            }

            textureIndex = (int)mapFloor[(int)Math.Floor(floorPixelY / tileSize), (int)Math.Floor(floorPixelX / tileSize)];

            //If index is zero
            if (textureIndex == 0)
            {
                quadY2 = quadY1;
                quadY1 -= stepValue;
                continue;
            }

            //Calculating RGB variables
            int RGBCalc = ((int)Math.Floor(Textures.datas[textureIndex][1] / (tileSize / (floorPixelX % tileSize))) * Textures.datas[textureIndex][1] * 3) +
                          ((int)Math.Floor(Textures.datas[textureIndex][0] / (tileSize / (floorPixelY % tileSize))) * 3);

            //Calculating shading and lighting with distance
            float shadeCalc = floorPixelDistance * distanceShade;

            float r = (Textures.datas[textureIndex][2 + RGBCalc] - shadeCalc) / 255f;
            float g = (Textures.datas[textureIndex][2 + RGBCalc + 1] - shadeCalc) / 255f;
            float b = (Textures.datas[textureIndex][2 + RGBCalc + 2] - shadeCalc) / 255f;

            vertexAttributesList.AddRange(new float[]
            {
                        quadX1 + debugBorder,
                        quadX2 - debugBorder,
                        quadY1 + debugBorder,
                        quadY2 - debugBorder,
                        r,
                        g,
                        b
            });

            //Incrementing Y1 and Y2
            quadY2 = quadY1;
            quadY1 -= stepValue;
        }
    }

    void ComputeWalls(int nthRay, int textureIndex, int wallSide, float wallHeight, float rayLength, float rayTilePosition)
    {
        int texWidth = Textures.datas[textureIndex][0];
        int texHeight = Textures.datas[textureIndex][1];

        float quadX1 = nthRay * wallWidth + screenHorizontalOffset;
        float quadX2 = (nthRay + 1) * wallWidth + screenHorizontalOffset;

        float quadY1 = (ClientSize.Y / 2) - (wallHeight / 2) + pitch;

        //Shading and lighting with distance
        float shadeCalc = rayLength * distanceShade;

        //Optimizing with shade (if the shade is strong enough to make everything black, just paint the line black)
        int shadeLimit = 255;

        if (shadeCalc >= shadeLimit)
        {
            float quadY2 = (ClientSize.Y / 2) + (wallHeight / 2) + pitch;
            VertexLoader(
                quadX1,
                quadX2,
                quadY1,
                quadY2,
                1f,
                0f,
                0f
            );
        }
        else
        {
            float quadY2 = (ClientSize.Y / 2) - (wallHeight / 2) + (wallHeight / texHeight) + pitch;

            //Drawing pixels in lines from up to down (walls)
            for (int k = 0; k < texHeight; k++)
            {
                //Ensuring that the graphical image stays within the interpolated screen size
                if (quadY2 < screenVerticalOffset || quadY1 > (screenVerticalOffset + minimumScreenHeight))
                {
                    quadY1 = quadY2;
                    quadY2 += (wallHeight / texHeight);

                    continue;
                }
                else
                {
                    //If Y1 has reached the top of the screen, it may be equal to it
                    quadY1 =
                    (quadY1 < screenVerticalOffset && quadY2 > screenVerticalOffset) ?
                    screenVerticalOffset :
                    quadY1;

                    //If Y2 has reached the bottom of the screen, it may be equal to it
                    quadY2 =
                    (quadY1 < (screenVerticalOffset + minimumScreenHeight) && quadY2 > (screenVerticalOffset + minimumScreenHeight)) ?
                    (screenVerticalOffset + minimumScreenHeight) :
                    quadY2;

                    //Calcuating RGB value's index in texture array
                    int RGBCalc = (wallSide == 1 || wallSide == 3) ?
                        //Mirroring wrong textures
                        (int)(Math.Floor(Math.Clamp((tileSize - rayTilePosition) / tileSize, 0, 0.9999) * texWidth) * 3) + (k * ((texHeight) * 3)) :
                        (int)(Math.Floor(Math.Clamp(rayTilePosition / tileSize, 0, 0.9999) * texWidth) * 3) + (k * ((texHeight) * 3));

                    float r = (Textures.datas[textureIndex][2 + RGBCalc] - shadeCalc) / 255f;
                    float g = (Textures.datas[textureIndex][2 + RGBCalc + 1] - shadeCalc) / 255f;
                    float b = (Textures.datas[textureIndex][2 + RGBCalc + 2] - shadeCalc) / 255f;

                    VertexLoader(
                        quadX1 + debugBorder,
                        quadX2 - debugBorder,
                        quadY1 + debugBorder,
                        quadY2 - debugBorder,
                        r,
                        g,
                        b
                    );

                    quadY1 = quadY2;
                    quadY2 += (wallHeight / texHeight);
                }
            }
        }
    }
}
