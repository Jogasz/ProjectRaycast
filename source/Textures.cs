using System;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public static class Textures
{
    //Texture datas
    //Structure:
    // Index   data
    //  [1]     [0]
    //Datas:
    // [0] = width
    // [1] = height
    // [2] = R
    // [3] = G
    // [4] = B
    // [5] = R
    // [6] = G
    // [7] = B...
    public static int[][] datas { get; private set; }

    //Loading PNG's and populating datas[][]
    public static void Load()
    {
        // Candidate folders to look for PNGs (project root or inside assets)
        string[] candidates = new[] { Path.Combine("textures"), Path.Combine("assets", "textures") };

        string foundDir = candidates.FirstOrDefault(Directory.Exists);
        if (foundDir == null)
        {
            Console.WriteLine("No 'textures' directory found. Searched: " + string.Join(", ", candidates));
            // Ensure datas has the required empty zeroth row
            datas = new int[1][] { new int[0] };
            return;
        }

        // Get png files (alphabetical)
        var pngFiles = Directory.GetFiles(foundDir, "*.png").OrderBy(p => p).ToArray();
        if (pngFiles.Length == 0)
        {
            Console.WriteLine($"No PNG files found in '{foundDir}'");
            datas = new int[1][] { new int[0] };
            return;
        }

        datas = new int[1 + pngFiles.Length][];
        datas[0] = new int[0];

        for (int i = 0; i < pngFiles.Length; i++)
        {
            string path = pngFiles[i];
            try
            {
                datas[i + 1] = ReadPngMerged(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load texture '{path}': {ex.Message}");
                datas[i + 1] = new int[0];
            }
        }
    }

    // Read a PNG into merged format [w,h,R,G,B,...]
    private static int[] ReadPngMerged(string path)
    {
        using Image<Rgba32> img = Image.Load<Rgba32>(path);
        int w = img.Width;
        int h = img.Height;
        int[] merged = new int[2 + w * h * 3];
        merged[0] = w;
        merged[1] = h;
        int idx = 2;
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                Rgba32 p = img[x, y];
                merged[idx++] = p.R;
                merged[idx++] = p.G;
                merged[idx++] = p.B;
            }
        }
        return merged;
    }
}