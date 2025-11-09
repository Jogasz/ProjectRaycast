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

            float WallWidth = (float)ScreenWidth / (float)Settings.Graphics.RayCount;

            //Drawing walls
            GL.Begin(PrimitiveType.Quads);
            for (int i = 0; i < Settings.Graphics.RayCount; i++)
            {
                //No wall
                if (Engine.RayDatas[i, 5] == 0)
                {
                    continue;
                }
                //Wall
                else {
                    int[][] path = null;
                    switch (Engine.RayDatas[i, 5])
                    {
                        case 1:
                            path = Textures.bricksTexture;
                            break;
                        case 2:
                            path = Textures.mossyBricksTexture;
                            break;
                    }
                    for (int k = 0; k < path[0][1]; k++) {
                        //Console.WriteLine("k: " + k);
                        //Console.WriteLine("Méret: " + Textures.bricksTexture[1].Length);
                        //Console.WriteLine("szél: " + Textures.bricksTexture[0][0]);
                        //Console.WriteLine("hossz: " + Textures.bricksTexture[0][1]);
                        //Console.WriteLine(0 + k * (Textures.bricksTexture[0][0] * 3) + ". : " + Textures.bricksTexture[1][0 + k * (Textures.bricksTexture[0][0] * 3)]);
                        //Console.WriteLine(1 + k * (Textures.bricksTexture[0][0] * 3) + ". : " + Textures.bricksTexture[1][1 + k * (Textures.bricksTexture[0][0] * 3)]);
                        //Console.WriteLine(2 + k * (Textures.bricksTexture[0][0] * 3) + ". : " + Textures.bricksTexture[1][2 + k * (Textures.bricksTexture[0][0] * 3)]);
                        //Console.WriteLine(0 + (int)Math.Floor(Engine.RayDatas[i, 3] / (TileSize / (float)Textures.bricksTexture[0][0])));
                        int r = path[1][(0 + ((int)Math.Floor(Engine.RayDatas[i, 3] / (TileSize / (float)path[0][0]))) * 3) + k * (path[0][0] * 3)];
                        int g = path[1][(1 + ((int)Math.Floor(Engine.RayDatas[i, 3] / (TileSize / (float)path[0][0]))) * 3) + k * (path[0][0] * 3)];
                        int b = path[1][(2 + ((int)Math.Floor(Engine.RayDatas[i, 3] / (TileSize / (float)path[0][0]))) * 3) + k * (path[0][0] * 3)];

                        GL.Color3(r / 255f, g / 255f, b / 255f);
                        GL.Vertex2(i * WallWidth, (ScreenHeight / 2) - (Engine.RayDatas[i, 6] / 2) + (k * (Engine.RayDatas[i, 6] / path[0][1])));
                        GL.Vertex2((i + 1) * WallWidth, (ScreenHeight / 2) - (Engine.RayDatas[i, 6] / 2) + (k * (Engine.RayDatas[i, 6] / path[0][1])));
                        GL.Vertex2((i + 1) * WallWidth, (ScreenHeight / 2) - (Engine.RayDatas[i, 6] / 2) + ((k + 1) * (Engine.RayDatas[i, 6] / path[0][1])));
                        GL.Vertex2(i * WallWidth, (ScreenHeight / 2) - (Engine.RayDatas[i, 6] / 2) + ((k + 1) * (Engine.RayDatas[i, 6] / path[0][1])));
                    }
                    //Engine.TestTexture2[k, (int)Math.Floor(Engine.RayDatas[i, 3] / (TileSize / (float)Engine.TestTexture2.GetLength(1)))] == 1
                };
            }
            GL.End();

            Screen.SwapBuffers();
        };

        Screen.Run();
    }
}
