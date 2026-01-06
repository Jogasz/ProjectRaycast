using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

internal sealed class Texture : IDisposable
{
    public int Handle { get; }

    //Texture paths
    static string[] paths =
    {
        //Defaults
        "assets/textures/planks.png",
        "assets/textures/mossy_planks.png",
        "assets/textures/stonebricks.png",
        "assets/textures/mossy_stonebricks.png",
        //Doors
        "assets/textures/door_stonebricks.png",
        "assets/textures/door_mossy_stonebricks.png",
        //Windows
        "assets/textures/window_stonebricks.png",
        "assets/textures/window_mossy_stonebricks.png"
    };

    //Textures
    public static List<Texture?> textures = new();

    //Map datas
    public static int mapCeilingTex { get; set; }
    public static int mapFloorTex { get; set; }
    public static int mapWallsTex { get; set; }
    public static Vector2i mapSize { get; set; }

    public static void LoadAll(int[,] mapWalls, int[,] mapCeiling, int[,] mapFloor)
    {
        textures.Clear();

        //Dummy texture
        textures.Add(null);

        try
        {
            mapCeilingTex = CreateMapTexture(mapCeiling);
            mapFloorTex = CreateMapTexture(mapFloor);
            mapWallsTex = CreateMapTexture(mapWalls);

            mapSize = (mapWalls.GetLength(1), mapWalls.GetLength(0));

            foreach (var path in paths)
            {
                textures.Add(new Texture(path));
            }

            Console.WriteLine(" - TEXTURES have been loaded!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($" - Something went wrong while loading TEXTURES...\n{ex}");
        }
    }

    static int CreateMapTexture(int[,] map)
    {
        Vector2i size = (map.GetLength(1), map.GetLength(0));
        int[] data = new int[size.X * size.Y];

        for (int y = 0; y < size.Y; y++)
        {
            for (int x = 0; x < size.X; x++)
            {
                data[y * size.X + x] = map[y, x];
            }
        }

        int handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, handle);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        GL.TexImage2D(
            TextureTarget.Texture2D,
            level: 0,
            internalformat: PixelInternalFormat.R32i,
            width: size.X,
            height: size.Y,
            border: 0,
            format: PixelFormat.RedInteger,
            type: PixelType.Int,
            pixels: data);

        GL.BindTexture(TextureTarget.Texture2D, 0);

        return handle;
    }

    public static void Bind(int textureIndex, TextureUnit unit = TextureUnit.Texture0)
    {
        //If index is-or smaller than 0, or bigger than num of textures, UNBIND
        if (textureIndex < 0 || textureIndex >= textures.Count)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            return;
        }

        //If index is null, UNBIND
        Texture? texture = textures[textureIndex];
        if (texture is null)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            return;
        }

        //If texture exists, BIND
        texture.Use(unit);
    }

    public Texture(string path)
    {
        // Generate a blank texture and bind it.
        Handle = GL.GenTexture();
        Use();

        // Basic sampling/wrapping defaults (safe, common choice).
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        // Pixel-art filtering (no smoothing)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        // stb_image loads from the top-left, OpenGL expects bottom-left.
        StbImage.stbi_set_flip_vertically_on_load(1);

        using var stream = File.OpenRead(path);
        ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

        // Upload pixel data to the GPU.
        GL.TexImage2D(
            TextureTarget.Texture2D,
            level: 0,
            internalformat: PixelInternalFormat.Rgba,
            width: image.Width,
            height: image.Height,
            border: 0,
            format: PixelFormat.Rgba,
            type: PixelType.UnsignedByte,
            pixels: image.Data);

        // Generate mipmaps (optional, but recommended with the min filter above).
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
    }

    public void Use(TextureUnit unit = TextureUnit.Texture0)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }

    public void Dispose()
    {
        // Safely delete the GL resource if it was created.
        if (Handle != 0)
        {
            GL.DeleteTexture(Handle);
        }

        GC.SuppressFinalize(this);
    }
}