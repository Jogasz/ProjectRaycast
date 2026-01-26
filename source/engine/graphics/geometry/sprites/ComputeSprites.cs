using OpenTK.Mathematics;
using System;

namespace Engine;

internal partial class RayCasting
{
    public static void ComputeSprites(
        Vector2 playerPosition,
        int tileSize,
        float playerAngle,
        int fovDegrees,
        float minimumScreenSize,
        float screenHorizontalOffset,
        float screenVerticalOffset,
        float pitch)
    {
        int spritesCount = Level.Sprites.Count;

        float dirX = MathF.Cos(playerAngle);
        float dirY = MathF.Sin(playerAngle);

        float fovRad = fovDegrees * (MathF.PI / 180f);
        float planeScale = MathF.Tan(fovRad / 2f);

        float planeX = -dirY * planeScale;
        float planeY = dirX * planeScale;

        float centerX = screenHorizontalOffset + (minimumScreenSize / 2f);
        float centerY = screenVerticalOffset + (minimumScreenSize / 2f) - pitch;

        // this is the "focal length" in pixels for the square viewport
        float halfScreen = minimumScreenSize / 2f;

        // Inverse determinant for [dir plane] matrix
        float invDetBase = (planeX * dirY - dirX * planeY);

        for (int i = 0; i < spritesCount; i++)
        {
            float spriteX = (Level.Sprites[i].Position.X + 0.5f) * tileSize;
            float spriteY = (Level.Sprites[i].Position.Y + 0.5f) * tileSize;

            float relX = spriteX - playerPosition.X;
            float relY = spriteY - playerPosition.Y;

            float invDet = 1.0f / invDetBase;

            float transformX = invDet * (dirY * relX - dirX * relY);
            float transformY = invDet * (-planeY * relX + planeX * relY);

            if (transformY <= 0.01f)
                continue;

            // Correct screen mapping: offset is additive, not scaled
            float screenXCenter = centerX + (transformX / transformY) * halfScreen;

            float size = (tileSize / transformY) * halfScreen;

            float x1 = screenXCenter - size;
            float x2 = screenXCenter + size;

            float yBottom = centerY - size;
            float yTop = yBottom + size * 2;

            float minX = screenHorizontalOffset;
            float maxX = screenHorizontalOffset + minimumScreenSize;
            float minY = screenVerticalOffset;
            float maxY = screenVerticalOffset + minimumScreenSize;

            if (x2 < minX || x1 > maxX || yTop < minY || yBottom > maxY)
                continue;

            Shader.spriteVertexAttribList.AddRange(new float[]
            {
                x1, x2, yTop, yBottom,
                1.0f, 1.0f, 0.0f
            });
        }
    }
}
