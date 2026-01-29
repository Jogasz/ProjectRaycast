using OpenTK.Mathematics;
using System;

namespace Engine;

internal partial class RayCasting
{
    public static void ComputeSprites(
        Vector2 playerPosition,
        int tileSize,
        float playerAngle,
        int FOV,
        float minimumScreenSize,
        float screenHorizontalOffset,
        float screenVerticalOffset,
        float pitch)
    {
        //Number of sprites on the map
        int spritesCount = Level.Sprites.Count;

        //Player's directon vectors in radians
        float dirX = MathF.Cos(playerAngle);
        float dirY = MathF.Sin(playerAngle);

        //Convert FOV degrees to radians
        float FOVrad = FOV * (MathF.PI / 180f);

        //Scaling the plane with FOV
        float planeScale = MathF.Tan(FOVrad / 2f);

        //Perpendicualr distance of the plane
        float planeX = -dirY * planeScale;
        float planeY = dirX * planeScale;

        //Center of the square viewport in pixels.
        float centerX = screenHorizontalOffset + (minimumScreenSize / 2f);
        float centerY = screenVerticalOffset + (minimumScreenSize / 2f) - pitch;

        // Half width of the square viewport in pixels. We use this as projection scale.
        // Intuition: camera projection maps normalized camera X into pixels via halfScreen.
        float halfScreen = minimumScreenSize / 2f;

        //Inverting the 2x2 matrix direction plane
        float invDetBase = (planeX * dirY - dirX * planeY);

        //If determinant is ~0, the projection is invalid (Might never happen if we have a correct FOV value)
        if (MathF.Abs(invDetBase) < 1e-6f)
            return;

        //Inverting the determinant
        float invDet = 1.0f / invDetBase;

        for (int i = 0; i < spritesCount; i++)
        {
            //World position
            float spriteX = (Level.Sprites[i].Position.X + 0.5f) * tileSize;
            float spriteY = (Level.Sprites[i].Position.Y + 0.5f) * tileSize;

            //Sprite's distance from pkayer
            float relX = spriteX - playerPosition.X;
            float relY = spriteY - playerPosition.Y;

            //Transforming to camera-space
            float transformX = invDet * (dirY * relX - dirX * relY);
            float transformY = invDet * (-planeY * relX + planeX * relY);

            //If sprite is too close or behind the camera, skip
            if (transformY <= 0.01f)
                continue;

            //Camera space to screen coordinates
            float screenXCenter = centerX + (transformX / transformY) * halfScreen;

            //Sprites size on the screen
            float size = (tileSize / transformY) * halfScreen;
            
            //Screen coordinates
            float quadX1 = screenXCenter - size;
            float quadX2 = screenXCenter + size;

            float quadY2 = centerY - size;
            float quadY1 = quadY2 + size * 2;

            //If quad is outside the limit, skip sprite
            if (quadX2 < screenHorizontalOffset ||
                quadX1 > screenHorizontalOffset + minimumScreenSize ||
                quadY1 < screenVerticalOffset ||
                quadY2 > screenVerticalOffset + minimumScreenSize)
                continue;

            ShaderHandler.spriteVertexAttribList.AddRange(new float[]
            {
                quadX1,
                quadX2,
                quadY1,
                quadY2,
                1.0f,
                1.0f,
                0.0f
            });
        }
    }
}
