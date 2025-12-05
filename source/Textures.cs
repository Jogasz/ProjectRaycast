using System;
using System.IO;

public static class Textures {
    //Default textures
    public static int[][] planksTexture { get; private set; }
    public static int[][] mossyPlanksTexture { get; private set; }
    public static int[][] stoneBricksTexture { get; private set; }
    public static int[][] mossyStoneBricksTexture { get; private set; }

    //Door textures
    public static int[][] doorStoneBricksTexture { get; private set; }
    public static int[][] doorMossyStoneBricksTexture { get; private set; }

    //Window textures
    public static int[][] windowStoneBricksTexture { get; private set; }
    public static int[][] windowMossyStoneBricksTexture { get; private set; }

    //Painting textures
    public static int[][] painting_a_stoneBricksTexture { get; private set; }
    public static int[][] painting_b_stoneBricksTexture { get; private set; }
    public static int[][] painting_a_mossyStoneBricksTexture { get; private set; }
    public static int[][] painting_b_mossyStoneBricksTexture { get; private set; }

    //Test textures
    public static int[][] floorTestTexture { get; private set; }
    public static void Load() {
        //Default textures
        planksTexture = Read("assets/textures/planks.txt");
        mossyPlanksTexture = Read("assets/textures/mossy_planks.txt");
        stoneBricksTexture = Read("assets/textures/stonebricks.txt");
        mossyStoneBricksTexture = Read("assets/textures/mossy_stonebricks.txt");

        //Door textures
        doorStoneBricksTexture = Read("assets/textures/door_stonebricks.txt");
        doorMossyStoneBricksTexture = Read("assets/textures/door_mossy_stonebricks.txt");

        //Window textures
        windowStoneBricksTexture = Read("assets/textures/window_stonebricks.txt");
        windowMossyStoneBricksTexture = Read("assets/textures/window_mossy_stonebricks.txt");

        //Painting textures
        painting_a_stoneBricksTexture = Read("assets/textures/painting_a_stonebricks.txt");
        painting_b_stoneBricksTexture = Read("assets/textures/painting_b_stonebricks.txt");
        painting_a_mossyStoneBricksTexture = Read("assets/textures/painting_a_mossy_stonebricks.txt");
        painting_b_mossyStoneBricksTexture = Read("assets/textures/painting_b_mossy_stonebricks.txt");

        //Test textures
        floorTestTexture = Read("assets/textures/floorTest.txt");
    }
    public static int[][] Read(string path) {
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            int[][] texture = new int[2][];
            texture[0] = new int[2];
            texture[1] = new int[lines.Length - 3];
            for (int i = 0; i < lines.Length; i++) {
                if (i == 0 || i == 2)
                {
                    continue;
                }
                else if (i == 1) {
                    string[] parts = lines[1].Split(' ');
                    texture[0][0] = int.Parse(parts[0]);
                    texture[0][1] = int.Parse(parts[1]);
                }
                else {
                    texture[1][i - 3] = int.Parse(lines[i]);
                }
            };
            return texture;
        }
        else {
            Console.WriteLine("The file: '" + path + "' does not exist!");
            return new int[0][];
        }
    }
}