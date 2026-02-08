using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Mathematics;

public class Level
{
    public static int[,] mapWalls;
    public static int[,] mapCeiling;
    public static int[,] mapFloor;

    public static List<SpriteData> Sprites { get; private set; } = new();

    public void Load()
    {
        string path = "assets/maps/map01.json";

        if (!File.Exists(path))
            throw new FileNotFoundException($"Map file not found:\n - '{path}'");

        string jsonText = File.ReadAllText(path);

        var json = JsonConvert.DeserializeObject<MapData>(jsonText)
            ?? throw new InvalidOperationException($"Failed to deserialize map file:\n - '{path}'");

        int rows, cols;

        rows = json.MapCeiling.Count;
        cols = json.MapCeiling[0].Count;
        mapCeiling = new int[rows, cols];
        for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
                mapCeiling[y, x] = json.MapCeiling[y][x];

        rows = json.MapWalls.Count;
        cols = json.MapWalls[0].Count;
        mapWalls = new int[rows, cols];
        for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
                mapWalls[y, x] = json.MapWalls[y][x];

        rows = json.MapFloor.Count;
        cols = json.MapFloor[0].Count;
        mapFloor = new int[rows, cols];
        for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
                mapFloor[y, x] = json.MapFloor[y][x];

        Sprites = json.Sprites ?? new List<SpriteData>();
    }

    public sealed class SpriteData
    {
        public int Type { get; set; }
        public int Id { get; set; }
        public bool isInteracted { get; set; } = false;
        public bool State { get; set; } = true;
        public int Health { get; set; }
        public Vector2 Position { get; set; }
    }

    private sealed class MapData
    {
        public List<List<int>> MapCeiling { get; set; } = new();
        public List<List<int>> MapWalls { get; set; } = new();
        public List<List<int>> MapFloor { get; set; } = new();
        public List<SpriteData>? Sprites { get; set; }
    }
}
