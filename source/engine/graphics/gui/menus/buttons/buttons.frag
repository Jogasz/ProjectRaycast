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
    vec3 menuColor;
    if (texIndex == 0) menuColor = vec3(0.0, 1.0, 0.0); 
    else if (texIndex == 1) menuColor = vec3(0.0, 0.0, 1.0); 

    FragColor = vec4(menuColor, 1.0);
}
//==============================================================