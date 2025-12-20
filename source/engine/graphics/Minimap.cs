using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine;

internal partial class Engine
{
    void Minimap()
    {
        float minimapSize = minimumScreenWidth / 5f;
        float minimapTileSize = minimapSize / Math.Max(mapWalls.GetLength(0), mapWalls.GetLength(1));

        //Default minimap background filler
        VertexLoader(
            screenHorizontalOffset + minimumScreenWidth - minimapSize,
            screenHorizontalOffset + minimumScreenWidth,
            screenVerticalOffset,
            screenVerticalOffset + minimapSize,
            0f,
            1f,
            0f
        );

        //Tiles in minimap
        for (int x = 0; x < mapWalls.GetLength(1); x++)
        {
            for (int y = 0; y < mapWalls.GetLength(0); y++)
            {
                float r, g, b;
                if (mapWalls[y, x] == 0)
                {
                    r = 1f;
                    g = 1f;
                    b = 1f;
                }
                else
                {
                    r = 0f;
                    g = 0f;
                    b = 0f;
                }
                vertexAttributesList.AddRange(new float[]
                {
                        (screenHorizontalOffset + minimumScreenWidth - minimapSize) + (x * minimapTileSize),
                        (screenHorizontalOffset + minimumScreenWidth - minimapSize) + (x * minimapTileSize) + minimapTileSize,
                        screenVerticalOffset + (y * minimapTileSize),
                        screenVerticalOffset + (y * minimapTileSize) + minimapTileSize,
                        r,
                        g,
                        b
                });
            }
        }

        float minimapPlayerPositionX1 = minimapSize / ((mapWalls.GetLength(1) * tileSize) / playerPosition.X);
        float minimapPlayerPositionX2 = minimapSize / ((mapWalls.GetLength(1) * tileSize) / playerPosition.X);
        float minimapPlayerPositionY1 = minimapSize / ((mapWalls.GetLength(0) * tileSize) / playerPosition.Y);
        float minimapPlayerPositionY2 = minimapSize / ((mapWalls.GetLength(0) * tileSize) / playerPosition.Y);

        //Player on minimap
        vertexAttributesList.AddRange(new float[]
        {
                (screenHorizontalOffset + minimumScreenWidth - minimapSize) + minimapPlayerPositionX1 - 2f,
                (screenHorizontalOffset + minimumScreenWidth - minimapSize) + minimapPlayerPositionX2 + 2f,
                screenVerticalOffset + minimapPlayerPositionY1 - 2f,
                screenVerticalOffset + minimapPlayerPositionY2 + 2f,
                1f,
                0f,
                0f
        });
    }
}
