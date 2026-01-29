using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

internal sealed class Texture : IDisposable
{
    public int Handle { get; }

    static readonly string[] texturePaths =
    {
        "assets/textures/planks.png",
        "assets/textures/mossy_planks.png",
        "assets/textures/stonebricks.png",
        "assets/textures/mossy_stonebricks.png",
        "assets/textures/door_stonebricks.png",
        "assets/textures/door_mossy_stonebricks.png",
        "assets/textures/window_stonebricks.png",
        "assets/textures/window_mossy_stonebricks.png"
    };

    static readonly string mainMenuPath = "assets/textures/gui/mainmenu.png";

    // Unified list:
    // 0: null (dummy)
    // 1: mainmenu.png
    // 2..4: reserved (maps are separate int handles, but indices stay reserved)
    // 5.. : tile textures
    public static List<Texture?> textures = new();

    // (Keep these if other code references them; they won't be used for binding anymore)
    public static List<Texture?> sprites = new();
    public static List<Texture?> images = new();

    // Map textures (R32i)
    public static int mapCeilingTex { get; private set; }
    public static int mapFloorTex { get; private set; }
    public static int mapWallsTex { get; private set; }
    public static Vector2i mapSize { get; private set; }

    public static void LoadAll(int[,] mapWalls, int[,] mapCeiling, int[,] mapFloor)
    {
        textures.Clear();

        try
        {
            mapCeilingTex = CreateMapTexture(mapCeiling);
            mapFloorTex = CreateMapTexture(mapFloor);
            mapWallsTex = CreateMapTexture(mapWalls);

            mapSize = (mapWalls.GetLength(1), mapWalls.GetLength(0));

            foreach (var texturePath in texturePaths)
                textures.Add(new Texture(texturePath));

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
            for (int x = 0; x < size.X; x++)
                data[y * size.X + x] = map[y, x];

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
        if (textureIndex < 0 || textureIndex >= textures.Count)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            return;
        }

        Texture? texture = textures[textureIndex];
        if (texture is null)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            return;
        }

        texture.Use(unit);
    }

    public Texture(string path)
    {
        Handle = GL.GenTexture();
        Use();

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        StbImage.stbi_set_flip_vertically_on_load(1);

        using var stream = File.OpenRead(path);
        ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

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

        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
    }

    public void Use(TextureUnit unit = TextureUnit.Texture0)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }

    public void Dispose()
    {
        if (Handle != 0)
            GL.DeleteTexture(Handle);

        GC.SuppressFinalize(this);
    }
}