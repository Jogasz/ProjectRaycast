//Declaration of shader version and core profile functionality
    //Fragment shader is calculating the color output for the pixels
#version 330 core
//==============================================================
//In-and outgoing variables
    //The input variable from Vertex Shader (same name and type)
in float vTexId;;

    //Vec4 that defines the final color output that we should calculate ourselves
out vec4 FragColor;
//==============================================================
//main() method entry point
void main()
{
    vec3 baseClr;
    
    if (vTexId == 0) baseClr = vec3(1.0, 0.0, 0.0);
    else if (vTexId == 1) baseClr = vec3(0.0, 1.0, 0.0);
    else if (vTexId == 2) baseClr = vec3(0.0, 0.0, 1.0);
    else if (vTexId == 3) baseClr = vec3(0.0, 1.0, 1.0);

    FragColor = vec4(baseClr, 1.0);
}
//==============================================================