using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Security.Cryptography;

namespace Engine;

internal partial class Engine
{
    //Texture atlas size's
    Vector2 objectAtlasSize = (360, 252);
    Vector2 itemAtlasSize = (288, 72);

    float objectSpriteCellSize = 36;
    float itemSpriteCellSize = 36;

    /* Sprite translator
     * Types: Objects, Pick-up items, enemies
     * Type 0 (Objects)
     * - ID 0: Torch
     * - ID 1: Purple torch
     * - ID 2: Bronze Chest
     * - ID 3: Silver Chest
     * - ID 4: Gold Chest
     * - ID 5: Diamond Chest
     * - ID 6: Void Chest
     * Type 1 (Pick-up items)
     * - ID 4: Heal
     * - ID 5: Ammo (No guns in game right now)
     * Type 2 (Enemies)
     * - ID 6: Void Sentinel - Initiate (Level 1)
     * - ID 7: Void Sentinel - Enforcer (Level 2)
     * - ID 8: Void Sentinel - Reaver   (Level 3)
     * - ID 9: Sentinel Prime - K'hrovan
     * - ID 10: Jiggler
     * - ID 11: Korvax
     */

    //Types are used to seperate the functionality of the sprites
    //ID's are used to configure the sprites one-by-one and give them unique textures

    //Animation running time accumulator
    static float _spriteAnimTime;

    struct SpriteAnimConfig
    {
        public int FrameCount;
        public float Fps;
        public SpriteAnimConfig(int frameCount, float fps)
        {
            FrameCount = frameCount;
            Fps = fps;
        }
    }

    //Sprite animation config (Number of Frames, Frames per Second)
    static readonly SpriteAnimConfig[] SpriteAnimTable =
    [
        //Type 0
        new SpriteAnimConfig(frameCount:10, fps:8f),
        //Type 1
        new SpriteAnimConfig(frameCount:8, fps:10f),
    ];

    void ComputeSprites()
    {
        // Advance sprite animation time (seconds)
        _spriteAnimTime += deltaTime;

        //Number of sprites on the map
        int spritesCount = Level.Sprites.Count;

        //Player's directon vectors in radians
        Vector2 playerDirection = (MathF.Cos(playerAngle), MathF.Sin(playerAngle));

        //Convert FOV degrees to radians
        float FOVrad = FOV * (MathF.PI / 180f);

        //Scaling the plane with FOV
        float planeScale = MathF.Tan(FOVrad / 2f);

        //Perpendicular distance of the plane
        Vector2 perpPlaneDist = (-playerDirection.Y * planeScale, playerDirection.X * planeScale);

        //Center of the square viewport in pixels.
        Vector2 viewportCenter = (
            screenHorizontalOffset + (minimumScreenSize / 2f),
            screenVerticalOffset + (minimumScreenSize / 2f) - pitch);

        float halfScreen = minimumScreenSize / 2f;

        //Inverting the 2x2 matrix direction plane
        float invDetBase = (perpPlaneDist.X * playerDirection.Y - playerDirection.X * perpPlaneDist.Y);

        //If determinant is ~0, the projection is invalid (Might never happen if we have a correct FOV value)
        if (MathF.Abs(invDetBase) < 1e-6f)
            return;

        //Inverting the determinant
        float invDet = 1.0f / invDetBase;

        //Building draw order to simulate distance
        var drawOrder = new int[spritesCount];
        var spriteDist = new float[spritesCount];

        for (int i = 0; i < spritesCount; i++)
        {
            drawOrder[i] = i;
            float sx = (Level.Sprites[i].Position.X + 0.5f) * tileSize;
            float sy = (Level.Sprites[i].Position.Y + 0.5f) * tileSize;
            float dx = sx - playerPosition.X;
            float dy = sy - playerPosition.Y;
            spriteDist[i] = dx * dx + dy * dy;
        }

        Array.Sort(drawOrder, (a, b) => spriteDist[b].CompareTo(spriteDist[a]));

        //Loop through the sorted (ordered) sprites
        for (int oi = 0; oi < drawOrder.Length; oi++)
        {
            int i = drawOrder[oi];

            //If sprite is turned off, skip
            if (Level.Sprites[i].State == false) continue;

            // World position
            Vector2 spriteWorldPos = (
                (Level.Sprites[i].Position.X + 0.5f) * tileSize,
                (Level.Sprites[i].Position.Y + 0.5f) * tileSize);

            //Sprite's distance from player
            Vector2 relSpriteDist = (
                spriteWorldPos.X - playerPosition.X,
                spriteWorldPos.Y - playerPosition.Y);

            //Transforming to camera-space
            Vector2 transCamera = (
                invDet * (playerDirection.Y * relSpriteDist.X - playerDirection.X * relSpriteDist.Y),
                invDet * (-perpPlaneDist.Y * relSpriteDist.X + perpPlaneDist.X * relSpriteDist.Y));

            //If sprite is too close or behind the camera, skip
            if (transCamera.Y <= 5f)
                continue;

            //Camera space to screen coordinates
            float screenXCenter = viewportCenter.X + (transCamera.X / transCamera.Y) * halfScreen;

            //Sprite's size on the screen
            float spriteSize = (tileSize / transCamera.Y) * halfScreen;
            
            //Screen coordinates
            float quadX1 = screenXCenter - spriteSize;
            float quadX2 = screenXCenter + spriteSize;
            float quadY2 = viewportCenter.Y - spriteSize;
            float quadY1 = quadY2 + spriteSize * 2;

            //If quad is outside the limit, skip sprite
            if (quadX2 < screenHorizontalOffset ||
                quadX1 > screenHorizontalOffset + minimumScreenSize ||
                quadY1 < screenVerticalOffset ||
                quadY2 > screenVerticalOffset + minimumScreenSize)
                continue;

            int sType = Level.Sprites[i].Type;
            int sId = Level.Sprites[i].Id;
            Vector2 sPos = Level.Sprites[i].Position;

            //=======================
            //If sprite is an object
            //=======================
            if (sType == 0)
            {
                HandleObjects(
                    sType,
                    sId,
                    i,
                    spriteWorldPos,
                    quadX1,
                    quadX2,
                    quadY1,
                    quadY2);
            }

            //=======================
            //If sprite is an item
            //=======================
            else if (sType == 1)
            {
                HandleItems(
                    sType,
                    sId,
                    i,
                    spriteWorldPos,
                    quadX1,
                    quadX2,
                    quadY1,
                    quadY2);
            }
        }
    }

    //Items could be animated sprites
    void HandleItems(
    int sType,
    int sId,
    int i,
    Vector2 spriteWorldPos,
    float quadX1,
    float quadX2,
    float quadY1,
    float quadY2)
    {
        // Horizontal animation based on sprite type config (one row per type)
        float u0 =0f;
        float u1 = itemSpriteCellSize / itemAtlasSize.X;

        var cfg = SpriteAnimTable[sType];
        if (cfg.FrameCount >1 && cfg.Fps >0f)
        {
            int frame = (int)(_spriteAnimTime * cfg.Fps) % cfg.FrameCount;
            u0 = (frame * itemSpriteCellSize) / itemAtlasSize.X;
            u1 = ((frame +1) * itemSpriteCellSize) / itemAtlasSize.X;
        }

        // Vertical stride based on sprite ID (top-down in atlas)
        float v0 =1 - ((sId +1) * itemSpriteCellSize / itemAtlasSize.Y);
        float v1 =1 - (sId * itemSpriteCellSize / itemAtlasSize.Y);

        UploadSprite(
        quadX1,
        quadX2,
        quadY1,
        quadY2,
        u0,
        v0,
        u1,
        v1,
        sType,
        sId);
    }

    //Objects are non-moving, rarely interactable, could be animated sprites
    void HandleObjects(
        int sType,
        int sId,
        int i,
        Vector2 spriteWorldPos,
        float quadX1,
        float quadX2,
        float quadY1,
        float quadY2
        )
    {
        //Default texture UV rect's width
        float u0 =0f;
        float u1 =1f;

        // Simple animation for torches (ID0,1)
        if (sId ==0 || sId ==1)
        {
            var cfg = SpriteAnimTable[sType];
            if (cfg.FrameCount >1 && cfg.Fps >0f)
            {
                int frame = (int)(_spriteAnimTime * cfg.Fps) % cfg.FrameCount;
                u0 = (frame * objectSpriteCellSize) / objectAtlasSize.X;
                u1 = ((frame +1) * objectSpriteCellSize) / objectAtlasSize.X;
            }
            else
            {
                u0 =0f;
                u1 = objectSpriteCellSize / objectAtlasSize.X;
            }
        }
        else if (sId >=2)
        {
            // Only allow chest interaction when player is close enough
            float interactDist =0.75f * tileSize;
            float dx = spriteWorldPos.X - playerPosition.X;
            float dy = spriteWorldPos.Y - playerPosition.Y;
            bool isNear = (dx * dx + dy * dy) <= (interactDist * interactDist);

            //First time chest opening
            if (!Level.Sprites[i].isInteracted && isNear && KeyboardState.IsKeyPressed(Keys.E))
                Level.Sprites[i].isInteracted = true;

            if (!Level.Sprites[i].isInteracted)
            {
                u0 =0f;
                u1 = objectSpriteCellSize / objectAtlasSize.X;
            }

            else
            {
                u0 = objectSpriteCellSize / objectAtlasSize.X;
                u1 =2 * objectSpriteCellSize / objectAtlasSize.X;
            }
        }

        //Texture UV rect's height (Vertical texture stride is based on ID)
        float v0 =1 - ((sId +1) * objectSpriteCellSize / objectAtlasSize.Y);
        float v1 =1 - (sId * objectSpriteCellSize / objectAtlasSize.Y);

        UploadSprite(
        quadX1,
        quadX2,
        quadY1,
        quadY2,
        u0,
        v0,
        u1,
        v1,
        sType,
        sId);
    }

    //Universal vertex attribute uploader
    static void UploadSprite(
        float quadX1,
        float quadX2,
        float quadY1,
        float quadY2,
        float u0,
        float v0,
        float u1,
        float v1,
        int sType,
        int sId)
    {
        ShaderHandler.SpriteVertexAttribList.AddRange(new float[]
        {
            quadX1,
            quadX2,
            quadY1,
            quadY2,
            u0,
            v0,
            u1,
            v1,
            sType,
            sId
        });
    }
}
