using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Engine;

internal partial class ShaderHandler
{
    //Instance
    public static ShaderHandler? HUDShader { get; set; }
    //VBO, VAO
    static int HUDVAO { get; set; }
    static int HUDVBO { get; set; }
    //Containers
    public static List<float> HUDVertexAttribList { get; set; } = new List<float>();
    static float[]? HUDVertices { get; set; }

    static void LoadHUDShader(
        string vertexPath,
        string fragmentPath,
        Matrix4 projection)
    {
        HUDShader = new ShaderHandler(vertexPath, fragmentPath);
        //VAO, VBO Creating
        HUDVAO = GL.GenVertexArray();
        HUDVBO = GL.GenBuffer();
        //VAO, VBO Binding
        GL.BindVertexArray(HUDVAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, HUDVBO);
        //Attribute0 (aPos)
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        //Attribute1 (aTexIndex)
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 1, VertexAttribPointerType.Float, false, 5 * sizeof(float), 4 * sizeof(float));
        //Divisor
        GL.VertexAttribDivisor(0, 1);
        GL.VertexAttribDivisor(1, 1);

        GL.Disable(EnableCap.CullFace);
        //Unbind for safety
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        //Uniforms
        HUDShader.Use();
        HUDShader.SetMatrix4("uProjection", projection);

        // HUD textures are always bound to TextureUnit.Texture0..2
        HUDShader.SetInt("uTextures[0]", 0);
        HUDShader.SetInt("uTextures[1]", 1);
        HUDShader.SetInt("uTextures[2]", 2);
    }

    internal static void UpdateHUDUniforms()
    {
        HUDShader?.Use();
        HUDShader?.SetMatrix4("uProjection", projection);

        HUDShader?.SetInt("uTextures[0]", 0);
        HUDShader?.SetInt("uTextures[1]", 1);
        HUDShader?.SetInt("uTextures[2]", 2);
    }

    internal static void LoadBufferAndClearHUD()
    {
        //Making array
        HUDVertices = HUDVertexAttribList.ToArray();
        //Loading buffer
        GL.BindBuffer(BufferTarget.ArrayBuffer, HUDVBO);
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            HUDVertices.Length * sizeof(float),
            HUDVertices,
            BufferUsageHint.DynamicDraw);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        //CLEARING LIST
        HUDVertexAttribList.Clear();
    }

    internal static void DrawHUD()
    {
        // Bind HUD textures (sword, vignette, container) ->0..2
        HUDTextureManager.BindAll();

        HUDShader?.Use();

        //Sprites use explicit alpha in the fragment shader, so enable blending here.
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        //Binding and drawing
        GL.BindVertexArray(HUDVAO);
        int len = HUDVertices?.Length ?? 0;
        int instanceCount = len / 5;
        if (instanceCount > 0)
        {
            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, instanceCount);
        }

        //Restore state so other passes aren't affected.
        GL.Disable(EnableCap.Blend);
    }
}