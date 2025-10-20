//using OpenTK;
//using OpenTK.Graphics;
//using OpenTK.Graphics.OpenGL;
//using System;

//public class GraphicWindow
//{
//    public static int ScreenWidth = 1000;
//    public static int ScreenHeight = 800;


//    public void Run()
//    {
//        GameWindow Screen = new GameWindow(ScreenWidth, ScreenHeight, GraphicsMode.Default, "Graphic Screen");
//        WindowManager.SetupPixelCoordinates(Screen);

//        Screen.RenderFrame += (sender, e) =>
//        {
//            //Window color anf clear
//            GL.ClearColor(0f, 0f, 0f, 1f);
//            GL.Clear(ClearBufferMask.ColorBufferBit);

//            Engine.Update();
//            /*
//            //Updated variables
//            float WallWidth = Engine.WallWidth;
//            float[] WallHeights = Engine.WallHeights;

//            //Drawing walls
//            for (int i = 0; i < WallHeights.Length; i++)
//            {
//                GL.Color3(0f, 2f, 0f);
//                GL.Begin(PrimitiveType.Quads);
//                GL.Vertex2(i * WallWidth, (ScreenHeight / 2) - (WallHeights[i] / 2));
//                GL.Vertex2((i + 1) * WallWidth, (ScreenHeight / 2) - (WallHeights[i] / 2));
//                GL.Vertex2((i + 1) * WallWidth, (ScreenHeight / 2) + WallHeights[i]);
//                GL.Vertex2(i * WallWidth, (ScreenHeight / 2) + WallHeights[i]);
//                GL.End();
//            }
//            */
//            /*
//            //Calculating graphic wall width and height
//            WallWidth = (float)(GraphicWindow.ScreenWidth / RayCount);

//            for (int i = 0; i < RayCount; i++)
//            {
//                WallHeight = GraphicWindow.ScreenHeight - RayDistances[i];
//                WallHeights[i] = WallHeight;
//            }
//            //================================================================
//            */
//            Screen.SwapBuffers();
//        };

//        Screen.Run();
//    }
//}
