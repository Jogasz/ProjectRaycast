using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
public class GraphicWindow
{
    public static int TileSize = Settings.Gameplay.TileSize;
    public static int ScreenWidth;
    public static int ScreenHeight;
    public static int[,] MapFloor = Level.mapFloor;
    public static int[,] MapCeiling = Level.mapCeiling;
    public static OpenTK.Vector2 _lastMousePosition = new OpenTK.Vector2();

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

        Screen.CursorVisible = false;
        Screen.CursorGrabbed = true;

        //For test-only
        //Screen.VSync = VSyncMode.Off;

        WindowManager.SetupPixelCoordinates(Screen);

        Screen.RenderFrame += (sender, e) =>
        {
            //Window color and clear
            GL.ClearColor(0f, 0f, 0.1f, 1f);
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
            GL.Color3(0.3f, 0.3f, 0.95f);
            GL.Vertex2(screenHorizontalOffset, screenVerticalOffset);
            GL.Vertex2(screenHorizontalOffset + minimumScreenWidth, screenVerticalOffset);
            GL.Vertex2(screenHorizontalOffset + minimumScreenWidth, screenVerticalOffset + minimumScreenHeight);
            GL.Vertex2(screenHorizontalOffset, screenVerticalOffset + minimumScreenHeight);

            //Mouse
            OpenTK.Input.MouseState currentMouseState = OpenTK.Input.Mouse.GetState();
            OpenTK.Vector2 currentMousePosition = new OpenTK.Vector2(currentMouseState.X, currentMouseState.Y);
            OpenTK.Vector2 mouseOffset = currentMousePosition - _lastMousePosition;
            _lastMousePosition = currentMousePosition;

            if (mouseOffset.X != 0)
            {
                Engine.PlayerAngle += Settings.Player.MouseSensitivity * Engine.DeltaTime * mouseOffset.X;
                if (Engine.PlayerAngle > (2 * MathX.PI))
                {
                    Engine.PlayerAngle -= (2 * MathX.PI);
                }
            }

            int RGBCalc;
            float rowY, shadeCalc, r, g, b;

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

                //Ceiling Y position variables
                float ceilingPixelYTop = screenVerticalOffset;
                float ceilingPixelYBottom = screenVerticalOffset + WallWidth;

                float ceilingFloorPixelDistance,
                      ceilingPixelXWorldPosition,
                      ceilingPixelYWorldPosition,
                      floorPixelXWorldPosition,
                      floorPixelYWorldPosition;

                //Ceiling drawing
                while (ceilingPixelYTop < (screenVerticalOffset + ((minimumScreenHeight - WallHeight) / 2)))
                {
                    /* 
                    * If the pixel's top position is in the correct screen, but the bottom position is in the wall,
                    * the bottom position's Y value may be equal to the wall's top value.
                    */
                    ceilingPixelYBottom =
                    (ceilingPixelYBottom > (screenVerticalOffset + ((minimumScreenHeight - WallHeight) / 2))) ?
                    (screenVerticalOffset + ((minimumScreenHeight - WallHeight) / 2)) :
                    ceilingPixelYBottom;

                    //Y of the current pixel on the screen
                    rowY = (Screen.Height / 2) - (ceilingPixelYTop + ((ceilingPixelYBottom - ceilingPixelYTop) / 2));

                    ceilingFloorPixelDistance = ((cameraZ / rowY) * TileSize) / (float)Math.Abs(Math.Cos(Engine.PlayerAngle - Engine.RayDatas[i, 6]));

                    //World X position of the pixel
                    ceilingPixelXWorldPosition = Math.Abs(Engine.playerPosition.X + (Engine.RayDatas[i, 1] * ceilingFloorPixelDistance)) >= MapCeiling.GetLength(1) * TileSize ?
                        MapCeiling.GetLength(1) * TileSize - 0.0001f :
                        Math.Abs(Engine.playerPosition.X + (Engine.RayDatas[i, 1] * ceilingFloorPixelDistance));

                    //World Y position of the pixel
                    ceilingPixelYWorldPosition = Math.Abs(Engine.playerPosition.Y + (Engine.RayDatas[i, 2] * ceilingFloorPixelDistance)) >= MapCeiling.GetLength(0) * TileSize ?
                        MapCeiling.GetLength(0) * TileSize - 0.0001f :
                        Math.Abs(Engine.playerPosition.Y + (Engine.RayDatas[i, 2] * ceilingFloorPixelDistance));

                    //Textures
                    switch (MapCeiling[(int)Math.Floor(ceilingPixelYWorldPosition / TileSize), (int)Math.Floor(ceilingPixelXWorldPosition / TileSize)])
                    {
                        //No floor
                        case 0:
                            ceilingPixelYTop = ceilingPixelYBottom;
                            ceilingPixelYBottom += WallWidth;
                            continue;
                        //Floor
                        //Default textures
                        case 1:
                            path = Textures.planksTexture;
                            break;
                        case 2:
                            path = Textures.mossyPlanksTexture;
                            break;
                        case 3:
                            path = Textures.stoneBricksTexture;
                            break;
                    }

                    //Calculating RGB variables
                    RGBCalc = ((int)Math.Floor(path[0][1] / (TileSize / (ceilingPixelYWorldPosition % TileSize))) * path[0][1] * 3) +
                              ((int)Math.Floor(path[0][0] / (TileSize / (ceilingPixelXWorldPosition % TileSize))) * 3);

                    //Calculating shading and lighting with distance
                    shadeCalc = ceilingFloorPixelDistance * Settings.Graphics.DistanceShade;

                    r = (path[1][RGBCalc] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc] - shadeCalc) / 255f;
                    g = (path[1][RGBCalc + 1] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 1] - shadeCalc) / 255f;
                    b = (path[1][RGBCalc + 2] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 2] - shadeCalc) / 255f;

                    //Drawing pixel
                    GL.Color3(r, g, b);
                    GL.Vertex2(floorCeilingPixelXLeft, ceilingPixelYTop);
                    GL.Vertex2(floorCeilingPixelXRight, ceilingPixelYTop);
                    GL.Vertex2(floorCeilingPixelXRight, ceilingPixelYBottom);
                    GL.Vertex2(floorCeilingPixelXLeft, ceilingPixelYBottom);

                    //Incremental increasing of the top and bottom values of the pixel
                    ceilingPixelYTop = ceilingPixelYBottom;
                    ceilingPixelYBottom += WallWidth;
                }

                //Floor Y position variables
                float floorPixelYTop = screenVerticalOffset + minimumScreenHeight - WallWidth;
                float floorPixelYBottom = screenVerticalOffset + minimumScreenHeight;

                //Floor drawing
                while (floorPixelYBottom > (screenVerticalOffset + (minimumScreenHeight / 2) + (WallHeight / 2)))
                {
                    /* 
                    * If the pixel's bottom position is in the correct screen, but the top position is in the wall,
                    * the top position's Y value may be equal to the wall's bottom value.
                    */
                    floorPixelYTop =
                    (floorPixelYTop < (screenVerticalOffset + (minimumScreenHeight / 2) + (WallHeight / 2))) ?
                    (screenVerticalOffset + (minimumScreenHeight / 2) + (WallHeight / 2)) :
                    floorPixelYTop;

                    //Y of the current pixel on the screen
                    rowY = floorPixelYTop + ((floorPixelYBottom - floorPixelYTop) / 2) - (Screen.Height / 2);

                    //Distance of the currently drawn pixel and fisheye correction
                    ceilingFloorPixelDistance = ((cameraZ / rowY) * TileSize) / (float)Math.Abs(Math.Cos(Engine.PlayerAngle - Engine.RayDatas[i, 6]));

                    //World position of the pixel
                    floorPixelXWorldPosition = Math.Abs(Engine.playerPosition.X + (Engine.RayDatas[i, 1] * ceilingFloorPixelDistance)) >= MapFloor.GetLength(1) * TileSize ?
                        MapFloor.GetLength(1) * TileSize - 0.0001f :
                        Math.Abs(Engine.playerPosition.X + (Engine.RayDatas[i, 1] * ceilingFloorPixelDistance));

                    //World Y position of the pixel
                    floorPixelYWorldPosition = Math.Abs(Engine.playerPosition.Y + (Engine.RayDatas[i, 2] * ceilingFloorPixelDistance)) >= MapFloor.GetLength(0) * TileSize ?
                        MapFloor.GetLength(0) * TileSize - 0.0001f :
                        Math.Abs(Engine.playerPosition.Y + (Engine.RayDatas[i, 2] * ceilingFloorPixelDistance));

                    //Textures
                    switch (MapFloor[(int)Math.Floor(floorPixelYWorldPosition / TileSize), (int)Math.Floor(floorPixelXWorldPosition / TileSize)])
                    {
                        //No floor
                        case 0:
                            floorPixelYBottom = floorPixelYTop;
                            floorPixelYTop -= WallWidth;
                            continue;
                        //Floor
                        //Default textures
                        case 1:
                            path = Textures.planksTexture;
                            break;
                        case 2:
                            path = Textures.mossyPlanksTexture;
                            break;
                        case 3:
                            path = Textures.stoneBricksTexture;
                            break;
                    }

                    //Calculating RGB variables
                    RGBCalc = ((int)Math.Floor(path[0][1] / (TileSize / (floorPixelYWorldPosition % TileSize))) * path[0][1] * 3) +
                              ((int)Math.Floor(path[0][0] / (TileSize / (floorPixelXWorldPosition % TileSize))) * 3);

                    //Calculating shading and lighting with distance
                    shadeCalc = ceilingFloorPixelDistance * Settings.Graphics.DistanceShade;

                    r = (path[1][RGBCalc] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc] - shadeCalc) / 255f;
                    g = (path[1][RGBCalc + 1] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 1] - shadeCalc) / 255f;
                    b = (path[1][RGBCalc + 2] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 2] - shadeCalc) / 255f;

                    //Drawing pixel
                    GL.Color3(r, g, b);
                    GL.Vertex2(floorCeilingPixelXLeft, floorPixelYTop);
                    GL.Vertex2(floorCeilingPixelXRight, floorPixelYTop);
                    GL.Vertex2(floorCeilingPixelXRight, floorPixelYBottom);
                    GL.Vertex2(floorCeilingPixelXLeft, floorPixelYBottom);

                    //Incremental increasing of the top and bottom values of the pixel
                    floorPixelYBottom = floorPixelYTop;
                    floorPixelYTop -= WallWidth;
                }

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
                    case 4:
                        path = Textures.windowStoneBricksTexture;
                        break;

                        //Window textures

                        //Paintings
                }

                //Calculating shading and lighting with distance
                shadeCalc = Engine.RayDatas[i, 0] * Settings.Graphics.DistanceShade;

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

                        //Mirroring wrong textures, Calculating RGB variables
                        RGBCalc = (Engine.RayDatas[i, 4] == 1 || Engine.RayDatas[i, 4] == 3) ?
                            RGBCalc = ((int)Math.Floor((TileSize - Engine.RayDatas[i, 3]) / (TileSize / (float)path[0][0])) * 3) + k * (path[0][0] * 3) :
                            RGBCalc = ((int)Math.Floor(Engine.RayDatas[i, 3] / (TileSize / (float)path[0][0])) * 3) + k * (path[0][0] * 3);

                        r = (path[1][RGBCalc] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc] - shadeCalc) / 255f;
                        g = (path[1][RGBCalc + 1] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 1] - shadeCalc) / 255f;
                        b = (path[1][RGBCalc + 2] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 2] - shadeCalc) / 255f;

                        //Drawing pixel
                        GL.Color3(r, g, b);
                        GL.Vertex2(i * WallWidth + screenHorizontalOffset, tempPixelCalcTop);
                        GL.Vertex2((i + 1) * WallWidth + screenHorizontalOffset, tempPixelCalcTop);
                        GL.Vertex2((i + 1) * WallWidth + screenHorizontalOffset, tempPixelCalcBottom);
                        GL.Vertex2(i * WallWidth + screenHorizontalOffset, tempPixelCalcBottom);
                    }
                }
            }

            GL.End();

            Screen.SwapBuffers();
        };
        Screen.Run();
    }
}
