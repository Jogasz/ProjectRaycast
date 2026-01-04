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

    //SHADER
    //Shaders
    Shader? defShader { get; set; }
    Shader? ceilingShader { get; set; }
    Shader? wallShader { get; set; }

    //VAO's, VBO's
    //Default
    int defVAO { get; set; }
    int defVBO { get; set; }
    internal static List<float> defVertexAttributesList { get; set; } = new List<float>();
    float[]? defVerticesArray { get; set; }

    //Ceiling
    int ceilingVAO { get; set; }
    int ceilingVBO { get; set; }
    internal static List<float> ceilingVertexAttributesList { get; set; } = new List<float>();
    float[]? ceilingVerticesArray { get; set; }

    //Walls
    int wallVAO { get; set; }
    int wallVBO { get; set; }
    internal static List<float> wallVertexAttributesList { get; set; } = new List<float>();
    float[]? wallVerticesArray { get; set; }

    Matrix4 projection { get; set; }

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

        //Loading textures
        Texture.LoadAll(mapWalls, mapCeiling, mapFloor);

        //Viewport and projection (bottom-left origin)
        projection = Utils.SetViewportAndProjection(ClientSize.X, ClientSize.Y);

        //Allowed screen's size (square game aspect ratio)
        minimumScreenHeight = ClientSize.Y > ClientSize.X ? ClientSize.X : ClientSize.Y;
        minimumScreenWidth = ClientSize.X > ClientSize.Y ? ClientSize.Y : ClientSize.X;

        //Offsets to center allowed screen
        screenHorizontalOffset = ClientSize.X > ClientSize.Y ? ((ClientSize.X - minimumScreenWidth) / 2) : 0;
        screenVerticalOffset = ClientSize.Y > ClientSize.X ? ((ClientSize.Y - minimumScreenHeight) / 2) : 0;

        //Render distance limiter
        renderDistance = Math.Min(renderDistance, Math.Max(mapWalls.GetLength(0), mapWalls.GetLength(1)));

        //Shaders
        Console.WriteLine("Loading shaders...");
        //Default
        //============================================================================
        defShader = new Shader("source/shader/shader.vert", "source/shader/shader.frag");

        //VAO, VBO
        defVAO = GL.GenVertexArray();
        defVBO = GL.GenBuffer();

        //VAO, VBO Binding
        GL.BindVertexArray(defVAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, defVBO);

        //Attribute0
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0,4, VertexAttribPointerType.Float, false,7 * sizeof(float),0);

        //Attribute1
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1,3, VertexAttribPointerType.Float, false,7 * sizeof(float),4 * sizeof(float));

        //Divisor
        GL.VertexAttribDivisor(0,1);
        GL.VertexAttribDivisor(1,1);

        //Disable face culling to avoid accidentally removing one triangle
        GL.Disable(EnableCap.CullFace);

        //Unbind for safety
        GL.BindBuffer(BufferTarget.ArrayBuffer,0);
        GL.BindVertexArray(0);

        //Projection matrix uniform
        defShader.Use();
        defShader.SetMatrix4("uProjection", projection);
        //============================================================================

        //Ceiling
        //============================================================================
        ceilingShader = new Shader("source/engine/graphics/geometry/ceiling/ceiling.vert", "source/engine/graphics/geometry/ceiling/ceiling.frag");

        //VAO, VBO
        ceilingVAO = GL.GenVertexArray();
        ceilingVBO = GL.GenBuffer();

        //VAO, VBO Binding
        GL.BindVertexArray(ceilingVAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, ceilingVBO);

        //Attribute0
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0,4, VertexAttribPointerType.Float, false,5 * sizeof(float),0);

        //Attribute1
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1,1, VertexAttribPointerType.Float, false,5 * sizeof(float),4 * sizeof(float));

        //Divisor
        GL.VertexAttribDivisor(0,1);
        GL.VertexAttribDivisor(1,1);

        //Disable face culling to avoid accidentally removing one triangle
        GL.Disable(EnableCap.CullFace);

        //Unbind for safety
        GL.BindBuffer(BufferTarget.ArrayBuffer,0);
        GL.BindVertexArray(0);

        //Projection matrix uniform
        ceilingShader.Use();
        ceilingShader.SetMatrix4("uProjMat", projection);

        ceilingShader.SetVector2("uClientSize", new Vector2(ClientSize.X, ClientSize.Y));
        ceilingShader.SetFloat("uTileSize", tileSize);
        ceilingShader.SetFloat("uDistanceShade", distanceShade);
        //============================================================================

        //Wall
        //============================================================================
        wallShader = new Shader("source/engine/graphics/geometry/walls/wall.vert", "source/engine/graphics/geometry/walls/wall.frag");

        //VAO, VBO
        wallVAO = GL.GenVertexArray();
        wallVBO = GL.GenBuffer();

        //VAO, VBO Binding
        GL.BindVertexArray(wallVAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, wallVBO);

        //Attribute0
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

        //Attribute1
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 1, VertexAttribPointerType.Float, false, 5 * sizeof(float), 4 * sizeof(float));

        //Divisor
        GL.VertexAttribDivisor(0, 1);
        GL.VertexAttribDivisor(1, 1);

        //Disable face culling to avoid accidentally removing one triangle
        GL.Disable(EnableCap.CullFace);

        //Unbind for safety
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        //Projection matrix uniform
        wallShader.Use();
        wallShader.SetMatrix4("uProjMat", projection);

        wallShader.SetVector2("uClientSize", new Vector2(ClientSize.X, ClientSize.Y));
        wallShader.SetFloat("uTileSize", tileSize);
        wallShader.SetFloat("uDistanceShade", distanceShade);
        //============================================================================

        //Starting stopwatch for Delta Time
        stopwatch.Start();
    }
    
    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        //Viewport and projection (bottom-left origin)
        projection = Utils.SetViewportAndProjection(ClientSize.X, ClientSize.Y);

        //Allowed screen's size (square game aspect ratio)
        minimumScreenHeight = ClientSize.Y > ClientSize.X ? ClientSize.X : ClientSize.Y;
        minimumScreenWidth = ClientSize.X > ClientSize.Y ? ClientSize.Y : ClientSize.X;

        //Offsets to center allowed screen
        screenHorizontalOffset = ClientSize.X > ClientSize.Y ? ((ClientSize.X - minimumScreenWidth) / 2) : 0;
        screenVerticalOffset = ClientSize.Y > ClientSize.X ? ((ClientSize.Y - minimumScreenHeight) / 2) : 0;

        //Update projection uniforms
        defShader?.Use();
        defShader?.SetMatrix4("uProjection", projection);
        ceilingShader?.Use();
        ceilingShader?.SetMatrix4("uProjMat", projection);
        ceilingShader?.SetFloat("uStepSize", wallWidth);
        ceilingShader?.SetVector2("uClientSize", new Vector2(ClientSize.X, ClientSize.Y));
        wallShader?.Use();
        wallShader?.SetMatrix4("uProjMat", projection);
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
        defVertexAttributesList.AddRange(new float[]
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

        //Loading buffer
        //Default
        //======================================================
        defVerticesArray = defVertexAttributesList.ToArray();

        GL.BindBuffer(BufferTarget.ArrayBuffer, defVBO);
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            defVerticesArray.Length * sizeof(float),
            defVerticesArray,
            BufferUsageHint.DynamicDraw);
        GL.BindBuffer(BufferTarget.ArrayBuffer,0);

        //CLEARING LIST
        defVertexAttributesList.Clear();
        //======================================================

        //Ceiling
        //======================================================
        ceilingVerticesArray = ceilingVertexAttributesList.ToArray();

        GL.BindBuffer(BufferTarget.ArrayBuffer, ceilingVBO);
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            ceilingVerticesArray.Length * sizeof(float),
            ceilingVerticesArray,
            BufferUsageHint.DynamicDraw);
        GL.BindBuffer(BufferTarget.ArrayBuffer,0);

        //CLEARING LIST
        ceilingVertexAttributesList.Clear();
        //======================================================
    }
    //=============================================================================================

    //Every frame's renderer (second-half)
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        //Clearing window
        GL.ClearColor(0.2f,0.2f,0.2f,1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        //Bindig textures
        //Map arrays
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, Texture.mapWallsTex);

        GL.ActiveTexture(TextureUnit.Texture1);
        GL.BindTexture(TextureTarget.Texture2D, Texture.mapFloorTex);

        GL.ActiveTexture(TextureUnit.Texture2);
        GL.BindTexture(TextureTarget.Texture2D, Texture.mapCeilingTex);

        //Images
        for (int i = 0; i < Texture.textures.Count; i++)
        {
            Texture.Bind(i, TextureUnit.Texture3 + i);
        }

        //Drawing default shader
        //=============================================================================
        defShader.Use();
        GL.BindVertexArray(defVAO);
        int defLen = defVerticesArray?.Length ??0;
        int instanceCount = defLen /7;
        if (instanceCount >0)
        {
            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip,0,4, instanceCount);
        }
        //=============================================================================

        //Ceiling
        //=============================================================================
        ceilingShader.Use();

        //Creating texture array uniform
        for (int i = 0; i < Texture.textures.Count; i++)
        {
            ceilingShader.SetInt($"uTextures[{i}]", 3 + i);
        }

        //Map arrays
        //ceilingShader.SetInt("uMapWalls", 0);
        //ceilingShader.SetInt("uMapFloor", 1);
        ceilingShader.SetInt("uMapCeiling", 2);
        ceilingShader.SetVector2("uMapSize", new Vector2(mapCeiling.GetLength(1), mapCeiling.GetLength(0)));

        ceilingShader.SetFloat("uStepSize", wallWidth);
        ceilingShader.SetVector2("uPlayerPos", new Vector2(playerPosition.X, playerPosition.Y));
        ceilingShader.SetFloat("uPlayerAngle", playerAngle);
        ceilingShader.SetFloat("uPitch", pitch);
        

        GL.BindVertexArray(ceilingVAO);
        int ceilLen = ceilingVerticesArray?.Length ??0;
        instanceCount = ceilLen /5;
        if (instanceCount >0)
        {
            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip,0,4, instanceCount);
        }
        //=============================================================================

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

        // Dispose shaders
        defShader?.Dispose();
        ceilingShader?.Dispose();
    }
}