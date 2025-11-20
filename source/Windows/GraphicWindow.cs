using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

public class GraphicWindow
{
    public static int TileSize = Settings.Gameplay.TileSize;
    public static int ScreenWidth;
    public static int ScreenHeight;

    public static void Run()
    {
        GameWindow Screen = new GameWindow
        (
            1920,
            1080,
            GraphicsMode.Default,
            "Graphic Screen",
            GameWindowFlags.Fullscreen
        );

        Screen.CursorVisible = false;
        Screen.CursorGrabbed = true;

        //For test-only
        //Screen.VSync = VSyncMode.Off;

        WindowManager.SetupPixelCoordinates(Screen);

        Screen.RenderFrame += (sender, e) =>
        {
            //Window color anf clear
            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            WindowManager.SetupPixelCoordinates(Screen);

            Engine.Update();

            int interpolatedScreenHeight = Screen.Height > Screen.Width ? Screen.Width : Screen.Height;
            int interpolatedScreenWidth = Screen.Width > Screen.Height ? Screen.Height : Screen.Width;

            int screenHorizontalOffset = Screen.Width > Screen.Height ? ((Screen.Width - interpolatedScreenWidth) / 2) : 0;
            int screenVerticalOffset = Screen.Height > Screen.Width ? ((Screen.Height - interpolatedScreenHeight) / 2) : 0;

            float WallWidth = (float)interpolatedScreenWidth / (float)Settings.Graphics.RayCount;

            //Drawing graphics
            GL.Begin(PrimitiveType.Quads);
            for (int i = 0; i < Settings.Graphics.RayCount; i++)
            {
                float WallHeight = (float)((TileSize * interpolatedScreenHeight) /
                    (Engine.RayDatas[i, 0] * (float)Math.Cos(Engine.PlayerAngle -
                    (Engine.PlayerAngle + Engine.FOVStart + i * Engine.RadBetweenRays))));

                //int tempIterator = 0;
                ////Floor and ceiling X position variables
                //float tempFloorCeilingXLeft = i * WallWidth + screenHorizontalOffset;
                //float tempFloorCeilingXRight = (i + 1) * WallWidth + screenHorizontalOffset;

                ////Floor Y position variables
                //float tempFloorYTop = (interpolatedScreenHeight / 2) + (WallHeight / 2) + (tempIterator * WallWidth);
                //float tempFloorYBottom = (interpolatedScreenHeight / 2) + (WallHeight / 2) + ((tempIterator + 1) * WallWidth);

                ////Ceiling Y position variables
                //float tempCeilingYTop = (interpolatedScreenHeight / 2) - (WallHeight / 2) - (tempIterator * WallWidth);
                //float tempCeilingYBottom = (interpolatedScreenHeight / 2) - (WallHeight / 2) - ((tempIterator + 1) * WallWidth);

                ////Floor drawing
                //while (tempFloorYTop < Screen.Height)
                //{
                //    GL.Color3(0.3f, 0.3f, 0.3f);
                //    GL.Vertex2(tempFloorCeilingXLeft, tempFloorYTop);
                //    GL.Vertex2(tempFloorCeilingXRight, tempFloorYTop);
                //    GL.Vertex2(tempFloorCeilingXRight, tempFloorYBottom);
                //    GL.Vertex2(tempFloorCeilingXLeft, tempFloorYBottom);

                //    tempFloorYTop += WallWidth;
                //    tempFloorYBottom += WallWidth;
                //    tempIterator += 1;
                //}
                //tempIterator = 0;

                ////Ceiling drawing
                //while (tempCeilingYTop > 0)
                //{
                //    GL.Color3(0.7f, 0.7f, 0.7f);
                //    GL.Vertex2(tempFloorCeilingXLeft, tempCeilingYTop);
                //    GL.Vertex2(tempFloorCeilingXRight, tempCeilingYTop);
                //    GL.Vertex2(tempFloorCeilingXRight, tempCeilingYBottom);
                //    GL.Vertex2(tempFloorCeilingXLeft, tempCeilingYBottom);

                //    tempCeilingYTop -= WallWidth;
                //    tempCeilingYBottom -= WallWidth;
                //    tempIterator += 1;
                //}
                //tempIterator = 0;

                int[][] path = null;

                //Textures
                switch (Engine.RayDatas[i, 5])
                {
                    //No wall
                    case 0:
                        continue;
                    //Wall
                    case 1:
                        path = Textures.bricksTexture;
                        break;
                    case 2:
                        path = Textures.stoneBricksTexture;
                        break;
                    case 3:
                        path = Textures.mossyBricksTexture;
                        break;
                }

                //Drawing pixels in lines from up to down (walls)
                for (int k = 0; k < path[0][1]; k++)
                {
                    //Calculating shading and lighting with distance
                    float shadeCalc = Engine.RayDatas[i, 0] * Settings.Graphics.DistanceShade;

                    //Mirroring wrong textures
                    int tempRGBCalc = (Engine.RayDatas[i, 4] == 1 || Engine.RayDatas[i, 4] == 3) ?
                        ((int)Math.Floor((TileSize - Engine.RayDatas[i, 3]) / (TileSize / (float)path[0][0])) * 3) + k * (path[0][0] * 3) :
                        ((int)Math.Floor(Engine.RayDatas[i, 3] / (TileSize / (float)path[0][0])) * 3) + k * (path[0][0] * 3);

                    //Calculating RGB
                    float r = (path[1][tempRGBCalc] - shadeCalc) / 255f;
                    float g = (path[1][1 + tempRGBCalc] - shadeCalc) / 255f;
                    float b = (path[1][2 + tempRGBCalc] - shadeCalc) / 255f;

                    float tempLineCalcTop = (Screen.Height / 2) - (WallHeight / 2) + (k * (WallHeight / path[0][1]));
                    float tempLineCalcBottom = (Screen.Height / 2) - (WallHeight / 2) + ((k + 1) * (WallHeight / path[0][1]));

                    GL.Color3(r, g, b);
                    GL.Vertex2(i * WallWidth + screenHorizontalOffset, tempLineCalcTop);
                    GL.Vertex2((i + 1) * WallWidth + screenHorizontalOffset, tempLineCalcTop);
                    GL.Vertex2((i + 1) * WallWidth + screenHorizontalOffset, tempLineCalcBottom);
                    GL.Vertex2(i * WallWidth + screenHorizontalOffset, tempLineCalcBottom);
                }
            }

            //Vertical overflow fix
            if (Screen.Height > Screen.Width) {
                //Top
                GL.Color3(0f, 0f, 0f);
                GL.Vertex2(0, 0);
                GL.Vertex2(interpolatedScreenWidth, 0);
                GL.Vertex2(interpolatedScreenWidth, (Screen.Height / 2) - (interpolatedScreenHeight / 2));
                GL.Vertex2(0, (Screen.Height / 2) - (interpolatedScreenHeight / 2));

                //Bottom
                GL.Vertex2(0, (Screen.Height / 2) + (interpolatedScreenHeight / 2));
                GL.Vertex2(interpolatedScreenWidth, (Screen.Height / 2) + (interpolatedScreenHeight / 2));
                GL.Vertex2(interpolatedScreenWidth, Screen.Height);
                GL.Vertex2(0, Screen.Height);
            }

            GL.End();

            Screen.SwapBuffers();
        };
        Screen.Run();
    }
}
