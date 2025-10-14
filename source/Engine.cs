using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Diagnostics;

public class Engine
{
    int FOV = Settings.Graphics.FOV;
    int RayCount = Settings.Graphics.RayCount;
    int TileSize = Settings.Gameplay.TileSize;
    int[,] grid = Map.grid;
    float MovementSpeed = Settings.Player.MovementSpeed;
    float MouseSensitivity = Settings.Player.MouseSensitivity;

    public float DeltaTime { get; private set; } = 0.0167f;
    public Vector2 playerPosition { get; private set; } = new Vector2(100, 100);
    public int PlayerWidth { get; private set; } = 10;
    public int PlayerHeight { get; private set; } = 10;
    public float PlayerAngle { get; private set; } = 0f;
    public float PlayerDeltaOffsetX { get; private set; } = 0f;
    public float PlayerDeltaOffsetY { get; private set; } = 0f;
    public float FOVUnit { get; private set; } = 0f;
    public float RadBetweenRays { get; private set; } = 0f;

    private static Stopwatch stopWatch = new Stopwatch();
    private float lastTime = 0f;

    public static void Start() {
        stopWatch.Start();
    }

    public void Update() {
        //DeltaTime calculation
        float CurrentTime = (float)stopWatch.Elapsed.TotalSeconds;
        DeltaTime = CurrentTime - lastTime;
        lastTime = CurrentTime;
        //================================================================

        //Player movement and rotation
        float DeltaMovementSpeed = MovementSpeed * DeltaTime;
        var keyboard = Keyboard.GetState();
        var PlayerPosition = playerPosition;

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

        //Forward
        if (keyboard.IsKeyDown(Key.W))
        {
            PlayerPosition.X += (float)Math.Cos(PlayerAngle) * DeltaMovementSpeed;
            PlayerPosition.Y += (float)Math.Sin(PlayerAngle) * DeltaMovementSpeed;
        }

        //Backward
        if (keyboard.IsKeyDown(Key.S))
        {
            PlayerPosition.X -= (float)Math.Cos(PlayerAngle) * DeltaMovementSpeed;
            PlayerPosition.Y -= (float)Math.Sin(PlayerAngle) * DeltaMovementSpeed;
        }
        playerPosition = PlayerPosition;
        //================================================================

        //FOV Calculation
        FOVUnit = -((float)(FOV * (MathX.PI / 180f)) / 2);
        RadBetweenRays = ((float)(FOV * (MathX.PI / 180f)) / (RayCount - 1));
        //================================================================

        //Wall checking
        //Vertical wall checking
        //If looking right


        //If looking left


        //Horizontal wall checking
        //If looking up


        //If looking down


        //================================================================
    }
}