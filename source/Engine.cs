using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

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

    //Player variables
    public static Vector2 playerPosition { get; private set; } = new Vector2(100, 100);
    public static int PlayerWidth { get; private set; } = 10;
    public static int PlayerHeight { get; private set; } = 10;
    public static float PlayerAngle { get; private set; } = 0f;
    public static float PlayerDeltaOffsetX { get; private set; } = 0f;
    public static float PlayerDeltaOffsetY { get; private set; } = 0f;
    public static int PlayerRadius { get; private set; } = 10;
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
    public static int VerticalRayCheckingCol { get; private set; } = 0;
    public static int VerticalRayCheckingRow { get; private set; } = 0;
    public static int HorizontalRayCheckingCol { get; private set; } = 0;
    public static int HorizontalRayCheckingRow { get; private set; } = 0;
    public static bool VerticalWallFound { get; private set; } = false;
    public static bool HorizontalWallFound { get; private set; } = false;
    public static float VerticalPythagoras { get; private set; } = 0f;
    public static float HorizontalPythagoras { get; private set; } = 0f;

    //Wall rendering variables
    public static float[,] RayDatas { get; private set; } = new float[RayCount, 6];

    private static Stopwatch stopWatch = new Stopwatch();
    private static float lastTime = 0f;

    public static void Start() {
        stopWatch.Start();
    }

    public static void EngineUpdate() {
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
            VerticalWallFound = false;
            HorizontalWallFound = false;

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

            VerticalRayCheckingCol = (int)Math.Floor(RayX / TileSize);
            VerticalRayCheckingRow = (int)Math.Floor(RayY / TileSize);

            while (DepthOfField < Settings.Graphics.DepthOfField && VerticalRayCheckingRow >= 0 && VerticalRayCheckingRow < MapWalls.GetLength(0) && VerticalRayCheckingCol >= 0 && VerticalRayCheckingCol < MapWalls.GetLength(1))
            {
                if (MapWalls[VerticalRayCheckingRow, VerticalRayCheckingCol] > 0)
                {
                    DepthOfField = Settings.Graphics.DepthOfField;
                    VerticalWallFound = true;
                }
                else
                {
                    RayX += OffsetX;
                    RayY += OffsetY;
                    DepthOfField++;
                    VerticalRayCheckingCol = (int)(RayX / TileSize);
                    VerticalRayCheckingRow = (int)(RayY / TileSize);
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

            HorizontalRayCheckingCol = (int)Math.Floor(RayX / TileSize);
            HorizontalRayCheckingRow = (int)Math.Floor(RayY / TileSize);

            while (DepthOfField < Settings.Graphics.DepthOfField && HorizontalRayCheckingRow >= 0 && HorizontalRayCheckingRow < MapWalls.GetLength(0) && HorizontalRayCheckingCol >= 0 && HorizontalRayCheckingCol < MapWalls.GetLength(1))
            {
                if (MapWalls[HorizontalRayCheckingRow, HorizontalRayCheckingCol] > 0)
                {
                    DepthOfField = Settings.Graphics.DepthOfField;
                    HorizontalWallFound = true;
                }
                else
                {
                    RayY += OffsetY;
                    RayX += OffsetX;
                    DepthOfField++;
                    HorizontalRayCheckingCol = (int)(RayX / TileSize);
                    HorizontalRayCheckingRow = (int)(RayY / TileSize);
                }
            }

            HorizontalPythagoras = (float)Math.Sqrt(Math.Pow(Math.Abs(PlayerPosition.X - RayX), 2) + Math.Pow(Math.Abs(PlayerPosition.Y - RayY), 2));

            //Ray data storage
            //RayDatas[i, 0]: Ray's length
            //RayDatas[i, 1]: Ray's X offset
            //RayDatas[i, 2]: Ray's Y offset
            //RayDatas[i, 3]: Wall height
            //RayDatas[i, 4]: Wall side (0 = No wall, 1 = Vertical, 2 = Horizontal)
            //RayDatas[i, 5]: Wall type for texturing (Not implemented yet)

            RayDatas[i, 0] = Math.Min(VerticalPythagoras, HorizontalPythagoras);
            RayDatas[i, 1] = PlayerPosition.X + (float)Math.Cos(RayAngle) * Math.Min(VerticalPythagoras, HorizontalPythagoras);
            RayDatas[i, 2] = PlayerPosition.Y + (float)Math.Sin(RayAngle) * Math.Min(VerticalPythagoras, HorizontalPythagoras);
            RayDatas[i, 3] = (float)(((TileSize * GraphicWindow.ScreenHeight) / (RayDatas[i, 0] * (float)Math.Cos(PlayerAngle - (PlayerAngle + FOVStart + i * RadBetweenRays)))) / 1.7);
            if (RayDatas[i, 0] == VerticalPythagoras && VerticalWallFound)
            {
                RayDatas[i, 4] = 1;
                RayDatas[i, 5] = MapWalls[VerticalRayCheckingRow, VerticalRayCheckingCol];
            }
            else if (RayDatas[i, 0] == HorizontalPythagoras && HorizontalWallFound)
            {
                RayDatas[i, 4] = 2;
                RayDatas[i, 5] = MapWalls[HorizontalRayCheckingRow, HorizontalRayCheckingCol];
            }
            else
            {
                RayDatas[i, 5] = 0;
            }

            RayAngle += RadBetweenRays;
            RayAngle = (RayAngle % MathX.Quadrant4 + MathX.Quadrant4) % MathX.Quadrant4;
        }
        //================================================================
    }
}