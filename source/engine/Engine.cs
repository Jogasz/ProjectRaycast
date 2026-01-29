using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
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
    float deltaLastTime { get; set; }

    //Player variables
    Vector2 playerPosition { get; set; } = new Vector2(250, 250);
    float playerAngle { get; set; } = 0f;
    const float playerCollisionRadius = 10f;
    float pitch { get; set; } = 0f;

    //Engine
    Stopwatch stopwatch { get; set; } = new Stopwatch();
    float FOVStart { get; set; }
    float radBetweenRays { get; set; }
    float wallWidth { get; set; }
    float minimumScreenSize { get; set; }
    float screenVerticalOffset { get; set; }
    float screenHorizontalOffset { get; set; }

    // Allowed-screen color animation
    readonly Vector3 allowedScreenBaseColor = new(0.3f, 0.5f, 0.9f);
    readonly Vector3 allowedScreenAltColor = new(1.0f, 0.0f, 0.0f); // red
    const float allowedScreenBlinkPeriodSeconds = 1.0f;
    //=============================================================================================
    public Engine(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
    {
        ClientSize = (width, height),
        Title = title,

        WindowState = WindowState.Fullscreen,
        WindowBorder = WindowBorder.Resizable,
    })
    {
        //CursorState = CursorState.Grabbed;
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

    bool isInMainMenu = true;
    bool isInPauseMenu = false;

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

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

        //No game logic until user is in menu
        if (isInMainMenu)
        {
            if (KeyboardState.IsKeyPressed(Keys.Escape))
                Close();

            else if (KeyboardState.IsKeyPressed(Keys.Enter))
            {
                CursorState = CursorState.Grabbed;
                isInMainMenu = false;
            }

            //Hud
            DrawGui.MainMenu(
                minimumScreenSize,
                screenVerticalOffset,
                screenHorizontalOffset);

            ShaderHandler.LoadBufferAndClear();

            return;
        }

        if (isInPauseMenu)
        {
            CursorState = CursorState.Normal;

            if (KeyboardState.IsKeyPressed(Keys.Enter))
            {
                CursorState = CursorState.Grabbed;
                isInPauseMenu = false;
            }

            else if (KeyboardState.IsKeyPressed(Keys.Escape))
                Close();

            return;
        }

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

        bool useAlt =
            ((int)MathF.Floor((float)stopwatch.Elapsed.TotalSeconds / allowedScreenBlinkPeriodSeconds) % 2) == 1;

        Vector3 color = useAlt ? allowedScreenAltColor : allowedScreenBaseColor;

        ShaderHandler.WindowVertexAttribList.AddRange(new float[]
        {
            screenHorizontalOffset,
            screenHorizontalOffset + minimumScreenSize,
            screenVerticalOffset,
            screenVerticalOffset + minimumScreenSize,
            color.X,
            color.Y,
            color.Z
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

        //DrawHUD();
        //=============================================================================================
        //
        //Console.WriteLine(pitch);
        //Loading shader buffer, clreaing attribute list
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
            ShaderHandler.DrawMainMenu();
            SwapBuffers();
            return;
        }

        ShaderHandler.DrawGame(
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

        ShaderHandler.DisposeAll();
    }
}