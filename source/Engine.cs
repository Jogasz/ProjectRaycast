using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Diagnostics;

public class Engine
{
    //Settings variables
    static int FOV = Settings.Graphics.FOV;
    static int RayCount = Settings.Graphics.RayCount;
    static int TileSize = Settings.Gameplay.TileSize;
    static int[,] MapWalls = Level.mapWalls;
    static float MovementSpeed = Settings.Player.MovementSpeed;
    static float MouseSensitivity = Settings.Player.MouseSensitivity;

    //Engine variables
    public static float DeltaTime { get; private set; } = 0.0167f;
    private static readonly object engineLock = new object();

    //Player variables
    public static Vector2 playerPosition { get; private set; } = new Vector2(100, 100);
    public static int PlayerWidth { get; private set; } = 10;
    public static int PlayerHeight { get; private set; } = 10;
    public static float PlayerAngle { get; private set; } = 0f;
    public static float PlayerDeltaOffsetX { get; private set; } = 0f;
    public static float PlayerDeltaOffsetY { get; private set; } = 0f;
    public static int PlayerRadius { get; private set; } = 10;
    public static int NextX { get; private set; } = 0;
    public static int NextY { get; private set; } = 0;
    public static bool BlockedX { get; private set; } = false;
    public static bool BlockedY { get; private set; } = false;

    //Raycasting variables
    public static float FOVStart { get; private set; } = 0f;
    public static float RadBetweenRays { get; private set; } = 0f;
    public static float RayAngle { get; private set; } = 0f;
    public static int DepthOfField { get; private set; } = 0;
    public static float RayX { get; private set; } = 0f;
    public static float RayY { get; private set; } = 0f;
    public static float OffsetX { get; private set; } = 0f;
    public static float OffsetY { get; private set; } = 0f;
    public static int CheckingMapCol { get; private set; } = 0;
    public static int CheckingMapRow { get; private set; } = 0;
    public static float VerticalPythagoras { get; private set; } = 0f;
    public static float HorizontalPythagoras { get; private set; } = 0f;
    public static float[,] RayDeltaDirections { get; private set; } = new float[Settings.Graphics.RayCount, 2];
    public static float[] RayDistances { get; private set; } = new float[Settings.Graphics.RayCount];
    public static float RayEndX { get; private set; } = 0f;
    public static float RayEndY { get; private set; } = 0f;

    //Wall rendering variables
    public static float WallWidth { get; private set; } = 0f;
    public static float[] WallHeights { get; private set; } = new float[Settings.Graphics.RayCount];

    private static Stopwatch stopWatch = new Stopwatch();
    private static float lastTime = 0f;

    public static void Start() {
        stopWatch.Start();
    }

    public static void Update() {
        lock (engineLock) {
            //DeltaTime calculation
            float CurrentTime = (float)stopWatch.Elapsed.TotalSeconds;
            DeltaTime = CurrentTime - lastTime;
            //For testing FPS
            //Console.WriteLine(Math.Floor(1 / DeltaTime) + "FPS");
            lastTime = CurrentTime;
            //================================================================

            //Player movement, rotaion and collision detection
            float DeltaMovementSpeed = MovementSpeed * DeltaTime;
            var keyboard = Keyboard.GetState();
            var PlayerPosition = playerPosition;

            PlayerDeltaOffsetX = (float)Math.Cos(PlayerAngle);
            PlayerDeltaOffsetY = (float)Math.Sin(PlayerAngle);

            //Strife left
            if (keyboard.IsKeyDown(Key.A))
            {
                PlayerAngle -= MouseSensitivity * DeltaTime;
                if (PlayerAngle < 0)
                {
                    PlayerAngle += (2 * MathX.PI);
                }
                PlayerDeltaOffsetX = (float)Math.Cos(PlayerAngle);
                PlayerDeltaOffsetY = (float)Math.Sin(PlayerAngle);
            }

            //Strife right
            if (keyboard.IsKeyDown(Key.D))
            {
                PlayerAngle += MouseSensitivity * DeltaTime;
                if (PlayerAngle > (2 * MathX.PI))
                {
                    PlayerAngle -= (2 * MathX.PI);
                }
                PlayerDeltaOffsetX = (float)Math.Cos(PlayerAngle);
                PlayerDeltaOffsetY = (float)Math.Sin(PlayerAngle);
            }

            //Forward movement and forward collision detection
            if (keyboard.IsKeyDown(Key.W))
            {
                BlockedX =
                    MapWalls[(int)((PlayerPosition.Y - PlayerRadius) / TileSize), (int)(((int)(PlayerPosition.X + PlayerRadius + (float)Math.Cos(PlayerAngle) * DeltaMovementSpeed)) / TileSize)] > 0 ||
                    MapWalls[(int)((PlayerPosition.Y - PlayerRadius) / TileSize), (int)(((int)(PlayerPosition.X - PlayerRadius + (float)Math.Cos(PlayerAngle) * DeltaMovementSpeed)) / TileSize)] > 0 ||
                    MapWalls[(int)((PlayerPosition.Y + PlayerRadius) / TileSize), (int)(((int)(PlayerPosition.X + PlayerRadius + (float)Math.Cos(PlayerAngle) * DeltaMovementSpeed)) / TileSize)] > 0 ||
                    MapWalls[(int)((PlayerPosition.Y + PlayerRadius) / TileSize), (int)(((int)(PlayerPosition.X - PlayerRadius + (float)Math.Cos(PlayerAngle) * DeltaMovementSpeed)) / TileSize)] > 0;

                BlockedY =
                    MapWalls[(int)(((int)(PlayerPosition.Y + PlayerRadius + (float)Math.Sin(PlayerAngle) * DeltaMovementSpeed)) / TileSize), (int)((PlayerPosition.X - PlayerRadius) / TileSize)] > 0 ||
                    MapWalls[(int)(((int)(PlayerPosition.Y - PlayerRadius + (float)Math.Sin(PlayerAngle) * DeltaMovementSpeed)) / TileSize), (int)((PlayerPosition.X - PlayerRadius) / TileSize)] > 0 ||
                    MapWalls[(int)(((int)(PlayerPosition.Y + PlayerRadius + (float)Math.Sin(PlayerAngle) * DeltaMovementSpeed)) / TileSize), (int)((PlayerPosition.X + PlayerRadius) / TileSize)] > 0 ||
                    MapWalls[(int)(((int)(PlayerPosition.Y - PlayerRadius + (float)Math.Sin(PlayerAngle) * DeltaMovementSpeed)) / TileSize), (int)((PlayerPosition.X + PlayerRadius) / TileSize)] > 0;

                if (!BlockedX)
                {
                    PlayerPosition.X += (float)Math.Cos(PlayerAngle) * DeltaMovementSpeed;
                }

                if (!BlockedY)
                {
                    PlayerPosition.Y += (float)Math.Sin(PlayerAngle) * DeltaMovementSpeed;
                }
            }

            //Backward movement and backward collision detection
            if (keyboard.IsKeyDown(Key.S))
            {
                BlockedX =
                    MapWalls[(int)((PlayerPosition.Y - PlayerRadius) / TileSize), (int)(((int)(PlayerPosition.X + PlayerRadius - (float)Math.Cos(PlayerAngle) * DeltaMovementSpeed)) / TileSize)] > 0 ||
                    MapWalls[(int)((PlayerPosition.Y - PlayerRadius) / TileSize), (int)(((int)(PlayerPosition.X - PlayerRadius - (float)Math.Cos(PlayerAngle) * DeltaMovementSpeed)) / TileSize)] > 0 ||
                    MapWalls[(int)((PlayerPosition.Y + PlayerRadius) / TileSize), (int)(((int)(PlayerPosition.X + PlayerRadius - (float)Math.Cos(PlayerAngle) * DeltaMovementSpeed)) / TileSize)] > 0 ||
                    MapWalls[(int)((PlayerPosition.Y + PlayerRadius) / TileSize), (int)(((int)(PlayerPosition.X - PlayerRadius - (float)Math.Cos(PlayerAngle) * DeltaMovementSpeed)) / TileSize)] > 0;

                BlockedY =
                    MapWalls[(int)(((int)(PlayerPosition.Y + PlayerRadius - (float)Math.Sin(PlayerAngle) * DeltaMovementSpeed)) / TileSize), (int)((PlayerPosition.X - PlayerRadius) / TileSize)] > 0 ||
                    MapWalls[(int)(((int)(PlayerPosition.Y - PlayerRadius - (float)Math.Sin(PlayerAngle) * DeltaMovementSpeed)) / TileSize), (int)((PlayerPosition.X - PlayerRadius) / TileSize)] > 0 ||
                    MapWalls[(int)(((int)(PlayerPosition.Y + PlayerRadius - (float)Math.Sin(PlayerAngle) * DeltaMovementSpeed)) / TileSize), (int)((PlayerPosition.X + PlayerRadius) / TileSize)] > 0 ||
                    MapWalls[(int)(((int)(PlayerPosition.Y - PlayerRadius - (float)Math.Sin(PlayerAngle) * DeltaMovementSpeed)) / TileSize), (int)((PlayerPosition.X + PlayerRadius) / TileSize)] > 0;

                if (!BlockedX)
                {
                    PlayerPosition.X -= (float)Math.Cos(PlayerAngle) * DeltaMovementSpeed;
                }

                if (!BlockedY)
                {
                    PlayerPosition.Y -= (float)Math.Sin(PlayerAngle) * DeltaMovementSpeed;
                }
            }

            playerPosition = PlayerPosition;
            //================================================================

            //Mouse input for rotation

            //================================================================

            //FOV Calculation
            FOVStart = -((float)(FOV * (MathX.PI / 180f)) / 2);
            RadBetweenRays = ((float)(FOV * (MathX.PI / 180f)) / (RayCount - 1));
            //================================================================

            //Casting rays until they hit a wall
            RayAngle = PlayerAngle + FOVStart;

            for (int i = 0; i < RayCount; i++)
            {
                RayAngle = (RayAngle % MathX.Quadrant4 + MathX.Quadrant4) % MathX.Quadrant4;

                //Vertical wall check
                DepthOfField = 0;
                if (RayAngle > MathX.Quadrant3 || RayAngle < MathX.Quadrant1)
                {
                    //Player is looking right
                    RayX = (float)Math.Floor(PlayerPosition.X / TileSize) * TileSize + TileSize;
                    RayY = PlayerPosition.Y - ((TileSize - (PlayerPosition.X % TileSize)) * (float)Math.Tan(MathX.Quadrant4 - RayAngle));
                    OffsetX = TileSize;
                    OffsetY = -(OffsetX * (float)Math.Tan(MathX.Quadrant4 - RayAngle));
                }
                else if (RayAngle < MathX.Quadrant3 && RayAngle > MathX.Quadrant1)
                {
                    //Player is looking left
                    RayX = (float)Math.Floor(PlayerPosition.X / TileSize) * TileSize - 0.0001f;
                    RayY = PlayerPosition.Y - ((PlayerPosition.X % TileSize) * (float)Math.Tan(RayAngle));
                    OffsetX = -TileSize;
                    OffsetY = OffsetX * (float)Math.Tan(RayAngle);
                }

                CheckingMapCol = (int)Math.Floor(RayX / TileSize);
                CheckingMapRow = (int)Math.Floor(RayY / TileSize);

                while (DepthOfField < Settings.Graphics.DepthOfField && CheckingMapRow >= 0 && CheckingMapRow < MapWalls.GetLength(0) && CheckingMapCol >= 0 && CheckingMapCol < MapWalls.GetLength(1))
                {
                    if (MapWalls[CheckingMapRow, CheckingMapCol] == 1)
                    {
                        DepthOfField = Settings.Graphics.DepthOfField;
                    }
                    else
                    {
                        RayX += OffsetX;
                        RayY += OffsetY;
                        DepthOfField++;
                        CheckingMapCol = (int)(RayX / TileSize);
                        CheckingMapRow = (int)(RayY / TileSize);
                    }
                }

                VerticalPythagoras = (float)Math.Sqrt(Math.Pow(Math.Abs(PlayerPosition.X - RayX), 2) + Math.Pow(Math.Abs(PlayerPosition.Y - RayY), 2));

                //Horizontal wall check
                DepthOfField = 0;
                if (RayAngle > MathX.Quadrant2)
                {
                    //Player is looking up
                    RayY = (float)Math.Floor(PlayerPosition.Y / TileSize) * TileSize - 0.0001f;
                    RayX = PlayerPosition.X + (PlayerPosition.Y % TileSize) / (float)Math.Tan(MathX.Quadrant4 - RayAngle);
                    OffsetY = -TileSize;
                    OffsetX = TileSize / (float)Math.Tan(MathX.Quadrant4 - RayAngle);
                }

                if (RayAngle < MathX.Quadrant2)
                {
                    //Player is looking down
                    RayY = (float)Math.Floor(PlayerPosition.Y / TileSize) * TileSize + TileSize;
                    RayX = PlayerPosition.X - (TileSize - (PlayerPosition.Y % TileSize)) / (float)Math.Tan(MathX.Quadrant4 - RayAngle);
                    OffsetY = TileSize;
                    OffsetX = -OffsetY / (float)Math.Tan(MathX.Quadrant4 - RayAngle);
                }

                CheckingMapCol = (int)Math.Floor(RayX / TileSize);
                CheckingMapRow = (int)Math.Floor(RayY / TileSize);

                while (DepthOfField < Settings.Graphics.DepthOfField && CheckingMapRow >= 0 && CheckingMapRow < MapWalls.GetLength(0) && CheckingMapCol >= 0 && CheckingMapCol < MapWalls.GetLength(1))
                {
                    if (MapWalls[CheckingMapRow, CheckingMapCol] == 1)
                    {
                        DepthOfField = Settings.Graphics.DepthOfField;
                    }
                    else
                    {
                        RayY += OffsetY;
                        RayX += OffsetX;
                        DepthOfField++;
                        CheckingMapCol = (int)(RayX / TileSize);
                        CheckingMapRow = (int)(RayY / TileSize);
                    }
                }

                HorizontalPythagoras = (float)Math.Sqrt(Math.Pow(Math.Abs(PlayerPosition.X - RayX), 2) + Math.Pow(Math.Abs(PlayerPosition.Y - RayY), 2));

                RayDistances[i] = Math.Min(VerticalPythagoras, HorizontalPythagoras);

                RayEndX = PlayerPosition.X + (float)Math.Cos(RayAngle) * Math.Min(VerticalPythagoras, HorizontalPythagoras);
                RayEndY = PlayerPosition.Y + (float)Math.Sin(RayAngle) * Math.Min(VerticalPythagoras, HorizontalPythagoras);

                RayDeltaDirections[i, 0] = RayEndX;
                RayDeltaDirections[i, 1] = RayEndY;

                RayAngle += RadBetweenRays;
                RayAngle = (RayAngle % MathX.Quadrant4 + MathX.Quadrant4) % MathX.Quadrant4;
            }
            //================================================================

            //Calculating graphic wall width and height
            WallWidth = (float)(GraphicWindow.ScreenWidth / RayCount);

            for (int i = 0; i < RayCount; i++)
            {
                float rayAngle = PlayerAngle + FOVStart + i * RadBetweenRays;
                float correctedDist = RayDistances[i] * (float)Math.Cos(PlayerAngle - rayAngle);
                WallHeights[i] = (float)(((TileSize * GraphicWindow.ScreenHeight) / correctedDist) / 1.7);
            }
            //================================================================
        }
    }
}