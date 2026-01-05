using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Engine;

internal class Shader
{
    public int Handle;

    public Shader(string vertexPath, string fragmentPath)
    {
        //Handlers for the induvidual shaders
            //Vertex Shader
        int VertexShader;
            //Fragment Shader
        int FragmentShader;

        //If file is not found
        if (!File.Exists(vertexPath))
            throw new FileNotFoundException($"Vertex shader file not found:\n - '{vertexPath}'");

        if (!File.Exists(fragmentPath))
            throw new FileNotFoundException($"Fragment shader file not found:\n - '{fragmentPath}'");

        //Loading shader files
        string VertexShaderSource = File.ReadAllText(vertexPath);
        string FragmentShaderSource = File.ReadAllText(fragmentPath);

        //Generating the shaders and binding the source code to the shaders
        VertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(VertexShader, VertexShaderSource);

        FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(FragmentShader, FragmentShaderSource);

        //Compiling the Vertex Shader and checking for errors
        GL.CompileShader(VertexShader);

        //Compiling the Fragment Shader and checking for errors
        GL.CompileShader(FragmentShader);

        int VertexCompileSucces;
        int FragmentCompileSucces;

        //Getting the status of the compiler
        GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out VertexCompileSucces);

        //Vertex compiling error
        if (VertexCompileSucces == 0)
        {
            string infoLog = string.IsNullOrWhiteSpace(GL.GetShaderInfoLog(VertexShader)) ? "Unknown" : GL.GetShaderInfoLog(VertexShader);
            throw new InvalidOperationException($"Compiling vertex shader has failed:\n - '{vertexPath}'\n - Reason: '{infoLog}'");
        }

        //Getting the status of the compiler
        GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out FragmentCompileSucces);

        //Fragment compiling error
        if (FragmentCompileSucces == 0)
        {
            string infoLog = string.IsNullOrWhiteSpace(GL.GetShaderInfoLog(FragmentShader)) ? "Unknown" : GL.GetShaderInfoLog(FragmentShader);
            throw new InvalidOperationException($"Compiling fragment shader has failed:\n - '{fragmentPath}'\n - Reason: '{infoLog}'");
        }

        //Linking shaders into a program that can be run on the GPU
        Handle = GL.CreateProgram();

        GL.AttachShader(Handle, VertexShader);
        GL.AttachShader(Handle, FragmentShader);

        GL.LinkProgram(Handle);

        //Writing out error if there is one
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int linkingSuccess);

        //Program linking error
        if (linkingSuccess == 0)
        {
            string infoLog = string.IsNullOrWhiteSpace(GL.GetProgramInfoLog(Handle)) ? "Unknown" : GL.GetProgramInfoLog(Handle);
            throw new InvalidOperationException($"Linking shader program has failed:\n - Vertex: '{vertexPath}'\n - Fragment: '{fragmentPath}'\n - Reason: '{infoLog}'");
        }

        GL.DetachShader(Handle, VertexShader);
        GL.DetachShader(Handle, FragmentShader);
        GL.DeleteShader(FragmentShader);
        GL.DeleteShader(VertexShader);
    }
    //Method to be able to use the Shader handler program
    public void Use()
    {
        GL.UseProgram(Handle);
    }

    //Cleaning up the handle after this class dies
    private bool disposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            GL.DeleteProgram(Handle);

            disposedValue = true;
        }
    }

    ~Shader()
    {
        if (disposedValue == false)
        {
            Console.WriteLine("GPU Resource leak!");
        }
    }

    public void SetMatrix4(string name, Matrix4 matrix)
    {
        int location = GL.GetUniformLocation(Handle, name);
        if (location == -1) return;
        
        GL.UniformMatrix4(location, false, ref matrix);
    }

    //Float uniform setter
    public void SetFloat(string name, float value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        if (location == -1) return;
        GL.Uniform1(location, value);
    }

    //Int uniform setter
    public void SetInt(string name, int value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        if (location == -1) return;
        GL.Uniform1(location, value);
    }

    //Vec2 uniform setter
    public void SetVector2(string name, Vector2 value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        if (location == -1) return;
        GL.Uniform2(location, value);
    }

    //Vec3 uniform setter
    public void SetVector3(string name, Vector3 value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        if (location == -1) return;
        GL.Uniform3(location, value);
    }

    //Vec4 uniform setter
    public void SetVector4(string name, Vector4 value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        if (location == -1) return;
        GL.Uniform4(location, value);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    static Matrix4 projection { get; set; }

    //DefShader
    //====================================================================================
        //Instance
    public static Shader? defShader { get; set; }
        //VBO, VAO
    static int defVAO { get; set; }
    static int defVBO { get; set; }
        //Containers
    public static List<float> defVertexAttribList { get; set; } = new List<float>();
    static float[]? defVertices { get; set; }
    //====================================================================================

    //CeilingShader
    //====================================================================================
        //Instance
    public static Shader? ceilingShader { get; set; }
        //VBO, VAO
    static int ceilingVAO { get; set; }
    static int ceilingVBO { get; set; }
        //Containers
    public static List<float> ceilingVertexAttribList { get; set; } = new List<float>();
    static float[]? ceilingVertices { get; set; }
    //====================================================================================

    //WallShader
    //====================================================================================
    //Instance
    public static Shader? wallShader { get; set; }
    //VBO, VAO
    static int wallVAO { get; set; }
    static int wallVBO { get; set; }
    //Containers
    public static List<float> wallVertexAttribList { get; set; } = new List<float>();
    static float[]? wallVertices { get; set; }
    //====================================================================================

    //OnLoad
    public static void LoadAll(Vector2i ClientSize, Vector2 minimumScreen)
    {
        //Viewport and projection (bottom-left origin)
        projection = Matrix4.CreateOrthographicOffCenter(0f, ClientSize.X, 0f, ClientSize.Y, -1f, 1f);

        LoadDefShader(
            "source/shader/shader.vert",
            "source/shader/shader.frag",
            projection);

        LoadCeilingShader(
            "source/engine/graphics/geometry/ceiling/ceiling.vert",
            "source/engine/graphics/geometry/ceiling/ceiling.frag",
            projection,
            minimumScreen);

        LoadWallShader(
            "source/engine/graphics/geometry/wall/wall.vert",
            "source/engine/graphics/geometry/wall/wall.frag",
            projection);
    }

    static void LoadDefShader(
        string vertexPath,
        string fragmentPath,
        Matrix4 projection)
    {
        defShader = new Shader(vertexPath, fragmentPath);
            //VAO, VBO Creating
        defVAO = GL.GenVertexArray();
        defVBO = GL.GenBuffer();
            //VAO, VBO Binding
        GL.BindVertexArray(defVAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, defVBO);
            //Attribute0
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);
            //Attribute1
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 4 * sizeof(float));
            //Divisor
        GL.VertexAttribDivisor(0, 1);
        GL.VertexAttribDivisor(1, 1);
            //Disable face culling to avoid accidentally removing one triangle
        GL.Disable(EnableCap.CullFace);
            //Unbind for safety
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
            //Uniforms
        defShader.Use();
        defShader.SetMatrix4("uProjection", projection);
    }

    static void LoadCeilingShader(
        string vertexPath,
        string fragmentPath,
        Matrix4 projection,
        Vector2 minimumScreen)
    {
        ceilingShader = new Shader(vertexPath, fragmentPath);
            //VAO, VBO
        ceilingVAO = GL.GenVertexArray();
        ceilingVBO = GL.GenBuffer();
            //VAO, VBO Binding
        GL.BindVertexArray(ceilingVAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, ceilingVBO);
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
        ceilingShader.Use();
        ceilingShader.SetMatrix4("uProjMat", projection);
        ceilingShader.SetVector2("uMinimumScreen", new Vector2(minimumScreen.X, minimumScreen.Y));
        ceilingShader.SetFloat("uTileSize", Settings.Gameplay.tileSize);
        ceilingShader.SetFloat("uDistanceShade", Settings.Graphics.distanceShade);
    }

    static void LoadWallShader(
        string vertexPath,
        string fragmentPath,
        Matrix4 projection)
    {
        wallShader = new Shader(vertexPath, fragmentPath);
        //VAO, VBO
        wallVAO = GL.GenVertexArray();
        wallVBO = GL.GenBuffer();
        //VAO, VBO Binding
        GL.BindVertexArray(wallVAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, wallVBO);
        //Attribute0 - Vertex
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
        //Attribute1 - wallheight
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 1, VertexAttribPointerType.Float, false, 8 * sizeof(float), 4 * sizeof(float));
        //Attribute2 - rayLength
        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 8 * sizeof(float), 5 * sizeof(float));
        //Attribute3 - rayTilePosition
        GL.EnableVertexAttribArray(3);
        GL.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
        //Attribute4 - textureIndex
        GL.EnableVertexAttribArray(4);
        GL.VertexAttribPointer(4, 1, VertexAttribPointerType.Float, false, 8 * sizeof(float), 7 * sizeof(float));
        //Divisor
        GL.VertexAttribDivisor(0, 1);
        GL.VertexAttribDivisor(1, 1);
        GL.VertexAttribDivisor(2, 1);
        GL.VertexAttribDivisor(3, 1);
        GL.VertexAttribDivisor(4, 1);
        //Disable face culling to avoid accidentally removing one triangle
        GL.Disable(EnableCap.CullFace);
        //Unbind for safety
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        //Uniforms
        wallShader.Use();
        wallShader.SetMatrix4("uProjMat", projection);
    }

    //OnFramebufferResize
    public static void UpdateUniforms(Vector2i ClientSize, Vector2 minimumScreen)
    {
        projection = Matrix4.CreateOrthographicOffCenter(0f, ClientSize.X, 0f, ClientSize.Y, -1f, 1f);

        //Uniforms
            //DefShader
        defShader?.Use();
        defShader?.SetMatrix4("uProjection", projection);
            //CeilingShader
        ceilingShader?.Use();
        ceilingShader?.SetMatrix4("uProjMat", projection);
        ceilingShader?.SetVector2("uMinimumScreen", new Vector2(minimumScreen.X, minimumScreen.Y));
        wallShader?.Use();
        wallShader?.SetMatrix4("uProjMat", projection);
    }

    //OnUpdateFrame
    public static void LoadBufferAndClear()
    {
        //DefShader
        //==============================================
            //Making array
        defVertices = defVertexAttribList.ToArray();
            //Loading buffer
        GL.BindBuffer(BufferTarget.ArrayBuffer, defVBO);
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            defVertices.Length * sizeof(float),
            defVertices,
            BufferUsageHint.DynamicDraw);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            //CLEARING LIST
        defVertexAttribList.Clear();
        //==============================================

        //DefShader
        //==============================================
            //Making array
        ceilingVertices = ceilingVertexAttribList.ToArray();
            //Loading buffer
        GL.BindBuffer(BufferTarget.ArrayBuffer, ceilingVBO);
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            ceilingVertices.Length * sizeof(float),
            ceilingVertices,
            BufferUsageHint.DynamicDraw);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            //CLEARING LIST
        ceilingVertexAttribList.Clear();
        //==============================================

        //WallShader
        //==============================================
        //Making array
        wallVertices = wallVertexAttribList.ToArray();
        //Loading buffer
        GL.BindBuffer(BufferTarget.ArrayBuffer, wallVBO);
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            wallVertices.Length * sizeof(float),
            wallVertices,
            BufferUsageHint.DynamicDraw);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        //CLEARING LIST
        wallVertexAttribList.Clear();
        //==============================================
    }

    //OnRenderFrame
    public static void Draw(
        float wallWidth,
        Vector2 playerPosition,
        float playerAngle,
        float pitch)
    {
        //Bindig textures
        //===========================================================================
            //Map arrays
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, Texture.mapWallsTex);
        GL.ActiveTexture(TextureUnit.Texture1);
        GL.BindTexture(TextureTarget.Texture2D, Texture.mapFloorTex);
        GL.ActiveTexture(TextureUnit.Texture2);
        GL.BindTexture(TextureTarget.Texture2D, Texture.mapCeilingTex);
            //Images
        for (int i = 0; i < Texture.textures.Count; i++)
        {
            Texture.Bind(i, TextureUnit.Texture3 + i);
        }
        //===========================================================================

        //DefShader
        //===========================================================================
        defShader?.Use();
            //Binding and drawing
        GL.BindVertexArray(defVAO);
        int defLen = defVertices?.Length ?? 0;
        int instanceCount = defLen / 7;
        if (instanceCount > 0)
        {
            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, instanceCount);
        }
        //===========================================================================

        //CeilingShader
        //===========================================================================
        ceilingShader?.Use();
            //Creating texture array uniform
        for (int i = 0; i < Texture.textures.Count; i++)
        {
            ceilingShader?.SetInt($"uTextures[{i}]", 3 + i);
        }
            //Uniforms
        ceilingShader?.SetInt("uMapCeiling", 2);
        ceilingShader?.SetVector2("uMapSize", new Vector2(Level.mapCeiling.GetLength(1), Level.mapCeiling.GetLength(0)));
        ceilingShader?.SetFloat("uStepSize", wallWidth);
        ceilingShader?.SetVector2("uPlayerPos", new Vector2(playerPosition.X, playerPosition.Y));
        ceilingShader?.SetFloat("uPlayerAngle", playerAngle);
        ceilingShader?.SetFloat("uPitch", pitch);
            //Binding and drawing
        GL.BindVertexArray(ceilingVAO);
        int ceilLen = ceilingVertices?.Length ?? 0;
        instanceCount = ceilLen / 5;
        if (instanceCount > 0)
        {
            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, instanceCount);
        }
        //===========================================================================

        //WallShader
        //===========================================================================
        wallShader?.Use();
            //Creating texture array uniform
        for (int i = 0; i < Texture.textures.Count; i++)
        {
            wallShader?.SetInt($"uTextures[{i}]", 3 + i);
        }
            //Uniforms
        wallShader?.SetInt("uMapWalls", 0);
        wallShader?.SetVector2("uMapSize", new Vector2(Level.mapCeiling.GetLength(1), Level.mapCeiling.GetLength(0)));
            //Binding and drawing
        GL.BindVertexArray(wallVAO);
        int wallLen = wallVertices?.Length ?? 0;
        instanceCount = wallLen / 8;
        if (instanceCount > 0)
        {
            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, instanceCount);
        }
        //===========================================================================
    }

    //OnUnload
    public static void DisposeAll()
    {
        //Dispose shaders
        defShader?.Dispose();
        ceilingShader?.Dispose();
        wallShader?.Dispose();
    }
}