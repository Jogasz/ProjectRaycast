using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Diagnostics;

namespace Engine;

internal partial class Engine : GameWindow
{
    //Avarage FPS tester
    List<int> FPSList = new List<int>();

    //Settings
    int FOV = Settings.Graphics.FOV;
    int rayCount { get; set; } = Settings.Graphics.rayCount;
    int renderDistance = Settings.Graphics.renderDistance;
    float distanceShade = Settings.Graphics.distanceShade;
    int tileSize = Settings.Gameplay.tileSize;
    readonly int[,] mapCeiling = Level.mapCeiling;
    readonly int[,] mapFloor = Level.mapFloor;
    int[,] mapWalls = Level.mapWalls;
    float playerMovementSpeed = Settings.Player.movementSpeed;
    float mouseSensitivity = Settings.Player.mouseSensitivity;

    //DeltaTime
    float deltaTime { get; set; }
    float lastTime { get; set; }

    //Player variables
    Vector2 playerPosition { get; set; } = new Vector2(75, 75);
    float playerAngle { get; set; } = 0f;
    const float playerCollisionRadius = 10f;
    float pitch { get; set; } = 0f;

    //Engine
    Stopwatch stopwatch { get; set; } = new Stopwatch();
    float FOVStart { get; set; }
    float radBetweenRays { get; set; }
    float wallWidth { get; set; }
    int minimumScreenWidth { get; set; }
    int minimumScreenHeight { get; set; }
    int screenVerticalOffset { get; set; }
    int screenHorizontalOffset { get; set; }
    //=============================================================================================
    public Engine(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
    {
        ClientSize = (width, height),
        Title = title,

        WindowState = WindowState.Normal,
        WindowBorder = WindowBorder.Resizable,
    })
    {
        CursorState = CursorState.Grabbed;
        VSync = VSyncMode.On;
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        //Viewport
        Utils.SetViewport(ClientSize.X, ClientSize.Y);

        //Allowed screen's size (square game aspect ratio)
        minimumScreenHeight = ClientSize.Y > ClientSize.X ? ClientSize.X : ClientSize.Y;
        minimumScreenWidth = ClientSize.X > ClientSize.Y ? ClientSize.Y : ClientSize.X;

        //Offsets to center allowed screen
        screenHorizontalOffset = ClientSize.X > ClientSize.Y ? ((ClientSize.X - minimumScreenWidth) / 2) : 0;
        screenVerticalOffset = ClientSize.Y > ClientSize.X ? ((ClientSize.Y - minimumScreenHeight) / 2) : 0;

        //Render distance limiter
        renderDistance = Math.Min(renderDistance, Math.Max(mapWalls.GetLength(0), mapWalls.GetLength(1)));

        //Loading textures
        Texture.LoadAll(mapWalls, mapCeiling, mapFloor);

        //Loading shaders
        try
        {
            Shader.LoadAll(
                ClientSize,
                new Vector2(minimumScreenWidth, minimumScreenHeight),
                new Vector2(screenHorizontalOffset, screenVerticalOffset));
            Console.WriteLine(" - Shaders have been loaded!");
        }
        catch (FileNotFoundException noFileEx)
        {
            Console.WriteLine(noFileEx);
        }
        catch (InvalidOperationException invOpEx)
        {
            Console.WriteLine(invOpEx);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Shader: Something went wrong...\n - {e}");
        }

        stopwatch.Start();
    }
    
    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        //Viewport
        Utils.SetViewport(ClientSize.X, ClientSize.Y);

        //Allowed screen's size (square game aspect ratio)
        minimumScreenHeight = ClientSize.Y > ClientSize.X ? ClientSize.X : ClientSize.Y;
        minimumScreenWidth = ClientSize.X > ClientSize.Y ? ClientSize.Y : ClientSize.X;

        //Offsets to center allowed screen
        screenHorizontalOffset = ClientSize.X > ClientSize.Y ? ((ClientSize.X - minimumScreenWidth) / 2) : 0;
        screenVerticalOffset = ClientSize.Y > ClientSize.X ? ((ClientSize.Y - minimumScreenHeight) / 2) : 0;

        Shader.UpdateUniforms(
            ClientSize,
            new Vector2(minimumScreenWidth, minimumScreenHeight),
            new Vector2(screenHorizontalOffset, screenVerticalOffset));
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        //DeltaTime
        //=========================================================================================
        float currentTime = (float)stopwatch.Elapsed.TotalSeconds;
        deltaTime = currentTime - lastTime;
        lastTime = currentTime;
        //=========================================================================================

        //FPS Counter
        FPSList.Add((int)Math.Floor(1 / deltaTime));
        //Console.WriteLine((int)Math.Floor(1 / deltaTime));
        //=========================================================================================

        //Keybinds, controls and collision
        //1. Distributor
        Controls(KeyboardState, MouseState);

        //(Inside Distributor):
        //2. Movement and collison
        //3. Jump
        //4. Mouse
        //5. Closing program
        //6. Fullscreen
        //7. Mouse grab
        //=========================================================================================

        //Allowed screen's color
        Shader.defVertexAttribList.AddRange(new float[]
        {
            screenHorizontalOffset,
            screenHorizontalOffset + minimumScreenWidth,
            screenVerticalOffset,
            screenVerticalOffset + minimumScreenHeight,
            1f,
            0f,
            0f
        });
        //=========================================================================================

        //Raycount limiter
        rayCount = Math.Min(Settings.Graphics.rayCount, minimumScreenWidth);
        // recompute wallWidth here so Engine and RayCasting use same value
        wallWidth = (float)minimumScreenWidth / Math.Max(1, rayCount);
        //=============================================================================================
        
        //Raycasting:
        //1. RayCast
        //RayCast();
        RayCasting.Run(
            ClientSize,
            FOV,
            rayCount,
            tileSize,
            distanceShade,
            minimumScreenWidth,
            minimumScreenHeight,
            screenHorizontalOffset,
            screenVerticalOffset,
            playerAngle,
            playerPosition,
            pitch,
            mapWalls,
            mapFloor,
            mapCeiling,
            renderDistance
        );
        //Graphic's position adn color calculator's (Inside Logic):
        //2. Ceiling
        //3. Floor
        //4. Walls
        //=============================================================================================

        //Hud
        //DrawHUD();
        //=============================================================================================
        //
        //Console.WriteLine(pitch);
        //Loading shader buffer, clreaing attribute list
        Shader.LoadBufferAndClear();
    }
    //=============================================================================================

    //Every frame's renderer (second-half)
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        //Clearing window
        GL.ClearColor(0.2f,0.2f,0.2f,1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        Shader.Draw(
            wallWidth,
            playerPosition,
            playerAngle,
            pitch);

        SwapBuffers();
    }
    
    protected override void OnUnload()
    {
        base.OnUnload();

        //Avarage fps
        int avarageFPS =0;

        foreach (int FPS in FPSList)
        {
            avarageFPS += FPS;
        }

        avarageFPS /= FPSList.Count;
        Console.WriteLine($"The avarage FPS was: {avarageFPS}");

        Shader.DisposeAll();
    }
}