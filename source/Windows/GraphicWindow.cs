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
        GameWindow Screen = new GameWindow(1000, 800, GraphicsMode.Default, "Graphic Screen");

        //For test-only
        Screen.VSync = VSyncMode.Off;

        WindowManager.SetupPixelCoordinates(Screen);

        Screen.RenderFrame += (sender, e) =>
        {
            //Window color anf clear
            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            WindowManager.SetupPixelCoordinates(Screen);

            Engine.Update();

            float WallWidth = (float)Screen.Width / (float)Settings.Graphics.RayCount;

            //Drawing graphics
            GL.Begin(PrimitiveType.Quads);
            for (int i = 0; i < Settings.Graphics.RayCount; i++)
            {

                //int tempIterator = 0;
                ////Floor and ceiling X position variables
                //float tempFloorCeilingXLeft = i * WallWidth;
                //float tempFloorCeilingXRight = (i + 1) * WallWidth;
                
                ////Floor Y position variables
                //float tempFloorYTop = (Screen.Height / 2) + (Engine.RayDatas[i, 6] / 2) + (tempIterator * WallWidth);
                //float tempFloorYBottom = (Screen.Height / 2) + (Engine.RayDatas[i, 6] / 2) + ((tempIterator + 1) * WallWidth);
                
                ////Ceiling Y position variables
                //float tempCeilingYTop = (Screen.Height / 2) - (Engine.RayDatas[i, 6] / 2) - (tempIterator * WallWidth);
                //float tempCeilingYBottom = (Screen.Height / 2) - (Engine.RayDatas[i, 6] / 2) - ((tempIterator + 1) * WallWidth);
                
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

                int[][] path = Textures.missingTexture;

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

                if (Engine.RayDatas[i, 5] != 0 && Engine.RayDatas[i, 5] != 1 && Engine.RayDatas[i, 5] != 2 && Engine.RayDatas[i, 5] != 3)
                {
                    Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!");
                    Console.WriteLine("Engine.RayDatas[" + i + ", 5]: " + Engine.RayDatas[i, 5]);
                    Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!");
                }

                //Drawing pixels in lines from up to down (walls)
                for (int k = 0; k < path[0][1]; k++)
                {
                    int tempRGBCalc = ((int)Math.Floor(Engine.RayDatas[i, 3] / (TileSize / (float)path[0][0])) * 3) + k * (path[0][0] * 3);

                    float shadeCalc = Engine.RayDatas[i, 0] * Settings.Graphics.DistanceShade;

                    float r = (path[1][tempRGBCalc] - shadeCalc) / 255f;
                    float g = (path[1][1 + tempRGBCalc] - shadeCalc) / 255f;
                    float b = (path[1][2 + tempRGBCalc] - shadeCalc) / 255f;

                    float tempLineCalcTop = (Screen.Height / 2) - (Engine.RayDatas[i, 6] / 2) + (k * (Engine.RayDatas[i, 6] / path[0][1]));
                    float tempLineCalcBottom = (Screen.Height / 2) - (Engine.RayDatas[i, 6] / 2) + ((k + 1) * (Engine.RayDatas[i, 6] / path[0][1]));

                    GL.Color3(r, g, b);
                    GL.Vertex2(i * WallWidth, tempLineCalcTop);
                    GL.Vertex2((i + 1) * WallWidth, tempLineCalcTop);
                    GL.Vertex2((i + 1) * WallWidth, tempLineCalcBottom);
                    GL.Vertex2(i * WallWidth, tempLineCalcBottom);
                }
            }
            GL.End();

            Screen.SwapBuffers();
        };
        Screen.Run();
    }
}
