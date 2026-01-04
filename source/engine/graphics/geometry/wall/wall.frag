//Fragment shader is calculating the color output for the pixels

//Declaration of shader version and core profile functionality
//==============================================================
#version 330 core
//==============================================================

//Incoming and outgoing variables, uniforms
//==============================================================
in vec2 vStripQuadY;

    //Final outgoing RGBA of the quad (out)
out vec4 FragColor;
//==============================================================

//Entry point
//==============================================================
void main()
{
        //Returning the correct color
    FragColor = vec4(0.1, 0.2, 0.3, 1.0);
}
//==============================================================