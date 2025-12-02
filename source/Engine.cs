using OpenTK;
using OpenTK.Input;
using System;
using System.ComponentModel.Design;
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

    //Player variables
    public static Vector2 playerPosition { get; private set; } = new Vector2(100, 100);
    public static int PlayerWidth { get; private set; } = 10;
    public static int PlayerHeight { get; private set; } = 10;
    public static float PlayerAngle { get; set; } = 0f;
    public static float PlayerDeltaOffsetX { get; set; } = 0f;
    public static float PlayerDeltaOffsetY { get; set; } = 0f;
    public static int PlayerRadius { get; private set; } = 10;
    public static bool BlockedX { get; private set; } = false;
    public static bool BlockedY { get; private set; } = false;

    //Raycasting variables
    public static float FOVStart { get; private set; } = 0f;
    public static float RadBetweenRays { get; private set; } = 0f;
    public static float RayAngle { get; private set; } = 0f;
    public static int RenderDistance { get; private set; } = Settings.Graphics.RenderDistance;
    public static int RenderDistanceIterator { get; private set; } = 0;
    public static float VerticalRayHitX { get; private set; } = 0f;
    public static float VerticalRayHitY { get; private set; } = 0f;
    public static float HorizontalRayHitX { get; private set; } = 0f;
    public static float HorizontalRayHitY { get; private set; } = 0f;
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
    public static float[,] RayDatas { get; private set; } = new float[RayCount, 7];

    private static Stopwatch stopWatch = new Stopwatch();
    private static float lastTime = 0f;

    public static void Start() {
        stopWatch.Start();
    }

    public static void Update() {
        //DeltaTime calculation
        float CurrentTime = (float)stopWatch.Elapsed.TotalSeconds;
        DeltaTime = CurrentTime - lastTime;
        lastTime = CurrentTime;

        //For showing FPS
        //Console.WriteLine(Math.Floor(1 / DeltaTime) + "FPS");
        //================================================================

        //Player controls, rotaion and collision detection
        float DeltaMovementSpeed = MovementSpeed * DeltaTime;
        var keyboard = Keyboard.GetState();
        var PlayerPosition = playerPosition;

        //Escape - exiting game
        if (keyboard.IsKeyDown(Key.Escape))
        {
            Environment.Exit(0);
        }

        //Temporary variables for less calculation in movement collision (Not sure but doesn't seem like the best solution...)
        int BlockedX_Y_Minus = (int)((PlayerPosition.Y - PlayerRadius) / TileSize);
        int BlockedX_Y_Plus = (int)((PlayerPosition.Y + PlayerRadius) / TileSize);
        int BlockedY_X_Minus = (int)((PlayerPosition.X - PlayerRadius) / TileSize);
        int BlockedX_X_Plus = (int)((PlayerPosition.X + PlayerRadius) / TileSize);

        //Forward and backward variables
        PlayerDeltaOffsetX = (float)Math.Cos(PlayerAngle);
        PlayerDeltaOffsetY = (float)Math.Sin(PlayerAngle);

        //Forward
        int forwardBlockedX_X_Plus = (int)(((int)(PlayerPosition.X + PlayerRadius + PlayerDeltaOffsetX * DeltaMovementSpeed)) / TileSize);
        int forwardBlockedX_X_Minus = (int)(((int)(PlayerPosition.X - PlayerRadius + PlayerDeltaOffsetX * DeltaMovementSpeed)) / TileSize);
        int forwardBlockedY_Y_Plus = (int)(((int)(PlayerPosition.Y + PlayerRadius + PlayerDeltaOffsetY * DeltaMovementSpeed)) / TileSize);
        int forwardBlockedY_Y_Minus = (int)(((int)(PlayerPosition.Y - PlayerRadius + PlayerDeltaOffsetY * DeltaMovementSpeed)) / TileSize);

        //Backward
        int backwardBlockedX_X_Plus = (int)(((int)(PlayerPosition.X + PlayerRadius - PlayerDeltaOffsetX * DeltaMovementSpeed)) / TileSize);
        int backwardBlockedX_X_Minus = (int)(((int)(PlayerPosition.X - PlayerRadius - PlayerDeltaOffsetX * DeltaMovementSpeed)) / TileSize);
        int backwardBlockedY_Y_Plus = (int)(((int)(PlayerPosition.Y + PlayerRadius - PlayerDeltaOffsetY * DeltaMovementSpeed)) / TileSize);
        int backwardBlockedY_Y_Minus = (int)(((int)(PlayerPosition.Y - PlayerRadius - PlayerDeltaOffsetY * DeltaMovementSpeed)) / TileSize);

        //Left and right variables
        float PlayerDeltaOffsetXMinusQuadrant = (float)Math.Cos(PlayerAngle - MathX.Quadrant1);
        float PlayerDeltaOffsetYMinusQuadrant = (float)Math.Sin(PlayerAngle - MathX.Quadrant1);

        //Left
        int leftBlockedX_X_Plus = (int)(((int)(PlayerPosition.X + PlayerRadius + PlayerDeltaOffsetXMinusQuadrant * DeltaMovementSpeed)) / TileSize);
        int leftBlockedX_X_Minus = (int)(((int)(PlayerPosition.X - PlayerRadius + PlayerDeltaOffsetXMinusQuadrant * DeltaMovementSpeed)) / TileSize);
        int leftBlockedY_Y_Plus = (int)(((int)(PlayerPosition.Y + PlayerRadius + PlayerDeltaOffsetYMinusQuadrant * DeltaMovementSpeed)) / TileSize);
        int leftBlockedY_Y_Minus = (int)(((int)(PlayerPosition.Y - PlayerRadius + PlayerDeltaOffsetYMinusQuadrant * DeltaMovementSpeed)) / TileSize);

        //Right
        int rightBlockedX_X_Plus = (int)(((int)(PlayerPosition.X + PlayerRadius - PlayerDeltaOffsetXMinusQuadrant * DeltaMovementSpeed)) / TileSize);
        int rightBlockedX_X_Minus = (int)(((int)(PlayerPosition.X - PlayerRadius - PlayerDeltaOffsetXMinusQuadrant * DeltaMovementSpeed)) / TileSize);
        int rightBlockedY_Y_Plus = (int)(((int)(PlayerPosition.Y + PlayerRadius - PlayerDeltaOffsetYMinusQuadrant * DeltaMovementSpeed)) / TileSize);
        int rightBlockedY_Y_Minus = (int)(((int)(PlayerPosition.Y - PlayerRadius - PlayerDeltaOffsetYMinusQuadrant * DeltaMovementSpeed)) / TileSize);

        //Forward movement and forward collision detection
        if (keyboard.IsKeyDown(Key.W))
        {
            BlockedX =
                MapWalls[BlockedX_Y_Minus, forwardBlockedX_X_Plus] > 0 ||
                MapWalls[BlockedX_Y_Minus, forwardBlockedX_X_Minus] > 0 ||
                MapWalls[BlockedX_Y_Plus, forwardBlockedX_X_Plus] > 0 ||
                MapWalls[BlockedX_Y_Plus, forwardBlockedX_X_Minus] > 0;

            BlockedY =
                MapWalls[forwardBlockedY_Y_Plus, BlockedY_X_Minus] > 0 ||
                MapWalls[forwardBlockedY_Y_Minus, BlockedY_X_Minus] > 0 ||
                MapWalls[forwardBlockedY_Y_Plus, BlockedX_X_Plus] > 0 ||
                MapWalls[forwardBlockedY_Y_Minus, BlockedX_X_Plus] > 0;

            if (!BlockedX)
            {
                PlayerPosition.X += PlayerDeltaOffsetX * DeltaMovementSpeed;
            }

            if (!BlockedY)
            {
                PlayerPosition.Y += PlayerDeltaOffsetY * DeltaMovementSpeed;
            }
        }

        //Left movement and left collision detection
        if (keyboard.IsKeyDown(Key.A))
        {
            BlockedX =
                MapWalls[BlockedX_Y_Minus, leftBlockedX_X_Plus] > 0 ||
                MapWalls[BlockedX_Y_Minus, leftBlockedX_X_Minus] > 0 ||
                MapWalls[BlockedX_Y_Plus, leftBlockedX_X_Plus] > 0 ||
                MapWalls[BlockedX_Y_Plus, leftBlockedX_X_Minus] > 0;

            BlockedY =
                MapWalls[leftBlockedY_Y_Plus, BlockedY_X_Minus] > 0 ||
                MapWalls[leftBlockedY_Y_Minus, BlockedY_X_Minus] > 0 ||
                MapWalls[leftBlockedY_Y_Plus, BlockedX_X_Plus] > 0 ||
                MapWalls[leftBlockedY_Y_Minus, BlockedX_X_Plus] > 0;

            if (!BlockedX)
            {
                PlayerPosition.X += PlayerDeltaOffsetXMinusQuadrant * DeltaMovementSpeed;
            }

            if (!BlockedY)
            {
                PlayerPosition.Y += PlayerDeltaOffsetYMinusQuadrant * DeltaMovementSpeed;
            }
        }

        //Backward movement and backward collision detection
        if (keyboard.IsKeyDown(Key.S))
        {
            BlockedX =
                MapWalls[BlockedX_Y_Minus, backwardBlockedX_X_Plus] > 0 ||
                MapWalls[BlockedX_Y_Minus, backwardBlockedX_X_Minus] > 0 ||
                MapWalls[BlockedX_Y_Plus, backwardBlockedX_X_Plus] > 0 ||
                MapWalls[BlockedX_Y_Plus, backwardBlockedX_X_Minus] > 0;

            BlockedY =
                MapWalls[backwardBlockedY_Y_Plus, BlockedY_X_Minus] > 0 ||
                MapWalls[backwardBlockedY_Y_Minus, BlockedY_X_Minus] > 0 ||
                MapWalls[backwardBlockedY_Y_Plus, BlockedX_X_Plus] > 0 ||
                MapWalls[backwardBlockedY_Y_Minus, BlockedX_X_Plus] > 0;

            if (!BlockedX)
            {
                PlayerPosition.X -= PlayerDeltaOffsetX * DeltaMovementSpeed;
            }

            if (!BlockedY)
            {
                PlayerPosition.Y -= PlayerDeltaOffsetY * DeltaMovementSpeed;
            }
        }

        //Right movement and Right collision detection
        if (keyboard.IsKeyDown(Key.D))
        {
            BlockedX =
                MapWalls[BlockedX_Y_Minus, rightBlockedX_X_Plus] > 0 ||
                MapWalls[BlockedX_Y_Minus, rightBlockedX_X_Minus] > 0 ||
                MapWalls[BlockedX_Y_Plus, rightBlockedX_X_Plus] > 0 ||
                MapWalls[BlockedX_Y_Plus, rightBlockedX_X_Minus] > 0;

            BlockedY =
                MapWalls[rightBlockedY_Y_Plus, BlockedY_X_Minus] > 0 ||
                MapWalls[rightBlockedY_Y_Minus, BlockedY_X_Minus] > 0 ||
                MapWalls[rightBlockedY_Y_Plus, BlockedX_X_Plus] > 0 ||
                MapWalls[rightBlockedY_Y_Minus, BlockedX_X_Plus] > 0;

            if (!BlockedX)
            {
                PlayerPosition.X -= PlayerDeltaOffsetXMinusQuadrant * DeltaMovementSpeed;
            }

            if (!BlockedY)
            {
                PlayerPosition.Y -= PlayerDeltaOffsetYMinusQuadrant * DeltaMovementSpeed;
            }
        }

            playerPosition = PlayerPosition;
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
            if (RayAngle > MathX.Quadrant3 || RayAngle < MathX.Quadrant1)
            {
                //Player is looking right
                VerticalRayHitX = (float)Math.Floor(PlayerPosition.X / TileSize) * TileSize + TileSize;
                VerticalRayHitY = PlayerPosition.Y - ((TileSize - (PlayerPosition.X % TileSize)) * (float)Math.Tan(MathX.Quadrant4 - RayAngle));
                OffsetX = TileSize;
                OffsetY = -(OffsetX * (float)Math.Tan(MathX.Quadrant4 - RayAngle));
            }
            else if (RayAngle < MathX.Quadrant3 && RayAngle > MathX.Quadrant1)
            {
                //Player is looking left
                VerticalRayHitX = (float)Math.Floor(PlayerPosition.X / TileSize) * TileSize - 0.0001f;
                VerticalRayHitY = PlayerPosition.Y - ((PlayerPosition.X % TileSize) * (float)Math.Tan(RayAngle));
                OffsetX = -TileSize;
                OffsetY = OffsetX * (float)Math.Tan(RayAngle);
            }

            VerticalRayCheckingCol = (int)Math.Floor(VerticalRayHitX / TileSize);
            VerticalRayCheckingRow = (int)Math.Floor(VerticalRayHitY / TileSize);

            //Resetting iterator variable
            RenderDistanceIterator = 0;

            while (RenderDistanceIterator < RenderDistance && VerticalRayCheckingRow >= 0 && VerticalRayCheckingRow < MapWalls.GetLength(0) && VerticalRayCheckingCol >= 0 && VerticalRayCheckingCol < MapWalls.GetLength(1))
            {
                if (MapWalls[VerticalRayCheckingRow, VerticalRayCheckingCol] > 0)
                {
                    RenderDistanceIterator = RenderDistance;
                    VerticalWallFound = true;
                }
                else
                {
                    VerticalRayHitX += OffsetX;
                    VerticalRayHitY += OffsetY;
                    RenderDistanceIterator++;
                    VerticalRayCheckingCol = (int)(VerticalRayHitX / TileSize);
                    VerticalRayCheckingRow = (int)(VerticalRayHitY / TileSize);
                }
            }

            VerticalPythagoras = (float)Math.Sqrt(Math.Pow(Math.Abs(PlayerPosition.X - VerticalRayHitX), 2) + Math.Pow(Math.Abs(PlayerPosition.Y - VerticalRayHitY), 2));

            //Horizontal wall check
            if (RayAngle > MathX.Quadrant2)
            {
                //Player is looking up
                HorizontalRayHitY = (float)Math.Floor(PlayerPosition.Y / TileSize) * TileSize - 0.0001f;
                HorizontalRayHitX = PlayerPosition.X + (PlayerPosition.Y % TileSize) / (float)Math.Tan(MathX.Quadrant4 - RayAngle);
                OffsetY = -TileSize;
                OffsetX = TileSize / (float)Math.Tan(MathX.Quadrant4 - RayAngle);
            }

            if (RayAngle < MathX.Quadrant2)
            {
                //Player is looking down
                HorizontalRayHitY = (float)Math.Floor(PlayerPosition.Y / TileSize) * TileSize + TileSize;
                HorizontalRayHitX = PlayerPosition.X - (TileSize - (PlayerPosition.Y % TileSize)) / (float)Math.Tan(MathX.Quadrant4 - RayAngle);
                OffsetY = TileSize;
                OffsetX = -OffsetY / (float)Math.Tan(MathX.Quadrant4 - RayAngle);
            }

            HorizontalRayCheckingCol = (int)Math.Floor(HorizontalRayHitX / TileSize);
            HorizontalRayCheckingRow = (int)Math.Floor(HorizontalRayHitY / TileSize);

            //Resetting iterator variable
            RenderDistanceIterator = 0;

            while (RenderDistanceIterator < RenderDistance && HorizontalRayCheckingRow >= 0 && HorizontalRayCheckingRow < MapWalls.GetLength(0) && HorizontalRayCheckingCol >= 0 && HorizontalRayCheckingCol < MapWalls.GetLength(1))
            {
                if (MapWalls[HorizontalRayCheckingRow, HorizontalRayCheckingCol] > 0)
                {
                    RenderDistanceIterator = RenderDistance;
                    HorizontalWallFound = true;
                }
                else
                {
                    HorizontalRayHitY += OffsetY;
                    HorizontalRayHitX += OffsetX;
                    RenderDistanceIterator++;
                    HorizontalRayCheckingCol = (int)(HorizontalRayHitX / TileSize);
                    HorizontalRayCheckingRow = (int)(HorizontalRayHitY / TileSize);
                }
            }

            HorizontalPythagoras = (float)Math.Sqrt(Math.Pow(Math.Abs(PlayerPosition.X - HorizontalRayHitX), 2) + Math.Pow(Math.Abs(PlayerPosition.Y - HorizontalRayHitY), 2));

            //Ray data storage
            //RayDatas[i, 0]: Ray's length
            //RayDatas[i, 1]: Ray's delta X
            //RayDatas[i, 2]: Ray's delta Y
            //RayDatas[i, 3]: VerticalRayHitY or HorizontalRayHitX
            //RayDatas[i, 4]: Wall side (1 = left, 2 = right, 3 = top, 4 = bottom)
            //RayDatas[i, 5]: Wall type (0 = No wall, 0> = Wall)
            //RayDatas[i, 6]: Ray's angle

            RayDatas[i, 0] = Math.Min(VerticalPythagoras, HorizontalPythagoras);
            RayDatas[i, 1] = (float)Math.Cos(RayAngle);
            RayDatas[i, 2] = (float)Math.Sin(RayAngle);
            if (RayDatas[i, 0] == VerticalPythagoras && VerticalWallFound)
            {
                RayDatas[i, 3] = VerticalRayHitY % TileSize;
                //If wall is vertical
                //If wall side is left
                if (RayAngle > MathX.Quadrant1 && RayAngle < MathX.Quadrant3) {
                    RayDatas[i, 4] = 1;
                }
                //If wall side is right
                else if (RayAngle > MathX.Quadrant3 || RayAngle < MathX.Quadrant1)
                {
                    RayDatas[i, 4] = 2;
                }
                RayDatas[i, 5] = MapWalls[VerticalRayCheckingRow, VerticalRayCheckingCol];
            }
            else if (RayDatas[i, 0] == HorizontalPythagoras && HorizontalWallFound)
            {
                RayDatas[i, 3] = HorizontalRayHitX % TileSize;
                //If wall is horizontal
                //If wall side is top
                if (RayAngle > 0 && RayAngle < MathX.PI)
                {
                    RayDatas[i, 4] = 3;
                }
                //If wall side is bottom
                else if (RayAngle > MathX.PI && RayAngle < MathX.Quadrant4)
                {
                    RayDatas[i, 4] = 4;
                }
                RayDatas[i, 5] = MapWalls[HorizontalRayCheckingRow, HorizontalRayCheckingCol];
            }
            else
            {
                RayDatas[i, 5] = 0;
            }
            RayDatas[i, 6] = RayAngle;

            RayAngle += RadBetweenRays;
            RayAngle = (RayAngle % MathX.Quadrant4 + MathX.Quadrant4) % MathX.Quadrant4;
        }
        //================================================================
    }
}