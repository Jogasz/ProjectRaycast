using System;
using System.IO;
using Newtonsoft.Json;

public static class Settings
{
    public static PlayerSettings Player;
    public static GraphicsSettings Graphics;
    public static GameplaySettings Gameplay;

    //Handles loading and storing all game settings from settings.json
    public static void Load()
    {
        string jsonText = File.ReadAllText("settings.json");

        var data = JsonConvert.DeserializeObject<SettingsData>(jsonText);

        Player = data.Player;
        Graphics = data.Graphics;
        Gameplay = data.Gameplay;
    }

    //Represents the structure of the JSON data for deserialization
    private class SettingsData
    {
        public PlayerSettings Player { get; set; }
        public GraphicsSettings Graphics { get; set; }
        public GameplaySettings Gameplay { get; set; }
    }

    //Stores player-related settings like speed and mouse sensitivity
    public class PlayerSettings
    {
        public int Health { get; private set; }
        public int Armor { get; private set; }
        public int Stamina { get; private set; }
        public float MovementSpeed { get; private set; }
        public float MouseSensitivity { get; private set; }


        [JsonConstructor]
        public PlayerSettings(int health, int armor, int stamina, float movementSpeed, float mouseSensitivity)
        {
            Health = health;
            Armor = armor;
            Stamina = stamina;
            MovementSpeed = movementSpeed * 10;
            MouseSensitivity = mouseSensitivity / 5;
        }
    }

    public class GraphicsSettings
    {
        public int FOV { get; private set; }
        public int RayCount { get; private set; }
        public int DepthOfField { get; private set; }
        public float VerticalShade { get; private set; }
        public float HorizontalShade { get; private set; }

        [JsonConstructor]
        public GraphicsSettings(int fov, int rayCount, int depthOfField, float verticalShade, float horizontalShade) {
            FOV = fov;
            RayCount = rayCount;
            DepthOfField = depthOfField;
            VerticalShade = verticalShade;
            HorizontalShade = horizontalShade;
        }
    }

    public class GameplaySettings
    {
        public int TileSize { get; private set; }
        public bool ShowDebug { get; private set; }

        [JsonConstructor]
        public GameplaySettings(int tileSize, bool showDebug) {
            TileSize = tileSize;
            ShowDebug = showDebug;
        }
    }
}
