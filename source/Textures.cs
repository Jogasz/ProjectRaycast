using System;
using System.IO;

public static class Textures {
    public static int[][] bricksTexture { get; private set; }
    public static int[][] mossyBricksTexture { get; private set; }
    public static int[][] stoneBricksTexture { get; private set; }
    public static void Load() {
        bricksTexture = Read("assets/textures/bricks.txt");
        mossyBricksTexture = Read("assets/textures/mossy_bricks.txt");
        stoneBricksTexture = Read("assets/textures/stonebricks.txt");
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