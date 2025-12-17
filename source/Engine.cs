using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ProjectRaycast.source.shader;
using System.Diagnostics;

internal class Engine : GameWindow
{
    //=============================================================================================
    //Pre-declared variables
        //Avarage FPS tester
    List<int> FPSList = new List<int>();
        //Settings
    int FOV = Settings.Graphics.FOV;
    int rayCount = Settings.Graphics.rayCount;
    int renderDistance = Settings.Graphics.renderDistance;
    float distanceShade = Settings.Graphics.distanceShade;
    int tileSize = Settings.Gameplay.tileSize;
    int[,] mapCeiling = Level.mapCeiling;
    int[,] mapFloor = Level.mapFloor;
    int[,] mapWalls = Level.mapWalls;
    float playerMovementSpeed = Settings.Player.movementSpeed;
        //DeltaTime
    float deltaTime { get; set; }
    float lastTime { get; set; }
        //Player variables
    Vector2 playerPosition { get; set; } = new Vector2(75, 75);
    float playerAngle { get; set; } = 0f;
    const float playerCollisionRadius = 10f;
        //Engine variables
    Stopwatch stopwatch { get; set; } = new Stopwatch();
    float FOVStart { get; set; }
    float RadBetweenRays { get; set; }
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
    /*
     *Translator for shaderVerticies:
     *
     * 0: X1
     * 1: X2
     * 2: Y1
     * 3: Y2
     * 4: R
     * 5: G
     * 6: B
     * 
     * (x2, y1) ┌──────┐ (x1, y1)
     *          │      │
     *          │      │
     * (x2, y2) └──────┘ (x1, y2)
     * 
     * 0 -> (x1, y1)  (top-right)
     * 1 -> (x2, y1)  (top-left)
     * 2 -> (x1, y2)  (bottom-right)
     * 3 -> (x2, y2)  (bottom-left)
     * 
     * first triangle: 0-1-2
     * second triangle: 2-1-3
     * 
     */
    private List<float> vertexAttributesList { get; set; } = new List<float>();
    //=============================================================================================
    //---
    public Engine(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
    {
        ClientSize = (width, height),
        Title = title,
        //VSync = VSyncMode.Off
    })
    {}
    //=============================================================================================
    //Viewport method
    public void ViewportSetUp(int width, int height)
    {
        GL.Viewport(0, 0, width, height);
        projection = Matrix4.CreateOrthographicOffCenter(0f, width, height, 0f, -1f, 1f);
        if (shader != null)
        {
            shader.Use();
            shader.SetMatrix4("uProjection", projection);
        }

        //Window's default color
        GL.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);
    }
    //=============================================================================================
    //Gets called when window first loads
    protected override void OnLoad()
    {
        base.OnLoad();

        Console.WriteLine("Starting program...");

        //Viewport setup
        ViewportSetUp(ClientSize.X, ClientSize.Y);

        shader = new Shader("source/shader/shader.vert", "source/shader/shader.frag");

        // VAO, VBO
        VertexArrayObject = GL.GenVertexArray();
        VertexBufferObject = GL.GenBuffer();

        // instance VBO (vec4 + vec3 = 7 floats per instance)
        GL.BindVertexArray(VertexArrayObject);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
        
        // attribute 0 = vec4 aPosition (x1,x2,y1,y2)  -> per-instance
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);

        // attribute 1 = vec3 aColor -> per-instance
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 4 * sizeof(float));

        // Important: mark these attributes as per-instance (divisor = 1)
        GL.VertexAttribDivisor(0, 1);
        GL.VertexAttribDivisor(1, 1);

        // Disable face culling to avoid accidentally removing one triangle
        GL.Disable(EnableCap.CullFace);

        // Unbind for safety
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        //Starting stopwatch for Delta Time
        stopwatch.Start();
    }
    //=============================================================================================
    //Gets called when window is resized
    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        //Viewport setup
        ViewportSetUp(e.Width, e.Height);
    }
    //=============================================================================================
    //Every frame's logic updater (first-half)
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        //DeltaTime
        //=============================================================================================
        float currentTime = (float)stopwatch.Elapsed.TotalSeconds;
        deltaTime = currentTime - lastTime;
        lastTime = currentTime;
        //=============================================================================================

        //FPS Counter
        //=============================================================================================
        FPSList.Add((int)Math.Floor(1 / deltaTime));
        //Console.WriteLine(Math.Floor(1 / deltaTime) + "FPS");
        //=============================================================================================

        //Keybinds, controls and collision
        //=============================================================================================
        var keyboard = KeyboardState;
        //Escape
        if (keyboard.IsKeyDown(Keys.Escape))
        {
            Console.WriteLine("Escape was pressed!");
            Close();
        }

            //Not binded
        if
        (
            keyboard.IsKeyDown(Keys.Space) ||
            keyboard.IsKeyDown(Keys.Tab) ||
            keyboard.IsKeyDown(Keys.E)
        )
        {
            Console.WriteLine("- Key is not yet binded!");
        }

        if (keyboard.IsKeyDown(Keys.Right))
        {
            playerAngle += 1f * deltaTime;
            if (playerAngle > MathX.Quadrant4)
            {
                playerAngle -= (MathX.Quadrant4);
            }
        }
        if (keyboard.IsKeyDown(Keys.Left))
        {
            playerAngle -= 1f * deltaTime;
            if (playerAngle < 0)
            {
                playerAngle += (MathX.Quadrant4);
            }
        }

        //Movement and collision
        float playerDeltaMovementSpeed = playerMovementSpeed * deltaTime;
        var _PlayerPosition = playerPosition;
        Vector2 movementVector;
        Vector2 rotatedVector;
        bool IsXBlocked = false;
        bool IsYBlocked = false;

            //Checking the movement input
        movementVector.X = (keyboard.IsKeyDown(Keys.A) ? 1f : 0f) + (keyboard.IsKeyDown(Keys.D) ? -1f : 0f);
        movementVector.Y = (keyboard.IsKeyDown(Keys.W) ? 1f : 0f) + (keyboard.IsKeyDown(Keys.S) ? -1f : 0f);

            //Checking if the movement vector's magnitude is higher than 1
        if (MathX.Hypotenuse(movementVector.X, movementVector.Y) > 1f)
        {
            movementVector.Normalize();
        }

            //Rotating the movement vector to the angle of the player
        rotatedVector.X = (float)(movementVector.X * Math.Cos(playerAngle - MathX.Quadrant1)) - (float)(movementVector.Y * Math.Sin(playerAngle - MathX.Quadrant1));
        rotatedVector.Y = (float)(movementVector.X * Math.Sin(playerAngle - MathX.Quadrant1)) + (float)(movementVector.Y * Math.Cos(playerAngle - MathX.Quadrant1));

            //Sprint
        if (keyboard.IsKeyDown(Keys.W) && keyboard.IsKeyDown(Keys.LeftShift))
        {
            playerDeltaMovementSpeed = playerAngle * playerMovementSpeed * 1.7f;
            FOV = (int)(Settings.Graphics.FOV / 1.05f);
        }
        else
        {
            playerDeltaMovementSpeed = deltaTime * playerMovementSpeed;
            FOV = Settings.Graphics.FOV;
        }

            //Temporary variables to reduce calculations
            //XBlocked
        int tempXBlocked_Y_P = (int)((_PlayerPosition.Y + playerCollisionRadius) / tileSize);
        int tempXBlocked_Y_M = (int)((_PlayerPosition.Y - playerCollisionRadius) / tileSize);
        int tempXBlocked_X_P = (int)(((int)(_PlayerPosition.X + playerCollisionRadius + rotatedVector.X * playerDeltaMovementSpeed)) / tileSize);
        int tempXBlocked_X_M = (int)(((int)(_PlayerPosition.X - playerCollisionRadius + rotatedVector.X * playerDeltaMovementSpeed)) / tileSize);
            //YBlocked
        int tempYBlocked_Y_P = (int)(((int)(_PlayerPosition.Y + playerCollisionRadius + rotatedVector.Y * playerDeltaMovementSpeed)) / tileSize);
        int tempYBlocked_Y_M = (int)(((int)(_PlayerPosition.Y - playerCollisionRadius + rotatedVector.Y * playerDeltaMovementSpeed)) / tileSize);
        int tempYBlocked_X_P = (int)((_PlayerPosition.X + playerCollisionRadius) / tileSize);
        int tempYBlocked_X_M = (int)((_PlayerPosition.X - playerCollisionRadius) / tileSize);

            //Collision checking
        IsXBlocked =
                _PlayerPosition.X - playerCollisionRadius + rotatedVector.X * playerDeltaMovementSpeed <= 0f ||
                _PlayerPosition.X + playerCollisionRadius + rotatedVector.X * playerDeltaMovementSpeed >= (mapWalls.GetLength(1) * tileSize) ||
                mapWalls[tempXBlocked_Y_P, tempXBlocked_X_P] > 0 ||
                mapWalls[tempXBlocked_Y_P, tempXBlocked_X_M] > 0 ||
                mapWalls[tempXBlocked_Y_M, tempXBlocked_X_P] > 0 ||
                mapWalls[tempXBlocked_Y_M, tempXBlocked_X_M] > 0;

        IsYBlocked =
                _PlayerPosition.Y - playerCollisionRadius + rotatedVector.Y * playerDeltaMovementSpeed <= 0f ||
                _PlayerPosition.Y + playerCollisionRadius + rotatedVector.Y * playerDeltaMovementSpeed >= (mapWalls.GetLength(0) * tileSize) ||
                mapWalls[tempYBlocked_Y_P, tempYBlocked_X_P] > 0 ||
                mapWalls[tempYBlocked_Y_M, tempYBlocked_X_P] > 0 ||
                mapWalls[tempYBlocked_Y_P, tempYBlocked_X_M] > 0 ||
                mapWalls[tempYBlocked_Y_M, tempYBlocked_X_M] > 0;

            //Allowing player to move if the collision checker gave permission
        if (!IsXBlocked)
        {
            _PlayerPosition.X += rotatedVector.X * playerDeltaMovementSpeed;
        }
        if (!IsYBlocked)
        {
            _PlayerPosition.Y += rotatedVector.Y * playerDeltaMovementSpeed;
        }

        playerPosition = _PlayerPosition;
        //=============================================================================================

        //Allowed screen
        //=============================================================================================
        //Using the minimum of the screen's dimensions to keep a square aspect ratio
        minimumScreenHeight = ClientSize.Y > ClientSize.X ? ClientSize.X : ClientSize.Y;
        minimumScreenWidth = ClientSize.X > ClientSize.Y ? ClientSize.Y : ClientSize.X;
        //Calculating horizontal and vertical screen offset to center the game if needed
        screenHorizontalOffset = ClientSize.X > ClientSize.Y ? ((ClientSize.X - minimumScreenWidth) / 2) : 0;
        screenVerticalOffset = ClientSize.Y > ClientSize.X ? ((ClientSize.Y - minimumScreenHeight) / 2) : 0;
        //=============================================================================================

        //Raycount limiter
        //=============================================================================================
        rayCount = Math.Min(rayCount, (int)(minimumScreenWidth / 4f));
        //=============================================================================================

        //FOV calculation
        //=============================================================================================
        FOVStart = -((float)(FOV * (Math.PI / 180f)) / 2);
        RadBetweenRays = ((float)(FOV * (Math.PI / 180f)) / (rayCount - 1));
        //=============================================================================================

        //Variables for raycasting and graphic calculations
        //=============================================================================================
        float rayAngle = playerAngle + FOVStart;
            //Wall width
        float wallWidth = (float)minimumScreenWidth / rayCount;
        int RGBCalc;
        float shadeCalc, rowY;
            //Z position of camera (height of the player)
        float cameraZ = minimumScreenHeight / 2f;
        //=============================================================================================

        //Background for the game
        //=============================================================================================
        //Declaring temporary drawing parameter holder
        //float[] tempGameBackgroundVerticies = new float[7];

        //Wall array translator
        //newLine[0] = x1,4
        //newLine[1] = x2,3
        //newLine[2] = y1,2
        //newLine[3] = y3,4
        //newLine[4] = r
        //newLine[5] = g
        //newLine[6] = b
        vertexAttributesList.AddRange(new float[] 
        {
            screenHorizontalOffset,
            screenHorizontalOffset + minimumScreenWidth,
            screenVerticalOffset,
            screenVerticalOffset + minimumScreenHeight,
            0f,
            0f,
            1f
        });

        //tempGameBackgroundVerticies[0] = screenHorizontalOffset;
        //tempGameBackgroundVerticies[1] = screenHorizontalOffset + minimumScreenWidth;
        //tempGameBackgroundVerticies[2] = screenVerticalOffset;
        //tempGameBackgroundVerticies[3] = screenVerticalOffset + minimumScreenHeight;
        //tempGameBackgroundVerticies[4] = 0f;
        //tempGameBackgroundVerticies[5] = 0f;
        //tempGameBackgroundVerticies[6] = 0f;

        ////Adding temporary array the the wall list of arrays
        //vertexAttributesList.Add(tempGameBackgroundVerticies);
        //=============================================================================================

        //Raycasting | Ceiling, floor and wall rendering calculations
        //=============================================================================================
        for (int i = 0; i < rayCount; i++)
        {
            rayAngle = (rayAngle % MathX.Quadrant4 + MathX.Quadrant4) % MathX.Quadrant4;

            //Vertical wall check variables
            float offsetX = 0f,
                  offsetY = 0f,
                  verticalRayHitX = 0f,
                  verticalRayHitY = 0f,
                  verticalHypotenuse = 0f;

            int verticalRayCheckingCol,
                verticalRayCheckingRow;

            bool verticalWallFound = false;

            //Pre-defining variables for vertical wall checking
                //If player is looking right
            if (rayAngle > MathX.Quadrant3 || rayAngle < MathX.Quadrant1)
            {
                verticalRayHitX =
                    MathX.Clamp
                    (
                        (float)Math.Floor(playerPosition.X / tileSize) * tileSize + tileSize,
                        0,
                        (float)(mapWalls.GetLength(1) * tileSize - 0.0001)
                    );
                verticalRayHitY =
                    MathX.Clamp
                    (
                        playerPosition.Y - ((tileSize - (playerPosition.X % tileSize)) * (float)Math.Tan(MathX.Quadrant4 - rayAngle)),
                        0,
                        (float)(mapWalls.GetLength(0) * tileSize - 0.0001)
                    );
                offsetX = tileSize;
                offsetY = -(offsetX * (float)Math.Tan(MathX.Quadrant4 - rayAngle));
            }
                //If player is looking left
            else if (rayAngle < MathX.Quadrant3 && rayAngle > MathX.Quadrant1)
            {
                verticalRayHitX =
                    MathX.Clamp
                    (
                        (float)Math.Floor(playerPosition.X / tileSize) * tileSize - 0.0001f,
                        0,
                        (float)(mapWalls.GetLength(1) * tileSize - 0.0001)
                    );
                verticalRayHitY =
                    MathX.Clamp
                    (
                        playerPosition.Y - ((playerPosition.X % tileSize) * (float)Math.Tan(rayAngle)),
                        0,
                        (float)(mapWalls.GetLength(0) * tileSize - 0.0001)
                    );
                offsetX = -tileSize;
                offsetY = offsetX * (float)Math.Tan(rayAngle);
            }

            //Grid checker
            verticalRayCheckingCol = (int)Math.Floor(verticalRayHitX / tileSize);
            verticalRayCheckingRow = (int)Math.Floor(verticalRayHitY / tileSize);

            //Setting iterator
            int renderDistanceIterator = 0;

            //Cycle to find a wall
            while
            (
                renderDistanceIterator < renderDistance &&
                !verticalWallFound &&
                verticalRayHitX != (float)(mapWalls.GetLength(1) * tileSize - 0.0001f) &&
                verticalRayHitX != 0 &&
                verticalRayHitY != (float)(mapWalls.GetLength(0) * tileSize - 0.0001f) &&
                verticalRayHitY != 0
            )
            {
                //If wall is found
                if (mapWalls[verticalRayCheckingRow, verticalRayCheckingCol] > 0)
                {
                    renderDistanceIterator = renderDistance;
                    verticalWallFound = true;
                }
                //If wall isn't found
                else
                {
                    verticalRayHitX =
                        MathX.Clamp
                        (
                            verticalRayHitX + offsetX,
                            0,
                            (float)(mapWalls.GetLength(1) * tileSize - 0.0001)
                        );
                    verticalRayHitY =
                        MathX.Clamp
                        (
                            verticalRayHitY + offsetY,
                            0,
                            (float)(mapWalls.GetLength(0) * tileSize - 0.0001)
                        );
                    renderDistanceIterator++;
                    verticalRayCheckingCol = (int)(verticalRayHitX / tileSize);
                    verticalRayCheckingRow = (int)(verticalRayHitY / tileSize);
                }
            }

            //Vertical hypotenuse for line's length
            verticalHypotenuse = (float)Math.Sqrt(Math.Pow(playerPosition.X - verticalRayHitX, 2) + Math.Pow(playerPosition.Y - verticalRayHitY, 2));

            //Horizontal wall check variables
            float horizontalRayHitX = 0f,
                  horizontalRayHitY = 0f,
                  horizontalHypotenuse = 0f;

            int horizontalRayCheckingCol = 0,
                horizontalRayCheckingRow = 0;

            bool horizontalWallFound = false;

            //Pre-defining variables for horizontal wall checking
                //If player is looking up
            if (rayAngle > MathX.Quadrant2)
            {
                horizontalRayHitY =
                    MathX.Clamp
                    (
                        (float)Math.Floor(playerPosition.Y / tileSize) * tileSize - 0.0001f,
                        0,
                        (float)(mapWalls.GetLength(0) * tileSize - 0.0001)
                    );
                horizontalRayHitX =
                    MathX.Clamp
                    (
                        playerPosition.X + (playerPosition.Y % tileSize) / (float)Math.Tan(MathX.Quadrant4 - rayAngle),
                        0,
                        (float)(mapWalls.GetLength(1) * tileSize - 0.0001)
                    );
                offsetY = -tileSize;
                offsetX = tileSize / (float)Math.Tan(MathX.Quadrant4 - rayAngle);
            }
            //If player is looking down
            if (rayAngle < MathX.Quadrant2)
            {
                horizontalRayHitY =
                    MathX.Clamp
                    (
                        (float)Math.Floor(playerPosition.Y / tileSize) * tileSize + tileSize,
                        0,
                        (float)(mapWalls.GetLength(0) * tileSize - 0.0001)
                    );
                horizontalRayHitX =
                    MathX.Clamp
                    (
                        playerPosition.X - (tileSize - (playerPosition.Y % tileSize)) / (float)Math.Tan(MathX.Quadrant4 - rayAngle),
                        0,
                        (float)(mapWalls.GetLength(1) * tileSize - 0.0001)
                    );
                offsetY = tileSize;
                offsetX = -offsetY / (float)Math.Tan(MathX.Quadrant4 - rayAngle);
            }

            //Grid checker
            horizontalRayCheckingCol = (int)Math.Floor(horizontalRayHitX / tileSize);
            horizontalRayCheckingRow = (int)Math.Floor(horizontalRayHitY / tileSize);

            //Resetting iterator variable
            renderDistanceIterator = 0;

            //Cycle to find a wall
            while
            (
                renderDistanceIterator < renderDistance &&
                !horizontalWallFound &&
                horizontalRayHitY != (float)(mapWalls.GetLength(0) * tileSize - 0.0001f) &&
                horizontalRayHitY != 0 &&
                horizontalRayHitX != (float)(mapWalls.GetLength(1) * tileSize - 0.0001f) &&
                horizontalRayHitX != 0
            )
            {
                //If wall is found
                if (mapWalls[horizontalRayCheckingRow, horizontalRayCheckingCol] > 0)
                {
                    renderDistanceIterator = renderDistance;
                    horizontalWallFound = true;
                }
                else
                {
                    //If wall isn't found
                    horizontalRayHitY =
                        MathX.Clamp
                        (
                            horizontalRayHitY + offsetY,
                            0,
                            (float)(mapWalls.GetLength(0) * tileSize - 0.0001)
                        );
                    horizontalRayHitX =
                        MathX.Clamp
                        (
                            horizontalRayHitX + offsetX,
                            0,
                            (float)(mapWalls.GetLength(1) * tileSize - 0.0001)
                        );
                    renderDistanceIterator++;
                    horizontalRayCheckingCol = (int)(horizontalRayHitX / tileSize);
                    horizontalRayCheckingRow = (int)(horizontalRayHitY / tileSize);
                }
            }

            //Horizontal hypotenuse for line's length
            horizontalHypotenuse = (float)Math.Sqrt(Math.Pow(playerPosition.X - horizontalRayHitX, 2) + Math.Pow(playerPosition.Y - horizontalRayHitY, 2));

            float rayLength = Math.Min(verticalHypotenuse, horizontalHypotenuse);
            float rayTilePosition = 0;
            int wallSide = 0;
            int wallType = 0;
            //If wall is vertical
            if (rayLength == verticalHypotenuse && verticalWallFound)
            {
                //Ray's end's position relative to the tile
                rayTilePosition = verticalRayHitY % tileSize;

                //If wall side is left
                if (rayAngle > MathX.Quadrant1 && rayAngle < MathX.Quadrant3)
                {
                    wallSide = 1;
                }
                //If wall side is right
                else if (rayAngle > MathX.Quadrant3 || rayAngle < MathX.Quadrant1)
                {
                    wallSide = 2;
                }

                //What type of wall did the ray hit (if out of bounds, 0)
                wallType = mapWalls[verticalRayCheckingRow, verticalRayCheckingCol];
            }
            //If wall is horizontal
            else if (rayLength == horizontalHypotenuse && horizontalWallFound)
            {
                //Ray's end's position relative to the tile
                rayTilePosition = horizontalRayHitX % tileSize;

                //If wall side is top
                if (rayAngle > 0 && rayAngle < Math.PI)
                {
                    wallSide = 3;
                }
                //If wall side is bottom
                else if (rayAngle > Math.PI && rayAngle < MathX.Quadrant4)
                {
                    wallSide = 4;
                }

                //What type of wall did the ray hit (if out of bounds, 0)
                wallType = mapWalls[horizontalRayCheckingRow, horizontalRayCheckingCol];
            }
            //No wall hit
            else
            {
                wallType = 0;
            }

            float wallHeight = (float)((tileSize * minimumScreenHeight) /
                (rayLength * (float)Math.Cos(playerAngle -
                (playerAngle + FOVStart + i * RadBetweenRays))));

            int[][] path = null;
            int pitch = 0;

            //Ceiling calculation
            //=============================================================================================
            float ceilingFloorPixelDistance,
                  ceilingPixelXWorldPosition,
                  ceilingPixelYWorldPosition,
                  floorPixelXWorldPosition,
                  floorPixelYWorldPosition;

            //Floor and ceiling X position variables
            float floorCeilingPixelXLeft = i * wallWidth + screenHorizontalOffset;
            float floorCeilingPixelXRight = (i + 1) * wallWidth + screenHorizontalOffset;

            float ceilingStep = Math.Max(4f, wallWidth);

            //Ceiling Y position variables
            float ceilingPixelYTop = screenVerticalOffset;
            float ceilingPixelYBottom = screenVerticalOffset + ceilingStep;

            while (ceilingPixelYTop < (screenVerticalOffset + ((minimumScreenHeight - wallHeight) / 2)) + pitch)
            {
                /* 
                * If the pixel's top position is in the correct screen, but the bottom position is in the wall,
                * the bottom position's Y value may be equal to the wall's top value.
                */

                ceilingPixelYBottom =
                (ceilingPixelYBottom > (screenVerticalOffset + ((minimumScreenHeight - wallHeight) / 2)) + pitch) ?
                (screenVerticalOffset + ((minimumScreenHeight - wallHeight) / 2)) + pitch :
                ceilingPixelYBottom;

                //Y of the current pixel on the screen
                rowY = (ClientSize.Y / 2) - (ceilingPixelYTop + ((ceilingPixelYBottom - ceilingPixelYTop) / 2)) + pitch;

                //Distance of the pixel from the player
                ceilingFloorPixelDistance = ((cameraZ / rowY) * tileSize) / (float)Math.Cos(playerAngle - rayAngle);

                //World X position of the pixel
                ceilingPixelXWorldPosition = playerPosition.X + ((float)Math.Cos(rayAngle) * ceilingFloorPixelDistance);

                //World Y position of the pixel
                ceilingPixelYWorldPosition = playerPosition.Y + ((float)Math.Sin(rayAngle) * ceilingFloorPixelDistance);

                //Textures
                path =
                (ceilingPixelXWorldPosition >= mapCeiling.GetLength(1) * tileSize ||
                ceilingPixelXWorldPosition < 0f ||
                ceilingPixelYWorldPosition >= mapCeiling.GetLength(0) * tileSize ||
                ceilingPixelYWorldPosition < 0f) ?
                    null :
                    TextureTranslator((int)mapCeiling[(int)Math.Floor(ceilingPixelYWorldPosition / tileSize), (int)Math.Floor(ceilingPixelXWorldPosition / tileSize)]);

                if (path == null)
                {
                    ceilingPixelYTop = ceilingPixelYBottom;
                    ceilingPixelYBottom += ceilingStep;
                    continue;
                }

                //Calculating RGB variables
                RGBCalc = ((int)Math.Floor(path[0][1] / (tileSize / (ceilingPixelYWorldPosition % tileSize))) * path[0][1] * 3) +
                          ((int)Math.Floor(path[0][0] / (tileSize / (ceilingPixelXWorldPosition % tileSize))) * 3);

                //Calculating shading and lighting with distance
                shadeCalc = ceilingFloorPixelDistance * distanceShade;

                //Declaring temporary drawing parameter holder
                //float[] tempCeilingVertices = new float[7];

                //Wall array translator
                //newLine[0] = x1,4
                //newLine[1] = x2,3
                //newLine[2] = y1,2
                //newLine[3] = y3,4
                //newLine[4] = r
                //newLine[5] = g
                //newLine[6] = b

                vertexAttributesList.AddRange(new float[]
                {
                    floorCeilingPixelXLeft,
                    floorCeilingPixelXRight,
                    ceilingPixelYTop,
                    ceilingPixelYBottom,
                    (path[1][RGBCalc] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc] - shadeCalc) / 255f,
                    (path[1][RGBCalc + 1] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 1] - shadeCalc) / 255f,
                    (path[1][RGBCalc + 2] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 2] - shadeCalc) / 255f
                });

                //tempCeilingVertices[0] = floorCeilingPixelXLeft;
                //tempCeilingVertices[1] = floorCeilingPixelXRight;
                //tempCeilingVertices[2] = ceilingPixelYTop;
                //tempCeilingVertices[3] = ceilingPixelYBottom;
                //tempCeilingVertices[4] = (path[1][RGBCalc] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc] - shadeCalc) / 255f;
                //tempCeilingVertices[5] = (path[1][RGBCalc + 1] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 1] - shadeCalc) / 255f;
                //tempCeilingVertices[6] = (path[1][RGBCalc + 2] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 2] - shadeCalc) / 255f;

                //Adding temporary array the the wall list of arrays
                //vertexAttributesList.Add(tempCeilingVertices);

                //Incremental increasing of the top and bottom values of the pixel
                ceilingPixelYTop = ceilingPixelYBottom;
                ceilingPixelYBottom += ceilingStep;
            }

            //=============================================================================================

            //Wall calculation
            //Textures
            path = TextureTranslator(wallType);

            if (path == null)
            {
                continue;
            }

            //Shading and lighting with distance
            shadeCalc = rayLength * distanceShade;

            //Optimizing with shade (if the shade would have been strong enaougn to make everything black, just paint the line black)
            int shadeOptimizationLimit = 255;

            if (shadeCalc >= shadeOptimizationLimit)
            {
                float lineCalcTop = (ClientSize.Y / 2) - (wallHeight / 2) + pitch;
                float lineCalcBottom = (ClientSize.Y / 2) - (wallHeight / 2) + (path[0][1] * (wallHeight / path[0][1])) + pitch;

                //Declaring temporary drawing parameter holder
                //float[] tempWallVerticies = new float[7];

                //Wall array translator
                //newLine[0] = x1,4
                //newLine[1] = x2,3
                //newLine[2] = y1,2
                //newLine[3] = y3,4
                //newLine[4] = r
                //newLine[5] = g
                //newLine[6] = b

                vertexAttributesList.AddRange(new float[]
                {
                    i * wallWidth + screenHorizontalOffset,
                    (i + 1) * wallWidth + screenHorizontalOffset,
                    lineCalcTop,
                    lineCalcBottom,
                    0f,
                    0f,
                    0f
                });

                //tempWallVerticies[0] = i * wallWidth + screenHorizontalOffset;
                //tempWallVerticies[1] = (i + 1) * wallWidth + screenHorizontalOffset;
                //tempWallVerticies[2] = lineCalcTop;
                //tempWallVerticies[3] = lineCalcBottom;
                //tempWallVerticies[4] = 1f;
                //tempWallVerticies[5] = 0f;
                //tempWallVerticies[6] = 0f;

                ////Adding temporary array the the wall list of arrays
                //vertexAttributesList.Add(tempWallVerticies);

                continue;
            }

            float pixelCalcTop = (ClientSize.Y / 2) - (wallHeight / 2) + pitch;
            float pixelCalcBottom = (ClientSize.Y / 2) - (wallHeight / 2) + (wallHeight / path[0][1]) + pitch;

            //Drawing pixels in lines from up to down (walls)
            for (int k = 0; k < path[0][1]; k++)
            {
                //Ensuring that the graphical image stays within the interpolated screen size
                if (pixelCalcBottom < screenVerticalOffset || pixelCalcTop > (screenVerticalOffset + minimumScreenHeight))
                {
                    pixelCalcTop = pixelCalcBottom;
                    pixelCalcBottom += (wallHeight / path[0][1]);

                    continue;
                }
                else
                {
                    /* 
                     * If the pixel's bottom position is inside the correct screen, but the top position sticks out,
                     * the top position's Y value may be equal to the allowed screen's top value.
                    */
                    pixelCalcTop =
                    (pixelCalcTop < screenVerticalOffset && pixelCalcBottom > screenVerticalOffset) ?
                    screenVerticalOffset :
                    pixelCalcTop;

                    /* 
                     * If the pixel's top position is inside the correct screen, but the bottom position sticks out,
                     * the bottom position's Y value may be equal to the allowed screen's bottom value.
                    */
                    pixelCalcBottom =
                    (pixelCalcTop < (screenVerticalOffset + minimumScreenHeight) && pixelCalcBottom > (screenVerticalOffset + minimumScreenHeight)) ?
                    (screenVerticalOffset + minimumScreenHeight) :
                    pixelCalcBottom;

                    //Mirroring wrong textures, Calculating RGB variables
                    RGBCalc = (wallSide == 1 || wallSide == 3) ?
                        ((int)Math.Floor((tileSize - rayTilePosition) / (tileSize / (float)path[0][0])) * 3) + k * (path[0][0] * 3) :
                        ((int)Math.Floor(rayTilePosition / (tileSize / (float)path[0][0])) * 3) + k * (path[0][0] * 3);

                    RGBCalc = (RGBCalc == 3888) ?
                        3885 :
                        RGBCalc;

                    //Declaring temporary drawing parameter holder
                    //float[] tempWallVerticies = new float[7];

                    //Wall array translator
                    //newLine[0] = x1,4
                    //newLine[1] = x2,3
                    //newLine[2] = y1,2
                    //newLine[3] = y3,4
                    //newLine[4] = r
                    //newLine[5] = g
                    //newLine[6] = b

                    vertexAttributesList.AddRange(new float[]
                    {
                        i * wallWidth + screenHorizontalOffset,
                        (i + 1) * wallWidth + screenHorizontalOffset,
                        pixelCalcTop,
                        pixelCalcBottom,
                        (path[1][RGBCalc] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc] - shadeCalc) / 255f,
                        (path[1][RGBCalc + 1] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 1] - shadeCalc) / 255f,
                        (path[1][RGBCalc + 2] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 2] - shadeCalc) / 255f
                    });

                    //tempWallVerticies[0] = i * wallWidth + screenHorizontalOffset;
                    //tempWallVerticies[1] = (i + 1) * wallWidth + screenHorizontalOffset;
                    //tempWallVerticies[2] = pixelCalcTop;
                    //tempWallVerticies[3] = pixelCalcBottom;
                    //tempWallVerticies[4] = (path[1][RGBCalc] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc] - shadeCalc) / 255f;
                    //tempWallVerticies[5] = (path[1][RGBCalc + 1] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 1] - shadeCalc) / 255f;
                    //tempWallVerticies[6] = (path[1][RGBCalc + 2] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 2] - shadeCalc) / 255f;

                    ////Adding temporary array the the wall list of arrays
                    //vertexAttributesList.Add(tempWallVerticies);

                    pixelCalcTop = pixelCalcBottom;
                    pixelCalcBottom += (wallHeight / path[0][1]);
                }
            }

            //Incrementing rayAngle for next ray
            rayAngle = ((rayAngle + RadBetweenRays) % MathX.Quadrant4 + MathX.Quadrant4) % MathX.Quadrant4;
        }
        //=============================================================================================

        shaderVertices = vertexAttributesList.ToArray();

        //Loading buffer
        if (shaderVertices != null && shaderVertices.Length > 0)
        {
            verticesReady = true;

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            GL.BufferData(BufferTarget.ArrayBuffer,
                          shaderVertices.Length * sizeof(float),
                          shaderVertices,
                          BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        else
        {
            verticesReady = false;
        }

        vertexAttributesList.Clear();
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

        shader.Use();
        GL.BindVertexArray(VertexArrayObject);

        int instanceCount = shaderVertices.Length / 7;

        if (instanceCount > 0)
        {
            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, instanceCount);
        }

        SwapBuffers();
    }
    //=============================================================================================
    //Gets called when window is closed
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

    public int[][] TextureTranslator(int x)
    {
        //No wall hit
        int[][] path = null;

        switch (x)
        {
            //Wall hit
            //Default textures
            case 1:
                path = Textures.planksTexture;
                break;
            case 2:
                path = Textures.mossyPlanksTexture;
                break;
            case 3:
                path = Textures.stoneBricksTexture;
                break;
            case 4:
                path = Textures.mossyStoneBricksTexture;
                break;

            //Door textures
            case 5:
                path = Textures.doorStoneBricksTexture;
                break;
            case 6:
                path = Textures.doorMossyStoneBricksTexture;
                break;

            //Window textures
            case 7:
                path = Textures.windowStoneBricksTexture;
                break;
            case 8:
                path = Textures.windowMossyStoneBricksTexture;
                break;

            //Painting textures
            case 9:
                path = Textures.painting_a_stoneBricksTexture;
                break;
            case 10:
                path = Textures.painting_b_stoneBricksTexture;
                break;
            case 11:
                path = Textures.painting_a_mossyStoneBricksTexture;
                break;
            case 12:
                path = Textures.painting_b_mossyStoneBricksTexture;
                break;

            //Test textures
            case 13:
                path = Textures.floorTestTexture;
                break;
        }
        return path;
    }
}

//Öteltek holnapra:
//- TexCoords kitanulása
//- Tömbök minimalizálása rajzolásnál
//- Optimalizálási lehetőségek átgondolása
//- Padló és tető rajzolása, megnézni változott e a teljesítmény jó vagy rossz irányba
//(Ha a padló és tető közül az egyikét kiszámolod, a másik értékeit elég invertálni "TALÁN"...)

//Minimap
//GL.Begin(PrimitiveType.Quads);
//GL.Color3(0f, 1f, 0f);
//GL.Vertex2(0, 0);
//GL.Vertex2(mapWalls.GetLength(1) * tileSize, 0);
//GL.Vertex2(mapWalls.GetLength(1) * tileSize, mapWalls.GetLength(0) * tileSize);
//GL.Vertex2(0, mapWalls.GetLength(0) * tileSize);
//GL.End();

//GL.Begin(PrimitiveType.Quads);
//for (int x = 0; x < mapWalls.GetLength(1); x++)
//{
//    for (int y = 0; y < mapWalls.GetLength(0); y++)
//    {
//        if (mapWalls[y, x] == 0) { GL.Color3(0.3f, 0.3f, 0.3f); }
//        else { GL.Color3(0f, 0f, 0f); }
//        //Left top
//        GL.Vertex2(tileSize * x + 1f, tileSize * y + 1f);
//        //Right top
//        GL.Vertex2(tileSize * x + tileSize - 1f, tileSize * y + 1f);
//        //Right bottom
//        GL.Vertex2(tileSize * x + tileSize - 1f, tileSize * y + tileSize - 1f);
//        //Left bottom
//        GL.Vertex2(tileSize * x + 1f, tileSize * y + tileSize - 1f);
//    }
//}
////Drawing player
//GL.Color3(0f, 1f, 0f);
//GL.Vertex2(playerPosition.X - 10f / 2f, playerPosition.Y - 10f / 2f);
//GL.Vertex2(playerPosition.X + 10f / 2f, playerPosition.Y - 10f / 2f);
//GL.Vertex2(playerPosition.X + 10f / 2f, playerPosition.Y + 10f / 2f);
//GL.Vertex2(playerPosition.X - 10f / 2f, playerPosition.Y + 10f / 2f);
//GL.End();
////Drawing rays
//GL.LineWidth(1f);
//GL.Begin(PrimitiveType.Lines);
//for (int i = 0; i < rayCount; i++)
//{
//    //No wall hit (no render)
//    /*
//     * rayDatas[i, 0] = ray's length
//     * rayDatas[i, 1] = ray's x length (cos(rayAngle) * length)
//     * rayDatas[i, 2] = ray's y length (sin(rayAngle) * length)
//     */
//    if (rayDatas[i, 0] == 0)
//    {
//        GL.Color3(0f, 0f, 0f);
//    }
//    else
//    {
//        GL.Color3(1f, 0f, 0f);
//    }
//    GL.Vertex2(playerPosition.X, playerPosition.Y);
//    GL.Vertex2(playerPosition.X + rayDatas[i, 1], playerPosition.Y + rayDatas[i, 2]);
//}
//GL.End();

////Drawing direction ray
//GL.LineWidth(6f);
//GL.Begin(PrimitiveType.Lines);
//GL.Color3(0f, 1f, 1f);
//GL.Vertex2(playerPosition.X, playerPosition.Y);
//GL.Vertex2(playerPosition.X + Math.Cos(playerAngle) * 30, playerPosition.Y + Math.Sin(playerAngle) * 30);
//GL.End();

//vertexDatas Translator
// 0.: X1 0 - 3
// 1.: X2 1 - 2
// 2.: Y1 0 - 1
// 3.: Y2 2 - 3
// 4.: R all vertex
// 5.: G all vertex
// 6.: B all vertex

/*  0            1
 *   ____________
 *   |          |
 *   |          |
 *   |          |
 *   |          |
 *   |          |
 *   ------------
 *  3            2
 */