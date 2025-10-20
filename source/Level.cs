using Newtonsoft.Json;
using OpenTK;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL;

public class Level
{
    public static int[,] mapWalls;
    public static int[,] mapCeiling;
    public static int[,] mapFloor;

    public void Load()
    {
        string jsonText = File.ReadAllText("assets/maps/map01.json");

        var data = JsonConvert.DeserializeObject<MapData>(jsonText);

        var rows = data.MapWalls.Count;
        var cols = data.MapWalls[0].Count;

        mapWalls = new int[rows, cols];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                mapWalls[y, x] = data.MapWalls[y][x];
            }
        }
    }

    private class MapData
    {
        public List<List<int>> MapWalls { get; set; }
    }
}
