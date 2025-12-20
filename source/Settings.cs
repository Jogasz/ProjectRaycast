using System;
using System.IO;
using Newtonsoft.Json;

internal class Settings
{
    public static PlayerSettings Player;
    public static GraphicsSettings Graphics;
    public static GameplaySettings Gameplay;

    //Handles loading and storing all game settings from settings.json
    public static void Load()
    {
        string rawText = File.ReadAllText("settings.json");

        var convertedData = JsonConvert.DeserializeObject<SettingsData>(rawText);

        Player = convertedData.Player;
        Graphics = convertedData.Graphics;
        Gameplay = convertedData.Gameplay;
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
        public int health { get; private set; }
        public int armor { get; private set; }
        public int stamina { get; private set; }
        public float movementSpeed { get; private set; }
        public float mouseSensitivity { get; private set; }


        [JsonConstructor]
        public PlayerSettings(int Health, int Armor, int Stamina, float MovementSpeed, float MouseSensitivity)
        {
            health = Health;
            armor = Armor;
            stamina = Stamina;
            movementSpeed = MovementSpeed * 10;
            mouseSensitivity = MouseSensitivity;
        }
    }

    public class GraphicsSettings
    {
        public int FOV { get; private set; }
        public int rayCount { get; private set; }
        public int renderDistance { get; private set; }
        public float distanceShade { get; private set; }

        [JsonConstructor]
        public GraphicsSettings(int fov, int RayCount, int RenderDistance, float DistanceShade) {
            FOV = fov;
            rayCount = RayCount;
            renderDistance = RenderDistance;
            distanceShade = (DistanceShade / 10);
        }
    }

    public class GameplaySettings
    {
        public int tileSize { get; private set; }

        [JsonConstructor]
        public GameplaySettings(int TileSize) {
            tileSize = TileSize;
        }
    }
}
