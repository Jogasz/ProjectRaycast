using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

public class GraphicWindow
{
    public static int TileSize = Settings.Gameplay.TileSize;
    public static int ScreenWidth;
    public static int ScreenHeight;
    public static int[,] MapFloor = Level.mapFloor;

    public static void Run()
    {
        GameWindow Screen = new GameWindow
        (
            1000,
            1000,
            GraphicsMode.Default,
            "Graphic Screen"
            //GameWindowFlags.Fullscreen
        );

        //Screen.CursorVisible = false;
        //Screen.CursorGrabbed = true;

        //For test-only
        //Screen.VSync = VSyncMode.Off;

        WindowManager.SetupPixelCoordinates(Screen);

        Screen.RenderFrame += (sender, e) =>
        {
            //Window color and clear
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            //Setting up screen pixel coordinates
            WindowManager.SetupPixelCoordinates(Screen);

            //Updating the engine and it's variables
            Engine.Update();

            GL.Begin(PrimitiveType.Quads);

            //Using the minimum of the screen's dimensions to keep a square aspect ratio
            int minimumScreenHeight = Screen.Height > Screen.Width ? Screen.Width : Screen.Height;
            int minimumScreenWidth = Screen.Width > Screen.Height ? Screen.Height : Screen.Width;

            //Calculating horizontal and vertical screen offset to center the game if needed
            int screenHorizontalOffset = Screen.Width > Screen.Height ? ((Screen.Width - minimumScreenWidth) / 2) : 0;
            int screenVerticalOffset = Screen.Height > Screen.Width ? ((Screen.Height - minimumScreenHeight) / 2) : 0;

            //Calculating wall width
            float WallWidth = (float)minimumScreenWidth / (float)Settings.Graphics.RayCount;

            //Allowed screen's background color
            GL.Color3(0f, 0f, 0f);
            GL.Vertex2(screenHorizontalOffset, screenVerticalOffset);
            GL.Vertex2(screenHorizontalOffset + minimumScreenWidth, screenVerticalOffset);
            GL.Vertex2(screenHorizontalOffset + minimumScreenWidth, screenVerticalOffset + minimumScreenHeight);
            GL.Vertex2(screenHorizontalOffset, screenVerticalOffset + minimumScreenHeight);

            int RGBCalc;
            float shadeCalc, r, g, b;

            //Z position of camera (Right now its exactly the middle of the screen)
            float cameraZ = minimumScreenHeight / 2f;

            //Drawing graphics
            for (int i = 0; i < Settings.Graphics.RayCount; i++)
            {
                int[][] path = null;

                //Calculating wall height using interpolated values for movie look
                float WallHeight = (float)((TileSize * minimumScreenHeight) /
                    (Engine.RayDatas[i, 0] * (float)Math.Cos(Engine.PlayerAngle -
                    (Engine.PlayerAngle + Engine.FOVStart + i * Engine.RadBetweenRays))));

                //Floor and ceiling X position variables
                float floorCeilingPixelXLeft = i * WallWidth + screenHorizontalOffset;
                float floorCeilingPixelXRight = (i + 1) * WallWidth + screenHorizontalOffset;

                //Floor Y position variables
                float floorPixelYTop = (minimumScreenHeight / 2) + (WallHeight / 2) + screenVerticalOffset;
                float floorPixelYBottom = (minimumScreenHeight / 2) + (WallHeight / 2) +  WallWidth + screenVerticalOffset;

                //Ceiling Y position variables
                float ceilingPixelYTop = (minimumScreenHeight / 2) - (WallHeight / 2) -  WallWidth + screenVerticalOffset;
                float ceilingPixelYBottom = (minimumScreenHeight / 2) - (WallHeight / 2) + screenVerticalOffset;

                float ceilingPixelDistance,
                      ceilingPixelXWorldPosition,
                      ceilingPixelYWorldPosition,
                      floorPixelDistance,
                      floorPixelXWorldPosition,
                      floorPixelYWorldPosition;

                //Ceiling drawing
                while (ceilingPixelYBottom > screenVerticalOffset)
                {
                    /* 
                    * If the pixel's bottom position is inside the correct screen, but the top position sticks out,
                    * the top position's Y value may be equal to the allowed screen's top value.
                    */
                    ceilingPixelYTop =
                    (ceilingPixelYTop < screenVerticalOffset) ?
                    screenVerticalOffset :
                    ceilingPixelYTop;

                    //Y of the current pixel on the screen
                    float rowY = (Screen.Height / 2) - (ceilingPixelYTop + ((ceilingPixelYBottom - ceilingPixelYTop) / 2));

                    ceilingPixelDistance = (cameraZ / rowY) * TileSize;

                    shadeCalc = ceilingPixelDistance * Settings.Graphics.DistanceShade;

                    r = (100f - shadeCalc) < 0 ? 0f : (100f - shadeCalc) / 255f;
                    g = (100f - shadeCalc) < 0 ? 0f : (100f - shadeCalc) / 255f;
                    b = (100f - shadeCalc) < 0 ? 0f : (100f - shadeCalc) / 255f;

                    GL.Color3(r, g, b);
                    GL.Vertex2(floorCeilingPixelXLeft, ceilingPixelYTop);
                    GL.Vertex2(floorCeilingPixelXRight, ceilingPixelYTop);
                    GL.Vertex2(floorCeilingPixelXRight, ceilingPixelYBottom);
                    GL.Vertex2(floorCeilingPixelXLeft, ceilingPixelYBottom);

                    ceilingPixelYBottom = ceilingPixelYTop;
                    ceilingPixelYTop -= WallWidth;
                }

                //Floor drawing
                while (floorPixelYTop < (screenVerticalOffset + minimumScreenHeight))
                {
                    /* 
                    * If the pixel's top position is inside the correct screen, but the bottom position sticks out,
                    * the bottom position's Y value may be equal to the allowed screen's bottom value.
                    */
                    floorPixelYBottom =
                    (floorPixelYBottom > (screenVerticalOffset + minimumScreenHeight)) ?
                    (screenVerticalOffset + minimumScreenHeight) :
                    floorPixelYBottom;

                    //Y of the current pixel on the screen
                    float rowY = floorPixelYTop + ((floorPixelYBottom - floorPixelYTop) / 2) - (Screen.Height / 2);

                    //Distance of the currently drawn pixel and fisheye correction
                    floorPixelDistance = ((cameraZ / rowY) * TileSize) / ((float)Math.Cos(Engine.PlayerAngle - Engine.RayDatas[i, 6]));

                    //World position of the pixel
                    floorPixelXWorldPosition = Engine.playerPosition.X + (Engine.RayDatas[i, 1] * floorPixelDistance);
                    floorPixelYWorldPosition = Engine.playerPosition.Y + (Engine.RayDatas[i, 2] * floorPixelDistance);

                    int mapFloorTextureNum = MapFloor[(int)Math.Floor(floorPixelYWorldPosition / TileSize), (int)Math.Floor(floorPixelXWorldPosition / TileSize)];

                    //Textures
                    switch (mapFloorTextureNum)
                    {
                        //No floor
                        case 0:
                            continue;
                        //Floor
                        //Default textures
                        case 1:
                            path = Textures.stoneBricksTexture;
                            break;
                    }

                    RGBCalc = ((int)Math.Floor(path[0][1] / (TileSize / (floorPixelYWorldPosition % TileSize))) * path[0][1] * 3) +
                              ((int)Math.Floor(path[0][0] / (TileSize / (floorPixelXWorldPosition % TileSize))) * 3);

                    shadeCalc = floorPixelDistance * Settings.Graphics.DistanceShade;

                    r = (path[1][RGBCalc] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc] - shadeCalc) / 255f;
                        g = (path[1][RGBCalc + 1] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 1] - shadeCalc) / 255f;
                        b = (path[1][RGBCalc + 2] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 2] - shadeCalc) / 255f;

                    GL.Color3(r, g, b);
                    GL.Vertex2(floorCeilingPixelXLeft, floorPixelYTop);
                    GL.Vertex2(floorCeilingPixelXRight, floorPixelYTop);
                    GL.Vertex2(floorCeilingPixelXRight, floorPixelYBottom);
                    GL.Vertex2(floorCeilingPixelXLeft, floorPixelYBottom);

                    floorPixelYTop = floorPixelYBottom;
                    floorPixelYBottom += WallWidth;
                }

                path = null;

                //Textures
                switch (Engine.RayDatas[i, 5])
                {
                    //No wall
                    case 0:
                        continue;
                    //Wall
                    //Default textures
                    case 1:
                        path = Textures.bricksTexture;
                        break;
                    case 2:
                        path = Textures.stoneBricksTexture;
                        break;
                    case 3:
                        path = Textures.mossyBricksTexture;
                        break;

                        //Window textures

                        //Paintings
                }

                //Drawing pixels in lines from up to down (walls)
                for (int k = 0; k < path[0][1]; k++)
                {
                    //Temporal calculation for line positions to avoid repetition
                    float tempPixelCalcTop = (Screen.Height / 2) - (WallHeight / 2) + (k * (WallHeight / path[0][1]));
                    float tempPixelCalcBottom = (Screen.Height / 2) - (WallHeight / 2) + ((k + 1) * (WallHeight / path[0][1]));

                    //Ensuring that the graphical image stays within the interpolated screen size
                    if (tempPixelCalcBottom < screenVerticalOffset || tempPixelCalcTop > (screenVerticalOffset + minimumScreenHeight))
                    {
                        continue;
                    }
                    else
                    {
                        /* 
                         * If the pixel's bottom position is inside the correct screen, but the top position sticks out,
                         * the top position's Y value may be equal to the allowed screen's top value.
                        */
                        tempPixelCalcTop =
                        (tempPixelCalcTop < screenVerticalOffset && tempPixelCalcBottom > screenVerticalOffset) ?
                        screenVerticalOffset :
                        tempPixelCalcTop;

                        /* 
                         * If the pixel's top position is inside the correct screen, but the bottom position sticks out,
                         * the bottom position's Y value may be equal to the allowed screen's bottom value.
                        */
                        tempPixelCalcBottom =
                        (tempPixelCalcTop < (screenVerticalOffset + minimumScreenHeight) && tempPixelCalcBottom > (screenVerticalOffset + minimumScreenHeight)) ?
                        (screenVerticalOffset + minimumScreenHeight) :
                        tempPixelCalcBottom;

                        //Mirroring wrong textures
                        RGBCalc = (Engine.RayDatas[i, 4] == 1 || Engine.RayDatas[i, 4] == 3) ?
                            ((int)Math.Floor((TileSize - Engine.RayDatas[i, 3]) / (TileSize / (float)path[0][0])) * 3) + k * (path[0][0] * 3) :
                            ((int)Math.Floor(Engine.RayDatas[i, 3] / (TileSize / (float)path[0][0])) * 3) + k * (path[0][0] * 3);

                        //Calculating shading and lighting with distance
                        shadeCalc = Engine.RayDatas[i, 0] * Settings.Graphics.DistanceShade;

                        //Calculating RGB
                        r = (path[1][RGBCalc] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc] - shadeCalc) / 255f;
                        g = (path[1][RGBCalc + 1] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 1] - shadeCalc) / 255f;
                        b = (path[1][RGBCalc + 2] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 2] - shadeCalc) / 255f;

                        //Drawing the line
                        GL.Color3(r, g, b);
                        GL.Vertex2(i * WallWidth + screenHorizontalOffset, tempPixelCalcTop);
                        GL.Vertex2((i + 1) * WallWidth + screenHorizontalOffset, tempPixelCalcTop);
                        GL.Vertex2((i + 1) * WallWidth + screenHorizontalOffset, tempPixelCalcBottom);
                        GL.Vertex2(i * WallWidth + screenHorizontalOffset, tempPixelCalcBottom);
                    }
                }
            }

            //Testing movie look
            //GL.Begin(PrimitiveType.Lines);
            //GL.Color3(1f, 0f, 0f);

            //GL.Vertex2(screenHorizontalOffset, screenVerticalOffset);
            //GL.Vertex2(screenHorizontalOffset + minimumScreenWidth, screenVerticalOffset);

            //GL.Vertex2(screenHorizontalOffset + minimumScreenWidth, screenVerticalOffset);
            //GL.Vertex2(screenHorizontalOffset + minimumScreenWidth, screenVerticalOffset + minimumScreenHeight);

            //GL.Vertex2(screenHorizontalOffset + minimumScreenWidth, screenVerticalOffset + minimumScreenHeight);
            //GL.Vertex2(screenHorizontalOffset, screenVerticalOffset + minimumScreenHeight);

            //GL.Vertex2(screenHorizontalOffset, screenVerticalOffset + minimumScreenHeight);
            //GL.Vertex2(screenHorizontalOffset, screenVerticalOffset);

            GL.End();

            Screen.SwapBuffers();
        };
        Screen.Run();
    }
}
