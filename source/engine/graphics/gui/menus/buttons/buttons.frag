//Declaration of shader version and core profile functionality
 //Fragment shader is calculating the color output for the pixels
#version 330 core
//==============================================================
//In-and outgoing variables
 //The input variable from Vertex Shader (same name and type)
in float texIndex;

//Interpolated UV
in vec2 vUv;

 //Vec4 that defines the final color output that we should calculate ourselves
out vec4 FragColor;

//Buttons sheet
uniform sampler2D uButtonsAtlas;
//==============================================================
void main()
{
 //CHANGED: sample from atlas
 vec4 tex = texture(uButtonsAtlas, vUv);

 if (tex.a <=0.0 || distance(tex.rgb, vec3(255, 0, 220) / 255) < 0.1) discard;

 FragColor = tex;
}
//==============================================================