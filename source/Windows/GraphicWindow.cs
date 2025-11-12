using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

public class GraphicWindow
{
    public static int TileSize = Settings.Gameplay.TileSize;
    public static int ScreenWidth = 1000;
    public static int ScreenHeight = 800;
    public static float VerticalShade = Settings.Graphics.VerticalShade;
    public static float HorizontalShade = Settings.Graphics.HorizontalShade;

    public void Run()
    {
        GameWindow Screen = new GameWindow(ScreenWidth, ScreenHeight, GraphicsMode.Default, "Graphic Screen");

        //For test-only
        //Screen.VSync = VSyncMode.Off;

        WindowManager.SetupPixelCoordinates(Screen);

        Screen.RenderFrame += (sender, e) =>
        {
            //Window color anf clear
            GL.ClearColor(0.4f, 0.9f, 1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            WindowManager.SetupPixelCoordinates(Screen);

            float WallWidth = (float)Screen.Width / (float)Settings.Graphics.RayCount;

            //Drawing floor
            for (int i = 0; i < Settings.Graphics.RayCount; i++) {
                int[][] path = null;

                //Textures
                switch (Engine.RayDatas[i, 5])
                {
                    //Bricks
                    case 1:
                        path = Textures.bricksTexture;
                        break;
                    //Mossy Bricks
                    case 2:
                        path = Textures.mossyBricksTexture;
                        break;
                    //Test
                    case 3:
                        path = Textures.testTexture;
                        break;
                }

                for (int k = 0; k < path[0][1]; k++)
                {
                    int tempRGBCalc = ((int)Math.Floor(Engine.RayDatas[i, 3] / (TileSize / (float)path[0][0])) * 3) + k * (path[0][0] * 3);

                    float r = path[1][tempRGBCalc] / 255f;
                    float g = path[1][1 + tempRGBCalc] / 255f;
                    float b = path[1][2 + tempRGBCalc] / 255f;

                    float tempLineCalcTop = (Screen.Height / 2) - (Engine.RayDatas[i, 6] / 2) + (k * (Engine.RayDatas[i, 6] / path[0][1]));
                    float tempLineCalcBottom = (Screen.Height / 2) - (Engine.RayDatas[i, 6] / 2) + ((k + 1) * (Engine.RayDatas[i, 6] / path[0][1]));

                    GL.Color3(r, g, b);
                    GL.Vertex2(i * WallWidth, tempLineCalcTop);
                    GL.Vertex2((i + 1) * WallWidth, tempLineCalcTop);
                    GL.Vertex2((i + 1) * WallWidth, tempLineCalcBottom);
                    GL.Vertex2(i * WallWidth, tempLineCalcBottom);
                }
            }

            ////Drawing graphics
            //GL.Begin(PrimitiveType.Quads);
            //for (int i = 0; i < Settings.Graphics.RayCount; i++)
            //{
            //    ////Testing purposes
            //    //if (Engine.RayDatas[i, 5] != 0 && Engine.RayDatas[i, 5] != 1 && Engine.RayDatas[i, 5] != 2 && Engine.RayDatas[i, 5] != 3)
            //    //{
            //    //    Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            //    //    Console.WriteLine(Engine.RayDatas[i, 5]);
            //    //    Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            //    //}

            //    //No wall
            //    if (Engine.RayDatas[i, 5] == 0)
            //    {
            //        continue;
            //    }
            //    //Wall
            //    else
            //    {
            //        int[][] path = null;

            //        //Textures
            //        switch (Engine.RayDatas[i, 5])
            //        {
            //            //Bricks
            //            case 1:
            //                path = Textures.bricksTexture;
            //                break;
            //            //Mossy Bricks
            //            case 2:
            //                path = Textures.mossyBricksTexture;
            //                break;
            //            //Test
            //            case 3:
            //                path = Textures.testTexture;
            //                break;
            //        }

            //        //Drawing pixels in lines from up to down (walls)
            //        for (int k = 0; k < path[0][1]; k++)
            //        {
            //            int tempRGBCalc = ((int)Math.Floor(Engine.RayDatas[i, 3] / (TileSize / (float)path[0][0])) * 3) + k * (path[0][0] * 3);

            //            float r = path[1][tempRGBCalc] / 255f;
            //            float g = path[1][1 + tempRGBCalc] / 255f;
            //            float b = path[1][2 + tempRGBCalc] / 255f;

            //            float tempLineCalcTop = (Screen.Height / 2) - (Engine.RayDatas[i, 6] / 2) + (k * (Engine.RayDatas[i, 6] / path[0][1]));
            //            float tempLineCalcBottom = (Screen.Height / 2) - (Engine.RayDatas[i, 6] / 2) + ((k + 1) * (Engine.RayDatas[i, 6] / path[0][1]));

            //            GL.Color3(r, g, b);
            //            GL.Vertex2(i * WallWidth, tempLineCalcTop);
            //            GL.Vertex2((i + 1) * WallWidth, tempLineCalcTop);
            //            GL.Vertex2((i + 1) * WallWidth, tempLineCalcBottom);
            //            GL.Vertex2(i * WallWidth, tempLineCalcBottom);
            //        }
            //    };
            //}
            GL.End();

            Screen.SwapBuffers();
        };

        Screen.Run();
    }
}
