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
        //Reading text in file
        string path = File.ReadAllText("assets/maps/map01.json");

        //Deserialize text to data
        var fileText = JsonConvert.DeserializeObject<MapData>(path);

        int rows, cols;

        rows = fileText.MapCeiling.Count;
        cols = fileText.MapCeiling[0].Count;

        mapCeiling = new int[rows, cols];

        //Declaring mapCeiling[]
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                mapCeiling[y, x] = fileText.MapCeiling[y][x];
            }
        }

        rows = fileText.MapWalls.Count;
        cols = fileText.MapWalls[0].Count;

        mapWalls = new int[rows, cols];

        //Declaring mapWalls[]
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                mapWalls[y, x] = fileText.MapWalls[y][x];
            }
        }

        rows = fileText.MapFloor.Count;
        cols = fileText.MapFloor[0].Count;

        mapFloor = new int[rows, cols];

        //Declaring mapFloor[]
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                mapFloor[y, x] = fileText.MapFloor[y][x];
            }
        }
    }

    private class MapData
    {
        public List<List<int>> MapCeiling { get; set; }
        public List<List<int>> MapWalls { get; set; }
        public List<List<int>> MapFloor { get; set; }
    }
}
