using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;

namespace Engine;

internal partial class Engine : GameWindow
{
    internal bool isSaveState = false;

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
    float deltaLastTime { get; set; }

    //Player variables
    Vector2 playerPosition { get; set; } = new Vector2(250, 250);
    float playerAngle { get; set; } = 0f;
    const float playerCollisionRadius = 10f;
    float pitch { get; set; } = 0f;

    internal bool escConsumed;

    internal bool isInMainMenu = true;
    internal bool isInPauseMenu = false;

    //Engine
    Stopwatch stopwatch { get; set; } = new Stopwatch();
    float FOVStart { get; set; }
    float radBetweenRays { get; set; }
    float wallWidth { get; set; }
    float minimumScreenSize { get; set; }
    float screenVerticalOffset { get; set; }
    float screenHorizontalOffset { get; set; }

    //=============================================================================================
    public Engine(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
    {
        ClientSize = (width, height),
        Title = title,

        //WindowState = WindowState.Fullscreen,
        WindowBorder = WindowBorder.Resizable,
    })
    {
        VSync = VSyncMode.On;
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        //Viewport
        Utils.SetViewport(ClientSize.X, ClientSize.Y);

        //Allowed screen's size (square game aspect ratio)
        minimumScreenSize = ClientSize.Y > ClientSize.X ? ClientSize.X : ClientSize.Y;

        //Offsets to center allowed screen
        screenHorizontalOffset = ClientSize.X > ClientSize.Y ? ((ClientSize.X - minimumScreenSize) / 2) : 0;
        screenVerticalOffset = ClientSize.Y > ClientSize.X ? ((ClientSize.Y - minimumScreenSize) / 2) : 0;

        //Render distance limiter
        renderDistance = Math.Min(renderDistance, Math.Max(mapWalls.GetLength(0), mapWalls.GetLength(1)));

        //Loading textures
        try
        {
            Texture.LoadAll(mapWalls, mapCeiling, mapFloor);
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
            Console.WriteLine($"Textures: Something went wrong...\n - {e}");
        }

        //Loading shaders
        try
        {
            ShaderHandler.LoadAll(
                ClientSize,
                minimumScreenSize,
                new Vector2(screenHorizontalOffset, screenVerticalOffset));
            Console.WriteLine(" - SHADERS have been loaded!");
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
        minimumScreenSize = ClientSize.Y > ClientSize.X ? ClientSize.X : ClientSize.Y;

        //Offsets to center allowed screen
        screenHorizontalOffset = ClientSize.X > ClientSize.Y ? ((ClientSize.X - minimumScreenSize) / 2) : 0;
        screenVerticalOffset = ClientSize.Y > ClientSize.X ? ((ClientSize.Y - minimumScreenSize) / 2) : 0;
        
        ShaderHandler.UpdateUniforms(
            ClientSize,
            minimumScreenSize,
            new Vector2(screenHorizontalOffset, screenVerticalOffset));
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        //Release -> allow next press
        if (!KeyboardState.IsKeyDown(Keys.Escape))
            escConsumed = false;

        //DeltaTime
        //=========================================================================================
        float currentTime = (float)stopwatch.Elapsed.TotalSeconds;
        deltaTime = currentTime - deltaLastTime;
        deltaLastTime = currentTime;
        //=========================================================================================

        //FPS Counter
        FPSList.Add((int)Math.Floor(1 / deltaTime));
        //Console.WriteLine((int)Math.Floor(1 / deltaTime));
        //=========================================================================================

        if (!isInMainMenu && !isInPauseMenu && KeyboardState.IsKeyPressed(Keys.Escape))
        {
            CursorState = CursorState.Normal;
            isInPauseMenu = true;
            escConsumed = true;
        }

        if (isInMainMenu)
        {
            MainMenu();
            ShaderHandler.LoadBufferAndClearMenus();
            return;
        }

        if (isInPauseMenu)
        {
            PauseMenu();
            ShaderHandler.LoadBufferAndClearMenus();
            return;
        }

        //Handling controls
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

        ShaderHandler.WindowVertexAttribList.AddRange(new float[]
        {
            screenHorizontalOffset,
            screenHorizontalOffset + minimumScreenSize,
            screenVerticalOffset,
            screenVerticalOffset + minimumScreenSize,
            0.3f,
            0.5f,
            0.9f
        });
        //=========================================================================================

        //Raycount limiter
        rayCount = Math.Min(Settings.Graphics.rayCount, (int)minimumScreenSize);
        // recompute wallWidth here so Engine and RayCasting use same value
        wallWidth = minimumScreenSize / Math.Max(1, rayCount);
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
            minimumScreenSize,
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
        //Graphic's position and color calculator's (Inside Logic):
        //2. Ceiling
        //3. Floor
        //4. Walls

        //5. Sprites
        RayCasting.ComputeSprites(
            playerPosition,
            tileSize,
            playerAngle,
            FOV,
            minimumScreenSize,
            screenHorizontalOffset,
            screenVerticalOffset,
            pitch);
        //=============================================================================================

        ShaderHandler.LoadBufferAndClear();
    }
    //=============================================================================================

    //Every frame's renderer (second-half)
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        //Clearing window
        GL.ClearColor(0.2f,0.2f,0.2f,1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        if (isInMainMenu)
        {
            ShaderHandler.DrawMenus();
            SwapBuffers();
            return;
        }

        ShaderHandler.DrawGame(
            wallWidth,
            playerPosition,
            playerAngle,
            pitch);

        if (isInPauseMenu)
        {
            ShaderHandler.DrawMenus();
        }

        SwapBuffers();
    }
    
    protected override void OnUnload()
    {
        base.OnUnload();

        //Avarage fps
        int avarageFPS = 0;

        foreach (int FPS in FPSList)
        {
            avarageFPS += FPS;
        }

        avarageFPS /= FPSList.Count;
        Console.WriteLine($"The avarage FPS was: {avarageFPS}");

        ShaderHandler.DisposeAll();
    }
}