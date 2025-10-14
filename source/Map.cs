using Newtonsoft.Json;
using OpenTK;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL;

public class Map
{
    public static int[,] grid;

    public void Load()
    {
        string jsonText = File.ReadAllText("assets/maps/map01.json");

        var data = JsonConvert.DeserializeObject<MapData>(jsonText);

        var rows = data.Grid.Count;
        var cols = data.Grid[0].Count;

        grid = new int[rows, cols];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                grid[y, x] = data.Grid[y][x];
            }
        }
    }

    private class MapData
    {
        public List<List<int>> Grid { get; set; }
    }
}
