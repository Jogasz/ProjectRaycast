//Declaration of shader version and core profile functionality
    //Fragment shader is calculating the color output for the pixels
#version 330 core
//==============================================================
//In-and outgoing variables
    //The input variable from Vertex Shader (same name and type)
in float texIndex;

    //Vec4 that defines the final color output that we should calculate ourselves
out vec4 FragColor;
//==============================================================
void main()
{
    vec4 menuColor;
    if (texIndex == 0) menuColor = vec4(0.0, 1.0, 0.0, 1.0); 
    else if (texIndex == 1) menuColor = vec4(0.0, 0.0, 1.0, 0.3); 

    FragColor = menuColor;
}
//==============================================================