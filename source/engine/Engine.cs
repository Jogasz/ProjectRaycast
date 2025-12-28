using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using ProjectRaycast.source.shader;
using System.Diagnostics;

namespace Engine;

internal partial class Engine : GameWindow
{
    //Debug
    //Border for quads to showcase placement
    const float debugBorder = 0f;

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

    //Shader
    Matrix4 projection { get; set; }
    int VertexBufferObject { get; set; }
    int VertexArrayObject { get; set; }
    Shader shader { get; set; }
    float[] shaderVertices { get; set; } = Array.Empty<float>();
    bool verticesReady { get; set; } = false;
    private List<float> vertexAttributesList { get; set; } = new List<float>();
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
        VSync = VSyncMode.Off;
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        //Render distance limiter
        renderDistance = Math.Min(renderDistance, Math.Max(mapWalls.GetLength(0), mapWalls.GetLength(1)));

        Console.WriteLine("Starting program...");

        //Viewport setup
        ViewportSetUp(ClientSize.X, ClientSize.Y);

        shader = new Shader("source/shader/shader.vert", "source/shader/shader.frag");

        //VAO, VBO
        VertexArrayObject = GL.GenVertexArray();
        VertexBufferObject = GL.GenBuffer();

        //Instance VBO (vec4 + vec3 = 7 floats per instance)
        GL.BindVertexArray(VertexArrayObject);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
        
        //Attribute 0 = vec4 aPosition (x1,x2,y1,y2) -> per-instance
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);

        //Attribute 1 = vec3 aColor -> per-instance
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 4 * sizeof(float));

        //Mark these attributes as per-instance (divisor = 1)
        GL.VertexAttribDivisor(0, 1);
        GL.VertexAttribDivisor(1, 1);

        //Disable face culling to avoid accidentally removing one triangle
        GL.Disable(EnableCap.CullFace);

        //Unbind for safety
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        //Starting stopwatch for Delta Time
        stopwatch.Start();
    }
    
    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);
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
        Console.WriteLine((int)Math.Floor(1 / deltaTime));
        //=========================================================================================

        //Keybinds, controls and collision
        // 1. Distributor
        Controls(KeyboardState, MouseState);

        //(Inside Distributor):
        // 2. Movement and collison
        // 3. Jump
        // 4. Mouse
        // 5. Closing program
        // 6. Fullscreen
        // 7. Mouse grab
        //=========================================================================================

        //Allowed screen
        // 1. Size
        //Using the minimum of the screen's dimensions to keep a square aspect ratio
        minimumScreenHeight = ClientSize.Y > ClientSize.X ? ClientSize.X : ClientSize.Y;
        minimumScreenWidth = ClientSize.X > ClientSize.Y ? ClientSize.Y : ClientSize.X;

        //Calculating horizontal and vertical screen offset to center the game if needed
        screenHorizontalOffset = ClientSize.X > ClientSize.Y ? ((ClientSize.X - minimumScreenWidth) / 2) : 0;
        screenVerticalOffset = ClientSize.Y > ClientSize.X ? ((ClientSize.Y - minimumScreenHeight) / 2) : 0;

        // 2. Color
        VertexLoader(
            screenHorizontalOffset,
            screenHorizontalOffset + minimumScreenWidth,
            screenVerticalOffset,
            screenVerticalOffset + minimumScreenHeight,
            0.3f,
            0.3f,
            0.8f
        );
        //=========================================================================================

        //Raycount limiter
        rayCount = Math.Min(Settings.Graphics.rayCount, minimumScreenWidth);
        //=============================================================================================

        //FOV calculation
        FOVStart = -((float)(FOV * (Math.PI / 180f)) / 2);
        radBetweenRays = ((float)(FOV * (Math.PI / 180f)) / (rayCount - 1));
        wallWidth = (float)minimumScreenWidth / rayCount;
        //=============================================================================================
        
        //Raycasting:
        // 1. RayCast
        RayCast();
        
        //Graphic's position adn color calculator's (Inside Logic):
        // 2. Ceiling
        // 3. Floor
        // 4. Walls
        //=============================================================================================

        //Hud
        DrawHUD();
        //=============================================================================================

        //Vertex loader
        shaderVertices = vertexAttributesList.ToArray();

        //Loading buffer
        if (shaderVertices != null && shaderVertices.Length > 0)
        {
            verticesReady = true;

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            GL.BufferData(
                BufferTarget.ArrayBuffer,
                shaderVertices.Length * sizeof(float),
                shaderVertices,
                BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        else
        {
            verticesReady = false;
        }

        //CLEARING LIST
        vertexAttributesList.Clear();
        //=============================================================================================
    }
    //=============================================================================================

    //Every frame's renderer (second-half)
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        //Viewport setup
        ViewportSetUp(ClientSize.X, ClientSize.Y);

        if (!verticesReady || shaderVertices.Length == 0)
        {
            SwapBuffers();
            return;
        }
        else
        {
            shader.Use();
            GL.BindVertexArray(VertexArrayObject);

            int instanceCount = shaderVertices.Length / 7;

            //Drawing graphics in one-go
            if (instanceCount > 0)
            {
                GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, instanceCount);
            }

            SwapBuffers();
        }
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
    }
}