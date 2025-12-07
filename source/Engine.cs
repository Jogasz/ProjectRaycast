using OpenTK;
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

    //Player variables
    public static Vector2 playerPosition { get; private set; } = new Vector2(100, 100);
    public static float PlayerAngle { get; set; } = 0f;
    public static int PlayerRadius { get; private set; } = 10;

    //Engine variables
    public static float DeltaTime { get; private set; } = 0.0167f;
    public static float FOVStart { get; private set; } = 0f;
    public static float RadBetweenRays { get; private set; } = 0f;
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

        //Keybinds, player controls and collision detection
        float DeltaMovementSpeed = MovementSpeed * DeltaTime;
        var keyboard = Keyboard.GetState();
        var PlayerPosition = playerPosition;

        //Escape - exiting game
        if (keyboard.IsKeyDown(Key.Escape))
        {
            Environment.Exit(0);
        }

        Vector2 movementVector;

        //Checking the movement input
        movementVector.X = (keyboard.IsKeyDown(Key.A) ? 1f : 0f) + (keyboard.IsKeyDown(Key.D) ? -1f : 0f);
        movementVector.Y = (keyboard.IsKeyDown(Key.W) ? 1f : 0f) + (keyboard.IsKeyDown(Key.S) ? -1f : 0f);

        //Checking if the movement vector's magnitude is higher than 1
        if (MathX.Hypotenuse(movementVector.X, movementVector.Y) > 1f)
        {
            movementVector.Normalize();
        }

        Vector2 rotatedVector;

        //Rotating the movement vector to the angle of the player
        rotatedVector.X = (float)(movementVector.X * Math.Cos(PlayerAngle - MathX.Quadrant1)) - (float)(movementVector.Y * Math.Sin(PlayerAngle - MathX.Quadrant1));
        rotatedVector.Y = (float)(movementVector.X * Math.Sin(PlayerAngle - MathX.Quadrant1)) + (float)(movementVector.Y * Math.Cos(PlayerAngle - MathX.Quadrant1));

        bool IsXBlocked = false;
        bool IsYBlocked = false;

        //Sprint
        if (keyboard.IsKeyDown(Key.W) && keyboard.IsKeyDown(Key.ShiftLeft))
        {
            DeltaMovementSpeed = DeltaTime * MovementSpeed * 1.7f;
            FOV = (int)(Settings.Graphics.FOV / 1.05f);
        }
        else
        {
            DeltaMovementSpeed = DeltaTime * MovementSpeed;
            FOV = Settings.Graphics.FOV;
        }

        //Collision checking
        IsXBlocked =
                PlayerPosition.X - PlayerRadius + rotatedVector.X * DeltaMovementSpeed <= 0f ||
                PlayerPosition.X + PlayerRadius + rotatedVector.X * DeltaMovementSpeed >= (MapWalls.GetLength(1) * TileSize) ||
                MapWalls[(int)((PlayerPosition.Y + PlayerRadius) / TileSize), (int)(((int)(PlayerPosition.X + PlayerRadius + rotatedVector.X * DeltaMovementSpeed)) / TileSize)] > 0 ||
                MapWalls[(int)((PlayerPosition.Y + PlayerRadius) / TileSize), (int)(((int)(PlayerPosition.X - PlayerRadius + rotatedVector.X * DeltaMovementSpeed)) / TileSize)] > 0 ||
                MapWalls[(int)((PlayerPosition.Y - PlayerRadius) / TileSize), (int)(((int)(PlayerPosition.X + PlayerRadius + rotatedVector.X * DeltaMovementSpeed)) / TileSize)] > 0 ||
                MapWalls[(int)((PlayerPosition.Y - PlayerRadius) / TileSize), (int)(((int)(PlayerPosition.X - PlayerRadius + rotatedVector.X * DeltaMovementSpeed)) / TileSize)] > 0;

        IsYBlocked =
                PlayerPosition.Y - PlayerRadius + rotatedVector.Y * DeltaMovementSpeed <= 0f ||
                PlayerPosition.Y + PlayerRadius + rotatedVector.Y * DeltaMovementSpeed >= (MapWalls.GetLength(0) * TileSize) ||
                MapWalls[(int)(((int)(PlayerPosition.Y + PlayerRadius + rotatedVector.Y * DeltaMovementSpeed)) / TileSize), (int)((PlayerPosition.X + PlayerRadius) / TileSize)] > 0 ||
                MapWalls[(int)(((int)(PlayerPosition.Y - PlayerRadius + rotatedVector.Y * DeltaMovementSpeed)) / TileSize), (int)((PlayerPosition.X + PlayerRadius) / TileSize)] > 0 ||
                MapWalls[(int)(((int)(PlayerPosition.Y + PlayerRadius + rotatedVector.Y * DeltaMovementSpeed)) / TileSize), (int)((PlayerPosition.X - PlayerRadius) / TileSize)] > 0 ||
                MapWalls[(int)(((int)(PlayerPosition.Y - PlayerRadius + rotatedVector.Y * DeltaMovementSpeed)) / TileSize), (int)((PlayerPosition.X - PlayerRadius) / TileSize)] > 0;

        //Allowing player to move if the collision checker gave permission
        if (!IsXBlocked)
        {
            PlayerPosition.X += rotatedVector.X * DeltaMovementSpeed;
        }

        if (!IsYBlocked)
        {
            PlayerPosition.Y += rotatedVector.Y * DeltaMovementSpeed;
        }

        playerPosition = PlayerPosition;
        //================================================================

        //FOV Calculation
        FOVStart = -((float)(FOV * (MathX.PI / 180f)) / 2);
        RadBetweenRays = ((float)(FOV * (MathX.PI / 180f)) / (RayCount - 1));
        //================================================================

        //Casting rays until they hit a wall
        float RayAngle = PlayerAngle + FOVStart;

        float VerticalRayHitX = 0f;
        float VerticalRayHitY = 0f;

        int VerticalRayCheckingCol = 0;
        int VerticalRayCheckingRow = 0;

        float OffsetX = 0f;
        float OffsetY = 0f;

        bool VerticalWallFound = false;
        bool HorizontalWallFound = false;

        for (int i = 0; i < RayCount; i++)
        {
            RayAngle = (RayAngle % MathX.Quadrant4 + MathX.Quadrant4) % MathX.Quadrant4;

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
            int RenderDistance = Settings.Graphics.RenderDistance;
            int RenderDistanceIterator = 0;

            while (RenderDistanceIterator < RenderDistance &&
                VerticalRayCheckingRow >= 0 &&
                VerticalRayCheckingRow < MapWalls.GetLength(0) &&
                VerticalRayCheckingCol >= 0 &&
                VerticalRayCheckingCol < MapWalls.GetLength(1))
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

            float VerticalPythagoras = (float)Math.Sqrt(Math.Pow(Math.Abs(PlayerPosition.X - VerticalRayHitX), 2) + Math.Pow(Math.Abs(PlayerPosition.Y - VerticalRayHitY), 2));

            float HorizontalRayHitX = 0f;
            float HorizontalRayHitY = 0f;

            int HorizontalRayCheckingCol = 0;
            int HorizontalRayCheckingRow = 0;

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

            float HorizontalPythagoras = (float)Math.Sqrt(Math.Pow(Math.Abs(PlayerPosition.X - HorizontalRayHitX), 2) + Math.Pow(Math.Abs(PlayerPosition.Y - HorizontalRayHitY), 2));

            //Ray data storage
            //RayDatas[i, 0]: Ray's length
            //RayDatas[i, 1]: Ray's delta X
            //RayDatas[i, 2]: Ray's delta Y
            //RayDatas[i, 3]: VerticalRayHitY or HorizontalRayHitX
            //RayDatas[i, 4]: Wall side (1 = left, 2 = right, 3 = top, 4 = bottom)
            //RayDatas[i, 5]: Wall type (0 = No wall, 0> = Wall)
            //RayDatas[i, 6]: Ray's angle

            //Length of the ray
            RayDatas[i, 0] = Math.Min(VerticalPythagoras, HorizontalPythagoras);

            //Ray's delta x
            RayDatas[i, 1] = (float)Math.Cos(RayAngle);

            //Ray's delta y
            RayDatas[i, 2] = (float)Math.Sin(RayAngle);

            //If wall is a vertical wall
            if (RayDatas[i, 0] == VerticalPythagoras && VerticalWallFound)
            {
                //Ray's end's position relative to the tile
                RayDatas[i, 3] = VerticalRayHitY % TileSize;

                //If wall side is left
                if (RayAngle > MathX.Quadrant1 && RayAngle < MathX.Quadrant3) {
                    RayDatas[i, 4] = 1;
                }
                //If wall side is right
                else if (RayAngle > MathX.Quadrant3 || RayAngle < MathX.Quadrant1)
                {
                    RayDatas[i, 4] = 2;
                }

                //What type of wall did the ray hit (if out of bounds, 0)
                RayDatas[i, 5] = MapWalls[VerticalRayCheckingRow, VerticalRayCheckingCol];

            }

            //If wall is a horizontal wall
            else if (RayDatas[i, 0] == HorizontalPythagoras && HorizontalWallFound)
            {
                //Ray's end's position relative to the tile
                RayDatas[i, 3] = HorizontalRayHitX % TileSize;

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

                //What type of wall did the ray hit (if out of bounds, 0)
                RayDatas[i, 5] = MapWalls[HorizontalRayCheckingRow, HorizontalRayCheckingCol];
            }

            //If there was no wall hit (maybe because of render distance)
            else
            {
                RayDatas[i, 5] = 0;
            }

            //The current ray's angle
            RayDatas[i, 6] = RayAngle;
            
            //Incrementing the value of the ray's angle for the next ray in FOV
            RayAngle = ((RayAngle + RadBetweenRays) % MathX.Quadrant4 + MathX.Quadrant4) % MathX.Quadrant4;
        }
        //================================================================
    }
}