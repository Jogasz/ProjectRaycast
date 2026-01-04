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

        //Loading shader files
        string VertexShaderSource = File.ReadAllText(vertexPath);
        string FragmentShaderSource = File.ReadAllText(fragmentPath);

        //Generating the shaders and binding the source code to the shaders
        VertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(VertexShader, VertexShaderSource);

        FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(FragmentShader, FragmentShaderSource);

        Console.WriteLine($"==============================================================");
        Console.WriteLine($"Compiling:\n - {vertexPath}\n - {fragmentPath}");

        //Compiling the Vertex Shader and checking for errors
        GL.CompileShader(VertexShader);

        //Compiling the Fragment Shader and checking for errors
        GL.CompileShader(FragmentShader);

        int VertexCompileSucces;
        int FragmentCompileSucces;

        //Getting the status of the compiler
        GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out VertexCompileSucces);

        //Writing out error if there is one
        if (VertexCompileSucces == 0)
        {
            string infoLog = GL.GetShaderInfoLog(VertexShader);
            Console.WriteLine($"Compiling vertex shader has failed!\n - {vertexPath}");
            Console.WriteLine(infoLog);
        }

        //Getting the status of the compiler
        GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out FragmentCompileSucces);

        //Writing out error if there is one
        if (FragmentCompileSucces == 0)
        {
            string infoLog = GL.GetShaderInfoLog(FragmentShader);
            Console.WriteLine($"Compiling fragment shader has failed!\n - {fragmentPath}");
            Console.WriteLine(infoLog);
        }

        if (VertexCompileSucces != 0 && FragmentCompileSucces != 0) Console.WriteLine("Compiling was succesful!\n");

        //Linking shaders into a program that can be run on the GPU
        Handle = GL.CreateProgram();

        GL.AttachShader(Handle, VertexShader);
        GL.AttachShader(Handle, FragmentShader);

        GL.LinkProgram(Handle);

        Console.WriteLine($"Linking:\n - {vertexPath}\n - {fragmentPath}");

        //Writing out error if there is one
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(Handle);
            Console.WriteLine("Linking has failed!\n");
            Console.WriteLine(infoLog);
        }
        else
        {
            Console.WriteLine("Linking was succesful!\n");
        }

        GL.DetachShader(Handle, VertexShader);
        GL.DetachShader(Handle, FragmentShader);
        GL.DeleteShader(FragmentShader);
        GL.DeleteShader(VertexShader);
        Console.WriteLine($"==============================================================");
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
}