using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

internal sealed class Texture : IDisposable
{
    public int Handle { get; }

    public Texture(string path)
    {
        // Generate a blank texture and bind it.
        Handle = GL.GenTexture();
        Use();

        // Basic sampling/wrapping defaults (safe, common choice).
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

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