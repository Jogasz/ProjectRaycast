using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Engine;

internal partial class ShaderHandler
{
    //Instance
    public static ShaderHandler? ButtonsShader { get; set; }
    //VBO, VAO
    static int ButtonsVAO { get; set; }
    static int ButtonsVBO { get; set; }
    //Containers
    public static List<float> ButtonsVertexAttribList { get; set; } = new List<float>();
    static float[]? ButtonsVertices { get; set; }

    internal static void LoadButtonsShader(
        string vertexPath,
        string fragmentPath,
        Matrix4 projection)
    {
        ButtonsShader = new ShaderHandler(vertexPath, fragmentPath);
        //VAO, VBO Creating
        ButtonsVAO = GL.GenVertexArray();
        ButtonsVBO = GL.GenBuffer();
        //VAO, VBO Binding
        GL.BindVertexArray(ButtonsVAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, ButtonsVBO);
        //Attribute0
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        //Attribute1
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 1, VertexAttribPointerType.Float, false, 5 * sizeof(float), 4 * sizeof(float));
        //Divisor
        GL.VertexAttribDivisor(0, 1);
        GL.VertexAttribDivisor(1, 1);
        //Disable face culling to avoid accidentally removing one triangle
        GL.Disable(EnableCap.CullFace);
        //Unbind for safety
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        //Uniforms
        ButtonsShader.Use();
        ButtonsShader.SetMatrix4("uProjection", projection);
    }

    internal static void UpdateButtonsUniforms()
    {
        //ButtonsShader
        ButtonsShader?.Use();
        ButtonsShader?.SetMatrix4("uProjection", projection);
    }

    internal static void LoadBufferAndClearButtons()
    {
        //Making array
        ButtonsVertices = ButtonsVertexAttribList.ToArray();
        //Loading buffer
        GL.BindBuffer(BufferTarget.ArrayBuffer, ButtonsVBO);
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            ButtonsVertices.Length * sizeof(float),
            ButtonsVertices,
            BufferUsageHint.DynamicDraw);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        //CLEARING LIST
        ButtonsVertexAttribList.Clear();
    }

    internal static void DrawButtons()
    {
        ButtonsShader?.Use();
        //Binding and drawing
        GL.BindVertexArray(ButtonsVAO);
        int menusLen = ButtonsVertices?.Length ?? 0;
        int instanceCount = menusLen / 5;
        if (instanceCount > 0)
        {
            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, instanceCount);
        }
    }
}