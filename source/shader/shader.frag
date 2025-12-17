//Declaration of shader version and core profile functionality
    //Fragment shader is calculating the color output for the pixels
#version 330 core
//==============================================================
//In-and outgoing variables
    //The input variable from Vertex Shader (same name and type)
in vec3 quadColor;

    //Vec4 that defines the final color output that we should calculate ourselves
out vec4 FragColor;
//==============================================================
//main() method entry point
void main()
{
        //ourColor is a vec3 (R,G,B) + including default 1.0 Alpha
    FragColor = vec4(quadColor, 1.0);
}
//==============================================================